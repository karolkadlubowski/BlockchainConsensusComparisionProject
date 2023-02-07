using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using BKiZA.ProofOfWork.Nodes.Events;
using BKiZA.Shared.Nodes;
using BKiZA.Shared.Nodes.Events;

namespace BKiZA.ProofOfWork.Nodes;

public class Miner : INode
{
    private readonly List<User> _users;
    private List<Transaction> _transactions;

    private readonly Dictionary<string, int> _blocksVerifications = new Dictionary<string, int>();

    public string NodeId { get; }
    public BlockChain BlockChain { get; }
    public User UserAccount { get; }

    public IReadOnlyList<User> CurrentUsers => _users
        .OrderBy(node => node.NodeId)
        .ToArray();

    public IReadOnlyList<Transaction> CurrentTransactions => _transactions
        .ToArray();

    private Miner(string nodeId)
    {
        NodeId = nodeId;
        BlockChain = BlockChain.Initial;
        _users = new List<User>();
        _transactions = new List<Transaction>();
        UserAccount = new User(NodeId);
    }

    private Miner(string nodeId, Miner initialNode)
    {
        NodeId = nodeId;
        BlockChain = new BlockChain(initialNode.BlockChain.Chain);

        UserAccount = new User(NodeId);
        _users = new List<User>(initialNode.CurrentUsers.Select(u => (User)u.Clone()));
        _users.Add((User)initialNode.UserAccount.Clone());

        _transactions = new List<Transaction>(initialNode.CurrentTransactions);
    }

    private Miner(Miner miner)
    {
        NodeId = miner.NodeId;
        BlockChain = new BlockChain(new HashSet<Block>(miner.BlockChain.Chain));
        _users = new List<User>(miner._users.Select(u => (User)u.Clone()));
        _transactions = new List<Transaction>(miner._transactions.Select(t
            => new Transaction(t.TransactionId, t.From, t.To, t.Value, t.Timestamp)));
        UserAccount = (User)miner.UserAccount.Clone();
        _blocksVerifications = miner._blocksVerifications;
    }

    public static Miner CreateRoot(string nodeId)
        => new Miner(nodeId);

    public static Miner CreateNew(Miner initialNode)
        => new Miner($"{Guid.NewGuid()}", initialNode: initialNode);

    public UserNodeAdded CreateUser()
    {
        var user = new User($"{Guid.NewGuid()}");

        return new UserNodeAdded(user);
    }

    public (MinerBlockDigged Event, long DigTime) DigBlock()
    {
        var previousBlockHash = BlockChain.Previous.Hash;
        var currentBlockIndex = BlockChain.Previous.Index + 1;
        var nonce = 0L;

        var diggedHash = string.Empty;

        var timer = Stopwatch.StartNew();
        while (true)
        {
            diggedHash = CreateBlockHash(previousBlockHash, currentBlockIndex, nonce);

            if (diggedHash.StartsWith("000"))
            {
                break;
            }

            nonce++;
        }

        var random = new Random();
        Thread.Sleep(TimeSpan.FromSeconds(random.Next(1, 5)));

        var digTime = timer.ElapsedMilliseconds;
        timer.Reset();

        var transactions = _transactions.Take(5);

        var diggedBlock = Block.FromPreviousBlock(BlockChain.Previous,
            diggedHash,
            transactions,
            nonce);
        _blocksVerifications[diggedBlock.Hash] = 0;

        return (new MinerBlockDigged(diggedBlock, NodeId), digTime);
    }

    public MinerBlockVerified VerifyBlock(Block block, string minerNodeId)
    {
        var hashToVerify = CreateBlockHash(BlockChain.Previous.Hash,
            block.Index,
            block.Nonce);

        var isAccepted = block.Hash == hashToVerify;

        return new MinerBlockVerified(block, minerNodeId, isAccepted);
    }

    public MinerBlockCommitted TryToCommitBlock(MinerBlockVerified minerBlockVerified, int minersCount)
    {
        if (_blocksVerifications.TryGetValue(minerBlockVerified.Block.Hash, out var currentBlockVerificationsCount) is false)
        {
            return null;
        }

        if (minerBlockVerified.Accepted is false)
        {
            return null;
        }

        var neededVerificationsCount = new VerificationsCount(minersCount);

        currentBlockVerificationsCount++;
        _blocksVerifications[minerBlockVerified.Block.Hash] = currentBlockVerificationsCount;

        if (currentBlockVerificationsCount >= neededVerificationsCount)
        {
            _blocksVerifications.Remove(minerBlockVerified.Block.Hash);
            return new MinerBlockCommitted(minerBlockVerified.Block, NodeId);
        }

        return null;
    }

    public void AppendBlock(Block block, string minerId)
    {
        if (BlockChain.Chain.Contains(block) is false)
        {
            BlockChain.Chain.Add(block);

            foreach (var transaction in block.Transactions)
            {
                var user = _users.FirstOrDefault(u => u.NodeId == transaction.To);

                if (user is not null)
                {
                    user.Balance += transaction.Value;
                }
            }
        }

        _transactions = _transactions
            .Except(block.Transactions)
            .ToList();

        if (NodeId != minerId)
        {
            var minerUser = _users.First(u => u.NodeId == minerId);
            minerUser.Balance += Constants.BlockCommittedPrice;
        }
        else
        {
            UserAccount.Balance += Constants.BlockCommittedPrice;
        }
    }

    public void AppendTransaction(Transaction transaction)
    {
        if (_transactions.Contains(transaction) is false)
        {
            _transactions.Add(transaction);

            var transactionSender = _users.FirstOrDefault(u => u.NodeId == transaction.From);

            if (transactionSender is not null)
            {
                transactionSender.Balance -= transaction.Value;
            }
        }
    }

    public void AppendNode(INode node)
    {
        switch (node)
        {
            case Miner miner:
                TryAddMiner(miner);
                break;
            case User user:
                TryAddUser(user);
                break;
        }
    }

    private static string CreateBlockHash(string previousBlockHash, int currentBlockIndex, long nonce)
    {
        var diggedHash = string.Empty;
        var valueToHash = $"{previousBlockHash}:{currentBlockIndex}:{nonce}";

        using (var hasher = SHA256.Create())
        {
            var diggedHashBytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(valueToHash));
            diggedHash = Convert.ToBase64String(diggedHashBytes);
            // Console.WriteLine($"{valueToHash} -> {nonce} | {diggedHash}");
        }

        return diggedHash;
    }

    private void TryAddMiner(Miner miner)
    {
        if (NodeId == miner.NodeId)
        {
            return;
        }

        if (_users.Any(m => m.NodeId == miner.NodeId))
        {
            return;
        }

        _users.Add(miner.UserAccount);
    }

    private void TryAddUser(User user)
    {
        if (_users.Any(u => u.NodeId == user.NodeId))
        {
            return;
        }

        _users.Add(user);
    }

    public object Clone()
        => new Miner(this);
}
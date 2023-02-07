using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using BKiZA.ProofOfStake.Nodes.Events;
using BKiZA.Shared.Nodes;
using BKiZA.Shared.Nodes.Events;

namespace BKiZA.ProofOfStake.Nodes;

public class Validator : INode
{
    private readonly List<Validator> _validators;
    private readonly List<User> _users;
    private List<Transaction> _transactions;

    private readonly Dictionary<string, int> _blocksVerifications = new Dictionary<string, int>();

    public string NodeId { get; }

    public BlockChain BlockChain { get; }
    public User UserAccount { get; }
    public decimal Stake { get; }

    [JsonIgnore]
    public IReadOnlyList<Validator> CurrentValidators => _validators
        .OrderBy(node => node.NodeId)
        .ToArray();

    public IReadOnlyList<User> CurrentUsers => _users
        .OrderBy(node => node.NodeId)
        .ToArray();

    public IReadOnlyList<Transaction> CurrentTransactions => _transactions
        .ToArray();

    private Validator(User userNode)
    {
        NodeId = userNode.NodeId;
        BlockChain = BlockChain.Initial;
        _users = new List<User>();
        _transactions = new List<Transaction>();
        UserAccount = userNode;
        UserAccount.Balance = 0;
        Stake = 1;
        _validators = new List<Validator>();
    }

    private Validator(Validator initialNode, User userNode)
    {
        NodeId = userNode.NodeId;
        BlockChain = initialNode.BlockChain;
        _validators = new[] {initialNode}
            .Concat(initialNode.CurrentValidators)
            .ToList();

        _users = initialNode.CurrentUsers.ToList();
        _users.Add(initialNode.UserAccount);
        _users.RemoveAll(user => user.NodeId == userNode.NodeId);

        _transactions = initialNode.CurrentTransactions.ToList();

        Stake = userNode.Balance;
        UserAccount = userNode;
        UserAccount.Balance = 0;
    }

    private Validator(Validator validator)
    {
        NodeId = validator.NodeId;
        BlockChain = validator.BlockChain;
        _users = validator.CurrentUsers.ToList();
        _transactions = validator.CurrentTransactions.ToList();
        UserAccount = validator.UserAccount;
        _validators = validator._validators;
        _blocksVerifications = validator._blocksVerifications;
        Stake = validator.Stake;
    }

    public static Validator CreateRoot(User user)
        => new Validator(user);

    public static Validator TransformUserToValidator(Validator initialNode, User userNode)
        => new Validator(initialNode, userNode);

    public UserNodeAdded CreateUser()
    {
        var user = new User($"{Guid.NewGuid()}");

        return new UserNodeAdded(user);
    }

    public (ValidatorBlockDigged Event, long DigTime) DigBlock()
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

        var diggedTime = timer.ElapsedMilliseconds;
        timer.Reset();

        var transactions = _transactions.Take(5);

        var diggedBlock = Block.FromPreviousBlock(BlockChain.Previous,
            diggedHash,
            transactions,
            nonce);
        _blocksVerifications[diggedBlock.Hash] = 0;

        return (new ValidatorBlockDigged(diggedBlock), diggedTime);
    }

    private static string CreateBlockHash(string previousBlockHash, int currentBlockIndex, long nonce)
    {
        var diggedHash = string.Empty;
        var valueToHash = $"{previousBlockHash}:{currentBlockIndex}:{nonce}";

        using (var hasher = SHA256.Create())
        {
            var diggedHashBytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(valueToHash));
            diggedHash = Convert.ToBase64String(diggedHashBytes);
        }

        return diggedHash;
    }

    public ValidatorBlockVerified VerifyBlock(Block block)
    {
        var hashToVerify = CreateBlockHash(BlockChain.Previous.Hash,
            block.Index,
            block.Nonce);

        var isAccepted = block.Hash == hashToVerify;

        return new ValidatorBlockVerified(block, isAccepted);
    }

    public ValidatorBlockCommitted TryToCommitBlock(ValidatorBlockVerified ValidatorBlockVerified, int validatorsCount)
    {
        if (_blocksVerifications.TryGetValue(ValidatorBlockVerified.Block.Hash, out var currentBlockVerificationsCount) is false)
        {
            return null;
        }

        if (ValidatorBlockVerified.Accepted is false)
        {
            return null;
        }

        var neededVerificationsCount = new VerificationsCount(validatorsCount);

        currentBlockVerificationsCount++;
        _blocksVerifications[ValidatorBlockVerified.Block.Hash] = currentBlockVerificationsCount;

        if (currentBlockVerificationsCount >= neededVerificationsCount)
        {
            _blocksVerifications.Remove(ValidatorBlockVerified.Block.Hash);
            return new ValidatorBlockCommitted(ValidatorBlockVerified.Block, NodeId);
        }

        return null;
    }

    public void AppendBlock(Block block, string validatorId)
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

        if (NodeId != validatorId)
        {
            var validator = _users.First(u => u.NodeId == validatorId);
            validator.Balance += Constants.BlockCommittedPrice;
        }
        else
        {
            UserAccount.Balance += Constants.BlockCommittedPrice;
        }
    }

    public void AppendNode(INode node)
    {
        switch (node)
        {
            case Validator validator:
                TryAddValidator(validator);
                break;
            case User user:
                TryAddUser(user);
                break;
        }
    }

    private void TryAddValidator(Validator validator)
    {
        if (NodeId == validator.NodeId)
        {
            return;
        }

        if (_validators.Any(m => m.NodeId == validator.NodeId))
        {
            return;
        }

        // var userIndex = _users.FindIndex(user => user.NodeId == validator.NodeId);
        // var user = _users[userIndex];


        _validators.Add(validator);
    }

    private void TryAddUser(User user)
    {
        if (_users.Any(u => u.NodeId == user.NodeId))
        {
            return;
        }

        _users.Add(user);
    }

    public void DeleteUserFromListDuringTransformation()
    {
        _users.RemoveAll(user => user.NodeId == UserAccount.NodeId);
    }

    public string ChooseNextValidator()
    {
        decimal stakesSum = Stake;
        _validators.ForEach(stake => stakesSum += stake.Stake);

        var validatorRanges = new List<decimal>();

        decimal lastPlace = 0;

        var validatorsWithParent = _validators;
        validatorsWithParent.Add(this);

        validatorsWithParent.ForEach(delegate(Validator validator)
        {
            lastPlace += validator.Stake / stakesSum;
            validatorRanges.Add(lastPlace * 1000);
        });

        var number = new Random().Next(0, 1000);

        {
            if (number < validatorRanges[0])
                return _validators[0].NodeId;
        }


        for (int i = 1; i < validatorRanges.Count; i++)
        {
            if (number <= validatorRanges[i] && number > validatorRanges[i - 1])
            {
                return _validators[i].NodeId;
            }
        }

        return _validators[0].NodeId;
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

    public object Clone()
        => new Validator(this);

    public void ClearUserBalance(string nodeId)
    {
        var user = _users.FirstOrDefault(u => u.NodeId == nodeId);
        user.Balance = 0;
    }
}
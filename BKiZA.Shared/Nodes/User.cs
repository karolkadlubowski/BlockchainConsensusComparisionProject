using System;
using BKiZA.Shared.Nodes.Events;

namespace BKiZA.Shared.Nodes;

public class User : INode
{
    public string NodeId { get; }
    public decimal Balance { get; set; }

    public User(string nodeId, decimal balance = 0)
    {
        NodeId = nodeId;
        Balance = balance;
    }

    private User(User user)
    {
        NodeId = user.NodeId;
        Balance = user.Balance;
    }

    public static User Default => new User("A"); 

    public TransactionSent SendTransaction(string to)
    {
        var transactionValue = Constants.DigPriceMultiplier * Balance;

        if (Balance - transactionValue < decimal.Zero)
        {
            return null;
        }

        var transaction = new Transaction($"{Guid.NewGuid()}",
            NodeId,
            to,
            transactionValue,
            DateTime.UtcNow.Ticks);

        Balance -= transaction.Value;

        return new TransactionSent(transaction);
    }

    public object Clone()
        => new User(this);
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace BKiZA.Shared.Nodes;

public record Block
(
    int Index,
    string Hash,
    string PreviousHash,
    long Timestamp,
    List<Transaction> Transactions,
    long Nonce
)
{
    public static Block FromPreviousBlock(Block previous,
        string hash,
        IEnumerable<Transaction> transactions,
        long nonce)
        => new Block(previous.Index + 1,
            hash,
            previous.Hash,
            DateTime.UtcNow.Ticks,
            transactions.ToList(),
            nonce);
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace BKiZA.Shared.Nodes;

public record BlockChain(HashSet<Block> Chain)
{
    [JsonIgnore]
    public Block Previous => Chain.Last();

    public static BlockChain Initial
        => new BlockChain(new HashSet<Block>
        {
            new Block(0,
                $"{Guid.NewGuid()}",
                string.Empty,
                DateTime.UtcNow.Ticks,
                new List<Transaction>(),
                0)
        });
}
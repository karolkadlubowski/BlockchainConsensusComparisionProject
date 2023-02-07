using System;

namespace BKiZA.Shared.Nodes;

public interface INode : ICloneable
{
    string NodeId { get; }
}
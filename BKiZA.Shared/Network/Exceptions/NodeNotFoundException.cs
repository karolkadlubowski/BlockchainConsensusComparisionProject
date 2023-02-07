using System;

namespace BKiZA.Shared.Network.Exceptions;

public class NodeNotFoundException : Exception
{
    public NodeNotFoundException(string id) : base($"Node with ID: {id} not found")
    {
    }
}
namespace BKiZA.Shared.Nodes;

public record Transaction
(
    string TransactionId,
    string From,
    string To,
    decimal Value,
    long Timestamp
);
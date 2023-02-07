using BKiZA.Shared.Network;

namespace BKiZA.Shared.Nodes.Services;

public class UserService
{
    private readonly INetworkStorage<User> _userNetworkStorage;
    private readonly NetworkBroker _networkBroker;

    public UserService(INetworkStorage<User> userNetworkStorage,
        NetworkBroker networkBroker)
    {
        _userNetworkStorage = userNetworkStorage;
        _networkBroker = networkBroker;
    }

    public Transaction SendTransaction(string currentUserId, string receiverUserId)
    {
        var currentUser = _userNetworkStorage.Get(currentUserId);

        var transactionSent = currentUser.SendTransaction(receiverUserId);

        _userNetworkStorage.Update(currentUser);

        if (transactionSent is not null)
        {
            _networkBroker.Publish(transactionSent);
        }

        return transactionSent.Transaction;
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using BKiZA.ProofOfStake.Nodes;
using BKiZA.ProofOfStake.Nodes.Services;
using BKiZA.Shared.Network;
using BKiZA.Shared.Nodes;
using BKiZA.Shared.Nodes.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BKiZA.Sandbox;

public class ProofOfStakeService : BackgroundService
{
    private readonly ValidatorService _validatorService;
    private readonly UserService _userService;
    private readonly INetworkStorage<Validator> _validatorNetworkStorage;
    private readonly INetworkStorage<User> _userNetworkStorage;
    private readonly ILogger<ProofOfWorkService> _logger;

    public ProofOfStakeService(ValidatorService validatorService,
        UserService userService,
        INetworkStorage<Validator> validatorNetworkStorage,
        INetworkStorage<User> userNetworkStorage,
        ILogger<ProofOfWorkService> logger)
    {
        _validatorService = validatorService;
        _userService = userService;
        _validatorNetworkStorage = validatorNetworkStorage;
        _userNetworkStorage = userNetworkStorage;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            const string RootId = "A";
            const string CurrentUserId = "1";
            const string ReceiverUserId = "3";

            await Task.Delay(TimeSpan.FromSeconds(2));

            _validatorService.AddValidator(RootId, CurrentUserId);
            _validatorService.AddValidator(RootId, ReceiverUserId);

            await Task.CompletedTask;
            await Task.Delay(TimeSpan.FromSeconds(2));

            var validatorId = RootId;

            _validatorService.DigBlock(validatorId);

            // await Task.Delay(TimeSpan.FromSeconds(10));
            // _userService.SendTransactionToMiners("1","3");
            // _userService.SendTransactionToMiners("3","2");
            // _userService.SendTransactionToMiners("A","1");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
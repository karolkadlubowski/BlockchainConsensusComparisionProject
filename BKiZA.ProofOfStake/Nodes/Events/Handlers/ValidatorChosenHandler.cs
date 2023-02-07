using BKiZA.ProofOfStake.Nodes.Services;
using BKiZA.Shared.Infrastructure;

namespace BKiZA.ProofOfStake.Nodes.Events.Handlers;

public class ValidatorChosenHandler: IEventHandler<ValidatorChosen>
{
    private readonly ValidatorService _validatorService;

    public ValidatorChosenHandler(ValidatorService validatorService)
    {
        _validatorService = validatorService;
    }
    public void Handle(string nodeId, ValidatorChosen @event)
    {
        _validatorService.DigBlock(@event.NextValidatorId);
    }
}
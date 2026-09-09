using FluentValidation;
using Game.Application.DTOs;
using Game.Domain.Entities;

namespace Game.Application.Validators;

public class SetResultDtoValidator : AbstractValidator<SetResultDto>
{
    public SetResultDtoValidator()
    {
        RuleFor(x => x.SetNumber)
            .GreaterThan(0);

        RuleFor(x => x)
            .Must(set => GameSet.IsValidScore(
                set.TeamOneGames,
                set.TeamTwoGames,
                set.TieBreakTeamOne,
                set.TieBreakTeamTwo))
            .WithMessage(
                "A pontuação do set ou do tie-break é inválida.");
    }
}

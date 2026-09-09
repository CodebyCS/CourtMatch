using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Game.Application.DTOs;

namespace Game.Application.Validators
{
    public class CreateGameDtoValidator : AbstractValidator<CreateGameDto>
    {
        public CreateGameDtoValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty();

        RuleFor(x => x.CourtId)
            .NotEmpty();

        RuleFor(x => x.ScheduledAt)
            .Must(value => value.Kind == DateTimeKind.Utc)
            .WithMessage("A data do jogo deve estar em UTC.");

        RuleFor(x => x.ScheduledAt)
            .Must(value => value > DateTime.UtcNow)
            .WithMessage("A data do jogo deve ser futura.");

        RuleFor(x => x.ScheduledAt)
            .Must(value =>
                value.Ticks % TimeSpan.TicksPerSecond == 0)
            .WithMessage(
                "A data do jogo não pode incluir frações de segundo.");

        RuleFor(x => x.Participants)
            .NotNull();

        When(x => x.Participants is not null, () =>
        {
            RuleFor(x => x.Participants.Count)
                .InclusiveBetween(2, 4);

            RuleFor(x => x.Participants)
                .Must(participants => participants.All(p => p is not null))
                .WithMessage("A lista contém participantes inválidos.");

            When(x => x.Participants.All(p => p is not null), () =>
            {
                RuleForEach(x => x.Participants)
                    .SetValidator(new InitialParticipantDtoValidator());

                RuleFor(x => x.Participants)
                    .Must(participants =>
                        participants.Select(p => p.UserId)
                            .Distinct()
                            .Count() == participants.Count)
                    .WithMessage("Os jogadores não podem estar repetidos.");

                RuleFor(x => x.Participants)
                    .Must(participants =>
                        participants.GroupBy(p => p.TeamNumber)
                            .All(team => team.Count() <= 2))
                    .WithMessage(
                        "Cada equipa pode ter, no máximo, dois jogadores.");

                RuleFor(x => x.Participants)
                    .Must(participants =>
                        participants.Any(p => p.TeamNumber == 1) &&
                        participants.Any(p => p.TeamNumber == 2))
                    .WithMessage("Ambas as equipas devem ter jogadores.");
            });
        });
    }
}

        public class InitialParticipantDtoValidator : AbstractValidator<InitialParticipantDto>
        {
            public InitialParticipantDtoValidator()
            {
                RuleFor(x => x.UserId)
                    .NotEmpty();

                RuleFor(x => x.TeamNumber)
                    .Must(team => team is 1 or 2)
                    .WithMessage("A equipa deve ser 1 ou 2.");
            }
        }
}

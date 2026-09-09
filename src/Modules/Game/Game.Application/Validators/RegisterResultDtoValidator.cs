using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Game.Application.DTOs;
using Game.Domain.Entities;

namespace Game.Application.Validators;

    public class RegisterResultDtoValidator : AbstractValidator<RegisterResultDto>
    {
        public RegisterResultDtoValidator()
        {
            RuleFor(x => x.Sets)
                .NotNull()
                .WithMessage("Os sets são obrigatórios.");

            When(x => x.Sets is not null, () =>
            {
                RuleFor(x => x.Sets)
                    .NotEmpty()
                    .WithMessage("Indica pelo menos um set.");

                RuleFor(x => x.Sets)
                    .Must(sets => sets.All(set => set is not null))
                    .WithMessage("A lista contém sets inválidos.");

                When(x => x.Sets.All(set => set is not null), () =>
                {
                    RuleForEach(x => x.Sets)
                        .SetValidator(new SetResultDtoValidator());

                    RuleFor(x => x.Sets)
                        .Must(sets =>
                            sets.Select(set => set.SetNumber)
                                .Distinct()
                                .Count() == sets.Count)
                        .WithMessage(
                            "Os números dos sets não podem estar repetidos.");

                    RuleFor(x => x.Sets)
                        .Must(sets =>
                            sets.OrderBy(set => set.SetNumber)
                                .Select(set => set.SetNumber)
                                .SequenceEqual(
                                    Enumerable.Range(1, sets.Count)))
                        .WithMessage(
                            "Os sets devem estar numerados de 1 em diante.");

                    RuleFor(x => x.Sets)
                        .Must(sets =>
                            sets.Count(set =>
                                set.TeamOneGames > set.TeamTwoGames) !=
                            sets.Count(set =>
                                set.TeamTwoGames > set.TeamOneGames))
                        .WithMessage(
                            "O resultado final deve ter uma equipa vencedora.");
                });
            });
        }
    }





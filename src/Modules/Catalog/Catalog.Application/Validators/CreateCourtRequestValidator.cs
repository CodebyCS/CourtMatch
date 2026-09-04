using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalog.Application.DTOs;
using FluentValidation;

namespace Catalog.Application.Validators
{
    public class CreateCourtRequestValidator : AbstractValidator<CreateCourtRequest>
    {
        public CreateCourtRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.PricePerHour)
                .GreaterThan(0);
        }
    }
}

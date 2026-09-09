using Catalog.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Validators
{
    public class UpdateCourtRequestValidator : AbstractValidator<UpdateCourtRequest>
    {
        public UpdateCourtRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.PricePerHour)
                .GreaterThan(0);

            RuleFor(x => x.Status)
                .IsInEnum();
        }
    }
}

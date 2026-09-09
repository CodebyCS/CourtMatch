using Catalog.Application.DTOs;
using Catalog.Domain.Domain;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using FluentValidation;
using Shared.Contracts.Exceptions;

namespace Catalog.Application.Services
{
    public class CourtService : ICourtService
    {
        private readonly ICourtRepository _courtRepository;

        private readonly IValidator<CreateCourtRequest> _createValidator;

        private readonly IValidator<UpdateCourtRequest> _updateValidator;

        public CourtService(
            ICourtRepository courtRepository,
            IValidator<CreateCourtRequest> createValidator,
            IValidator<UpdateCourtRequest> updateValidator)
        {
            _courtRepository = courtRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;

        }

        public async Task<IEnumerable<CourtResponse>> GetAllCourtsAsync(CancellationToken cancellationToken)
        {
            var courts = await _courtRepository.GetAllAsync(cancellationToken);

            return courts.Select(ToResponse).ToList();
        }

        public async Task<CourtResponse> GetCourtByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
        {
            var court = await _courtRepository.GetByIdAsync(id, cancellationToken);

            if (court is null)
                throw new NotFoundException("Court not found.");

            return ToResponse(court);
        }

        public async Task<CourtResponse> CreateCourtAsync(
            CreateCourtRequest request,
            CancellationToken cancellationToken)
        {
            await ValidateCreateRequestAsync(request, cancellationToken);

            var court = new Court
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                IsIndoor = request.IsIndoor,
                PricePerHour = request.PricePerHour,
                Status = CourtStatus.Active
            };

            await _courtRepository.AddAsync(court, cancellationToken);

            return ToResponse(court);
        }

        public async Task UpdateCourtAsync(
            Guid id,
            UpdateCourtRequest request,
            CancellationToken cancellationToken)
        {
            await ValidateUpdateRequestAsync(request, cancellationToken);

            var court = await _courtRepository.GetByIdAsync(id, cancellationToken);

            if (court is null)
                throw new NotFoundException("Court not found.");

            court.Name = request.Name.Trim();
            court.IsIndoor = request.IsIndoor;
            court.PricePerHour = request.PricePerHour;
            court.Status = request.Status;

            await _courtRepository.UpdateAsync(court, cancellationToken);
        }

        public async Task DeleteCourtAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            var deleted = await _courtRepository.DeleteAsync(id, cancellationToken);

            if (!deleted)
                throw new NotFoundException("Court not found.");
        }

        public async Task BlockCourtAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            var court = await _courtRepository.GetByIdAsync(id, cancellationToken);

            if (court is null)
                throw new NotFoundException("Court not found.");

            court.Status = CourtStatus.UnderMaintenance;

            await _courtRepository.UpdateAsync(court, cancellationToken);
        }

        private async Task ValidateCreateRequestAsync(
            CreateCourtRequest request,
            CancellationToken cancellationToken)
        {
            var validation = await _createValidator.ValidateAsync(
                request,
                cancellationToken);

            if (!validation.IsValid)
            {
                var errors = string.Join(
                    " ",
                    validation.Errors.Select(error => error.ErrorMessage));

                throw new BadRequestException(errors);
            }
        }

        private async Task ValidateUpdateRequestAsync(
            UpdateCourtRequest request,
            CancellationToken cancellationToken)
        {
            var validation = await _updateValidator.ValidateAsync(
                request,
                cancellationToken);

            if (!validation.IsValid)
            {
                var errors = string.Join(
                    " ",
                    validation.Errors.Select(error => error.ErrorMessage));

                throw new BadRequestException(errors);
            }
        }

        private static CourtResponse ToResponse(Court court)
        {
            return new CourtResponse
            {
                Id = court.Id,
                Name = court.Name,
                IsIndoor = court.IsIndoor,
                PricePerHour = court.PricePerHour,
                Status = court.Status
            };
        }

    }
}

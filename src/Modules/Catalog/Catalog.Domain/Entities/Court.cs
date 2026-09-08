using Catalog.Domain.Domain;

namespace Catalog.Domain.Entities;

public class Court
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public bool IsIndoor { get; set; }

    public decimal PricePerHour { get; set; }

    public CourtStatus Status { get; set; } = CourtStatus.Active;
}

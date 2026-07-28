namespace Catalog.Domain.Entities;

public class Court
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public bool IsIndoor { get; set; }

    public decimal PricePerHour { get; set; }

    public string Status { get; set; } = "Available";
}

namespace Catalog.Domain.Entities;

public class Equipment
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public int Stock { get; set; }

    public decimal RentalPrice { get; set; }
}
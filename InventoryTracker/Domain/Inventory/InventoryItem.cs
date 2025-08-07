using InventoryTracker.Domain.Identity;

namespace InventoryTracker.Domain.Inventory;

public class InventoryItem
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public InventoryItem(string id, string name, int quantity, decimal price, string category, DateTime? createdAt = null, DateTime? updatedAt = null)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        Price = price;
        Category = category;
        CreatedAt = createdAt ?? DateTime.Now;
        UpdatedAt = updatedAt ?? DateTime.Now;
    }

    public static InventoryItem Create(string name, int quantity, decimal price, string category)
    {
        var inventoryId = UUIDGenerator.Create();
        return new InventoryItem(inventoryId, name, quantity, price, category);
    }

    public static InventoryItem Restore(string id, string name, int quantity, decimal price, string category, DateTime createdAt, DateTime updatedAt)
    {
        return new InventoryItem(id, name, quantity, price, category, createdAt, updatedAt);
    }
}


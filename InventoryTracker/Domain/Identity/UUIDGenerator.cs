namespace InventoryTracker.Domain.Identity;

public class UUIDGenerator
{
    public static string Create() => Guid.NewGuid().ToString();
}


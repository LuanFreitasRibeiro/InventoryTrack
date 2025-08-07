using InventoryTracker.Application.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryTracker.Application.Usecases.Inventory;

public class DeleteItemByIdUseCase
{
    private readonly IInventoryRepository _inventoryRepository;
    public DeleteItemByIdUseCase(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }
    public async Task Execute(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            throw new ArgumentException("Item ID cannot be null or empty.", nameof(itemId));
        }
        var item = await _inventoryRepository.GetById(itemId);
        if (item == null)
        {
            throw new KeyNotFoundException($"Item with ID '{itemId}' not found.");
        }
        await _inventoryRepository.Delete(item.Id);
    }
}


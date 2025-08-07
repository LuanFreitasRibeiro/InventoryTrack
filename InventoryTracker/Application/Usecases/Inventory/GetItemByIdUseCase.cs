using InventoryTracker.Application.Repository;
using InventoryTracker.Domain.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryTracker.Application.Usecases.Inventory;

public class GetItemByIdUseCase
{
    private readonly IInventoryRepository _inventoryRepository;
    public GetItemByIdUseCase(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }
    public async Task<InventoryItem> Execute(string itemId)
    {
        var restoredItem = await _inventoryRepository.GetById(itemId);
        if (restoredItem == null)
        {
            throw new Exception($"Item with ID {itemId} not found.");
        }
        return restoredItem;
    }
}


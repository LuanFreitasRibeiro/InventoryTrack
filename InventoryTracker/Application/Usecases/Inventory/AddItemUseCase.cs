using InventoryTracker.Application.Repository;
using InventoryTracker.Domain.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryTracker.Application.Usecases.Inventory;

public class AddItemUseCase
{
    private readonly IInventoryRepository _inventoryRepository;
    public AddItemUseCase(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }
    public async Task Execute(string name, int quantity, decimal price, string category)
    {
        var item = InventoryItem.Create(name, quantity, price, category);
        await _inventoryRepository.Add(item);
    }
}


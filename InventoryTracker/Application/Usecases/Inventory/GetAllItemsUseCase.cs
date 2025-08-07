using InventoryTracker.Application.Repository;
using InventoryTracker.Domain.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryTracker.Application.Usecases.Inventory;

public class GetAllItemsUseCase
{
    private readonly IInventoryRepository _inventoryRepository;
    public GetAllItemsUseCase(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }
    public async Task<IEnumerable<InventoryItem>> Execute()
    {
        //need to implement pagination and sorting later
        var items = await _inventoryRepository.GetAll();
        return items;
    }
}

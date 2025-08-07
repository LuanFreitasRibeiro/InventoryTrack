using InventoryTracker.Domain.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryTracker.Application.Repository;

public interface IInventoryRepository
{
    Task Add(InventoryItem item);
    Task<InventoryItem> GetById(string id);
    Task<IEnumerable<InventoryItem>> GetAll();
    Task Update(InventoryItem item);
    Task Delete(string id);
}


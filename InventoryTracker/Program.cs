using InventoryTracker.Application.Repository;
using InventoryTracker.Application.Usecases.Inventory;
using InventoryTracker.Infrastructure.Database.Repositories.CsvFile;
using InventoryTracker.Infrastructure.Database.Repositories.SqlFile;
using InventoryTracker.Presentation.Console;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryTracker;

class Program
{
    static void Main()
    {
        var services = new ServiceCollection();

        // Uncomment the line below to use CSV file repository
        //services.AddSingleton<IInventoryRepository>(provider =>
        //    new CsvInventoryRepository("inventory.csv"));

        // Use SQL file repository need to ensure the database file path is correct
        services.AddSingleton<IInventoryRepository, InventoryRepository>();
        services.AddSingleton<ConsoleUI>();
        services.AddSingleton<GetAllItemsUseCase>();
        services.AddSingleton<AddItemUseCase>();
        services.AddSingleton<GetItemByIdUseCase>();
        services.AddSingleton<DeleteItemByIdUseCase>(); 
        var serviceProvider = services.BuildServiceProvider();
        var consoleUi = serviceProvider.GetRequiredService<ConsoleUI>();
        Task task = consoleUi.Run();
    }
}


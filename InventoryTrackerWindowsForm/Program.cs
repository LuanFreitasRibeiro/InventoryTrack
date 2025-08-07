using InventoryTracker.Application.Repository;
using InventoryTracker.Application.Usecases.Inventory;
using InventoryTracker.Infrastructure.Database.Repositories.CsvFile;
using InventoryTracker.Infrastructure.Database.Repositories.SqlFile;
using InventoryTrackerWindowsForm;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Forms;

namespace InventoryTracker.Presentation.WinForms
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            var services = new ServiceCollection();

            // uncomment the line below to use csv file repository
            //services.AddSingleton<IInventoryRepository>(provider =>
            //    new CsvInventoryRepository("inventory.csv"));

            // Use SQL file repository need to ensure the database file path is correct
            services.AddSingleton<IInventoryRepository, InventoryRepository>();
            services.AddSingleton<GetAllItemsUseCase>();
            services.AddSingleton<AddItemUseCase>();
            services.AddSingleton<GetItemByIdUseCase>();
            services.AddSingleton<DeleteItemByIdUseCase>();

            // Add your main form
            services.AddSingleton<MainForm>();

            var serviceProvider = services.BuildServiceProvider();

            ApplicationConfiguration.Initialize();
            System.Windows.Forms.Application.Run(serviceProvider.GetRequiredService<MainForm>());
        }
    }
}
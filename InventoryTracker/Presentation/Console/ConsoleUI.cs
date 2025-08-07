using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryTracker.Application.Usecases.Inventory;
using InventoryTracker.Domain.Inventory;

namespace InventoryTracker.Presentation.Console;

public class ConsoleUI
{
    private readonly GetAllItemsUseCase _getAllItemsUseCase;
    private readonly AddItemUseCase _addItemUseCase;
    private readonly GetItemByIdUseCase _getItemByIdUseCase;
    private readonly DeleteItemByIdUseCase _deleteItemByIdUseCase;

    public ConsoleUI(
        GetAllItemsUseCase getAllItemsUseCase,
        AddItemUseCase addItemUseCase,
        GetItemByIdUseCase getItemByIdUseCase,
        DeleteItemByIdUseCase deleteItemByIdUseCase
        )
    {
        _getAllItemsUseCase = getAllItemsUseCase;
        _addItemUseCase = addItemUseCase;
        _getItemByIdUseCase = getItemByIdUseCase;
        _deleteItemByIdUseCase = deleteItemByIdUseCase;

    }

    public async Task Run()
    {
        while (true)
        {
            ShowMenu();
            var input = ReadInput("Select an option: ");
            System.Console.WriteLine();
            switch (input)
            {
                case "1": await ListItemsAsync(); break;
                case "2": await AddItemAsync(); break;
                case "3": await ViewItemByIdAsync(); break;
                case "4": await RemoveItemByIdAsync(); break;
                case "0": System.Console.WriteLine("\nGoodbye!"); return;
                default: PrintError("Invalid option. Try again."); break;
            }
            System.Console.WriteLine();
        }
    }

    private void ShowMenu()
    {
        System.Console.WriteLine("===================================");
        System.Console.WriteLine("         INVENTORY TRACKER        ");
        System.Console.WriteLine("===================================");
        System.Console.WriteLine("1. List all items");
        System.Console.WriteLine("2. Add new item");
        System.Console.WriteLine("3. View item by ID");
        System.Console.WriteLine("4. Remove item by ID");
        System.Console.WriteLine("0. Exit");
        System.Console.WriteLine("===================================");
    }

    private static string ReadInput(string prompt)
    {
        System.Console.Write(prompt);
        return System.Console.ReadLine()?.Trim() ?? string.Empty;
    }

    private async Task ListItemsAsync()
    {
        var items = (await _getAllItemsUseCase.Execute()).OrderBy(item => item.Price).ToList();
        if (!items.Any())
        {
            PrintWarning("No items found.");
            return;
        }

        var headers = new[] { "ID", "Name", "Quantity", "Price", "Category" };

        var rows = items.Select(item => new[]
        {
            item.Id.ToString(),
            item.Name,
            item.Quantity.ToString(),
            item.Price.ToString("C"),
            item.Category
        }).ToList();

        rows.Insert(0, headers);

        int[] columnWidths = new int[headers.Length];
        for (int col = 0; col < headers.Length; col++)
        {
            columnWidths[col] = rows.Max(row => row[col].Length);
        }

        string rowFormat = "| " + string.Join(" | ", columnWidths.Select((width, i) =>
            i == 2 || i == 3 ? $"{{{i}, {width}}}" : $"{{{i}, -{width}}}")) + " |";

        string separator = new string('-', rowFormat.Length);

        System.Console.WriteLine(separator);
        System.Console.WriteLine(rowFormat, headers);
        System.Console.WriteLine(separator);

        foreach (var row in rows.Skip(1))
        {
            System.Console.WriteLine(rowFormat, row);
        }

        System.Console.WriteLine(separator);
    }

    private async Task AddItemAsync()
    {
        var name = ReadInput("Enter item name: ");
        if (string.IsNullOrWhiteSpace(name))
        {
            PrintError("Name cannot be empty.");
            return;
        }

        if (!int.TryParse(ReadInput("Enter quantity: "), out var quantity) || quantity < 0)
        {
            PrintError("Invalid quantity.");
            return;
        }

        if (!decimal.TryParse(ReadInput("Enter price: "), out var price) || price < 0)
        {
            PrintError("Invalid price.");
            return;
        }

        var category = ReadInput("Enter category (optional): ");
        await _addItemUseCase.Execute(name, quantity, price, category ?? string.Empty);
        PrintSuccess("Item added successfully.");
    }

    private async Task RemoveItemByIdAsync()
    {
        var id = ReadInput("Enter item ID: ");
        if (string.IsNullOrWhiteSpace(id))
        {
            PrintError("ID cannot be empty.");
            return;
        }

        var item = await _getItemByIdUseCase.Execute(id);
        if (item == null)
        {
            PrintWarning("Item not found.");
            return;
        }

        await _deleteItemByIdUseCase.Execute(id);
        PrintSuccess("Item removed successfully.");
    }

    private async Task ViewItemByIdAsync()
    {
        var id = ReadInput("Enter item ID: ");
        if (string.IsNullOrWhiteSpace(id))
        {
            PrintError("ID cannot be empty.");
            return;
        }

        var item = await _getItemByIdUseCase.Execute(id);
        if (item == null)
        {
            PrintWarning("Item not found.");
            return;
        }

        PrintItemDetails(item);
    }

    private static void PrintItemDetails(InventoryItem item)
    {
        System.Console.WriteLine("===================================");
        System.Console.WriteLine("           ITEM DETAILS           ");
        System.Console.WriteLine("===================================");
        System.Console.WriteLine($"ID        : {item.Id}");
        System.Console.WriteLine($"Name      : {item.Name}");
        System.Console.WriteLine($"Quantity  : {item.Quantity}");
        System.Console.WriteLine($"Price     : {item.Price:C}");
        System.Console.WriteLine($"Category  : {item.Category}");
        System.Console.WriteLine($"Created   : {item.CreatedAt:g}");
        System.Console.WriteLine($"Updated   : {item.UpdatedAt:g}");
        System.Console.WriteLine("===================================");
    }

    private static void PrintError(string message)
    {
        var prevColor = System.Console.ForegroundColor;
        System.Console.ForegroundColor = ConsoleColor.Red;
        System.Console.WriteLine($"[ERROR] {message}");
        System.Console.ForegroundColor = prevColor;
    }

    private static void PrintWarning(string message)
    {
        var prevColor = System.Console.ForegroundColor;
        System.Console.ForegroundColor = ConsoleColor.Yellow;
        System.Console.WriteLine($"[WARNING] {message}");
        System.Console.ForegroundColor = prevColor;
    }

    private static void PrintSuccess(string message)
    {
        var prevColor = System.Console.ForegroundColor;
        System.Console.ForegroundColor = ConsoleColor.Green;
        System.Console.WriteLine($"[SUCCESS] {message}");
        System.Console.ForegroundColor = prevColor;
    }
}


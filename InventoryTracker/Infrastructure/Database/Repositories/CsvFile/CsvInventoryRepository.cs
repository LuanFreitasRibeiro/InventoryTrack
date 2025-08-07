using InventoryTracker.Application.Repository;
using InventoryTracker.Domain.Inventory;
using System.Globalization;
using System.Text;

namespace InventoryTracker.Infrastructure.Database.Repositories.CsvFile;

public class CsvInventoryRepository : IInventoryRepository
{
    private readonly string _filePath;
    private readonly object _lock = new();

    public CsvInventoryRepository(string filePath)
    {
        _filePath = filePath;
        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "Id,Name,Quantity,Price,Category,CreatedAt,UpdatedAt\n");
        }
    }

    public async Task Add(InventoryItem item)
    {
        var line = ToCsv(item);
        lock (_lock)
        {
            File.AppendAllText(_filePath, line + "\n");
        }
        await Task.CompletedTask;
    }

    public async Task<InventoryItem> GetById(string id)
    {
        var items = await GetAll();
        return items.FirstOrDefault(i => i.Id == id);
    }

    public async Task<IEnumerable<InventoryItem>> GetAll()
    {
        var lines = File.ReadLines(_filePath).Skip(1);
        var items = new List<InventoryItem>();
        foreach (var line in lines)
        {
            var parts = line.Split(',');
            if (parts.Length < 7) continue;
            items.Add(InventoryItem.Restore(
                parts[0],
                parts[1].Trim('"'),
                int.Parse(parts[2]),
                decimal.Parse(parts[3], CultureInfo.InvariantCulture),
                parts[4].Trim('"'),
                DateTime.Parse(parts[5], null, DateTimeStyles.RoundtripKind),
                DateTime.Parse(parts[6], null, DateTimeStyles.RoundtripKind)
            ));
        }
        return items;
    }

    public async Task Update(InventoryItem item)
    {
        var items = (await GetAll()).ToList();
        var index = items.FindIndex(i => i.Id == item.Id);
        if (index >= 0)
        {
            items[index] = item;
            WriteAll(items);
        }
        await Task.CompletedTask;
    }

    public async Task Delete(string id)
    {
        var items = (await GetAll()).Where(i => i.Id != id).ToList();
        WriteAll(items);
        await Task.CompletedTask;
    }

    private void WriteAll(IEnumerable<InventoryItem> items)
    {
        var lines = new List<string> { "Id,Name,Quantity,Price,Category,CreatedAt,UpdatedAt" };
        lines.AddRange(items.Select(ToCsv));
        lock (_lock)
        {
            File.WriteAllLines(_filePath, lines);
        }
    }

    private static string ToCsv(InventoryItem item)
    {
        return string.Join(",",
            item.Id,
            Escape(item.Name),
            item.Quantity,
            item.Price.ToString(CultureInfo.InvariantCulture),
            Escape(item.Category),
            item.CreatedAt.ToString("o"),
            item.UpdatedAt.ToString("o")
        );
    }

    private static InventoryItem FromCsv(string line)
    {
        var parts = SplitCsv(line);
        return InventoryItem.Restore(
            parts[0],
            parts[1],
            int.Parse(parts[2]),
            decimal.Parse(parts[3], CultureInfo.InvariantCulture),
            parts[4],
            DateTime.Parse(parts[5], null, DateTimeStyles.RoundtripKind),
            DateTime.Parse(parts[6], null, DateTimeStyles.RoundtripKind)
        );
    }

    private static string Escape(string value) =>
        value.Contains(',') ? $"\"{value.Replace("\"", "\"\"")}\"" : value;

    private static string[] SplitCsv(string line)
    {
        var result = new List<string>();
        var inQuotes = false;
        var value = new StringBuilder();
        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] == '"' && (i == 0 || line[i - 1] != '\\'))
                inQuotes = !inQuotes;
            else if (line[i] == ',' && !inQuotes)
            {
                result.Add(value.ToString().Replace("\"\"", "\""));
                value.Clear();
            }
            else
                value.Append(line[i]);
        }
        result.Add(value.ToString().Replace("\"\"", "\""));
        return result.ToArray();
    }
}
using InventoryTracker.Application.Repository;
using InventoryTracker.Domain.Inventory;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InventoryTracker.Infrastructure.Database.Repositories.SqlFile;

public class InventoryRepository : IInventoryRepository
{
    private readonly string _connectionString =
        @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\luann\Documents\inventory.mdf;Integrated Security=True;Connect Timeout=30";

    public InventoryRepository()
    {
        EnsureTableExists().GetAwaiter().GetResult();
    }

    private async Task EnsureTableExists()
    {
        const string createTableSql = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='InventoryItems' AND xtype='U')
            CREATE TABLE InventoryItems (
                Id NVARCHAR(50) NOT NULL PRIMARY KEY,
                Name NVARCHAR(100) NOT NULL,
                Quantity INT NOT NULL,
                Price DECIMAL(18,2) NOT NULL,
                Category NVARCHAR(100) NOT NULL,
                CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
                UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
            )";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(createTableSql, connection);
        try
        {
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EnsureTableExists] Error: {ex.Message}");
            throw;
        }
    }

    public async Task Add(InventoryItem item)
    {
        const string sql = @"
            INSERT INTO InventoryItems (Id, Name, Quantity, Price, Category, CreatedAt, UpdatedAt)
            VALUES (@Id, @Name, @Quantity, @Price, @Category, @CreatedAt, @UpdatedAt)";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);

        command.Parameters.AddWithValue("@Id", item.Id);
        command.Parameters.AddWithValue("@Name", item.Name);
        command.Parameters.AddWithValue("@Quantity", item.Quantity);
        command.Parameters.AddWithValue("@Price", item.Price);
        command.Parameters.AddWithValue("@Category", item.Category);
        command.Parameters.AddWithValue("@CreatedAt", item.CreatedAt);
        command.Parameters.AddWithValue("@UpdatedAt", item.UpdatedAt);

        try
        {
            await connection.OpenAsync();
            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Add] Error: {ex.Message}");
            throw;
        }
    }

    public async Task<InventoryItem?> GetById(string id)
    {
        const string sql = @"
            SELECT Id, Name, Quantity, Price, Category, CreatedAt, UpdatedAt
            FROM InventoryItems
            WHERE Id = @Id";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        try
        {
            await connection.OpenAsync();
            var reader = command.ExecuteReader();

            if (await reader.ReadAsync())
            {
                return InventoryItem.Restore(
                    reader.GetString(0),
                    reader.GetString(1),
                    reader.GetInt32(2),
                    reader.GetDecimal(3),
                    reader.GetString(4),
                    reader.GetDateTime(5),
                    reader.GetDateTime(6)
                );
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GetById] Error: {ex.Message}");
            throw;
        }
    }

    public async Task<IEnumerable<InventoryItem>> GetAll()
    {
        const string sql = @"
            SELECT Id, Name, Quantity, Price, Category, CreatedAt, UpdatedAt
            FROM InventoryItems";

        var items = new List<InventoryItem>();

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);

        try
        {
            await connection.OpenAsync();
            var reader = command.ExecuteReader();

            while (await reader.ReadAsync())
            {
                items.Add(InventoryItem.Restore(
                    reader.GetString(0),
                    reader.GetString(1),
                    reader.GetInt32(2),
                    reader.GetDecimal(3),
                    reader.GetString(4),
                    reader.GetDateTime(5),
                    reader.GetDateTime(6)
                ));
            }

            return items;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GetAll] Error: {ex.Message}");
            throw;
        }
    }

    public async Task Update(InventoryItem item)
    {
        const string sql = @"
            UPDATE InventoryItems
            SET Name = @Name,
                Quantity = @Quantity,
                Price = @Price,
                Category = @Category,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);

        command.Parameters.AddWithValue("@Id", item.Id);
        command.Parameters.AddWithValue("@Name", item.Name);
        command.Parameters.AddWithValue("@Quantity", item.Quantity);
        command.Parameters.AddWithValue("@Price", item.Price);
        command.Parameters.AddWithValue("@Category", item.Category);
        command.Parameters.AddWithValue("@UpdatedAt", item.UpdatedAt);

        try
        {
            await connection.OpenAsync();
            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Update] Error: {ex.Message}");
            throw;
        }
    }

    public async Task Delete(string id)
    {
        const string sql = "DELETE FROM InventoryItems WHERE Id = @Id";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);

        command.Parameters.AddWithValue("@Id", id);

        try
        {
            await connection.OpenAsync();
            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Delete] Error: {ex.Message}");
            throw;
        }
    }
}

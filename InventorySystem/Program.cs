using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// b. Marker Interface
public interface IInventoryEntity
{
    int Id { get; }
}

// a. Immutable Inventory Record
public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

// c. Generic Inventory Logger
public class InventoryLogger<T> where T : IInventoryEntity
{
    private List<T> _log = new();
    private string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Add(T item)
    {
        _log.Add(item);
    }

    public List<T> GetAll()
    {
        return new List<T>(_log);
    }

    public void SaveToFile()
    {
        try
        {
            string jsonData = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
            using (StreamWriter writer = new StreamWriter(_filePath))
            {
                writer.Write(jsonData);
            }
            Console.WriteLine("Data saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving to file: {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine("File not found. No data loaded.");
                return;
            }

            using (StreamReader reader = new StreamReader(_filePath))
            {
                string jsonData = reader.ReadToEnd();
                var items = JsonSerializer.Deserialize<List<T>>(jsonData);
                _log = items ?? new List<T>();
            }
            Console.WriteLine("Data loaded successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading from file: {ex.Message}");
        }
    }
}

// f. InventoryApp
public class InventoryApp
{
    private InventoryLogger<InventoryItem> _logger;

    public InventoryApp(string filePath)
    {
        _logger = new InventoryLogger<InventoryItem>(filePath);
    }

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Laptop", 10, DateTime.Now));
        _logger.Add(new InventoryItem(2, "Mouse", 50, DateTime.Now));
        _logger.Add(new InventoryItem(3, "Keyboard", 30, DateTime.Now));
        _logger.Add(new InventoryItem(4, "Monitor", 15, DateTime.Now));
    }

    public void SaveData()
    {
        _logger.SaveToFile();
    }

    public void LoadData()
    {
        _logger.LoadFromFile();
    }

    public void PrintAllItems()
    {
        var items = _logger.GetAll();
        foreach (var item in items)
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Qty: {item.Quantity}, Added: {item.DateAdded}");
        }
    }
}

// g. Main Application Flow
class Program
{
    static void Main()
    {
        string filePath = "inventory.json";

        // First session
        var app = new InventoryApp(filePath);
        app.SeedSampleData();
        app.SaveData();

        // Simulate a new session (fresh memory)
        Console.WriteLine("\n--- New Session ---\n");
        var newApp = new InventoryApp(filePath);
        newApp.LoadData();
        newApp.PrintAllItems();
    }
}


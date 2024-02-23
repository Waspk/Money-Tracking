// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

List<Item> items = new();

LoadItems();

while (true)
{
    Console.WriteLine(">> Welcome to TrackMoney");
    Console.WriteLine($">> You have currently {CalculateBalance()} kr on your account.");
    Console.WriteLine(">> Pick an option:");
    Console.WriteLine(">> (1) Show items (Alt/Expense(s)/Income(s))");
    Console.WriteLine(">> (2) Add New Expense/Income");
    Console.WriteLine(">> (3) Edit Item (edit, remove)");
    Console.WriteLine(">> (4) Save and Quit");

    string choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            ShowItems();
            break;
        case "2":
            AddNewItem();
            break;
        case "3":
            EditOrRemoveItem();
            break;
        case "4":
            SaveItems();
            Environment.Exit(0);
            break;
        default:
            Console.WriteLine("Invalid option. Please try again.");
            break;
    }
}

void ShowItems()
{
    Console.WriteLine(">> Display (1) All (2) Expenses (3) Incomes");
    string displayChoice = Console.ReadLine();

    IEnumerable<Item> filteredItems;

    switch (displayChoice)
    {
        case "1":
            filteredItems = items;
            break;
        case "2":
            filteredItems = items.Where(item => item.Amount < 0);
            break;
        case "3":
            filteredItems = items.Where(item => item.Amount > 0);
            break;
        default:
            Console.WriteLine("Invalid option. Showing all items.");
            filteredItems = items;
            break;
    }

    Console.WriteLine(">> Display (1) Ascending (2) Descending");
    string sortOrderChoice = Console.ReadLine();

    switch (sortOrderChoice)
    {
        case "1":
            filteredItems = filteredItems.OrderBy(item => item.Month).ThenBy(item => item.Amount).ThenBy(item => item.Title);
            break;
        case "2":
            filteredItems = filteredItems.OrderByDescending(item => item.Month).ThenByDescending(item => item.Amount).ThenByDescending(item => item.Title);
            break;
        default:
            Console.WriteLine("Invalid option. Sorting in ascending order.");
            filteredItems = filteredItems.OrderBy(item => item.Month).ThenBy(item => item.Amount).ThenBy(item => item.Title);
            break;
    }

    Console.WriteLine(">> Displaying items:");
    foreach (var item in filteredItems)
    {
        Console.WriteLine($"{item.Month} - {item.Title}: {item.Amount} kr");
    }
}

void AddNewItem()
{
    Console.WriteLine(">> Enter Title:");
    string title = Console.ReadLine();

    Console.WriteLine(">> Enter Amount:");
    if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
    {
        Console.WriteLine("Invalid amount. Please enter a valid number.");
        return;
    }

    Console.WriteLine(">> Enter Month:");
    if (!int.TryParse(Console.ReadLine(), out int month))
    {
        Console.WriteLine("Invalid month. Please enter a valid number.");
        return;
    }

    items.Add(new Item { Title = title, Amount = amount, Month = month });
    Console.WriteLine("Item added successfully.");
}

void EditOrRemoveItem()
{
    Console.WriteLine(">> Enter the title of the item you want to edit/remove:");
    string titleToEdit = Console.ReadLine();

    Item itemToEdit = items.FirstOrDefault(item => item.Title.Equals(titleToEdit, StringComparison.OrdinalIgnoreCase));

    if (itemToEdit == null)
    {
        Console.WriteLine("Item not found.");
        return;
    }

    Console.WriteLine($"Editing item: {itemToEdit.Month} - {itemToEdit.Title}: {itemToEdit.Amount} kr");

    Console.WriteLine(">> (1) Edit (2) Remove");
    string editOrRemoveChoice = Console.ReadLine();

    switch (editOrRemoveChoice)
    {
        case "1":
            EditItem(itemToEdit);
            break;
        case "2":
            items.Remove(itemToEdit);
            Console.WriteLine("Item removed successfully.");
            break;
        default:
            Console.WriteLine("Invalid option.");
            break;
    }
}

void EditItem(Item itemToEdit)
{
    Console.WriteLine(">> Enter new Title (or press Enter to keep the current title):");
    string newTitle = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(newTitle))
    {
        itemToEdit.Title = newTitle;
    }

    Console.WriteLine(">> Enter new Amount (or press Enter to keep the current amount):");
    string newAmountInput = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(newAmountInput) && decimal.TryParse(newAmountInput, out decimal newAmount))
    {
        itemToEdit.Amount = newAmount;
    }

    Console.WriteLine(">> Enter new Month (or press Enter to keep the current month):");
    string newMonthInput = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(newMonthInput) && int.TryParse(newMonthInput, out int newMonth))
    {
        itemToEdit.Month = newMonth;
    }

    Console.WriteLine("Item edited successfully.");
}

void SaveItems()
{
    using (StreamWriter writer = new StreamWriter("items.txt"))
    {
        foreach (var item in items)
        {
            writer.WriteLine($"{item.Month},{item.Title},{item.Amount}");
        }
    }
    Console.WriteLine("Items saved to file.");
}

void LoadItems()
{
    if (File.Exists("items.txt"))
    {
        using (StreamReader reader = new StreamReader("items.txt"))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                if (values.Length == 3 && int.TryParse(values[0], out int month) && decimal.TryParse(values[2], out decimal amount))
                {
                    items.Add(new Item { Month = month, Title = values[1], Amount = amount });
                }
            }
        }
        Console.WriteLine("Items loaded from file.");
    }
}

decimal CalculateBalance()
{
    return items.Sum(item => item.Amount);
}

class Item
{
    public required string Title { get; set; }
    public decimal Amount { get; set; }
    public int Month { get; set; }
}

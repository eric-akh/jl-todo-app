using TodoApi.Models;

namespace TodoApi.Services;

public class TodoService
{
    private readonly List<TodoItem> _items = new();
    private int _nextId = 1;

    public IEnumerable<TodoItem> GetAll() => _items;

    public TodoItem Add(string title)
    {
        var item = new TodoItem
        {
            Id = _nextId++,
            Title = title,
            IsDone = false
        };

        _items.Add(item);
        return item;
    }

    public bool Delete(int id)
    {
        var item = _items.FirstOrDefault(x => x.Id == id);
        if (item == null) return false;
        _items.Remove(item);
        return true;
    }
}
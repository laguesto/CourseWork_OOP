using SportsLeague.Core.Interfaces;
using SportsLeague.Core.Models;

namespace SportsLeague.Core.Repositories;

public class Repository<T> : IRepository<T> where T : class, IEntity
{
    private readonly List<T> _items;

    public Repository(IEnumerable<T>? initialItems = null)
    {
        _items = initialItems?.ToList() ?? new List<T>();
    }

    public void Add(T item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        if (GetById(item.Id) != null)
            throw new InvalidOperationException("Запис із таким ідентифікатором уже існує.");

        _items.Add(item);
    }

    public bool Remove(Guid id)
    {
        T? item = GetById(id);
        return item != null && _items.Remove(item);
    }

    public T? GetById(Guid id)
    {
        return _items.FirstOrDefault(item => item.Id == id);
    }

    public List<T> GetAll()
    {
        return new List<T>(_items);
    }

    public List<T> Find(Predicate<T> condition)
    {
        return _items.FindAll(condition);
    }
}

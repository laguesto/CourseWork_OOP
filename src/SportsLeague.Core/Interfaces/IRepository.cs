using SportsLeague.Core.Models;

namespace SportsLeague.Core.Interfaces;

public interface IRepository<T> where T : class, IEntity
{
    void Add(T item);
    bool Remove(Guid id);
    T? GetById(Guid id);
    List<T> GetAll();
    List<T> Find(Predicate<T> condition);
}

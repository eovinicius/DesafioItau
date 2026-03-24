namespace CatalogoItau.Application.Abstractions.Data;

public interface IUnitOfWork : IDisposable
{
    void BeginTransaction();
    void Commit();
    void Rollback();
}
namespace CatalogoItau.Infrastructure.Percistence.Data;

using CatalogoItau.Application.Abstractions.Data;
using CatalogoItau.Infrastructure.Percistence;

using Microsoft.EntityFrameworkCore.Storage;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public void BeginTransaction()
    {
        _transaction ??= _context.Database.BeginTransaction();
    }

    public void Commit()
    {
        _context.SaveChanges();
        _transaction?.Commit();
        Dispose();
    }

    public void Rollback()
    {
        _transaction?.Rollback();
        Dispose();
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _transaction = null;
    }
}
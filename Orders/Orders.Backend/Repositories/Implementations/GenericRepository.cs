using Microsoft.EntityFrameworkCore;
using Orders.Backend.Data;
using Orders.Backend.Repositories.Interfaces;
using Orders.Shared.Responses;

namespace Orders.Backend.Repositories.Implementations;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly DataContext _context;
    private readonly DbSet<T> _entity;

    public GenericRepository(DataContext context)
    {
        _context = context;
        _entity = context.Set<T>();
    }

    public virtual async Task<ActionResponse<T>> AddAsync(T entity)
    {
        _context.Add(entity);

        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<T> { WasSucces = true, Result = entity };
        }
        catch (DbUpdateException)
        {
            return DBUpdateExceptionActionResponse();
        }
        catch (Exception exception)
        {
            return ExceptionActionResponse(exception);
        }
    }

    public virtual async Task<ActionResponse<T>> DeleteAsync(int id)
    {
        var row = await _entity.FindAsync(id);

        if (row is null)
        {
            return new ActionResponse<T> { WasSucces = false, Message = "Registro no encontrado." };
        }

        _entity.Remove(row);

        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<T> { WasSucces = true };
        }
        catch (Exception ex)
        {
            return new ActionResponse<T> { WasSucces = false, Message = "El elemento cuenta con registros relacionados. No se puede eliminar." };
        }
    }

    public virtual async Task<ActionResponse<T>> GetAsync(int id)
    {
        var row = await _entity.FindAsync(id);
        if (row is null) return new ActionResponse<T> { WasSucces = false, Message = "Registro no encontrado." };
        return new ActionResponse<T> { WasSucces = true, Result = row };
    }

    public virtual async Task<ActionResponse<IEnumerable<T>>> GetAsync() => new ActionResponse<IEnumerable<T>> { WasSucces = true, Result = await _entity.ToListAsync() };

    public virtual async Task<ActionResponse<T>> UpdateAsync(T entity)
    {
        _context.Update(entity);

        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<T> { WasSucces = true, Result = entity };
        }
        catch (DbUpdateException)
        {
            return DBUpdateExceptionActionResponse();
        }
        catch (Exception exception)
        {
            return ExceptionActionResponse(exception);
        }
    }

    private ActionResponse<T> ExceptionActionResponse(Exception exception) => new ActionResponse<T> { Message = exception.Message };

    private ActionResponse<T> DBUpdateExceptionActionResponse() => new ActionResponse<T> { Message = "Ya existe el registro en la base de datos." };
}
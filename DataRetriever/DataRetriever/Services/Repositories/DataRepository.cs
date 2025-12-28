using DataRetriever.Services.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataRetriever.Data.Repositories;

public interface IDataRepository
{
    Task<DataItem?> GetAsync(string id);
    Task<string> AddAsync(string id, string value);
}

public class SqlDataRepository : IDataRepository
{
    private readonly AppDbContext _dbContext;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public SqlDataRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DataItem?> GetAsync(string id)
    {
        return await _dbContext.DataItems.FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<string> AddAsync(string id, string value)
    {
        await _semaphore.WaitAsync();
        try
        {
            var entity = new DataItem { Id = id, Value = value };
            _dbContext.DataItems.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}

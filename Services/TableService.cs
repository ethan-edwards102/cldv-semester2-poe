using Azure;
using Azure.Data.Tables;

namespace ABCRetails.Services;

public class TableService
{
    private readonly TableServiceClient _ts;

    public TableService(IConfiguration configuration)
    {
        var connectionString = configuration["ConnectionStrings:StorageAccount"];
        
        _ts = new TableServiceClient(connectionString);
    }

    public async Task<T?> GetEntityAsync<T>(string table, string partKey, string rowKey) where T : class, ITableEntity
    {
        try
        {
            var tableClient = _ts.GetTableClient(table);
            
            var entity = await tableClient.GetEntityAsync<T>(partKey, rowKey);
            
            return entity;
        }
        catch (RequestFailedException ex)
        {
            return null;
        }
    }
    
    public async Task UpdateEntityAsync<T>(string table, T entity) where T : class, ITableEntity
    {
        var tableClient = _ts.GetTableClient(table);
        await tableClient.UpdateEntityAsync(entity, ETag.All, TableUpdateMode.Replace);
    }

    public async Task InsertEntityAsync(string table, ITableEntity entity)
    {
        var tableClient = _ts.GetTableClient(table);
        await tableClient.UpsertEntityAsync(entity);
    }
    
    public async Task RemoveEntityAsync(string table, string partKey, string rowKey)
    {
        var tableClient = _ts.GetTableClient(table);
        await tableClient.DeleteEntityAsync(partKey, rowKey);
    }

    public async Task<List<T>> ToListAsync<T>(string table) where T : class, ITableEntity
    {
        // Ensure table exists
        await CreateTableAsync(table);
        
        var tableClient = _ts.GetTableClient(table);
        var results = tableClient.QueryAsync<T>();
        var entities = new List<T>();

        await foreach (var e in results)
        {
            entities.Add(e);
        }
        
        return entities;
    }

    private async Task CreateTableAsync(string table)
    {
        await _ts.CreateTableIfNotExistsAsync(table);
    }
}
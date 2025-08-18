using System.ComponentModel.DataAnnotations;
using Azure;
using Azure.Data.Tables;

namespace ABCRetails.Models;

public class Product: ITableEntity
{
    public string PartitionKey { get; set; } = "PRODUCT";
    public string RowKey { get; set; } = Guid.NewGuid().ToString();
    
    // Azure Table fields
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    
    [Required] public string ProductName { get; set; }
    [Required] public decimal Price { get; set; }
    [Required] public string Description { get; set; }
    
    public string ProductPhotoURL { get; set; }
}
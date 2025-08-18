using System.ComponentModel.DataAnnotations;
using Azure;
using Azure.Data.Tables;

namespace ABCRetails.Models;

public class Order: ITableEntity
{
    public string PartitionKey { get; set; } = "ORDER";
    public string RowKey { get; set; } = Guid.NewGuid().ToString();
    
    // Azure Table fields
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    
    [Required] public int Quantity { get; set; }
    [Required] public decimal TotalPrice { get; set; }

}
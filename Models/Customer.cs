using System.ComponentModel.DataAnnotations;
using Azure;
using Azure.Data.Tables;

namespace ABCRetails.Models;

public class Customer : ITableEntity
{
    public string PartitionKey { get; set; } = "CUSTOMER";
    public string RowKey { get; set; } = Guid.NewGuid().ToString();

    // Azure Table fields
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    [Required] public string CustomerName { get; set; }
    [Required] public string EmailAddress { get; set; }
    [Required] public string PhoneNumber { get; set; }
    [Required] public string DeliveryAddress { get; set; }
}


using Azure;

namespace ABCRetails.Models;

public class EditOrderViewModel
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }

    // Azure Table fields
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    // When editing, product and customer details
    // can either be kept the same or changed
    public string? ProductRowKey { get; set; }
    public string? ProductName { get; set; }
    public string? PhotoURL { get; set; }

    public string? CustomerRowKey { get; set; }
    public string? CustomerName { get; set; }
    public string? DeliveryAddress { get; set; }
    
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}
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
    
    [Required] [Display(Name = "Product")]
    public string ProductName { get; set; }
    
    [Required] [Display(Name = "Product Photo")]
    public string PhotoURL { get; set; }
    
    [Required] [Display(Name = "Quantity")]
    public int Quantity { get; set; }
    
    [Required] [Display(Name = "Total Price")]
    public double TotalPrice { get; set; }
    
    [Required] [Display(Name = "Customer")]
    public string CustomerName { get; set; }
    
    [Required] [Display(Name = "Delivery Address")]
    public string DeliveryAddress { get; set; }
}
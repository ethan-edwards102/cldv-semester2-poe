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

    [Required] [Display(Name = "Customer Name")]
    public string CustomerName { get; set; }
    
    [Required] [Display(Name = "Email Address")]
    public string EmailAddress { get; set; }
    
    [Required] [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; }
    
    [Required] [Display(Name = "Delivery Address")]
    public string DeliveryAddress { get; set; }
}


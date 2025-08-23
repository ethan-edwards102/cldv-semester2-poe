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
    
    [Required] [Display(Name = "Product Name")]
    public string ProductName { get; set; }
    
    [Required] [Display(Name = "Price")]
    public double Price { get; set; }
    
    [Required] [Display(Name = "Description")]
    public string Description { get; set; }
    
    [Required] [Display(Name = "Product Photo")]
    public string PhotoURL { get; set; }
}
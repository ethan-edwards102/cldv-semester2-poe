using Azure;

namespace ABCRetails.Models;

public class EditProductViewModel
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    
    // Azure Table fields
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    
    public string ProductName { get; set; }
    public double Price { get; set; }
    public string Description { get; set; }
    
    public string PhotoURL { get; set; }
    public IFormFile? ImageFile { get; set; }
}
namespace ABCRetails.Models;

public class CreateProductViewModel
{
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    
    public IFormFile ImageFile { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace ABCRetails.Models;

public class CreateProductViewModel
{
    [Required] [Display(Name = "Product Name")]
    public string ProductName { get; set; }
    
    [Required] [Display(Name = "Price")]
    public double Price { get; set; }
    
    [Required] [Display(Name = "Description")]
    public string Description { get; set; }
    
    [Required] [Display(Name = "Product Photo")]
    public IFormFile ImageFile { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace ABCRetails.Models;

public class CreateOrderViewModel
{
    [Required] [Display(Name = "Customer")]
    public string CustomerRowKey { get; set; }
    
    [Required] [Display(Name = "Product")]
    public string ProductRowKey { get; set; }
    
    [Required] [Display(Name = "Quantity")]
    public int Quantity { get; set; }
}
namespace ABCRetails.Models;

public class CreateOrderViewModel
{
    public string CustomerRowKey { get; set; }
    public string ProductRowKey { get; set; }
    public int Quantity { get; set; }
}
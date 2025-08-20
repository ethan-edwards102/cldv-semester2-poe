using Azure;

namespace ABCRetails.Models;

public class OrderViewModel
{
    public string CustomerRowKey { get; set; }
    public string ProductRowKey { get; set; }
    public int Quantity { get; set; }
}
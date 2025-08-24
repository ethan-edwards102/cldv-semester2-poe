using ABCRetails.Models;
using ABCRetails.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;

namespace ABCRetails.Controllers
{
    public class OrderController : Controller
    {
        private readonly TableService _ts;
        private readonly QueueService _qs;

        public OrderController(TableService ts, QueueService qs)
        {
            _ts = ts;
            _qs = qs;
        }
        
        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var viewModel = new OrderIndexViewModel
            {
                Orders = await _ts.ToListAsync<Order>("orders"),
                OrderMessages = await _qs.PeekMessagesAsync("orders", 10)
            };
            
            return View(viewModel);
        }
        
        // GET: Orders/Details
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _ts.GetEntityAsync<Order>("orders", "ORDER", id);
            
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
        
        // GET: Orders/Create
        public async Task<IActionResult> Create()
        {
            await PopulateViewData();
            
            return View();
        }
        
        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create
        (
            [Bind("Quantity, CustomerRowKey, ProductRowKey")]
            CreateOrderViewModel viewModel
        )
        {
            if (ModelState.IsValid)
            {
                var customer = await _ts.GetEntityAsync<Customer>
                (
                    "customers",
                    "CUSTOMER",
                    viewModel.CustomerRowKey
                );
                
                var product = await _ts.GetEntityAsync<Product>
                (
                    "products",
                    "PRODUCT",
                    viewModel.ProductRowKey
                );

                if (customer == null || product == null)
                {
                    return NotFound();
                }

                var order = new Order();

                order.CustomerName = customer.CustomerName;
                order.DeliveryAddress = customer.DeliveryAddress;
                
                order.ProductName = product.ProductName;
                order.PhotoURL = product.PhotoURL;
                order.TotalPrice = product.Price * viewModel.Quantity;
                
                order.Quantity = viewModel.Quantity;
                
                await _ts.InsertEntityAsync("orders", order);
                await _qs.PushMessageAsync("orders", $"Order created for {order.Quantity} {order.ProductName}(s)");
                
                return RedirectToAction(nameof(Index));
            }
            
            await PopulateViewData();

            return View(viewModel);
        }

        // GET: Orders/Edit/
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _ts.GetEntityAsync<Order>("orders", "ORDER", id);

            if (order == null)
            {
                return NotFound();
            }
            
            // Convert the Order to an OrderViewModel
            var viewModel = new EditOrderViewModel
            {
                PartitionKey = order.PartitionKey,
                RowKey = order.RowKey,
                Timestamp = order.Timestamp,
                ETag = order.ETag,
                ProductName = order.ProductName,
                PhotoURL = order.PhotoURL,
                CustomerName = order.CustomerName,
                DeliveryAddress = order.DeliveryAddress,
                Quantity = order.Quantity,
                TotalPrice = order.TotalPrice
            };

            await PopulateViewData(true);
            
            return View(viewModel);
        }

        // POST: Orders/Edit/
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit
        (
            string id,
            
            [Bind("RowKey, PartitionKey, ETag, Timestamp, ProductName, PhotoURL, CustomerName, DeliveryAddress, TotalPrice, Quantity, CustomerRowKey, ProductRowKey")]
            EditOrderViewModel viewModel
        )
        {
            if (id != viewModel.RowKey)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var order = new Order
                    {
                        RowKey = viewModel.RowKey,
                        PartitionKey = viewModel.PartitionKey,
                        ETag = viewModel.ETag,
                        TotalPrice = viewModel.TotalPrice,
                        
                        Quantity = viewModel.Quantity
                    };
                    
                    // If a new Customer or Product is selected, retrieve the details,
                    // otherwise use the existing choices
                    if (!viewModel.CustomerRowKey.IsNullOrEmpty())
                    {
                        var customer = await _ts.GetEntityAsync<Customer>
                        (
                            "customers",
                            "CUSTOMER",
                            viewModel.CustomerRowKey
                        );

                        if (customer == null)
                        {
                            return NotFound();
                        }
                        
                        order.CustomerName = customer.CustomerName;
                        order.DeliveryAddress = customer.DeliveryAddress;
                    }
                    else
                    {
                        order.CustomerName = viewModel.CustomerName;
                        order.DeliveryAddress = viewModel.DeliveryAddress;
                    }
                    
                    if (!viewModel.ProductRowKey.IsNullOrEmpty())
                    {
                        var product = await _ts.GetEntityAsync<Product>
                        (
                            "products",
                            "PRODUCT",
                            viewModel.ProductRowKey
                        );
                        
                        if (product == null)
                        {
                            return NotFound();
                        }
                        
                        order.ProductName = product.ProductName;
                        order.PhotoURL = product.PhotoURL;
                        order.TotalPrice = product.Price * viewModel.Quantity;
                    }
                    else
                    {
                        order.ProductName = viewModel.ProductName;
                        order.PhotoURL = viewModel.PhotoURL;
                        order.TotalPrice = viewModel.Quantity;
                    }
                    
                    await _ts.UpdateEntityAsync<Order>("orders", order);
                }
                catch (Exception e)
                {
                    if (await _ts.GetEntityAsync<Order>("orders", "ORDER", id) == null)
                    {
                        return NotFound();
                    }

                    throw;
                }
                
                return RedirectToAction(nameof(Index));
            }

            await PopulateViewData(true);
            
            return View(viewModel);
        }

        // GET: Orders/Delete/
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _ts.GetEntityAsync<Order>("orders", "ORDER", id);
            
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var order = await _ts.GetEntityAsync<Order>("orders", "ORDER", id);
            
            if (order != null)
            {
                await _ts.RemoveEntityAsync("orders", "ORDER", id);
            }
            
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateViewData(bool nullOption = false)
        {
            var customers = await _ts.ToListAsync<Customer>("customers");
            var products = await _ts.ToListAsync<Product>("products");

            // Allow for an empty option, useful for
            // Edit operations - where the field may be unchanged
            if (nullOption)
            {
                customers.Add
                (
                    new Customer
                    {
                        RowKey = "",
                        CustomerName = "---",
                    }
                );
                
                products.Add
                (
                    new Product
                    {
                        RowKey = "",
                        ProductName = "---",
                    }
                );
            }
            
            ViewData["CustomerRowKey"] = new SelectList(customers, "RowKey", "CustomerName", "");
            ViewData["ProductRowKey"] = new SelectList(products, "RowKey", "ProductName", "");
        }
        
        // POST: Orders/Dispatch
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dispatch(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _ts.GetEntityAsync<Order>("orders", "ORDER", id);
            
            await _qs.PushMessageAsync("orders", $"Order dispatched for {order.Quantity} {order.ProductName}(s)");
            
            return RedirectToAction(nameof(Index));
        }
        
        // POST: Orders/MarkAsDelivered
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsDelivered(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _ts.GetEntityAsync<Order>("orders", "ORDER", id);
            
            await _qs.PushMessageAsync("orders", $"Order of {order.Quantity} {order.ProductName}(s) has been delivered");
            
            return RedirectToAction(nameof(Index));
        }
        
        // POST: Orders/ClearMessage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearMessage()
        {
            await _qs.PopMessageAsync("orders");
            
            return RedirectToAction(nameof(Index));
        }
    }
}

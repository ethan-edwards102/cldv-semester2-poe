using ABCRetails.Models;
using ABCRetails.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetails.Controllers
{
    public class CustomerController : Controller
    {
        private readonly TableService _ts;

        public CustomerController(TableService ts)
        {
            _ts = ts;
        }
        
        // GET: Customers
        public async Task<IActionResult> Index()
        {
            return View(await _ts.ToListAsync<Customer>("customers"));
        }
        
        // GET: Customers/Details
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _ts.GetEntity<Customer>("customers", "CUSTOMER", id);
            
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }
        
        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }
        
        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create
        (
            [Bind("CustomerName, EmailAddress, PhoneNumber, DeliveryAddress")]
            Customer customer
        )
        {
            if (ModelState.IsValid)
            {
                await _ts.AddAsync("customers", customer);
                return RedirectToAction(nameof(Index));
            }
            
            return View(customer);
        }
    }
}

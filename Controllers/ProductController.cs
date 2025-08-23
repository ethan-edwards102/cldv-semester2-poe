using ABCRetails.Models;
using ABCRetails.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetails.Controllers
{
    public class ProductController : Controller
    {
        private readonly TableService _ts;
        private readonly BlobService _bs;

        public ProductController(TableService ts, BlobService bs)
        {
            _ts = ts;
            _bs = bs;
        }
        
        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _ts.ToListAsync<Product>("products"));
        }
        
        // GET: Products/Details
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _ts.GetEntityAsync<Product>("products", "PRODUCT", id);

            Console.WriteLine(product.Price);
            
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        
        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }
        
        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create
        (
            [Bind("ProductName, Price, Description, ImageFile")]
            CreateProductViewModel viewModel
        )
        {
            if (ModelState.IsValid && viewModel.ImageFile.Length > 0)
            {
                var photoURL = await _bs.UploadFileAsync(viewModel.ImageFile);

                var product = new Product();

                product.ProductName = viewModel.ProductName;
                product.Price = viewModel.Price;
                product.PhotoURL = photoURL;
                product.Description = viewModel.Description;

                await _ts.InsertEntityAsync("products", product);
                return RedirectToAction(nameof(Index));
            }
            
            return View(viewModel);
        }
        
        // GET: Products/Edit/
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _ts.GetEntityAsync<Product>("products", "PRODUCT", id);

            if (product == null)
            {
                return NotFound();
            }
            
            // Convert Product into ViewModel
            var viewModel = new EditProductViewModel
            {
                 PartitionKey = product.PartitionKey, 
                 RowKey = product.RowKey, 
                 Timestamp = product.Timestamp, 
                 ETag = product.ETag, 
                 ProductName = product.ProductName, 
                 Price = product.Price, 
                 Description = product.Description, 
                 PhotoURL = product.PhotoURL
            };
            
            return View(viewModel);
        }

        // POST: Products/Edit/
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit
        (
            string id,
            
            [Bind("RowKey, PartitionKey, ETag, Timestamp, ProductName, Price, Description, PhotoURL, ImageFile")]
            EditProductViewModel viewModel
        )
        {
            if (id != viewModel.RowKey)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                Console.WriteLine(viewModel.Price);
                
                try
                {
                    // Check if a new photo was uploaded
                    var photoUrl = viewModel.PhotoURL;

                    if (viewModel.ImageFile != null && viewModel.ImageFile.Length > 0)
                    {
                        photoUrl = await _bs.UploadFileAsync(viewModel.ImageFile);
                    }
                    
                    // Convert ViewModel into Product
                    var product = new Product
                    {
                        PartitionKey = viewModel.PartitionKey, 
                        RowKey = viewModel.RowKey, 
                        Timestamp = viewModel.Timestamp, 
                        ETag = viewModel.ETag, 
                        ProductName = viewModel.ProductName, 
                        Price = viewModel.Price, 
                        Description = viewModel.Description, 
                        PhotoURL = photoUrl
                    };

                    Console.WriteLine(product.Price);
                    
                    await _ts.UpdateEntityAsync<Product>("products", product);
                }
                catch (Exception e)
                {
                    if (await _ts.GetEntityAsync<Product>("products", "PRODUCT", id) == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
                return RedirectToAction(nameof(Index));
            }
            
            return View(viewModel);
        }
        
        // GET: Products/Delete/
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _ts.GetEntityAsync<Product>("products", "PRODUCT", id);
            
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var product = await _ts.GetEntityAsync<Product>("products", "PRODUCT", id);
            
            if (product != null)
            {
                await _ts.RemoveEntityAsync("products", "PRODUCT", id);
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}

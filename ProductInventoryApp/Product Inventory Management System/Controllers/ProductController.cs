using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Product_Inventory_Management_System.Authorize;
using Product_Inventory_Management_System.Data;
using Product_Inventory_Management_System.Models;

namespace Product_Inventory_Management_System.Controllers
{
    [APIAttribute]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public ProductController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region Get Products
        /////// <summary>
        /////// Get : Product and filter by name
        /////// </summary>
        /////// <returns></returns>
        public async Task<IActionResult> Index(string searchTerm, int page = 1, int pageSize = 5)
        {
            var query = _dbContext.Products.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Name.ToLower().Contains(searchTerm.ToLower()));
                ViewData["SearchTerm"] = searchTerm;
            }

            var totalProducts = await query.CountAsync();

            var products = await query
                .Skip((page - 1) * pageSize) 
                .Take(pageSize) 
                .ToListAsync();

            var paginationModel = new PaginationModel
            {
                TotalItems = totalProducts,
                PageSize = pageSize,
                CurrentPage = page
            };

            ViewBag.Pagination = paginationModel;

            return View(products);
        }


        /// <summary>
        /// Get product details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> ViewDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = await _dbContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        #endregion

        #region Create Product
        /// <summary>
        /// Get: Product/Create
        /// </summary>
        /// <returns></returns>

        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Create new product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Price,Category,StockQuantity")] Product product)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Add(product);
                await _dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product added successfully!";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Failed to add product. Please try again.";
            return View(product);
        }
        #endregion

        #region Edit Product
        /// <summary>
        /// Edit the product details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _dbContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        /// <summary>
        /// Edit product details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,Category,StockQuantity")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dbContext.Update(product);
                    await _dbContext.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Product updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        TempData["ErrorMessage"] = "Product not found.";
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Failed to update product. Please try again.";
            return View(product);
        }
        #endregion

        #region Delete Product
        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _dbContext.Products.FindAsync(id);
            if (product != null)
            {
                _dbContext.Products.Remove(product);
                await _dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Product not found.";
            }

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Functions
        /// <summary>
        /// Check is product exists
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool ProductExists(int id)
        {
            return _dbContext.Products.Any(e => e.Id == id);
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductMVC.Models;

namespace ProductMVC.Controllers
{
    // [Route("[controller]")]
    public class productController : Controller
    {
        private readonly ProductDbContext _context;
        private readonly IWebHostEnvironment environment;

        public productController(ProductDbContext productDbContext, IWebHostEnvironment environment){
            this._context = productDbContext;
            this.environment = environment;
        }
        // Get : ProductController
        [HttpGet]
        public async Task<IActionResult> Index(int? page, int? PageSize) {
            int pageNumber = page ?? 1;
            int defaultPageSize = PageSize ?? 4;
            int totalRecords = await _context.CentralArea.CountAsync();
            var productList = await _context.CentralArea.OrderByDescending(x => x.Id).Skip((pageNumber -1) * defaultPageSize).Take(defaultPageSize).ToListAsync();
            productList = productList.OrderByDescending(x => x.Id).ToList();
            foreach (var product in productList) {
                if (product.PictureFileName == "" | product.PictureFileName == null) {
                    product.PictureFileName = "NoData.jpg";
                }
            }
            var pagedProductList = new PagedList<ProductViewModel>(productList, pageNumber, defaultPageSize, totalRecords);
            return View(pagedProductList);
        }
        // Get:ProductController/Create
        [HttpGet]
        public async Task<IActionResult> Create(){
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel addProductViewModel, IFormFile PictureFile){
            try
            {   string? strPictureFile = "NoData.jpg";
            if (PictureFile != null){
                string strDateTime = DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                strPictureFile = strDateTime + "_" + PictureFile.FileName;
                string? ImageFullPath = environment.WebRootPath + "/images/" + strPictureFile;
                using (var fileStream = new FileStream(ImageFullPath, FileMode.Create)) {
                    await PictureFile.CopyToAsync(fileStream);
                }
            }
                ProductViewModel productViewModel = new ProductViewModel(){
                    Id = addProductViewModel.Id,
                    Name = addProductViewModel.Name,
                    Description = addProductViewModel.Description,
                    BuyingPrice = addProductViewModel.BuyingPrice,
                    Supplier = addProductViewModel.Supplier,
                    PictureFileName = strPictureFile,
                    ManufacturingDate = addProductViewModel.ManufacturingDate,
                    PurchasingDate = addProductViewModel.PurchasingDate,
                    ExpirationDate = DateTime.Parse(addProductViewModel.ManufacturingDate).AddYears(1)
                };
                await _context.AddAsync(productViewModel);
                await _context.SaveChangesAsync();
                TempData["successMessage"] = $"New Product was Created({productViewModel.Name})";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) 
            {
                TempData["errorMessage"] = $"Message: {ex.Message}{Environment.NewLine}Stack Trace:{Environment.NewLine}{ex.StackTrace}";
                return View();
            }
        }
        // Get:ProductController/Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int Id){
            try
            {
                var product = await _context.CentralArea.SingleOrDefaultAsync(f => f.Id == Id);
                TempData["PictureFilePath"] = "/images/" + product.PictureFileName;
                TempData["ManufacturingDate"] = product.ManufacturingDate?.ToString();
                TempData["PurchasingDate"] = product.PurchasingDate?.ToString();
                if (product.Supplier != null) {
                    TempData["Supplier"] = product.Supplier?.ToString();
                } else {
                    TempData["Supplier"] = "Unknown";
                }
                return View(product);
            }
            catch (Exception ex) 
            {
                TempData["errorMessage"] = ex.Message + "<br/>" + ex.StackTrace;
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(ProductViewModel editProductViewModel, IFormFile PictureFile){
            try
            {
                var product = await _context.CentralArea.SingleOrDefaultAsync(f => f.Id == editProductViewModel.Id);
                if (product == null){
                    TempData["errorMessage"] = $"Product Not Found with Id{editProductViewModel.Id}";
                    return View("No data");
                } else {
                    product.Name = editProductViewModel.Name;                    
                    product.Description = editProductViewModel.Description;
                    product.BuyingPrice = editProductViewModel.BuyingPrice;
                    product.Supplier = editProductViewModel.Supplier;
                    product.ManufacturingDate = editProductViewModel.ManufacturingDate;
                    product.PurchasingDate = editProductViewModel.PurchasingDate;
                    var ManufacturingDate = DateTime.Parse(editProductViewModel.ManufacturingDate);
                    product.ExpirationDate = ManufacturingDate.AddYears(1);
                    if (PictureFile != null) {
                        string strDateTime = DateTime.Now.ToString("yyyyMMsHHmmss", CultureInfo.InvariantCulture);
                        string strPictureFile = strDateTime + "_" + PictureFile.FileName;
                        string PictureFullPath = environment.WebRootPath + "/images/" + strPictureFile;
                        using (var fileStream = new FileStream(PictureFullPath, FileMode.Create)) {
                            await PictureFile.CopyToAsync(fileStream);
                        }
                        product.PictureFileName = strPictureFile;
                    } else {
                        product.PictureFileName = editProductViewModel.PictureFileName;
                    }
                    await _context.SaveChangesAsync();
                    TempData["successMessage"] = $"Product Record was Edited ({editProductViewModel.Name}).";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex) 
            {
                TempData["errorMessage"] = ex.Message + "<br/>" + ex.StackTrace;
                return View();
            }
        }
        // Get:ProductController/Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int Id){
            try
            {
                var product = await _context.CentralArea.SingleOrDefaultAsync(f => f.Id == Id);
                TempData["PictureFilePath"] = "/images/" + product.PictureFileName;
                TempData["ManufacturingDate"] = product.ManufacturingDate?.ToString();
                TempData["PurchasingDate"] = product.PurchasingDate?.ToString();
                if (product.Supplier != null) {
                    TempData["Supplier"] = product.Supplier?.ToString();
                } else {
                    TempData["Supplier"] = "Unknown";
                }
                return View(product);
            }
            catch (Exception ex) 
            {
                TempData["errorMessage"] = ex.Message + "<br/>" + ex.StackTrace;
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(ProductViewModel deleteProductViewModel){
            try
            {
                var product = await _context.CentralArea.SingleOrDefaultAsync(f => f.Id == deleteProductViewModel.Id);
                if (product == null){
                    TempData["errorMessage"] = $"Product Not Found with Id{deleteProductViewModel.Id}";
                    return View("No data");
                } else {
                    _context.CentralArea.Remove(product);
                    await _context.SaveChangesAsync();
                    TempData["successMessage"] = $"Product Record was Deteted({deleteProductViewModel.Name})";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex) 
            {
                TempData["errorMessage"] = ex.Message + "<br/>" + ex.StackTrace;
                return View();
            }
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}
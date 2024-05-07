using AutoMapper;
using Dannys.Data;
using Dannys.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dannys.Areas.Admin.Controllers;
[Area("Admin")]
public class ProductController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly IMapper _mapper;

    public ProductController(AppDbContext context, IWebHostEnvironment env, IMapper mapper)
    {
        _context = context;
        _env = env;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        List<Product> products = await _context.Products
                                               .Include(x => x.ProductImgs)
                                               .Include(x => x.Category)
                                               .ToListAsync();


        return View(products);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _context.Categories.ToListAsync();

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductCreateDto dto)
    {
        ViewBag.Categories = await _context.Categories.ToListAsync();

        if (!ModelState.IsValid)
        {
            return View();
        }

        var isExistCategory = await _context.Categories.AnyAsync(x => x.Id == dto.CategoryId);
        if (!isExistCategory)
        {
            ModelState.AddModelError("CategoryId", "Category is not found");
            return View(dto);
        }


        if (_context.Products.Any(x => x.Name == dto.Name))
        {
            ModelState.AddModelError("", "Product already exists");
            return View(dto);
        }

        foreach (var file in dto.AdditionalFiles)
        {

            if (!file.CheckFileSize(2))
            {
                ModelState.AddModelError("AdditionalFiles", "Files cannot be more than 2mb");
                return View(dto);
            }


            if (!file.CheckFileType("image"))
            {
                ModelState.AddModelError("AdditionalFiles", "Files must be image type!");
                return View(dto);
            }

        }

        if (!dto.MainFile.CheckFileSize(2))
        {
            ModelState.AddModelError("MainFile", "Files cannot be more than 2mb");
            return View(dto);
        }
        if (!dto.MainFile.CheckFileType("image"))
        {
            ModelState.AddModelError("MainFile", "Files must be image type!");
            return View(dto);
        }

        Product product = _mapper.Map<Product>(dto);
        var mainFileName = await dto.MainFile.SaveFileAsync(_env.WebRootPath, "assets", "image", "productImgs");
        var mainProductImageCreate = CreateProduct(mainFileName, true, product);
        product.ProductImgs.Add(mainProductImageCreate);

        foreach (var file in dto.AdditionalFiles)
        {
            var filename = await file.SaveFileAsync(_env.WebRootPath, "assets", "image", "productImgs");
            var additionalProductImgs = CreateProduct(filename, false, product);
            product.ProductImgs.Add(additionalProductImgs);

        }

        await _context.Products.AddAsync(product);

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");

    }

    public ProductImg CreateProduct(string url, bool isMain, Product product)
    {
        return new ProductImg
        {
            Url = url,
            IsMain = isMain,
            Product = product
        };
    }

    public async Task<IActionResult> Update(int id)
    {

        if (id < 1) return NotFound();
        ViewBag.Categories = await _context.Categories.ToListAsync();

        var product = await _context.Products.Include(x => x.ProductImgs)
                                             .FirstOrDefaultAsync(x => x.Id == id);
        if (product == null) return NotFound();
        ProductUpdateDto dto = _mapper.Map<ProductUpdateDto>(product);
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Update(int id, ProductUpdateDto dto)
    {

        ViewBag.Categories = await _context.Categories.ToListAsync();

        var existProduct = await _context.Products.Include(x => x.ProductImgs).FirstOrDefaultAsync(x => x.Id == id);

        if (existProduct is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(dto);
        }


        var isExist = await _context.Products.AnyAsync(x => x.Name == dto.Name && x.Id != id);
        if (isExist)
        {
            ModelState.AddModelError("Name", "Product already exists");
            return View(dto);
        }

        var isExistCategory = await _context.Categories.AnyAsync(x => x.Id == dto.CategoryId);
        if (!isExistCategory)
        {
            ModelState.AddModelError("CategoryId", "Category is not found");
            return View(dto);
        }

        foreach (var file in dto.AdditionalFiles)
        {

            if (!file.CheckFileSize(2))
            {
                ModelState.AddModelError("AdditionalFiles", "Files cannot be more than 2mb");
                return View(dto);
            }


            if (!file.CheckFileType("image"))
            {
                ModelState.AddModelError("AdditionalFiles", "Files must be image type!");
                return View(dto);
            }


        }

        if (dto.AdditionalFiles is not null)
        {
            foreach (var file in dto.AdditionalFiles)
            {

                if (!file.CheckFileSize(2))
                {
                    ModelState.AddModelError("AdditionalFiles", "Files cannot be more than 2mb");
                    return View(dto);
                }


                if (!file.CheckFileType("image"))
                {
                    ModelState.AddModelError("AdditionalFiles", "Files must be image type!");
                    return View(dto);

                }
            }
        }
        if (dto.MainFile != null)
        {
            if (!dto.MainFile.CheckFileSize(2))
            {
                ModelState.AddModelError("MainFile", "File cannot be more than 2mb");
                return View(dto);
            }


            if (!dto.MainFile.CheckFileType("image"))
            {
                ModelState.AddModelError("MainFile", "File must be image type!");
                return View(dto);
            }


        }

        if (dto.AdditionalFiles?.Count > 0)
        {
            foreach (var item in existProduct.ProductImgs.Where(x => !x.IsMain))
            {
                item.Url.DeleteFile(_env.WebRootPath, "assets", "image", "productImgs");
                _context.ProductImgs.Remove(item);

            }
            foreach (var file in dto.AdditionalFiles)
            {
                var filename = await file.SaveFileAsync(_env.WebRootPath, "assets", "image", "productImgs");
                var additionalProductImages = CreateProduct(filename, false, existProduct);
                existProduct.ProductImgs.Add(additionalProductImages);

            }
        }


        if (dto.MainFile is not null)
        {
            existProduct.ProductImgs.FirstOrDefault(x => x.IsMain)?.Url.DeleteFile(_env.WebRootPath, "assets", "image", "ptoductImgs");
            var mainFileName = await dto.MainFile.SaveFileAsync(_env.WebRootPath, "assets", "image", "ptoductImgs");
            var mainProductImage = CreateProduct(mainFileName, true, existProduct);
            existProduct.ProductImgs.Add(mainProductImage);

        }

        existProduct = _mapper.Map(dto, existProduct);

        _context.Products.Update(existProduct);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));

    }

    public async Task<IActionResult> Detail(int id)
    {

        var existProduct = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
        if (existProduct is null) return BadRequest();


        var product = await _context.Products.Include(x => x.Category)
                                             .Include(x => x.ProductImgs)
                                             .FirstOrDefaultAsync(x => x.Id == id);
        if (product is null) return NotFound();
        return View(product);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);

        if (product is null)
            return NotFound();

        product.IsDeleted = true;

        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

}
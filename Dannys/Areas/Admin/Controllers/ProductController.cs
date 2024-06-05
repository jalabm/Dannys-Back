using AutoMapper;
using Dannys.Data;
using Dannys.Extensions;
using Dannys.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dannys.Areas.Admin.Controllers;
[Area("Admin")]
public class ProductController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly IMapper _mapper;
    private readonly CloudinaryService _cloudinaryService;

    public ProductController(AppDbContext context, IWebHostEnvironment env, IMapper mapper, CloudinaryService cloudinaryService)
    {
        _context = context;
        _env = env;
        _mapper = mapper;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<IActionResult> Index(int page=1)
    {

        int pageCount = (int)Math.Ceiling((decimal)_context.Products.Count() / 10);

        if (pageCount == 0)
            pageCount = 1;

        ViewBag.PageCount = pageCount;

        if (page > pageCount)
            page = pageCount;

        if (page <= 0)
            page = 1;

        ViewBag.CurrentPage = page;

        var products = await _context.Products.OrderByDescending(x => x.CreatedAt).Skip((page - 1) * 10).Take(10).Include(x=>x.Category).Include(x=>x.ProductImgs).ToListAsync();

        //var p = await _context.Products.Include(x => x.Category).Include(x => x.ProductImgs).ToListAsync();
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

        var mainFileName = await _cloudinaryService.FileCreateAsync(dto.MainFile);
        var mainProductImageCreate = CreateProductImage(mainFileName, true, product);
        product.ProductImgs.Add(mainProductImageCreate);

        foreach (var file in dto.AdditionalFiles)
        {
            var filename = await _cloudinaryService.FileCreateAsync(file);
            var additionalProductImgs = CreateProductImage(filename, false, product);
            product.ProductImgs.Add(additionalProductImgs);

        }


        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");

    }

    private ProductImg CreateProductImage(string url, bool isMain, Product product)
    {
        return new ProductImg
        {
            Url = url,
            IsMain = isMain,
            Product = product
        };
    }


    public async Task<IActionResult> TestData()
    {
        for (int i = 0; i < 100; i++)
        {
            Product product = new()
            {
                CategoryId=1,
                Description="DSAJKADSNLKDS",
                Discount=30,
                Price=30,
                Ingredients="SAsa",
                Name="Product",
                Porsion=1,
                
            };

            ProductImg img = new()
            {
                Product = product,
                Url= "https://res.cloudinary.com/dmhklgr4f/image/upload/v1716569721/vlwjtj0pirylshsvidfv.jpg",
                IsMain=true,
            };

            product.ProductImgs.Add(img);

            await _context.Products.AddAsync(product);
        }
        await _context.SaveChangesAsync();
        return Ok("Ok");
    }
    public async Task<IActionResult> Update(int id)
    {

        if (id < 1) return NotFound();
        ViewBag.Categories = await _context.Categories.ToListAsync();

        var product = await _context.Products.Include(x => x.ProductImgs)
                                             .FirstOrDefaultAsync(x => x.Id == id);
        if (product == null) return NotFound();
        ProductUpdateDto dto = _mapper.Map<ProductUpdateDto>(product);


        dto.ImagePaths = product.ProductImgs.Where(x => !x.IsMain).Select(x => x.Url).ToList();
        dto.ImageIds = product.ProductImgs.Where(x => !x.IsMain).Select(x => x.Id).ToList();
        dto.MainFileUrl = product.ProductImgs.FirstOrDefault(x => x.IsMain)?.Url ?? "null";
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Update(int id, ProductUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        ViewBag.Categories = await _context.Categories.ToListAsync();

        var existProduct = await _context.Products.Include(x => x.ProductImgs).FirstOrDefaultAsync(x => x.Id == id);

        if (existProduct is null)
        {
            return NotFound();
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





        var ExistedImages = existProduct.ProductImgs.Where(x => !x.IsMain).Select(x => x.Id).ToList();
        if (dto.ImageIds is not null)
        {
            ExistedImages = ExistedImages.Except(dto.ImageIds).ToList();

        }
        if (ExistedImages.Count > 0)
        {
            foreach (var imageId in ExistedImages)
            {
                var deletedImage = existProduct.ProductImgs.FirstOrDefault(x => x.Id == imageId);
                if (deletedImage is not null)
                {

                    existProduct.ProductImgs.Remove(deletedImage);

                    deletedImage.Url.DeleteFile(_env.WebRootPath, "assets", "image", "productImgs");
                }

            }
        }


        foreach (var file in dto.AdditionalFiles)
        {
            var filename = await _cloudinaryService.FileCreateAsync(file);
            var additionalProductImages = new ProductImg() { IsMain = false, Product = existProduct, Url = filename };
            existProduct.ProductImgs.Add(additionalProductImages);

        }


        if (dto.MainFile is not null)
        {
            var existMainImage = existProduct.ProductImgs.FirstOrDefault(x => x.IsMain);
            //remove exist image
            if (existMainImage is not null)
            {
                existProduct.ProductImgs.Remove(existMainImage);

            }



            var filename = await _cloudinaryService.FileCreateAsync(dto.MainFile);
            ProductImg image = new ProductImg() { IsMain = true, Product = existProduct, Url = filename };
            existProduct.ProductImgs.Add(image);

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
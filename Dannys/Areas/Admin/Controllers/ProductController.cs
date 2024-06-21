using System;
using System.Text;
using Dannys.Data;
using Dannys.Enums;
using Dannys.Extensions;
using Dannys.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dannys.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProductController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly IMapper _mapper;
    private readonly CloudinaryService _cloudinaryService;
    private readonly IEmailService _emailService;

    public ProductController(AppDbContext context, IWebHostEnvironment env, IMapper mapper, CloudinaryService cloudinaryService, IEmailService emailService)
    {
        _context = context;
        _env = env;
        _mapper = mapper;
        _cloudinaryService = cloudinaryService;
        _emailService = emailService;
    }

    public async Task<IActionResult> Index(int page = 1)
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

        var products = await _context.Products.OrderByDescending(x => x.CreatedAt).Skip((page - 1) * 10).Take(10).Include(x => x.Category).Include(x => x.ProductImgs).ToListAsync();

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







        var subscribes = await _context.Subscribes.ToListAsync();

        string url = Url.Action("Detail", "Shop", new { area = "", id = product.Id }, HttpContext.Request.Scheme) ?? "";

        string emailBody = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #ffffff;
            color: #000000;
            margin: 0;
            padding: 0;
        }}
        .container {{
            width: 100%;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
        }}
        .header {{
            background-color: #cc9933;
            padding: 20px;
            text-align: center;
            color: #ffffff;
        }}
        .content {{
            padding: 20px;
            background-color: #f9f9f9;
            color: #000000;
        }}
        .footer {{
            background-color: #cc9933;
            padding: 10px;
            text-align: center;
            color: #ffffff;
        }}
        .button {{
            display: inline-block;
            padding: 10px 20px;
            margin: 20px 0;
            color: #ffffff;
            background-color: #000000;
            text-decoration: none;
            border-radius: 5px;
        }}
        .product-image {{
            width: 50%;
            max-width: 50%;
            height: auto;
            margin: 20px auto !important;
        }}
        .img {{
            display: flex;
            width: 100%;
            align-items: center;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>We Have New Product!</h1>
        </div>
        <div class='content'>
            <h2>Dear Subscriber,</h2>
            <p>We are thrilled to introduce our latest product and we need your valuable feedback to make it even better!</p>
            <div class='img'>
                <img src='{mainFileName}' alt='New Product' class='product-image'>
            </div>
            <p>{product.Description}</p>
            <p>Click the button below to get started:</p>
            <a href='{url}' class='button'>Shop Now</a>
            <p>Thank you for being a valued subscriber and for your continued support.</p>
            <p>Best regards,</p>
            <p>The Danny's Team</p>
        </div>
        <div class='footer'>
            <p>&copy; 2024 Danny's. All rights reserved.</p>
        </div>
    </div>
</body>
</html>
";


        foreach (var subscribe in subscribes)
        {

            _emailService.SendEmail(subscribe.Email, "New Product", emailBody);
        }


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
                CategoryId = 1,
                Description = "DSAJKADSNLKDS",
                Discount = 30,
                Price = 30,
                Ingredients = "SAsa",
                Name = "Product",
                Porsion = 1,

            };

            ProductImg img = new()
            {
                Product = product,
                Url = "https://res.cloudinary.com/dmhklgr4f/image/upload/v1716569721/vlwjtj0pirylshsvidfv.jpg",
                IsMain = true,
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



    public async Task<IActionResult> GenerateRandomItems()
    {

        var rand = new Random();

        string[] productNames = { "Pizza", "Burger", "Pasta", "Salad", "Sushi", "Steak", "Soup", "Sandwich", "Fries", "Taco" };
        string[] descriptions = {
    "Absolutely delicious and incredibly satisfying, guaranteed to tantalize your taste buds and leave you craving more",
    "Richly flavorful and wonderfully savory, crafted with the finest ingredients to ensure a mouth-watering experience",
    "Exquisitely prepared and delightfully appetizing, perfect for those who appreciate the finer flavors in life",
    "Unbelievably scrumptious and utterly delectable, a culinary masterpiece that promises to please even the most discerning palates",
    "Indulgently savory and profoundly satisfying, offering a symphony of flavors that will leave you wanting another bite",
    "Delightfully aromatic and irresistibly tasty, each bite is a journey through layers of culinary excellence",
     "Sensationally delicious and remarkably satisfying, a culinary delight that excites the senses with every bite",
    "Exquisitely flavorful and tantalizingly aromatic, prepared with care to ensure a memorable dining experience",
    "Sumptuously savory and delightfully rich, a feast for the taste buds that promises pure gastronomic pleasure",
    "Irresistibly mouth-watering and profoundly delectable, designed to satisfy cravings with its bold and robust flavors",
    "Elegantly crafted and delightfully nuanced, offering a harmonious blend of textures and tastes in every dish",
    "Luxuriously indulgent and profoundly flavorful, a gourmet sensation that embodies culinary excellence",
    "Perfectly balanced and wonderfully appetizing, each ingredient complements the next to create a symphony of flavors",
    "Deliciously complex and deeply satisfying, designed for those who appreciate the artistry of fine cuisine",
    "Incredibly appetizing and expertly seasoned, a culinary masterpiece that elevates your dining experience",
    "Richly satisfying and irresistibly delicious, guaranteed to leave a lasting impression with its exceptional taste"

};
        string[] ingredients = {
  "Lasagna noodles, ground beef, ricotta cheese, mozzarella cheese, marinara sauce",
  "Chicken breasts, lemon juice, garlic powder, olive oil, fresh parsley",
  "Salmon fillets, soy sauce, honey, garlic, ginger",
  "Quinoa, black beans, corn, bell peppers, cilantro",
  "Eggs, bacon, cheddar cheese, spinach, bell peppers"
        };



        for (int i = 0; i < 50; i++)
        {
            string name = productNames[rand.Next(productNames.Length)];
            string description = descriptions[rand.Next(descriptions.Length)];
            string ingredient = ingredients[rand.Next(ingredients.Length)];
            int portion = rand.Next(100, 500);
            int discount = rand.Next(0, 30);
            decimal price = rand.Next(50, 200);
            int categoryId = rand.Next(1, 10);
            string[] images = {
            "https://res.cloudinary.com/dmhklgr4f/image/upload/v1718883383/ojorbjcli5mwxvbkem1z.jpg",
            "https://res.cloudinary.com/dmhklgr4f/image/upload/v1718883381/eqcdvcrilbunodbbqz6d.jpg",
            "https://res.cloudinary.com/dmhklgr4f/image/upload/v1718883379/zcravn4aygp6mtvhfuxm.jpg",
            "https://res.cloudinary.com/dmhklgr4f/image/upload/v1718883377/zfunpauzhsulqybziqms.jpg",
            "https://res.cloudinary.com/dmhklgr4f/image/upload/v1718883375/cggkhmekk4emsi17madn.jpg",
            "https://res.cloudinary.com/dmhklgr4f/image/upload/v1718883372/f46z1froeyczdsdvppss.jpg",
            "https://res.cloudinary.com/dmhklgr4f/image/upload/v1718883370/ktstfpvdtbrwtky2ic3u.jpg",
            "https://res.cloudinary.com/dmhklgr4f/image/upload/v1718883368/zt4xy6kvrkimemiwgxgk.jpg"
        };


            Product product = new()
            {
                Name = name,
                Description = description,
                Ingredients = ingredient,
                Porsion = portion,
                Discount = discount,
                Price = price,
                CategoryId = categoryId,


            };

            ProductImg img = new()
            {
                Product = product,
                Url = images[rand.Next(images.Length)],
                IsMain = true,
            };


            ProductImg additional1 = new() { Product = product, Url = images[rand.Next(images.Length)] };
            ProductImg additional2 = new() { Product = product, Url = images[rand.Next(images.Length)] };
            ProductImg additional3 = new() { Product = product, Url = images[rand.Next(images.Length)] };
            ProductImg additional4 = new() { Product = product, Url = images[rand.Next(images.Length)] };
            ProductImg additional5 = new() { Product = product, Url = images[rand.Next(images.Length)] };


            product.ProductImgs.Add(img);
            product.ProductImgs.Add(additional1);
            product.ProductImgs.Add(additional2);
            product.ProductImgs.Add(additional3);
            product.ProductImgs.Add(additional4);
            product.ProductImgs.Add(additional5);


            await _context.Products.AddAsync(product);
        }
        await _context.SaveChangesAsync();


        return Ok("ok");
    }
}
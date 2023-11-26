using AutoMapper;
using CrudApp1.Models;
using CrudApp1.Models.Viewmodels;
using CrudApp1.Repository.Abstract;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CrudApp1.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper mapper;
        private readonly IProductRepo productRepo;
        private readonly FileService fileService;

        public HomeController(ILogger<HomeController> logger, IMapper mapper, IProductRepo productRepo, FileService fileService)
        {
            _logger = logger;
            this.mapper = mapper;
            this.productRepo = productRepo;
            this.fileService = fileService;
        }

        public IActionResult Index()
        {
            try
            {
                var prods = productRepo.GetProducts();

                var products = mapper.Map<List<ProductViewModel>>(prods);

                return View(products);

            }
            catch(Exception ex)
            {
                return Json(ex);
            }
            
        }


        public IActionResult Create()
        {
           return View();
        }

        //Creating post controller
        [HttpPost]
        public IActionResult Create(CreateProdVm productreq)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    //first map vm to actual model
                    string imagePath=fileService.UploadImage(productreq.ImageFile);
                    var product = mapper.Map<Product>(productreq);
                    product.ImageUrl=imagePath;

                    bool created = productRepo.AddProduct(product);

                    if(created)
                    {
                        Alert("New Product Added", Enum.NotificationType.success);
                        return RedirectToAction("Index");
                    }
                    Alert("Unable to Add product", Enum.NotificationType.error);
                }
                return View(productreq);
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
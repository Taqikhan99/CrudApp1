using AutoMapper;
using CrudApp1.Models;
using CrudApp1.Models.Viewmodels;
using CrudApp1.Repository.Abstract;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CrudApp1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper mapper;
        private readonly IProductRepo productRepo;

        public HomeController(ILogger<HomeController> logger, IMapper mapper, IProductRepo productRepo)
        {
            _logger = logger;
            this.mapper = mapper;
            this.productRepo = productRepo;
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
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
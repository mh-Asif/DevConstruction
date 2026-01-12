using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ConstructionApp.WebUI.Controllers
{
    public class IMSController : Controller
    {
        private readonly IConfiguration _configuration;

        public IMSController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult CreateInvoice()
        {
            ViewBag.EnvironmentUrl = _configuration["ApiSettings:BaseUrl"];
            return View();
        }
        public IActionResult InvoiceList()
        {
            ViewBag.EnvironmentUrl = _configuration["ApiSettings:BaseUrl"];
            return View();
        }
        public IActionResult InvoiceApproval()
        {
            ViewBag.EnvironmentUrl = _configuration["ApiSettings:BaseUrl"];
            return View();
        }
        public IActionResult PaymentProcessing()
        {
            ViewBag.EnvironmentUrl = _configuration["ApiSettings:BaseUrl"];
            return View();
        }
        public IActionResult InvoiceReport()
        {
            ViewBag.EnvironmentUrl = _configuration["ApiSettings:BaseUrl"];
            return View();
        }
    }
}

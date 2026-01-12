using Microsoft.AspNetCore.Mvc;

namespace ConstructionApp.WebUI.Controllers
{
    public class InventoryController : Controller
    {
        public IActionResult StockDashboard()
        {
            return View();
        }
        public IActionResult Items()
        {
            return View();
        }
        public IActionResult StockOutDashboard()
        {
            return View();
        }
    }
}

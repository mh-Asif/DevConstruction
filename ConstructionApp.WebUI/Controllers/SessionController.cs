using Construction.Infrastructure.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace ConstructionApp.WebUI.Controllers
{
    public class SessionController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMemoryCache _memoryCache;
        public SessionController(ILogger<HomeController> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;
        }
        [HttpPost]
        public IActionResult SetUserSession([FromBody] UserSessionModel model)
        {
            if (model == null) return BadRequest();
            HttpContext.Session.SetInt32("UserId", (int)model.UserId);
           // HttpContext.Session.SetString("UserName", model.FullName);
            HttpContext.Session.SetInt32("UserType", (int)model.UserType);
            HttpContext.Session.SetString("EmailAddress", model.EmailAddress);
            HttpContext.Session.SetString("FullName", (model.FullName ==null?"":model.FullName));
            HttpContext.Session.SetInt32("UserId", (int)model.UserId);
            HttpContext.Session.SetString("EncUserId", CommonHelper.EncryptURLHTML(model.UserId.ToString()));
            HttpContext.Session.SetInt32("UserType", (int)model.UserType);
            HttpContext.Session.SetInt32("IsAdmin", (model.IsAdmin == true ? 1 : 0));
            if (model.UserType == 0)
                HttpContext.Session.SetInt32("UnitId", (int)model.UserId);
            else
                HttpContext.Session.SetInt32("UnitId", (int)model.UnitId);
            HttpContext.Session.SetInt32("RoleId", (int)model.RoleId);
            HttpContext.Session.SetInt32("DeptId", (int)model.DepartmentId);
            HttpContext.Session.SetInt32("DesiId", (int)model.JobTitleId);
            if (model.ProfileName != null)
            {
                model.ProfileName = Path.Combine("\\Profile", model.ProfileName);
                HttpContext.Session.SetString("Logo", model.ProfileName);
            }
            else
                HttpContext.Session.SetString("Logo", "");

            if (_memoryCache is MemoryCache memCache)
            {
                memCache.Clear();
            }
           // _memoryCache.Remove("Menu");
            // Add more as needed
            return Ok();
        }
    }

    public class UserSessionModel
    {
        public int? UserId { get; set; }
        public string? FullName { get; set; }
        public int? UserType { get; set; }
        public bool? IsAdmin { get; set; }
        public int? UnitId { get; set; }
        public int? RoleId { get; set; }
        public int? DepartmentId { get; set; }
        public int? JobTitleId { get; set; }
        public string? ProfileName { get; set; }
        public string? EmailAddress { get; set; }
        // Add more as needed
    }
}

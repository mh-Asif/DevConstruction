
using Construction.Infrastructure.Models;

using ConstructionApp.WebUI.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace ConstructionApp.WebUI.Controllers.ViewComponents
{
    [ViewComponent(Name = "MenuMapping")]
    public class MenuMappingComponent : ViewComponent
    {

        private readonly IMemoryCache _memoryCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
      
        public MenuMappingComponent(IHttpContextAccessor httpContextAccessor,IMemoryCache memoryCache)
        {

            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;

        }

        public async Task<IViewComponentResult> InvokeAsync()
        {

            var uId = HttpContext.Session.GetString("UserId");
            if (uId == null)
                _httpContextAccessor.HttpContext.Response.Redirect("/Home/Index");

            // string apiProjectUrl = "https://localhost:7013/api/ProjectAPI/";
            AccessMasterDTO objOutPut = new AccessMasterDTO();
            if (HttpContext.Session.GetInt32("UnitId") >0)
            {

                // objOutPut.AccessList = await _loginApiController.GetAccessDetails(HttpContext.Session.GetInt32("UnitId"), HttpContext.Session.GetInt32("RoleId"), HttpContext.Session.GetInt32("DeptId"), HttpContext.Session.GetInt32("DesiId"));

                //   HttpContext.Session.SetString("Menu", JsonConvert.SerializeObject(objOutPut));
                //List<ProjectsDTO> ProjectLst = new List<ProjectsDTO>();
                // return View("MenuMapping", objOutPut);

                //if (_memoryCache.TryGetValue("Menu", out List<AccessMasterDTO> cachedMenu))
                //{
                //    objOutPut.AccessList = cachedMenu;
                //    return View("MenuMapping", objOutPut);
                //}

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response;
                  //  bool IsClient = HttpContext.Session.GetInt32("UserType") > 1 && HttpContext.Session.GetInt32("UserType") == 2 ? true : false;
                    client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));    
                    if(HttpContext.Session.GetInt32("UserType")>1)
                    {
                        bool IsClient = HttpContext.Session.GetInt32("UserType") > 1 && HttpContext.Session.GetInt32("UserType") == 2 ? true : false;

                        response = await client.GetAsync("GetClientAccessDetails?unitId=" + HttpContext.Session.GetInt32("UnitId") + "&isClient=" + IsClient);
                    }
                    else
                    {
                         response = await client.GetAsync("GetAccessDetails?unitId=" + HttpContext.Session.GetInt32("UnitId") + "&roleId=" + HttpContext.Session.GetInt32("RoleId") + "&departmentId=" + HttpContext.Session.GetInt32("DeptId"));
                    }
                    
                   
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        objOutPut.AccessList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AccessMasterDTO>>(data);

                        //_memoryCache.Set("Menu", objOutPut.AccessList, TimeSpan.FromMinutes(30));
                      HttpContext.Session.SetString("Menu", JsonConvert.SerializeObject(objOutPut));

                    }

                    return View("MenuMapping", objOutPut);
                }

            }
            else
            {
                if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated && _httpContextAccessor.HttpContext.Request.Path.Value != "/Home/Index")
                {
                    _httpContextAccessor.HttpContext.Response.Redirect("/Home/Index");
                }


                return View("MenuMapping", objOutPut);
            }
           
        }
    }
}

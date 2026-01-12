
using Construction.Infrastructure.Models;
using ConstructionApp.WebUI.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace ConstructionApp.WebUI.Controllers.ViewComponents
{
    [ViewComponent(Name = "Notifications")]
    public class NotificationsComponent : ViewComponent
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _memoryCache;
        public NotificationsComponent(IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache)
        {

            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;

        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // string apiProjectUrl = "https://localhost:7013/api/ProjectAPI/";
            UserNotificationsDTO objOutPut = new UserNotificationsDTO();
            if (HttpContext.Session.GetInt32("UserId") > 0)
            {

                //  objOutPut.NotificationList = await _loginApiController.GetNotificationDetails(HttpContext.Session.GetInt32("UserId"));


                //  return View("ProjectDirectory", objOutPut);

                if (_memoryCache.TryGetValue("Notifications", out List<UserNotificationsDTO> cachedNotifications))
                {
                    objOutPut.NotificationList = cachedNotifications;
                    return View("Notifications", objOutPut);
                }
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiCommentsUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    //HttpResponseMessage response = await client.GetAsync("GetUserListing?userType=" + userType + "&logInUser="+ logInUser);
                    HttpResponseMessage response = await client.GetAsync("GetNotificationDetails?UserId=" + HttpContext.Session.GetInt32("UserId"));
                    //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        objOutPut.NotificationList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserNotificationsDTO>>(data);
                    }
                }
                //foreach (var item in objOutPut.NotificationList)
                //{

                //    // item. = CommonHelper.EncryptURLHTML(item.ProjectId.ToString());
                //    if (item.ProfileImage != null)
                //        item.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);

                //}
                   _memoryCache.Set("Notifications", objOutPut.NotificationList, TimeSpan.FromMinutes(30));
                

                return View("Notifications", objOutPut);
                //}

            }
            else
            {
                if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated && _httpContextAccessor.HttpContext.Request.Path.Value != "/Home/Index")
                {
                    _httpContextAccessor.HttpContext.Response.Redirect("/Home/Index");
                }


                return View("Notifications", objOutPut);
            }

        }
    }
}


using Construction.Infrastructure.Helper;
using Construction.Infrastructure.Models;

using ConstructionApp.WebUI.Helper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ConstructionApp.WebUI.Controllers.ViewComponents
{
    [ViewComponent(Name = "ProjectDirectory")]
    public class ProjectViewComponent : ViewComponent
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
       
        public ProjectViewComponent(IHttpContextAccessor httpContextAccessor)
        {

            _httpContextAccessor = httpContextAccessor;
          
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
           // string apiProjectUrl = "https://localhost:7013/api/ProjectAPI/";
            ProjectsDTO objOutPut = new ProjectsDTO();
            if (HttpContext.Session.GetInt32("UserId")>0)
            {

                //   objOutPut.ProjectList = _mapper.Map<List<ProjectsDTO>>(_loginApiController.GetProjects(HttpContext.Session.GetInt32("UserId")));

                // return View("ProjectDirectory", objOutPut);
                //List<ProjectsDTO> ProjectLst = new List<ProjectsDTO>();

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiProjectUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    //HttpResponseMessage response = await client.GetAsync("GetUserListing?userType=" + userType + "&logInUser="+ logInUser);
                    HttpResponseMessage response = await client.GetAsync("GetProjects?unitId=" + HttpContext.Session.GetInt32("UserId"));
                    //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        objOutPut.ProjectList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProjectsDTO>>(data);
                    }

                    return View("ProjectDirectory", objOutPut);
                }

            }
            else
            {
                if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated && _httpContextAccessor.HttpContext.Request.Path.Value != "/Home/Index")
                {
                    _httpContextAccessor.HttpContext.Response.Redirect("/Home/Index");
                }


                return View("ProjectDirectory", objOutPut);
            }
           
        }
    }
}

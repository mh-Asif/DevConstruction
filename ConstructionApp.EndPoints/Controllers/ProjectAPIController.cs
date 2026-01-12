using AutoMapper;
using Azure;
using Construction.Infrastructure.Helper;
using Construction.Infrastructure.KeyValues;
using Construction.Infrastructure.Models;
using ConstructionApp.Core.Entities;
using ConstructionApp.Core.Repository;
using ConstructionApp.Services.DBContext;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Linq.Expressions;
using System.Text;

namespace ConstructionApp.EndPoints.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectAPIController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProjectAPIController> _logger;
        private readonly IMapper _mapper;
        private readonly ConstDbContext _constDbContext;
        //private readonly MasterAPIsController _masterApiController;

        public ProjectAPIController(IUnitOfWork unitOfWork, ILogger<ProjectAPIController> logger, IMapper mapper, ConstDbContext constDbContext)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _constDbContext = constDbContext;
            //_masterApiController = masterApiController;
        }


        [HttpPost]
        [Route("SaveProject")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> SaveProject(ProjectsDTO inputDTO)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (!ModelState.IsValid)
                {
                    outPut.DisplayMessage = "Failed";
                    outPut.HttpStatusCode = 201;
                    return Ok(outPut);
                }

                if (inputDTO.ProjectId == 0)
                {
                    // Check for duplicate project name for the same creator
                    Expression<Func<Projects, bool>> duplicateExpr = a => a.ProjectName == inputDTO.ProjectName && a.CreatedBy == inputDTO.CreatedBy && a.IsActive == true;
                    if (_unitOfWork.Projects.Exists(duplicateExpr))
                    {
                        outPut.RespId = 0;
                        outPut.DisplayMessage = "Duplicate record";
                        outPut.HttpStatusCode = 201;
                        return Ok(outPut);
                    }
                    var entity = _mapper.Map<Projects>(inputDTO);
                    var inserted = _unitOfWork.Projects.Insert(entity);
                    _unitOfWork.Save();
                    var em = _mapper.Map<ProjectsDTO>(inserted);
                    outPut.RespId = em.ProjectId;
                    outPut.DisplayMessage = "Project Saved Successfully";
                    outPut.HttpStatusCode = 200;
                    return Ok(outPut);
                }
                else
                {
                    var projectEntity = await _unitOfWork.Projects.GetByIdAsync(Convert.ToInt32(inputDTO.ProjectId));
                    if (projectEntity == null)
                    {
                        outPut.HttpStatusCode = 404;
                        outPut.DisplayMessage = "Project not found.";
                        return Ok(outPut);
                    }
                    // Check for duplicate name (other than this project)
                    Expression<Func<Projects, bool>> duplicateExpr = a => a.ProjectId != inputDTO.ProjectId && a.ProjectName == inputDTO.ProjectName && a.CreatedBy == inputDTO.CreatedBy && a.IsActive == true;
                    if (_unitOfWork.Projects.Exists(duplicateExpr))
                    {
                        outPut.HttpStatusCode = 201;
                        outPut.DisplayMessage = "Duplicate Entry Found!";
                        return Ok(outPut);
                    }
                    // Update fields
                    projectEntity.Collaborators = inputDTO.Collaborators;
                    projectEntity.CategoryId = inputDTO.CategoryId;
                    projectEntity.StatusId = inputDTO.StatusId;
                    projectEntity.StartDate = inputDTO.StartDate;
                    projectEntity.EndDate = inputDTO.EndDate;
                    projectEntity.Description = inputDTO.Description;
                    projectEntity.ClientId = inputDTO.ClientId;
                    projectEntity.PriorityId = inputDTO.PriorityId;
                    projectEntity.UnitId = inputDTO.UnitId;
                    projectEntity.FileDetails = inputDTO.FileDetails;
                    projectEntity.IsActive = inputDTO.IsActive;
                    projectEntity.ProjectName = inputDTO.ProjectName;
                    projectEntity.UserId = inputDTO.UserId;
                    projectEntity.CreatedBy = inputDTO.CreatedBy;
                    projectEntity.CreatedOn = inputDTO.CreatedOn;

                    _unitOfWork.Projects.Update(projectEntity);
                    _unitOfWork.Save();
                    outPut.HttpStatusCode = 200;
                    outPut.DisplayMessage = "Project updated successfully!";
                    return Ok(outPut);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving project {nameof(SaveProject)}");
                outPut.DisplayMessage = "An error occurred while saving the project.";
                outPut.HttpStatusCode = 500;
                return Ok(outPut);
            }
        }

        [HttpGet]
        [Route("GetProjects")]
        [Produces("application/json", Type = typeof(ProjectsDTO))]
        public async Task<List<ProjectsDTO>> GetProjects(int unitId)
        {
            try
            {
                var allProjects = await _unitOfWork.Projects.GetAll(p => p.IsActive == true);

                var filteredProjects = allProjects.Where(p =>
                    p.UnitId == unitId ||
                    p.UserId == unitId ||
                    p.CreatedBy == unitId ||
                    (!string.IsNullOrEmpty(p.Collaborators) && p.Collaborators.Contains(unitId.ToString()))
                );

                var result = filteredProjects
                    .Select(p => new ProjectsDTO
                    {
                        UserId = p.UserId,
                        IsActive = p.IsActive,
                        PriorityId = p.PriorityId,
                        ProjectId = p.ProjectId,
                        FileDetails = p.FileDetails,
                        UnitId = p.UnitId,
                        ProjectName = p.ProjectName,
                        StartDate = p.StartDate,
                        EndDate = p.EndDate
                    })
                    .ToList(); // Use synchronous ToList here

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retrieving user {nameof(GetProjects)}");
                throw;
            }
        }
        //public async Task<ProjectsDTO> GetProjects(int unitId)
        //{
        //    try
        //    {
        //        var allProjects = _unitOfWork.Projects.GetAll(p => p.IsActive == true).Result;

        //        var filteredProjects = allProjects.Where(p =>
        //            p.UnitId == unitId ||
        //            p.UserId == unitId ||
        //            p.CreatedBy == unitId ||
        //            (!string.IsNullOrEmpty(p.Collaborators) && p.Collaborators.Contains(unitId.ToString()))
        //        );

        //        var result = filteredProjects
        //                       .Select(p => new ProjectsDTO()
        //                       {
        //                           UserId = p.UserId,
        //                           IsActive = p.IsActive,
        //                           PriorityId = p.PriorityId,
        //                           ProjectId = p.ProjectId,
        //                           FileDetails = p.FileDetails,
        //                           UnitId = p.UnitId,
        //                           ProjectName = p.ProjectName,
        //                           StartDate = p.StartDate,
        //                           EndDate = p.EndDate
        //                       }).ToListAsync();

        //        return result;

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Error in retriving user {nameof(GetProjects)}");
        //        throw;
        //    }
        //}

        [HttpGet]
        [Route("GetProjectById")]
        [Produces("application/json", Type = typeof(ProjectsDTO))]
        public async Task<IActionResult> GetProjectById(int pId)
        {
            try
            {
                ProjectsDTO outputDTO = _mapper.Map<ProjectsDTO>(await _unitOfWork.Projects.GetByIdAsync(pId));
                HttpResponseMessage httpMessage = new HttpResponseMessage();
                if (outputDTO == null)
                {
                    httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO);
                    outputDTO = CommonHelper.GetClassObject(outputDTO);
                }
                else
                    httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO, outputDTO.IsActive);

                outputDTO.HttpMessage = httpMessage;
                return Ok(outputDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving Project {nameof(GetProjectById)}");
                throw;
            }
        }

        [HttpGet]
        [Route("DeleteProject")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeleteProject(int pId)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    Projects outputMaster = _mapper.Map<Projects>(await _unitOfWork.Projects.GetByIdAsync(pId));
                    outputMaster.IsActive = false;
                    _unitOfWork.Projects.Update(outputMaster);
                    _unitOfWork.Save();
                    outPut.HttpStatusCode = 200;
                    outPut.DisplayMessage = "Country delete successfully!";
                    return Ok(outPut);
                }
                else
                {
                    outPut.HttpStatusCode = 201;
                    outPut.DisplayMessage = "Failed";
                    return Ok(outPut);
                }
                // return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while deleting Delete {nameof(DeleteProject)}");
                throw;
            }
        }


        [HttpGet]
        [Route("GetProjectDashboard")]
        [Produces("application/json", Type = typeof(ProjectDashboardDTO))]
        public async Task<List<ProjectDashboardDTO>?> GetProjectDashboard(int? unitId)
        {
           // var totalSw = System.Diagnostics.Stopwatch.StartNew();
            ProjectsDTO ProjectLst = new ProjectsDTO();
            var parms = new DynamicParameters();
          
            parms.Add(@"@UnitId", unitId, DbType.Int32);
            parms.Add(@"@PageNumber", 1, DbType.Int32);
            parms.Add(@"@PageSize", 20, DbType.Int32);

            var dbSw = System.Diagnostics.Stopwatch.StartNew();
            ProjectLst.ProjectDashboardList = (await _unitOfWork.ProjectDashboard.GetSPData<ProjectDashboardDTO>("usp_GetProjectDashboard", parms));
          

            //dbSw.Stop();
            //_logger.LogInformation($"DB call took {dbSw.ElapsedMilliseconds} ms");

            //totalSw.Stop();
            //_logger.LogInformation($"Total API call took {totalSw.ElapsedMilliseconds} ms");
            return ProjectLst.ProjectDashboardList;

        }

        [HttpGet]
        [Route("GetProjectSummery")]
        [Produces("application/json", Type = typeof(ProjectSummeryDTO))]
        public async Task<List<ProjectSummeryDTO>?> GetProjectSummery(int? unitId,int? projectId)
        {
            ProjectsDTO ProjectLst = new ProjectsDTO();
            var parms = new DynamicParameters();

            parms.Add(@"@UnitId", unitId, DbType.Int32);
            parms.Add(@"@ProjectId", projectId, DbType.Int32);

            ProjectLst.ProjectSummeryList = (await _unitOfWork.ProjectSummery.GetSPData<ProjectSummeryDTO>("usp_ProjectOverView", parms));
            return ProjectLst.ProjectSummeryList;
        }

        [HttpGet]
        [Route("GetProjectRoleSummery")]
        [Produces("application/json", Type = typeof(ProjectRoleSummeryDTO))]
        public async Task<List<ProjectRoleSummeryDTO>?> GetProjectRoleSummery(int? unitId, int? projectId)
        {
            ProjectsDTO ProjectLst = new ProjectsDTO();
            var parms = new DynamicParameters();

            parms.Add(@"@UnitId", unitId, DbType.Int32);
            parms.Add(@"@ProjectId", projectId, DbType.Int32);

            ProjectLst.ProjectRoleSummeryList = (await _unitOfWork.ProjectRoleSummery.GetSPData<ProjectRoleSummeryDTO>("usp_ProjectRoleOverView", parms));
            return ProjectLst.ProjectRoleSummeryList;
        }

        [HttpGet]
        [Route("ProjectDashboard")]
        [Produces("application/json", Type = typeof(ProjectsDashboardDTO))]
        //public async Task<List<ProjectsDashboardDTO>?> ProjectDashboard(DashboardAction inputs)
        public async Task<List<ProjectsDashboardDTO>?> ProjectDashboard(int? unitId, int? userId)
        {
            ProjectsDashboardDTO ProjectLst = new ProjectsDashboardDTO();
            var parms = new DynamicParameters();

            parms.Add(@"@UnitId", unitId, DbType.Int32);
            parms.Add(@"@UserId", userId, DbType.Int32);

            ProjectLst.DashboardList = (await _unitOfWork.ProjectsDashboard.GetSPData<ProjectsDashboardDTO>("usp_ProjectDashboard", parms));
            return ProjectLst.DashboardList;
        }
        [HttpGet]
        [Route("TaskDashboard")]
        [Produces("application/json", Type = typeof(TasksDashboardDTO))]
        public async Task<List<TasksDashboardDTO>?> TaskDashboard(int? unitId, int? userId)
        {
            ProjectsDashboardDTO ProjectLst = new ProjectsDashboardDTO();
            var parms = new DynamicParameters();

            parms.Add(@"@UnitId", unitId, DbType.Int32);
            parms.Add(@"@UserId", userId, DbType.Int32);

            ProjectLst.TaskList = (await _unitOfWork.TasksDashboard.GetSPData<TasksDashboardDTO>("usp_TaskDashboard", parms));
            return ProjectLst.TaskList;
        }


        [HttpGet]
        [Route("GetProjectDetailsForTemplate")]
        [Produces("application/json", Type = typeof(ProjectDashboardDTO))]
        public async Task<List<ProjectDashboardDTO>?> GetProjectDetailsForTemplate(int? Pid)
        {
            ProjectsDTO ProjectLst = new ProjectsDTO();
            var parms = new DynamicParameters();

            parms.Add(@"@ProjectId", Pid, DbType.Int32);

            ProjectLst.ProjectDashboardList = (await _unitOfWork.ProjectDashboard.GetSPData<ProjectDashboardDTO>("usp_GetProjectForTemplate", parms));
            return ProjectLst.ProjectDashboardList;
        }
    }
}

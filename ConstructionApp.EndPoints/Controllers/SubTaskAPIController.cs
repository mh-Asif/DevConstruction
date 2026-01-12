using AutoMapper;
using Construction.Infrastructure.Helper;
using Construction.Infrastructure.KeyValues;
using Construction.Infrastructure.Models;
using ConstructionApp.Core.Entities;
using ConstructionApp.Core.Repository;
using ConstructionApp.Services.DBContext;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Linq.Expressions;

namespace ConstructionApp.EndPoints.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubTaskAPIController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SubTaskAPIController> _logger;
        private readonly IMapper _mapper;
        private readonly ConstDbContext _constDbContext;


        public SubTaskAPIController(IUnitOfWork unitOfWork, ILogger<SubTaskAPIController> logger, IMapper mapper, ConstDbContext constDbContext)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _constDbContext = constDbContext;

        }

        [HttpPost]
        [Route("SaveSubTask")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> SaveSubTask(ProjectSubTasksDTO inputDTO)
        {
            try
            {
                //inputDTO.FullName = inputDTO.FirstName + " " + inputDTO.LastName;
                OutPutResponse outPut = new OutPutResponse();
                outPut.RespId = 0;
                if (ModelState.IsValid)
                {
                    if (inputDTO.SubTaskId == 0)
                    {
                        var ResponseId = _unitOfWork.ProjectSubTasks.Insert(_mapper.Map<ProjectSubTasks>(inputDTO));
                        outPut.RespId = ResponseId.SubTaskId;
                    }
                    else
                    {
                        ProjectSubTasks outputDetails = _mapper.Map<ProjectSubTasks>(await _unitOfWork.ProjectSubTasks.GetByIdAsync(Convert.ToInt32(inputDTO.SubTaskId)));
                        if (outputDetails != null)
                        {
                            outputDetails.Collaborators = inputDTO.Collaborators;
                          //  outputDetails.IsVendor = inputDTO.IsVendor;
                            outputDetails.StatusId = inputDTO.StatusId;
                            outputDetails.StartDate = inputDTO.StartDate;
                            outputDetails.EndDate = inputDTO.EndDate;
                            outputDetails.Description = inputDTO.Description;
                           // outputDetails.PhaseId = inputDTO.PhaseId;
                            outputDetails.PriorityId = inputDTO.PriorityId;
                            outputDetails.UnitId = inputDTO.UnitId;
                            outputDetails.ProjectId = inputDTO.ProjectId;
                           // outputDetails.VendorId = inputDTO.VendorId;
                            outputDetails.IsActive = inputDTO.IsActive;
                            outputDetails.SubTaskName = inputDTO.SubTaskName;
                            outputDetails.OwnerId = inputDTO.OwnerId;
                            outputDetails.CreatedBy = inputDTO.CreatedBy;
                            outputDetails.CreatedOn = inputDTO.CreatedOn;

                        }
                        Expression<Func<ProjectSubTasks, bool>> expression = a => a.SubTaskId != Convert.ToInt32(inputDTO.SubTaskId) && a.IsActive == true;
                        if (_unitOfWork.ProjectSubTasks.Exists(expression))
                        {
                            _unitOfWork.ProjectSubTasks.Update(_mapper.Map<ProjectSubTasks>(outputDetails));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Sub Task updated successfully!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "Duplicate Entry Found!";
                            return Ok(outPut);
                        }
                    }



                    outPut.DisplayMessage = "Sub Task Saved Successfully";
                    outPut.HttpStatusCode = 200;
                    return Ok(outPut);

                }
                else
                {
                    outPut.DisplayMessage = "Failed";
                    outPut.HttpStatusCode = 201;
                    return Ok(outPut);

                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving Task {nameof(SaveSubTask)}");
                throw;
            }
        }


        [HttpGet]
        [Route("GetSubTaskById")]
        [Produces("application/json", Type = typeof(ProjectSubTasksDTO))]
        public async Task<IActionResult> GetSubTaskById(int pId)
        {
            try
            {
                ProjectSubTasksDTO outputDTO = _mapper.Map<ProjectSubTasksDTO>(await _unitOfWork.ProjectSubTasks.GetByIdAsync(pId));
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
                _logger.LogError(ex, $"Error in retriving Task {nameof(GetSubTaskById)}");
                throw;
            }
        }

        [HttpGet]
        [Route("DeleteSubTask")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeleteSubTask(int pId)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    ProjectSubTasks outputMaster = _mapper.Map<ProjectSubTasks>(await _unitOfWork.ProjectSubTasks.GetByIdAsync(pId));
                    outputMaster.IsActive = false;
                    _unitOfWork.ProjectSubTasks.Update(outputMaster);
                    _unitOfWork.Save();
                    outPut.HttpStatusCode = 200;
                    outPut.DisplayMessage = "Sub Task deleted successfully!";
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
                _logger.LogError(ex, $"Error while deleting task {nameof(DeleteSubTask)}");
                throw;
            }
        }


        [HttpGet]
        [Route("GetTaskDashboard")]
        [Produces("application/json", Type = typeof(ProjectTasksDTO))]
        public async Task<List<TaskDashboardDTO>?> GetTaskDashboard(int? unitId, int? projectId, int? IsVandor)
        {
            ProjectTasksDTO ProjectLst = new ProjectTasksDTO();
            try
            {

                var parms = new DynamicParameters();
                parms.Add(@"@UnitId", unitId, DbType.Int32);
                parms.Add(@"@ProjectId", projectId, DbType.Int32);
                parms.Add(@"@IsVendor", IsVandor, DbType.Int32);
                ProjectLst.ProjectTaskList = (await _unitOfWork.TaskDashboard.GetSPData<TaskDashboardDTO>("usp_GetTaskDashboard", parms));
                return ProjectLst.ProjectTaskList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving Task {nameof(GetTaskDashboard)}");
                throw;
            }
        }



        [HttpGet]
        [Route("GetTaskDetailsForTemplate")]
        [Produces("application/json", Type = typeof(ProjectTasksDTO))]
        public async Task<List<TaskDashboardDTO>?> GetTaskDetailsForTemplate(int? taskId)
        {
            ProjectTasksDTO ProjectLst = new ProjectTasksDTO();
            try
            {

                var parms = new DynamicParameters();
                parms.Add(@"@TaskId", taskId, DbType.Int32);
                ProjectLst.ProjectTaskList = (await _unitOfWork.TaskDashboard.GetSPData<TaskDashboardDTO>("usp_GetTaskForTemplate", parms));
                return ProjectLst.ProjectTaskList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving Task {nameof(GetTaskDetailsForTemplate)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetTaskRoleSummery")]
        [Produces("application/json", Type = typeof(ProjectRoleSummeryDTO))]
        public async Task<List<ProjectRoleSummeryDTO>?> GetTaskRoleSummery(int? unitId, int? taskId)
        {
            ProjectTasksDTO ProjectLst = new ProjectTasksDTO();
            var parms = new DynamicParameters();

            parms.Add(@"@UnitId", unitId, DbType.Int32);
            parms.Add(@"@TaskId", taskId, DbType.Int32);

            ProjectLst.TaskRoleSummeryList = (await _unitOfWork.ProjectRoleSummery.GetSPData<ProjectRoleSummeryDTO>("usp_TaskRoleOverView", parms));
            return ProjectLst.TaskRoleSummeryList;
        }

        [HttpGet]
        [Route("GetSubTaskUsers")]
        [Produces("application/json", Type = typeof(ProjectRoleSummeryDTO))]
        public async Task<List<ProjectRoleSummeryDTO>?> GetSubTaskUsers(int? userId, int? taskId, int? projectId)
        {
            ProjectTasksDTO ProjectLst = new ProjectTasksDTO();
            var parms = new DynamicParameters();

            parms.Add(@"@UserId", userId, DbType.Int32);
            parms.Add(@"@TaskId", taskId, DbType.Int32);
            parms.Add(@"@ProjectId", projectId, DbType.Int32);

            ProjectLst.TaskRoleSummeryList = (await _unitOfWork.ProjectRoleSummery.GetSPData<ProjectRoleSummeryDTO>("usp_GetSubTaskUsers", parms));
            return ProjectLst.TaskRoleSummeryList;
        }

        [HttpGet]
        [Route("GetSubTaskDashboard")]
        [Produces("application/json", Type = typeof(SubTaskDashboardDTO))]
        public async Task<List<SubTaskDashboardDTO>?> GetSubTaskDashboard(int? userId, int? taskId)
        {
            ProjectSubTasksDTO ProjectLst = new ProjectSubTasksDTO();
            var parms = new DynamicParameters();
            parms.Add(@"@UserId", userId, DbType.Int32);
            parms.Add(@"@TaskId", taskId, DbType.Int32);          

            ProjectLst.SubTaskDashboard = (await _unitOfWork.SubTaskDashboard.GetSPData<SubTaskDashboardDTO>("usp_GetSubTaskDashboard", parms));
            return ProjectLst.SubTaskDashboard;
        }
    }
}

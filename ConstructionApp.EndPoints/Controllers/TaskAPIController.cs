using AutoMapper;
using Construction.Infrastructure.Helper;
using Construction.Infrastructure.KeyValues;
using Construction.Infrastructure.Models;
using ConstructionApp.Core.Entities;
using ConstructionApp.Core.Repository;
using ConstructionApp.Services.DBContext;
using Dapper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Linq.Expressions;

namespace ConstructionApp.EndPoints.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskAPIController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TaskAPIController> _logger;
        private readonly IMapper _mapper;
        private readonly ConstDbContext _constDbContext;


        public TaskAPIController(IUnitOfWork unitOfWork, ILogger<TaskAPIController> logger, IMapper mapper, ConstDbContext constDbContext)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _constDbContext = constDbContext;

        }

        [HttpPost]
        [Route("SaveTask")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> SaveTask(TasksDTO inputDTO)
        {
            var outPut = new OutPutResponse { RespId = 0 };
            try
            {
                if (!ModelState.IsValid)
                {
                    outPut.DisplayMessage = "Failed";
                    outPut.HttpStatusCode = 201;
                    return Ok(outPut);
                }

                if (inputDTO.TaskId == 0)
                {
                    var existingTask = _unitOfWork.ProjectTasks.GetAll(p => p.IsActive == true).Result.FirstOrDefault(t => t.TaskName == inputDTO.TaskName && t.ProjectId == inputDTO.ProjectId);
                    if (existingTask == null)
                    {
                        var entity = _mapper.Map<ProjectTasks>(inputDTO);
                        var response = _unitOfWork.ProjectTasks.Insert(entity);
                        outPut.RespId = response.TaskId;
                        outPut.DisplayMessage = "Task Saved Successfully";
                        outPut.HttpStatusCode = 200;
                    }
                    else
                    {
                        outPut.DisplayMessage = "Duplicate Task";
                        outPut.HttpStatusCode = 200;
                    }
                   
                    return Ok(outPut);
                }
                else
                {
                    // Check for duplicate before updating
                    Expression<Func<ProjectTasks, bool>> expression = a => a.TaskId != inputDTO.TaskId && a.IsActive == true;
                    if (_unitOfWork.ProjectTasks.Exists(expression))
                    {
                        var existing = await _unitOfWork.ProjectTasks.GetByIdAsync(inputDTO.TaskId);
                        if (existing != null)
                        {
                            // Only update necessary fields
                            existing.IsVendor = inputDTO.IsVendor;
                            existing.StatusId = inputDTO.StatusId;
                            existing.StartDate = inputDTO.StartDate;
                            existing.EndDate = inputDTO.EndDate;
                            existing.Description = inputDTO.Description;

                            if (inputDTO.FileName != null)
                                existing.FileName = inputDTO.FileName;

                            if (inputDTO.PhaseId > 0)
                            {
                                existing.PhaseId = inputDTO.PhaseId;
                                existing.PriorityId = inputDTO.PriorityId;
                                existing.TaskOwnerId = inputDTO.TaskOwnerId;
                                existing.VendorId = inputDTO.VendorId;
                                existing.Collaborators = inputDTO.Collaborators;
                            }
                            existing.UnitId = inputDTO.UnitId;
                            existing.ProjectId = inputDTO.ProjectId;
                            existing.IsActive = inputDTO.IsActive;
                            existing.TaskName = inputDTO.TaskName;
                            existing.CreatedBy = inputDTO.CreatedBy;
                            existing.CreatedOn = inputDTO.CreatedOn;

                            _unitOfWork.ProjectTasks.Update(existing);
                            _unitOfWork.Save();

                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Task updated successfully!";
                            return Ok(outPut);
                        }
                    }
                    else
                    {
                        outPut.HttpStatusCode = 201;
                        outPut.DisplayMessage = "Duplicate Entry Found!";
                        return Ok(outPut);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving Task {nameof(SaveTask)}");
                throw;
            }

            outPut.DisplayMessage = "Task Saved Successfully";
            outPut.HttpStatusCode = 200;
            return Ok(outPut);
        }


        [HttpGet]
        [Route("GetTaskById")]
        [Produces("application/json", Type = typeof(ProjectTasksDTO))]
        public async Task<IActionResult> GetTaskById(int pId)
        {
            try
            {
                ProjectTasksDTO outputDTO = _mapper.Map<ProjectTasksDTO>(await _unitOfWork.ProjectTasks.GetByIdAsync(pId));
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
                _logger.LogError(ex, $"Error in retriving Task {nameof(GetTaskById)}");
                throw;
            }
        }

        [HttpGet]
        [Route("DeleteTask")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeleteTask(int pId)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    ProjectTasks outputMaster = _mapper.Map<ProjectTasks>(await _unitOfWork.ProjectTasks.GetByIdAsync(pId));
                    outputMaster.IsActive = false;
                    _unitOfWork.ProjectTasks.Update(outputMaster);
                    _unitOfWork.Save();
                    outPut.HttpStatusCode = 200;
                    outPut.DisplayMessage = "Task deleted successfully!";
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
                _logger.LogError(ex, $"Error while deleting task {nameof(DeleteTask)}");
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
        [Route("GetTaskByProject")]
        [Produces("application/json", Type = typeof(ProjectTasksDTO))]
        public async Task<IActionResult> GetTaskByProject(int projectId)
        {
            try
            {
                var transactions = _unitOfWork.ProjectTasks.FindAllByExpression(x => x.ProjectId == projectId && x.IsActive == true);

                if (!transactions.Any())
                {
                    return Ok(new ProjectTasksDTO
                    {
                        TaskList = new List<ProjectTasksDTO>(),
                        DisplayMessage = "No tasks for this project",
                        HttpStatusCode = 404
                    });
                }

                var result = _mapper.Map<List<ProjectTasksDTO>>(transactions);

                var response = new ProjectTasksDTO
                {
                    TaskList = result,
                    HttpStatusCode = 200,
                    DisplayMessage = "Tasks retrieved successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving stock out transactions by item ID {nameof(GetTaskByProject)}");
                return Ok(new StockOutTransactionDTO
                {
                    DisplayMessage = "An error occurred while retrieving stock out transactions",
                    HttpStatusCode = 500
                });
            }
        }



    }
}

using AutoMapper;
using Construction.Infrastructure.Helper;
using Construction.Infrastructure.KeyValues;
using Construction.Infrastructure.Models;
using ConstructionApp.Core.Entities;
using ConstructionApp.Core.Repository;
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
    public class UserCommentsAPIController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserCommentsAPIController> _logger;
        private readonly IMapper _mapper;
        public UserCommentsAPIController(IUnitOfWork unitOfWork, ILogger<UserCommentsAPIController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("GetUserComments")]
        [Produces("application/json", Type = typeof(UserCommentsDTO))]
        public IEnumerable<UserCommentsDTO> GetUserComments(int? userId)
        {
            try
            {
                return (from UC in _unitOfWork.UserComments.GetAll(p => p.IsActive == true && p.UserId == userId).Result
                        join usr in _unitOfWork.UsersMaster.GetAll(p => p.IsActive == true && p.UserId == userId).Result on UC.UserId equals usr.UserId
                        select new UserCommentsDTO()
                        {
                            ID = UC.ID,
                            Comments = UC.Comments,
                            UserId = UC.UserId,
                            CreationDate = UC.CreationDate,
                            UserName = usr.FullName,
                            Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(usr.ProfileImage, 0, usr.ProfileImage.Length)
                        }).ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving Comments {nameof(GetUserComments)}");
                throw;
            }
        }

        [HttpPost]
        [Route("UsersComments")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> UsersComments(UserCommentsDTO inputDTO)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    if (Convert.ToInt32(inputDTO.ID) == 0)
                    {
                        UserCommentsDTO inputData = new UserCommentsDTO();
                        inputData.Comments = inputDTO.Comments;
                        inputData.Summary = inputDTO.Summary;
                        inputData.UserId = inputDTO.UserId;
                        inputData.ProjectId = inputDTO.ProjectId;
                        inputData.TaskId = inputDTO.TaskId;
                        inputData.SubTaskId = inputDTO.SubTaskId;
                        inputData.CreationDate = DateTime.Now;
                        inputData.IsActive = true;

                        _unitOfWork.UserComments.AddAsync(_mapper.Map<UserComments>(inputData));
                        _unitOfWork.Save();
                        outPut.HttpStatusCode = 200;
                        outPut.DisplayMessage = "Comments successfully saved!";
                        return Ok(outPut);
                    }
                    else
                    {
                        UserComments outputDetails = _mapper.Map<UserComments>(await _unitOfWork.UserComments.GetByIdAsync(Convert.ToInt32(inputDTO.ID)));
                        if (outputDetails != null)
                        {
                            outputDetails.Comments = inputDTO.Comments;
                            outputDetails.Summary = inputDTO.Summary;
                            outputDetails.UserId = inputDTO.UserId;
                            outputDetails.ProjectId = inputDTO.ProjectId;
                            outputDetails.TaskId = inputDTO.TaskId;
                            outputDetails.SubTaskId = inputDTO.SubTaskId;

                        }
                        Expression<Func<UserComments, bool>> expression = a => a.ID != Convert.ToInt32(inputDTO.ID) && a.IsActive == true;
                        if (_unitOfWork.UserComments.Exists(expression))
                        {
                            _unitOfWork.UserComments.Update(_mapper.Map<UserComments>(outputDetails));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Comments updated successfully!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "Comments updated failed!";
                            return Ok(outPut);
                        }
                    }
                }
                outPut.HttpStatusCode = 201;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in comments {nameof(UsersComments)}");
                outPut.HttpStatusCode = 400;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);
            }
        }

        [HttpGet]
        [Route("GetCommentsById")]
        [Produces("application/json", Type = typeof(UserCommentsDTO))]
        public async Task<IActionResult> GetCommentsById(int Id)
        {
            try
            {
                UserCommentsDTO outputDTO = _mapper.Map<UserCommentsDTO>(await _unitOfWork.UserComments.GetByIdAsync(Id));
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
                _logger.LogError(ex, $"Error in retriving Comments {nameof(GetCommentsById)}");
                throw;
            }
        }


        [HttpPost]
        [Route("DeleteComments")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeleteComments(UserCommentsDTO inputDTO)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    UserComments outputMaster = _mapper.Map<UserComments>(await _unitOfWork.UserComments.GetByIdAsync(inputDTO.ID));
                    outputMaster.IsActive = false;
                    _unitOfWork.UserComments.Remove(outputMaster);
                    _unitOfWork.Save();
                    outPut.HttpStatusCode = 200;
                    outPut.DisplayMessage = "Comment delete successfully!";
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
                _logger.LogError(ex, $"Error while deleting country {nameof(DeleteComments)}");
                throw;
            }
        }


        [HttpPost]
        [Route("GetCommentsDashboard")]
        [Produces("application/json", Type = typeof(CommentsDashboardDTO))]
        public async Task<List<CommentsDashboardDTO>?> GetCommentsDashboard(CommentsDashboardDTO objInputs)
        {
            CommentsDashboardDTO CommentsLst = new CommentsDashboardDTO();
            try
            {
                var parms = new DynamicParameters();
                parms.Add(@"@TaskId", objInputs.TaskId, DbType.Int32);
                parms.Add(@"@ProjectId", objInputs.ProjectId, DbType.Int32);
                parms.Add(@"@SubTaskId", objInputs.SubTaskId, DbType.Int32);
                parms.Add(@"@UserId", objInputs.UserId, DbType.Int32);
                parms.Add(@"@IsComment", objInputs.IsComment, DbType.Boolean);
                CommentsLst.CommentList = (await _unitOfWork.CommentsDashboard.GetSPData<CommentsDashboardDTO>("usp_CommentsDashboard", parms));
                return CommentsLst.CommentList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in Comments dashboard {nameof(GetCommentsDashboard)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetCommentsDetails")]
        [Produces("application/json", Type = typeof(CommentsDashboardDTO))]
        public async Task<List<CommentsDashboardDTO>?> GetCommentsDetails(int? TaskId,int? ProjectId,int? UserId)
        {
            CommentsDashboardDTO CommentsLst = new CommentsDashboardDTO();
            try
            {
                var parms = new DynamicParameters();
                parms.Add(@"@TaskId", TaskId, DbType.Int32);
                parms.Add(@"@ProjectId", ProjectId, DbType.Int32);               
                parms.Add(@"@UserId", UserId, DbType.Int32);              
                CommentsLst.CommentList = (await _unitOfWork.CommentsDashboard.GetSPData<CommentsDashboardDTO>("usp_CommentsForTemplate", parms));
                return CommentsLst.CommentList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in Comments dashboard {nameof(GetCommentsDetails)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetNotificationDetails")]
        [Produces("application/json", Type = typeof(UserNotificationsDTO))]
        public async Task<List<UserNotificationsDTO>?> GetNotificationDetails(int? UserId,int? Pid)
        {
            UserNotificationsDTO NotificationLst = new UserNotificationsDTO();
            try
            {
                if(Pid == null)
                {
                    Pid = 0;
                }
                var parms = new DynamicParameters();               
                parms.Add(@"@UserId", UserId, DbType.Int32);
                parms.Add(@"@ProjectId", Pid, DbType.Int32);
                NotificationLst.NotificationList = (await _unitOfWork.UserNotifications.GetSPData<UserNotificationsDTO>("usp_GetNotification", parms));
                return NotificationLst.NotificationList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in Notification dashboard {nameof(GetNotificationDetails)}");
                throw;
            }
        }



        [HttpPost]
        [Route("SaveNotification")]
        [Produces("application/json", Type = typeof(UserNotificationsDTO))]
        public async Task<int>? SaveNotification(UserNotificationsDTO objInputs)
        {
         //   UserNotificationsDTO CommentsLst = new UserNotificationsDTO();
            try
            {
                var parms = new DynamicParameters();
                parms.Add(@"@TaskId", objInputs.TaskId, DbType.Int32);
                parms.Add(@"@ProjectId", objInputs.ProjectId, DbType.Int32);
                parms.Add(@"@SubTaskId", objInputs.SubTaskId, DbType.Int32);
                parms.Add(@"@UserId", objInputs.UserId, DbType.Int32);
                parms.Add(@"@Heading", objInputs.Heading, DbType.String);
                parms.Add(@"@NotifyMessage", objInputs.NotifyMessage, DbType.String);
                var result= (await _unitOfWork.UserNotifications.GetStoredProcedure("usp_SaveNotification", parms));
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in Comments dashboard {nameof(SaveNotification)}");
                throw;
            }
        }

        [HttpGet]
        [Route("RemoveNotification")]
        [Produces("application/json", Type = typeof(bool))]
        public async Task<bool> RemoveNotification(int? Id)
        {
          //  UserNotificationsDTO NotificationLst = new UserNotificationsDTO();
            try
            {
                var parms = new DynamicParameters();
                parms.Add(@"@Id", Id, DbType.Int32);

             var status=  await _unitOfWork.UserNotifications.ExecuteQuery("update UserNotifications set NotifyStatus=1 where ID=@Id", parms) ;
                return status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in Notification dashboard {nameof(RemoveNotification)}");
                throw;
            }
        }

    }
}

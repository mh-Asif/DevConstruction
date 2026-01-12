using Construction.Infrastructure.Helper;
using Construction.Infrastructure.Models;
using System.Net.Mail;
using System.Net;
using Construction.Infrastructure.KeyValues;
using System.Data.Common;
using System.Data;
using System.Text;
using static System.Net.WebRequestMethods;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace ConstructionApp.WebUI.Helper
{
    public class ClientMail
    {
        //private readonly EmailService _emailService;

        //public ClientMail(EmailService emailService)
        //{
        //    _emailService = emailService;
        //}
        private string EmailSubject(ProjectDashboardDTO project)
        {
            string subject = "<p><strong>Dear " + project.ProjectName + ",</strong></p>";
            return subject;
        }

        private string EmailLogin(UsersMasterDTO loginDetails)
        {
            string subject = "<p><strong>Dear " + (loginDetails.FullName != null ? loginDetails.FullName : loginDetails.CompanyName) + ",  </strong></p>\r\n<p><a target=\"_blank\" href=\"http://208.110.72.220:91>http://208.110.72.220:91 </a></p>\r\n<p>Your login details are as follows:</p>\r\n<ul>\r\n<li>Email: " + (loginDetails.EmailAddress != null ? loginDetails.EmailAddress : loginDetails.BusinessEmail) + "</li>\r\n<li>Password: " + loginDetails.LoginPassword + "</li>\r\n</ul>\r\n<p>Team &ndash; Support</p>\r\n<p><em>This is an auto-generated e-mail. Kindly contact Admin in case of any queries.</em></p>";
            return subject;
        }

        private string EmailUrlBody(ExternalUsersDTO bodyMsg)
        {
            string subject = "<p><strong>Dear User, " + bodyMsg.CreatedBy + " has shared the link for view and download the file's, Url : <p><a target=\"_blank\" href=\"https://localhost:7128/public/myfiles/" + bodyMsg.UniqueId + " rel =\"noopener\">https://localhost:7128/public/myfiles/</a></p>" + bodyMsg.UniqueId + ",</strong></p><p>" + bodyMsg.CreatedBy + "</p>";
            return subject;
        }
        public async Task<bool>? SendMail(string emailId, string? message, string subject)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    //Credentials = new NetworkCredential("simplihr97@gmail.com", "simpli2pointo"),
                    Credentials = new NetworkCredential("mimicogroup2025@gmail.com", "elep umdg sxxy atyr"),
                    EnableSsl = true,
                };
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("mimicogroup2025@gmail.com"),
                    Subject = subject,
                    //Body = EmailSubject(project),
                    Body = message,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(emailId);
                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool>? MailSent(string emailId, string? message, string subject)
        {

            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("krasa@aisoftmind.com", "krasa Projects"),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(emailId);

                using var smtp = new SmtpClient("server.dnspark.in", 587)
                {
                    Credentials = new NetworkCredential("krasa@aisoftmind.com", "Krasa@2025"),
                    EnableSsl = true
                };

                //await smtp.SendMailAsync(mailMessage);
                 smtp.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
            //try
            //{
            //    await _emailService.SendEmailAsync(emailId, subject, message);
            //    return true;

            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}
        }

        public string RandomString()
        {
            try
            {
                string s = Guid.NewGuid().ToString("N").ToLower()
                      .Replace("1", "").Replace("o", "").Replace("0", "")
                      .Substring(0, 10);
                return s;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static string GetMailTemplate(string fileName)
        {
            string mailContent = string.Empty;
            string filePathName = Path.Combine(Environment.CurrentDirectory, $"MailTemplates\\{fileName}");
            try
            {
                using (StreamReader reader = System.IO.File.OpenText(filePathName))
                {
                    string fileContent = reader.ReadToEnd();
                    if (fileContent != null && fileContent != "")
                    {
                        return fileContent;
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return "";
        }

        public async Task<string>? SendProjectMail(ProjectsDTO userAction)
        {
            bool isMailSend = false;

            if (userAction != null)
            {

                int iCtr = 0;
                //createmail
                //  string actionPath = "https://localhost:7128//";
                // string actionPath = "https://simplihr2uat-web.azurewebsites.net/";
                string actionPath = "http://208.110.72.220:91//";
                string urlPath = "http://208.110.72.220:91/";
                //  string sSubject = "";
                StringBuilder mailBuilder = new StringBuilder();
                string sFileName = "project.html";
                string sTableData = string.Empty;
                try
                {

                    string subject = "", emailId = "", projectUrl = "";
                    String mailTemplate = GetMailTemplate(sFileName);
                    if (userAction.ProjectDashboardList.Count > 0)
                    {

                        string EmployeeEmailId = "";

                        foreach (var item in userAction.ProjectDashboardList)
                        {
                            projectUrl = urlPath + "Project/ProjectOverview/" + item.ProjectId;


                            if (userAction.IsCreated)
                                subject = item.CreatedBy + " assigned a project to you";
                            else
                                subject = item.CreatedBy + " edit a project";


                            if (item.UserProfile == null)
                                item.UserProfile = "MailFormats//user-default.png";
                            else
                                item.UserProfile = "profile//" + item.UserProfile;

                            if (item.OwnerProfile == null)
                                item.OwnerProfile = "MailFormats//user-default.png";
                            else
                                item.OwnerProfile = "profile//" + item.OwnerProfile;

                            var userImage = actionPath + item.UserProfile;
                            var ownerImage = actionPath + item.OwnerProfile;

                            // emailId = "mohd.asif@kabirtechnocrats.com";// item.Email;
                            //  var userImage = "data:image/png;base64," + Convert.ToBase64String(item.UserProfile, 0, item.UserProfile.Length);
                            // var ownerImage = "data:image/png;base64," + Convert.ToBase64String(item.OwnerProfile, 0, item.OwnerProfile.Length);

                            mailTemplate = mailTemplate.Replace("#logoImage#", actionPath + "MailFormats//main-logo.png");
                            mailTemplate = mailTemplate.Replace("#profileImage#", userImage);
                            mailTemplate = mailTemplate.Replace("#chkImage#", actionPath + "MailFormats//check.png");
                            mailTemplate = mailTemplate.Replace("#ownerImage#", ownerImage);
                            mailTemplate = mailTemplate.Replace("#ProjectUrl#", projectUrl);
                            mailTemplate = mailTemplate.Replace("#pDescription#", item.Description);

                            mailTemplate = mailTemplate.Replace("#scalender#", actionPath + "MailFormats//calender.png");
                            mailTemplate = mailTemplate.Replace("#Url#", urlPath);
                            mailTemplate = mailTemplate.Replace("#ecalender#", actionPath + "MailFormats//calender.png");

                            mailTemplate = mailTemplate.Replace("#subject#", subject);
                            mailTemplate = mailTemplate.Replace("#Company#", "");
                            mailTemplate = mailTemplate.Replace("#Project#", item.ProjectName);
                            mailTemplate = mailTemplate.Replace("#projectOwner#", item.ProjectOwner);
                            mailTemplate = mailTemplate.Replace("#stDate#", item.StartDate);
                            mailTemplate = mailTemplate.Replace("#enDate#", item.EndDate);
                            mailTemplate = mailTemplate.Replace("#pCategory#", item.Category);
                            mailTemplate = mailTemplate.Replace("#pPriority#", item.Priority);
                            mailTemplate = mailTemplate.Replace("#pStatus#", item.Status);



                        }
                        foreach (var emails in userAction.ProjectRoleSummeryList)
                        {
                            emailId = emailId + "," + emails.email;
                        }
                        emailId = emailId.TrimEnd(',');
                        //  emailId = "asif.k@ahatechnocrats.com,Mehroz@ahatechnocrats.com";
                        //   emailId = "mohd.asif@kabirtechnocrats.com";
                        var ProjectRole = userAction.ProjectRoleSummeryList.Where(p => p.IsProjectOwner == 0).ToList();
                        
                        if (ProjectRole.Count == 1)
                        {
                            mailTemplate = mailTemplate.Replace("#Collaborators1#", actionPath + "profile//" + (ProjectRole[0].ProfilePic == null ? "user-default.png" : ProjectRole[0].ProfilePic));
                            mailTemplate = mailTemplate.Replace("#Collaborators2#", null);
                            mailTemplate = mailTemplate.Replace("#Collaborators#", ProjectRole[0].UserName);
                        }
                        else if (ProjectRole.Count == 2)
                        {
                            mailTemplate = mailTemplate.Replace("#Collaborators1#", actionPath + "profile//" + (ProjectRole[0].ProfilePic == null ? "user-default.png" : ProjectRole[0].ProfilePic));
                            mailTemplate = mailTemplate.Replace("#Collaborators2#", actionPath + "profile//" + (ProjectRole[1].ProfilePic == null ? "user-default.png" : ProjectRole[1].ProfilePic));
                            mailTemplate = mailTemplate.Replace("#Collaborators#", ProjectRole[0].UserName + ", and " + ProjectRole[1].UserName);
                        }


                        isMailSend = await MailSent(emailId, mailTemplate.Replace('\"', '"'), subject);


                        return "Success";

                    }
                    return "Leave not found";
                }
                catch (Exception ex)
                {
                    return ex.InnerException.Message.ToString();
                }

            }
            return "Inputs are not correct";
            //return isMailSend;
        }

        public async Task<string>? SendTaskMail(ProjectTasksDTO userAction)
        {
            bool isMailSend = false;

            if (userAction != null)
            {

                int iCtr = 0;
                //createmail
                //  string actionPath = "https://localhost:7128//";
                // string actionPath = "https://simplihr2uat-web.azurewebsites.net/";
                string actionPath = "http://208.110.72.220:91//";
                string urlPath = "http://208.110.72.220:91/";
                // StringBuilder mailBuilder = new StringBuilder();
                string sFileName = "task.html";
                //  string sTableData = string.Empty;
                try
                {

                    string subject = "", emailId = "", projectUrl = "";
                    String mailTemplate = GetMailTemplate(sFileName);
                    if (userAction.ProjectTaskList.Count > 0)
                    {

                        string EmployeeEmailId = "";

                        foreach (var item in userAction.ProjectTaskList)
                        {
                            projectUrl = urlPath + "Task/GetTaskView/" + item.eTaskId;


                            if (userAction.IsCreated)
                            {
                                subject = item.CreatedBy + " assigned a task to you";
                            }
                            else
                                subject = item.CreatedBy + " edit a task";

                            // emailId =  item;
                            //var userImage = "data:image/png;base64," + Convert.ToBase64String(item.UserProfile, 0, item.UserProfile.Length);
                            //var ownerImage = "data:image/png;base64," + Convert.ToBase64String(item.OwnerProfile, 0, item.OwnerProfile.Length);

                             if(item.UserProfile == null)
                                item.UserProfile = "MailFormats//user-default.png";
                             else
                                item.UserProfile = "profile//" + item.UserProfile;

                            if (item.OwnerProfile == null)                            
                                item.OwnerProfile = "MailFormats//user-default.png";
                            else
                                item.OwnerProfile = "profile//" + item.OwnerProfile;

                            var userImage = actionPath + item.UserProfile;
                            var ownerImage = actionPath + item.OwnerProfile;

                            mailTemplate = mailTemplate.Replace("#logoImage#", actionPath + "MailFormats//main-logo.png");
                            mailTemplate = mailTemplate.Replace("#profileImage#", userImage);
                            mailTemplate = mailTemplate.Replace("#chkImage#", actionPath + "MailFormats//check.png");
                            mailTemplate = mailTemplate.Replace("#ownerImage#", ownerImage);
                            mailTemplate = mailTemplate.Replace("#TaskUrl#", projectUrl);
                            mailTemplate = mailTemplate.Replace("#tDescription#", item.Description);

                            mailTemplate = mailTemplate.Replace("#scalender#", actionPath + "MailFormats//calender.png");
                            mailTemplate = mailTemplate.Replace("#Url#", urlPath);
                            mailTemplate = mailTemplate.Replace("#ecalender#", actionPath + "MailFormats//calender.png");

                            mailTemplate = mailTemplate.Replace("#subject#", subject);
                            // mailTemplate = mailTemplate.Replace("#Company#", "");
                            mailTemplate = mailTemplate.Replace("#Task#", item.TaskName);
                            mailTemplate = mailTemplate.Replace("#TaskOwner#", item.TaskOwner);
                            mailTemplate = mailTemplate.Replace("#stDate#", item.StartDate);
                            mailTemplate = mailTemplate.Replace("#enDate#", item.EndDate);
                            mailTemplate = mailTemplate.Replace("#tPhase#", item.Phase);
                            mailTemplate = mailTemplate.Replace("#tPriority#", item.Priority);
                            mailTemplate = mailTemplate.Replace("#tStatus#", item.Status);

                            if (item.Vendor != null)
                                mailTemplate = mailTemplate.Replace("#Assign#", "Vendor / Contractor");
                            else
                                mailTemplate = mailTemplate.Replace("#Assign#", "In-house Team");




                        }
                        foreach (var emails in userAction.TaskRoleSummeryList)
                        {
                            emailId = emailId + "," + emails.email;
                        }
                        emailId = emailId.TrimEnd(',');
                        //  emailId = "asif.k@ahatechnocrats.com,Mehroz@ahatechnocrats.com";
                        var ProjectRole = userAction.TaskRoleSummeryList.Where(p => p.IsProjectOwner == 0).ToList();
                        if (ProjectRole.Count == 1)
                        {
                            mailTemplate = mailTemplate.Replace("#Collaborators1#", actionPath + "profile//" + (userAction.TaskRoleSummeryList[0].ProfilePic==null? "user-default.png": userAction.TaskRoleSummeryList[0].ProfilePic));
                            mailTemplate = mailTemplate.Replace("#Collaborators2#", null);
                            mailTemplate = mailTemplate.Replace("#Collaborators#", userAction.TaskRoleSummeryList[0].UserName);
                        }
                        else if (ProjectRole.Count == 2)
                        {
                            mailTemplate = mailTemplate.Replace("#Collaborators1#", actionPath + "profile//" + (userAction.TaskRoleSummeryList[0].ProfilePic == null ? "user-default.png" : userAction.TaskRoleSummeryList[0].ProfilePic));
                            mailTemplate = mailTemplate.Replace("#Collaborators2#", actionPath + "profile//" + (userAction.TaskRoleSummeryList[0].ProfilePic == null ? "user-default.png" : userAction.TaskRoleSummeryList[1].ProfilePic));
                            mailTemplate = mailTemplate.Replace("#Collaborators#", userAction.TaskRoleSummeryList[0].UserName + ", and " + userAction.TaskRoleSummeryList[1].UserName);
                        }
                        isMailSend = await MailSent(emailId, mailTemplate.Replace('\"', '"'), subject);


                        return "Success";

                    }
                    return "Leave not found";
                }
                catch (Exception ex)
                {
                    return ex.InnerException.Message.ToString();
                }

            }
            return "Inputs are not correct";
            //return isMailSend;
        }

        public async Task<string>? SendCommentsMail(ProjectTasksDTO userAction)
        {
            bool isMailSend = false;

            if (userAction != null)
            {

                int iCtr = 0;
                //createmail
                //  string actionPath = "https://localhost:7128//";
                // string actionPath = "https://simplihr2uat-web.azurewebsites.net/";
                string actionPath = "http://208.110.72.220//";
                string urlPath = "http://208.110.72.220/";
                StringBuilder mailBuilder = new StringBuilder();
                string sFileName = "comment.html";
                string sTableData = string.Empty;
                try
                {

                    string subject = "", emailId = "", projectUrl = "";
                    String mailTemplate = GetMailTemplate(sFileName);
                    if (userAction.ActivityList.Count > 0)
                    {

                        string EmployeeEmailId = "";

                        foreach (var item in userAction.ActivityList)
                        {
                            projectUrl = urlPath + "Task/GetTaskView/" + item.eTaskId;


                            if (userAction.IsCreated)
                            {
                                subject = item.FullName + " added a comment";
                            }
                            else
                            {
                                subject = item.FullName + " edit a comment";
                            }
                            //  emailId = "mohd.asif@kabirtechnocrats.com";item.Email;
                            var userImage = item.Base64ProfileImage;
                            // var ownerImage = "data:image/png;base64," + Convert.ToBase64String(item.OwnerProfile, 0, item.OwnerProfile.Length);

                            mailTemplate = mailTemplate.Replace("#logoImage#", actionPath + "MailFormats//main-logo.png");
                            mailTemplate = mailTemplate.Replace("#profileImage#", userImage);
                            // mailTemplate = mailTemplate.Replace("#chkImage#", actionPath + "MailFormats//check.png");
                            mailTemplate = mailTemplate.Replace("#Project#", item.ProjectName);
                            mailTemplate = mailTemplate.Replace("#TaskUrl#", projectUrl);
                            //  mailTemplate = mailTemplate.Replace("#tDescription#", item.Description);

                            // mailTemplate = mailTemplate.Replace("#scalender#", actionPath + "MailFormats//calender.png");
                            mailTemplate = mailTemplate.Replace("#Url#", urlPath);
                            //  mailTemplate = mailTemplate.Replace("#ecalender#", actionPath + "MailFormats//calender.png");

                            mailTemplate = mailTemplate.Replace("#subject#", subject);
                            mailTemplate = mailTemplate.Replace("#Comments#", userAction.Description);
                            mailTemplate = mailTemplate.Replace("#Task#", item.TaskName);
                            mailTemplate = mailTemplate.Replace("#TaskOwner#", item.FullName);
                            mailTemplate = mailTemplate.Replace("#cDate#", userAction.CreatedOn.ToString());
                            //  mailTemplate = mailTemplate.Replace("#enDate#", item.EndDate);
                            //mailTemplate = mailTemplate.Replace("#tPhase#", item.Phase);
                            //mailTemplate = mailTemplate.Replace("#tPriority#", item.Priority);
                            //mailTemplate = mailTemplate.Replace("#tStatus#", item.Status);

                            //if (item.Vendor != null)
                            //    mailTemplate = mailTemplate.Replace("#Assign#", "Vendor / Contractor");
                            //else
                            //    mailTemplate = mailTemplate.Replace("#Assign#", "In-house Team");




                        }
                        foreach (var emails in userAction.TaskRoleSummeryList)
                        {
                            emailId = emailId + "," + emails.email;
                        }
                        emailId = emailId.TrimEnd(',');

                        // emailId = "asif.k@ahatechnocrats.com,Mehroz@ahatechnocrats.com";
                        var ProjectRole = userAction.TaskRoleSummeryList.Where(p => p.IsProjectOwner == 0).ToList();
                        if (ProjectRole.Count == 1)
                        {
                            mailTemplate = mailTemplate.Replace("#Collaborators1#", userAction.TaskRoleSummeryList[0].Base64ProfileImage);
                            mailTemplate = mailTemplate.Replace("#Collaborators2#", null);
                            //   mailTemplate = mailTemplate.Replace("#Collaborators#", userAction.TaskRoleSummeryList[0].UserName);
                        }
                        else if (ProjectRole.Count == 2)
                        {
                            mailTemplate = mailTemplate.Replace("#Collaborators1#", userAction.TaskRoleSummeryList[0].Base64ProfileImage);
                            mailTemplate = mailTemplate.Replace("#Collaborators2#", userAction.TaskRoleSummeryList[1].Base64ProfileImage);
                            // mailTemplate = mailTemplate.Replace("#Collaborators#", userAction.TaskRoleSummeryList[0].UserName + ", and" + userAction.TaskRoleSummeryList[1].UserName);
                        }
                        else if (ProjectRole.Count == 3)
                        {
                            mailTemplate = mailTemplate.Replace("#Collaborators1#", userAction.TaskRoleSummeryList[0].Base64ProfileImage);
                            mailTemplate = mailTemplate.Replace("#Collaborators2#", userAction.TaskRoleSummeryList[1].Base64ProfileImage);
                            mailTemplate = mailTemplate.Replace("#Collaborators2#", userAction.TaskRoleSummeryList[2].Base64ProfileImage);
                            // mailTemplate = mailTemplate.Replace("#Collaborators#", userAction.TaskRoleSummeryList[0].UserName + ", and" + userAction.TaskRoleSummeryList[1].UserName);
                        }
                        isMailSend = await MailSent(emailId, mailTemplate.Replace('\"', '"'), subject);


                        return "Success";

                    }
                    return "Leave not found";
                }
                catch (Exception ex)
                {
                    return ex.InnerException.Message.ToString();
                }

            }
            return "Inputs are not correct";
            //return isMailSend;
        }

        public async Task<string>? SharedUrlOnMail(ExternalUsersDTO userAction)
        {
            bool isMailSend = false;

            if (userAction != null)
            {
                try
                {
                    string subject = "Karsa has share the link";
                    String mailTemplate = EmailUrlBody(userAction);

                    isMailSend = await MailSent(userAction.EmailId, mailTemplate.Replace('\"', '"'), subject);


                    return "Success";


                }
                catch (Exception ex)
                {
                    return ex.InnerException.Message.ToString();
                }

            }
            return "Inputs are not correct";
            //return isMailSend;
        }


        public async Task<bool>? UserLoginMail(UsersMasterDTO userAction)
        {
            bool isMailSend = false;

            if (userAction != null)
            {
                string subject = "Access Your Project Dashboard on Karsa CMS";
                string sFileName = "onboarding.html";
                //  string sTableData = string.Empty;
                try
                {

                    // string subject = "", emailId = "", projectUrl = "";
                    String mailTemplate = GetMailTemplate(sFileName);
                    if (userAction != null)
                    {

                        string EmployeeEmailId = "";

                        mailTemplate = mailTemplate.Replace("#FullName#", (userAction.FullName == null ? userAction.CompanyName : userAction.FullName));
                        mailTemplate = mailTemplate.Replace("#username#", (userAction.EmailAddress == null ? userAction.BusinessEmail : userAction.EmailAddress));
                        mailTemplate = mailTemplate.Replace("#password#", userAction.LoginPassword);
                        // mailTemplate = mailTemplate.Replace("#tStatus#", item.Status);

                        isMailSend = await MailSent((userAction.EmailAddress == null ? userAction.BusinessEmail : userAction.EmailAddress), mailTemplate.Replace('\"', '"'), subject);


                        return isMailSend;

                    }

                    // isMailSend = await SendMail((userAction.EmailAddress == null ? userAction.BusinessEmail : userAction.EmailAddress), mailTemplate.Replace('\"', '"'), subject);


                    // return isMailSend;

                }
                catch (Exception ex)
                {
                    return false;
                }


            }
            return false;
            //return isMailSend;
        }

        public async Task<bool>? ChangePasswordMail(UsersMasterDTO userAction)
        {
            bool isMailSend = false;

            if (userAction != null)
            {
                string subject = "Success! You’ve Updated Your Karsa CMS Password";
                string sFileName = "changepassword.html";
                //  string sTableData = string.Empty;
                try
                {

                    // string subject = "", emailId = "", projectUrl = "";
                    String mailTemplate = GetMailTemplate(sFileName);
                    if (userAction != null)
                    {

                        string EmployeeEmailId = "";

                        mailTemplate = mailTemplate.Replace("#FullName#", (userAction.FullName == null ? userAction.CompanyName : userAction.FullName));
                        mailTemplate = mailTemplate.Replace("#username#", (userAction.EmailAddress == null ? userAction.BusinessEmail : userAction.EmailAddress));
                        //  mailTemplate = mailTemplate.Replace("#password#", userAction.LoginPassword);
                        // mailTemplate = mailTemplate.Replace("#tStatus#", item.Status);

                        isMailSend = await MailSent((userAction.EmailAddress == null ? userAction.BusinessEmail : userAction.EmailAddress), mailTemplate.Replace('\"', '"'), subject);


                        return isMailSend;

                    }

                    // isMailSend = await SendMail((userAction.EmailAddress == null ? userAction.BusinessEmail : userAction.EmailAddress), mailTemplate.Replace('\"', '"'), subject);


                    // return isMailSend;

                }
                catch (Exception ex)
                {
                    return false;
                }


            }
            return false;
            //return isMailSend;
        }

        public async Task<bool>? ForgotPasswordMail(UsersMasterDTO userAction)
        {
            bool isMailSend = false;
          //  EmailService objEmail=new EmailService()
            if (userAction != null)
            {
                string subject = "Your Karsa CMS Password";
                string sFileName = "forgetpassword.html";
                //  string sTableData = string.Empty;
                try
                {

                    // string subject = "", emailId = "", projectUrl = "";
                    String mailTemplate = GetMailTemplate(sFileName);
                    if (userAction != null)
                    {
                       // userAction.EmailAddress = "mh.asif@hotmail.com";
                        string EmployeeEmailId = "";

                        mailTemplate = mailTemplate.Replace("#FullName#", (userAction.FullName == null ? userAction.CompanyName : userAction.FullName));
                        mailTemplate = mailTemplate.Replace("#username#", (userAction.EmailAddress == null ? userAction.BusinessEmail : userAction.EmailAddress));
                        mailTemplate = mailTemplate.Replace("#password#", userAction.LoginPassword);
                        // mailTemplate = mailTemplate.Replace("#tStatus#", item.Status);

                        isMailSend = await MailSent((userAction.EmailAddress == null ? userAction.BusinessEmail : userAction.EmailAddress), mailTemplate.Replace('\"', '"'), subject);
                       // isMailSend= await MailSent("mh.asif@hotmail.com", "Testing", "Subject");

                        return isMailSend;

                    }



                }
                catch (Exception ex)
                {
                    return false;
                }


            }
            return false;
            //return isMailSend;
        }
    }

    public class EmailSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool EnableSSL { get; set; }
        public string SenderName { get; set; }
    }


    public class EmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        //public async Task SendEmailAsync(string toEmail, string subject, string body)
        //{
        //    try
        //    {
        //        var message = new MailMessage
        //        {
        //            From = new MailAddress(_settings.Username, _settings.SenderName),
        //            Subject = subject,
        //            Body = body,
        //            IsBodyHtml = true
        //        };
        //        message.To.Add(toEmail);

        //        using var smtp = new SmtpClient(_settings.Host, _settings.Port)
        //        {
        //            Credentials = new NetworkCredential(_settings.Username, _settings.Password),
        //            EnableSsl = _settings.EnableSSL
        //        };

        //        await smtp.SendMailAsync(message);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception or handle it as needed
        //        Console.WriteLine($"Error sending email: {ex.Message}");
        //    }
        //}

        //public async Task SendEmailAsync(string toEmail, string subject, string body)
        //{
        //    try
        //    {
        //        var message = new MailMessage
        //        {
        //            From = new MailAddress(_settings.Username, _settings.SenderName),
        //            Subject = subject,
        //            Body = body,
        //            IsBodyHtml = true
        //        };
        //        message.To.Add(toEmail);

        //        using var smtp = new SmtpClient(_settings.Host, _settings.Port)
        //        {
        //            Credentials = new NetworkCredential(_settings.Username, _settings.Password),
        //            EnableSsl = _settings.EnableSSL
        //        };

        //        await smtp.SendMailAsync(message);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception or handle it as needed
        //        Console.WriteLine($"Error sending email: {ex.Message}");
        //    }
        //}
    }
}

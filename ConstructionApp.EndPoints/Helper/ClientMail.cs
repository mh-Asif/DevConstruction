using Construction.Infrastructure.Models;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace ConstructionApp.EndPoints.Helper
{
    public class ClientMail
    {
        private string EmailSubject(ProjectDashboardDTO project)
        {
            string subject = "<p><strong>Dear " + project.ProjectName + ",</strong></p>";
            return subject;
        }

        private string EmailUrlBody(ExternalUsersDTO bodyMsg)
        {
            string subject = "<p><strong>Dear User, " + bodyMsg.CreatedBy + " has shared the link for view and download the file's, Url : <p><a target=\"_blank\" href=\"http://208.110.72.220:91/public/myfiles/" + bodyMsg.UniqueId + "\" rel =\"noopener\">http://208.110.72.220:91/public/myfiles/</a></p>" + bodyMsg.OptMessage + ",</strong></p><p>" + bodyMsg.CreatedBy + "</p>";
            return subject;
        }
        public async Task<bool>? SendMail(string emailId, string? message, string subject,string? emailPath)
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
                if(!string.IsNullOrEmpty(emailPath))
                {
                    // attached 
                    string filePath = Path.Combine(emailPath, "Photo-files.pdf");
                    if (System.IO.File.Exists(filePath))
                    {
                        Attachment attachment = new Attachment(filePath);
                        mailMessage.Attachments.Add(attachment);
                    }
                }

                mailMessage.To.Add(emailId);
                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
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

                            // emailId = "mohd.asif@kabirtechnocrats.com";// item.Email;
                           // var userImage = "data:image/png;base64," + Convert.ToBase64String(item.UserProfile, 0, item.UserProfile.Length);
                           // var ownerImage = "data:image/png;base64," + Convert.ToBase64String(item.OwnerProfile, 0, item.OwnerProfile.Length);

                            mailTemplate = mailTemplate.Replace("#logoImage#", actionPath + "MailFormats//main-logo.png");
                            //mailTemplate = mailTemplate.Replace("#profileImage#", userImage);
                            mailTemplate = mailTemplate.Replace("#chkImage#", actionPath + "MailFormats//check.png");
                           // mailTemplate = mailTemplate.Replace("#ownerImage#", ownerImage);
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
                            mailTemplate = mailTemplate.Replace("#Collaborators1#", ProjectRole[0].Base64ProfileImage);
                            mailTemplate = mailTemplate.Replace("#Collaborators2#", null);
                            mailTemplate = mailTemplate.Replace("#Collaborators#", ProjectRole[0].UserName);
                        }
                        else if (ProjectRole.Count == 2)
                        {
                            mailTemplate = mailTemplate.Replace("#Collaborators1#", ProjectRole[0].Base64ProfileImage);
                            mailTemplate = mailTemplate.Replace("#Collaborators2#", ProjectRole[1].Base64ProfileImage);
                            mailTemplate = mailTemplate.Replace("#Collaborators#", ProjectRole[0].UserName + ", and" + userAction.ProjectRoleSummeryList[1].UserName);
                        }
                        isMailSend = await SendMail(emailId, mailTemplate.Replace('\"', '"'), subject,"");


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
                StringBuilder mailBuilder = new StringBuilder();
                string sFileName = "task.html";
                string sTableData = string.Empty;
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

                            emailId = "mohd.asif@kabirtechnocrats.com";// item.Email;
                          //  var userImage = "data:image/png;base64," + Convert.ToBase64String(item.UserProfile, 0, item.UserProfile.Length);
                           // var ownerImage = "data:image/png;base64," + Convert.ToBase64String(item.OwnerProfile, 0, item.OwnerProfile.Length);

                            mailTemplate = mailTemplate.Replace("#logoImage#", actionPath + "MailFormats//main-logo.png");
                           // mailTemplate = mailTemplate.Replace("#profileImage#", userImage);
                            mailTemplate = mailTemplate.Replace("#chkImage#", actionPath + "MailFormats//check.png");
                           // mailTemplate = mailTemplate.Replace("#ownerImage#", ownerImage);
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
                            mailTemplate = mailTemplate.Replace("#Collaborators1#", userAction.TaskRoleSummeryList[0].Base64ProfileImage);
                            mailTemplate = mailTemplate.Replace("#Collaborators2#", null);
                            mailTemplate = mailTemplate.Replace("#Collaborators#", userAction.TaskRoleSummeryList[0].UserName);
                        }
                        else if (ProjectRole.Count == 2)
                        {
                            mailTemplate = mailTemplate.Replace("#Collaborators1#", userAction.TaskRoleSummeryList[0].Base64ProfileImage);
                            mailTemplate = mailTemplate.Replace("#Collaborators2#", userAction.TaskRoleSummeryList[1].Base64ProfileImage);
                            mailTemplate = mailTemplate.Replace("#Collaborators#", userAction.TaskRoleSummeryList[0].UserName + ", and" + userAction.TaskRoleSummeryList[1].UserName);
                        }
                        isMailSend = await SendMail(emailId, mailTemplate.Replace('\"', '"'), subject,"");


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
                string actionPath = "http://208.110.72.220:91//";
                string urlPath = "http://208.110.72.220:91/";
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
                        isMailSend = await SendMail(emailId, mailTemplate.Replace('\"', '"'), subject,"");


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
                    string subject = "Krasa has share the link";
                    String mailTemplate = EmailUrlBody(userAction);

                    isMailSend = await SendMail(userAction.EmailId, mailTemplate.Replace('\"', '"'), subject, userAction.emailPath);


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
    }
}

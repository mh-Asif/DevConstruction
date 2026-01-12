using AutoMapper;
using Construction.Infrastructure.Helper;
using Construction.Infrastructure.Models;
using ConstructionApp.Core.Entities;
using ConstructionApp.Core.Repository;
using ConstructionApp.Services.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionApp.Services.Repository
{
    public class UsersMasterRepository : GenericRepository<UsersMaster>,IUsersMasterRepository
    {
        private readonly ConstDbContext _context;
        private readonly IMapper _mapper;
        public UsersMasterRepository(ConstDbContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }
        //public async Task<IList<UsersMasterDTO>> GetUserListing(Expression<Func<UsersMaster, bool>> expression)
        //{
        //    IList<UsersMasterDTO> outputData = new List<UsersMasterDTO>();
        //    outputData = _mapper.Map<IList<UsersMasterDTO>>(_context.UsersMasters.Where(expression)
        //    //.Include(x => x.Department).Include(x => x.WorkLocation)
        //    // .Include(x => x.EmployeeBankDetails.Where(p => p.IsActive == true)).Include(x => x.EmployeeContactDetails.Where(p => p.IsActive == true))
        //    //.Include(x => x.EmployeeFamilyDetails.Where(p => p.IsActive == true)).Include(x => x.EmployeeAcademicDetails.Where(p => p.IsActive == true))
        //    //.Include(x => x.EmployeeExperienceDetails.Where(p => p.IsActive == true)).Include(x => x.EmployeeCertificationDetails.Where(p => p.IsActive == true))
        //    //.Include(x => x.EmployeeReferenceDetails.Where(p => p.IsActive == true))
        //    .Select(p => p));
        //    //.Include(x => x.EmployeeLanguageDetails.Where(p => p.IsActive == true))

        //    foreach (var employee in outputData)
        //    {
        //        employee.ManagerName = "";
        //        employee.HODName = "";
        //        employee.EnycUserId = CommonHelper.Encrypt(Convert.ToString(employee.UserId));
        //        if (!(employee.MgrId == null || employee.MgrId == 0))
        //        {
        //            employee.ManagerName = _context.UsersMasters.Where(x => x.UserId == employee.MgrId).Select(p => p.FirstName).FirstOrDefault().ToString();
        //        }
        //        if (!(employee.HODId == null || employee.HODId == 0))
        //        {
        //            employee.HODName = _context.UsersMasters.Where(x => x.UserId == employee.HODId).Select(p =>p.FirstName).FirstOrDefault().ToString();
        //        }
        //        if (!(employee.JobTitleId == null || employee.JobTitleId == 0))
        //        {
        //            var jobtitlemaster = _context.JobTitleMasters.Where(x => x.JobTitleId == employee.JobTitleId);
        //            if (jobtitlemaster.Count() > 0)
        //            {
        //                employee.Designation = jobtitlemaster.Select(p => p.JobTitle).FirstOrDefault().ToString();
        //            }
        //            else
        //            {
        //                employee.Designation = "";
        //            }
        //        }
        //        if (!(employee.DepartmentId == null || employee.DepartmentId == 0))
        //        {
        //            var departmentMasters = _context.DepartmentMasters.Where(x => x.DepartmentId == employee.DepartmentId);
        //            if (departmentMasters.Count() > 0)
        //            {
        //                employee.Department = departmentMasters.Select(p => p.DepartmentName).FirstOrDefault().ToString();
        //            }
        //            else
        //            {
        //                employee.Department = "";
        //            }
        //        }
        //        employee.EmployeeStatus = (employee.EmpStatus==1? "Active" : employee.EmpStatus == 0? "InActive":"");
        //        employee.EmpType = (employee.EmpTypeId== "F" ? "Full Time" : employee.EmpTypeId == "P" ? "Part Time" : employee.EmpTypeId == "C" ? "Contract" : employee.EmpTypeId == "I" ? "Internship" : "");

        //    }
        //    return outputData;

        //}
    }
}

using ConstructionApp.Core.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionApp.Core.Entities
{
   // [Table("AccessMaster")]
    public partial class AccessMaster
    {
      //  [Key]
        public int AccessId { get; set; }
        public int RoleId { get; set; }
        public int? ModuleId { get; set; }
        public string? ModuleCode { get; set; }
        public string? ModuleName { get; set; }
        public int? UserId { get; set; }
        public int? DepartmentId { get; set; }
        //public int? DesignationId { get; set; }
        public int? UnitId { get; set; }
        public bool? IsView { get; set; }
        public bool? IsDelete { get; set; }
        public bool? IsAdd { get; set; }
        public bool? IsEdit { get; set; }
        public bool? IsApproval { get; set; }
        public bool? IsActive { get; set; }
       
        //[NavigationProperty]
        //public virtual ModuleMaster Module { get; set; } = null!;
    }
}

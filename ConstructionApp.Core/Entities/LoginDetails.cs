

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConstructionApp.Core.Entities
{
    [Table("LoginDetails")]
    public partial class LoginDetails
    {
        [Key]
        public int LoginId { get; set; }
        public string? LoginUser { get; set; }
        public string? MobileNo { get; set; }
        public string? LoginPassword { get; set; }
        public int? UserId { get; set; }
        public int? ClientId { get; set; }
        public int? UnitId { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}

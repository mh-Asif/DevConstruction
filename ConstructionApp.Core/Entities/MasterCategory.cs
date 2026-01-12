using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionApp.Core.Entities
{
    [Table("MasterCategory")]
    public partial class MasterCategory
    {
        [Key]
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public bool? IsActive { get; set; }
      


    }
}

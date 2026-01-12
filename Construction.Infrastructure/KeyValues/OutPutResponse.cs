using Construction.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construction.Infrastructure.KeyValues
{
    public class OutPutResponse
    {
		public string DisplayMessage { get; set; } = string.Empty;		
		public int? HttpStatusCode { get; set; } = 200;
        public int RespId { get; set; } = 0;

    }
}

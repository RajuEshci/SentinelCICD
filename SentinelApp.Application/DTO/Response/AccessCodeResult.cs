using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelApp.Application.DTO.Response
{
    public class AccessCodeResult
    {
        public bool Success { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}

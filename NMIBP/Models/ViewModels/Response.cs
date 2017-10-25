using NMIBP.BusinessLogic.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NMIBP.Models.ViewModels
{
    public class Response
    {
        public List<string> Headers { get; set; } = new List<string>();

        public List<Result> Results { get; set; } = new List<Result>();
    }
}
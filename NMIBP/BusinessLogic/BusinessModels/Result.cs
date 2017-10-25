using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NMIBP.BusinessLogic.BusinessModels
{
    public class Result
    {
        public string SearchPattern { get; set; } = "";

        public List<string> Times { get; set; } = new List<string>();
    }
}
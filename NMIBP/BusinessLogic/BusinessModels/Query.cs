using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NMIBP.BusinessLogic.BusinessModels
{
    public class Query
    {
        public List<string> Headers { get; set; }

        public string Insert { get; set; }

        public string Select { get; set; }

        public string Partial { get; set; }
    }
}
using NMIBP.BusinessLogic.BusinessModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NMIBP.Models.ViewModels
{
    public class AnalyzeMovie
    {
        [DataType(DataType.Date)]
        public DateTime Start { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        public DateTime End { get; set; } = DateTime.Now;

        public string Granulation { get; set; } = "Days";

        public Response Response { get; set; } = new Response();
    }
}
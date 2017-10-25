using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vmovie = NMIBP.Models.ViewModels.Movie;

namespace NMIBP.BusinessLogic.BusinessModels
{
    public class Response
    {
        public string SQLQuery { get; set; }

        public IEnumerable<Vmovie> Results { get; set; }
    }
}
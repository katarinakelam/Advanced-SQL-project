using NMIBP.BusinessLogic.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NMIBP.Models.ViewModels
{
    public class SearchMovie
    {
        public string Patterns { get; set; }

        public string Operator { get; set; } = "&";

        public string SearchType { get; set; } = "Semantic";

        public string SQLQuery { get; set; } = " ";

        public IEnumerable<Movie> Movies { get; set; } = new List<Movie>();
    }
}
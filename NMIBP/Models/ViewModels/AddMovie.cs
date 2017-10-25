using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NMIBP.Models.ViewModels
{
    public class AddMovie
    {
        public string Title { get; set; }

        public string Categories { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }
    }
}
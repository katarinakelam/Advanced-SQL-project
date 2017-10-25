using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NMIBP.DatabaseLogic
{
    public class Connection
    {
        public string Connect { get; } = "Server=192.168.56.12;Port=5432;User Id=postgres;Password=reverse;Database=movie;";
    }
}
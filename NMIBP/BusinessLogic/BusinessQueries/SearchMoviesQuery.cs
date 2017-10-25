using NMIBP.BusinessLogic.BusinessModels;
using NMIBP.DatabaseLogic;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace NMIBP.BusinessLogic.BusinessQueries
{
    public class SearchMoviesQuery : Connection
    {
        protected bool logQuery(ICollection<string> patterns, string operation)
        {
            using (var conn = new NpgsqlConnection(Connect))
            {
                conn.Open();
                var cmd = new NpgsqlCommand($"INSERT INTO analysis_data(query) VALUES ('{convertToQueryString(patterns, operation)}')", conn);
                return cmd.ExecuteNonQuery() != 0;
            }
        }

        private string convertToQueryString(ICollection<string> patterns, string operation)
        {
            var sb = new StringBuilder();

            foreach (var pattern in patterns.AsEnumerable())
            {
                sb.Append(pattern + operation);
            }
            sb.Remove(sb.Length - operation.Length, operation.Length);
            return sb.ToString();
        }
    }
}
using NMIBP.BusinessLogic.BusinessModels;
using Vmovie = NMIBP.Models.ViewModels.Movie;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace NMIBP.BusinessLogic.BusinessQueries
{
    public class FuzzySearchQuery : SearchMoviesQuery
    {
        public Response Execute(ICollection<string> patterns)
        {
            logQuery(patterns, "||");
            using (var conn = new NpgsqlConnection(Connect))
            {
                conn.Open();
                var similarities = Similarity(patterns);
                var comparision = Comparision(patterns);
                var cmd = new NpgsqlCommand($"select \n\tmovieid,\n" +
                    $"\ttitle headline,\n" +
                    $"\t{similarities} sim\n" +
                    $"from movie\n" +
                    $"where {comparision}\n" +
                    $"order by sim desc", conn);

                var reader = cmd.ExecuteReader();
                var movies = new List<Vmovie>();
                while (reader.Read())
                {
                    movies.Add(new Vmovie
                    {
                        ID = int.Parse(reader[0].ToString()),
                        Title = reader[1].ToString(),
                        Rank = double.Parse(reader[2].ToString())
                    });
                }

                return new Response { SQLQuery = cmd.CommandText, Results = movies };
            }
        }

        private string ConcatenateStrings(ICollection<string> patterns)
        {
            var sb = new StringBuilder();
            foreach (var p in patterns.AsEnumerable())
            {
                sb.AppendFormat("{0} ", p);
            }
            var temp = sb.ToString();
            return temp.Substring(0, temp.Length - 1);
        }

        private string Similarity(ICollection<string> patterns)
        {
            var sb = new StringBuilder();
            var sqlPattern = "similarity(title, '{0}') + ";
            foreach (var p in patterns.AsEnumerable())
            {
                sb.AppendFormat(sqlPattern, p);
            }
            sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }

        private string Comparision(ICollection<string> patterns)
        {
            var sb = new StringBuilder();
            var sqlPattern = "title % '{0}' OR ";
            foreach (var p in patterns.AsEnumerable())
            {
                sb.AppendFormat(sqlPattern, p);
            }
            sb.Remove(sb.Length - 3, 3);
            return sb.ToString();
        }
    }
}
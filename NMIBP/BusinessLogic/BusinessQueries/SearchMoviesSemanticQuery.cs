using NMIBP.BusinessLogic.BusinessModels;
using Vmovie = NMIBP.Models.ViewModels.Movie;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NMIBP.BusinessLogic.BusinessQueries
{
    public class SearchMoviesSemanticQuery : SearchMoviesQuery
    {
        public Response Execute(ICollection<string> patterns, string operation)
        {
            logQuery(patterns, operation);
            using (var conn = new NpgsqlConnection(Connect))
            {
                conn.Open();
                var ts_pattern = convertToSQLPatterns(patterns, operation);

                var cmd = new NpgsqlCommand($"select \n\tmovieid,\n" +
                    $"\tts_headline(title, to_tsquery('english', '{ts_pattern}')) headline,\n" +
                    $"\tts_rank(array[0.2,0.4,0.6,1.0], vector, to_tsquery('english', '{ts_pattern}')) rank\n" +
                    $"from movie\n" +
                    $"where vector @@ to_tsquery('english', '{ts_pattern}')\n" +
                    $"order by rank desc", conn);

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

        private string convertToSQLPatterns(ICollection<string> patterns, string operation)
        {
            var sql_patterns = new List<string>();
            foreach (var pattern in patterns.AsEnumerable())
            {
                sql_patterns.Add($"({pattern.Replace(" ", "&")})");
            }

            if (operation == "||") operation = "|";
            return string.Join(operation, sql_patterns);
        }
    }
}
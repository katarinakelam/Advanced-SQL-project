using NMIBP.BusinessLogic.BusinessModels;
using NMIBP.DatabaseLogic;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NMIBP.BusinessLogic.BusinessQueries
{
    public class GetMovieQuery : Connection
    {
        public Movie Execute(int id)
        {
            using (var conn = new NpgsqlConnection(Connect))
            {
                conn.Open();
                var cmd = new NpgsqlCommand($"select title, categories, summary, description from movie where movieid = {id}", conn);

                var reader = cmd.ExecuteReader();
                var mov = new Movie();
                while (reader.Read())
                {
                    mov.Title = reader[0].ToString();
                    mov.Categories = reader[1].ToString();
                    mov.Summary = reader[2].ToString();
                    mov.Description = reader[3].ToString();
                }

                return mov;
            }
        }
    }
}
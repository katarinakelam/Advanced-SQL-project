using System;
using Npgsql;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NMIBP.DatabaseLogic;
using NMIBP.BusinessLogic.BusinessModels;

namespace NMIBP.BusinessLogic.BusinessCommands
{
    public class AddMovieCommand : Connection
    {
        public bool Execute(Movie mov)
        {
            using (var conn = new NpgsqlConnection(Connect))
            {
                conn.Open();
                var cmd = new NpgsqlCommand(
                    "insert into movie (title, summary, description, categories, vector, title_vector)" +
                    "values(@title, @summary, @description, @categories, " +
                    "setweight(to_tsvector(coalesce(@title, '')), 'A') || setweight(to_tsvector(coalesce(@categories, '')), 'B') " +
                    "|| setweight(to_tsvector(coalesce(@summary,'')), 'C') || setweight(to_tsvector(coalesce(@description,'')), 'D'), " +
                    "to_tsvector('english', coalesce(@title)))", conn);
                cmd.Parameters.AddWithValue("@title", mov.Title);
                cmd.Parameters.AddWithValue("@summary", mov.Summary);
                cmd.Parameters.AddWithValue("@description", mov.Description);
                cmd.Parameters.AddWithValue("@categories", mov.Categories);
                return cmd.ExecuteNonQuery() != 0;
            }
        }

    }
}
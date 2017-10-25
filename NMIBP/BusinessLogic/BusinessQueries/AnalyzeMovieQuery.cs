using NMIBP.BusinessLogic.BusinessModels;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vresponse = NMIBP.Models.ViewModels.Response;

namespace NMIBP.BusinessLogic.BusinessQueries
{
    public class AnalyzeMovieQuery : SearchMoviesQuery
    {
        public Vresponse Execute(DateTime startDate, DateTime endDate, string gran)
        {
            using (var conn = new NpgsqlConnection(Connect))
            {
                conn.Open();

                var gen = gran == "Days" ? generateDaysQuery(startDate, endDate) : generateHoursQuery(startDate, endDate);
                using (var insert = new NpgsqlCommand(gen.Insert, conn))
                {
                    insert.ExecuteNonQuery();
                }

                var resp = new List<Result>();
                var select = new NpgsqlCommand(gen.Select, conn);
                using (var reader = select.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var res = new Result() { SearchPattern = reader[0].ToString() };
                        for (int i = 1; i <= gen.Headers.Count; i++)
                        {
                            res.Times.Add(reader[i].ToString());
                        }
                        resp.Add(res);
                    }
                }
                select.Dispose();

                using (var del = new NpgsqlCommand(gen.Partial, conn))
                {
                    del.ExecuteNonQuery();
                }

                return new Vresponse { Headers = gen.Headers, Results = resp };
            }
        }

        private Query generateDaysQuery(DateTime startDate, DateTime endDate)
        {
            var headers = new List<string>();
            var insert = "create table v (i text); insert into v values ";
            var select = "SELECT * " +
                "FROM crosstab(  $$SELECT query::text q, date(time)::text s, count(date(time))::text c" +
                $"                 from analysis_data where date(time) >= '{startDate.ToString("yyyy-MM-dd")}' and date(time) <= '{endDate.ToString("yyyy-MM-dd")}'" +
                "group by q, s order by q, s$$, $$SELECT i from v order by i$$) as piv (q text, ";
            var c = 0;
            for (var i = startDate; i <= endDate; i = i.AddDays(1))
            {
                var dateStr = i.ToString("yyyy-MM-dd");
                insert += $"('{dateStr}'), ";
                headers.Add(dateStr);
                select += $"c{c++} text, ";
            }
            insert = insert.TrimEnd(new char[] { ' ', ',' });
            select = select.TrimEnd(new char[] { ' ', ',' }) + ")";
            return new Query { Headers = headers, Insert = insert, Select = select, Partial = "drop table v" };
        }

        private Query generateHoursQuery(DateTime startDate, DateTime endDate)
        {
            var headers = new List<string>();
            var insert = "create table v (i text); insert into v values ";
            var select = "SELECT * " +
                "FROM crosstab(  $$SELECT query::text q, to_char(time, 'HH24') s, count(to_char(time, 'HH24'))::text c" +
                $"                 from analysis_data where date(time) >= '{startDate.ToString("yyyy-MM-dd")}' and date(time) <= '{endDate.ToString("yyyy-MM-dd")}'" +
                "group by q, s order by q, s$$, $$SELECT i from v order by i$$) as piv (q text, ";
            var c = 0;
            for (var h = 0; h < 24; h++)
            {
                var header = h.ToString("D2");
                headers.Add(header);
                insert += $"('{header}'), ";
                select += $"c{c++} text, ";
            }
            insert = insert.Substring(0, insert.Length - 2);
            select = select.Substring(0, select.Length - 2) + ")";
            return new Query { Headers = headers, Insert = insert, Select = select, Partial = "drop table v" };
        }
    }
}
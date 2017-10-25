using NMIBP.BusinessLogic.BusinessQueries;
using movie = NMIBP.BusinessLogic.BusinessModels.Movie;
using NMIBP.Models.ViewModels;
using Vmovie = NMIBP.Models.ViewModels.Movie;
using response = NMIBP.BusinessLogic.BusinessModels.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NMIBP.BusinessLogic.BusinessCommands;
using NMIBP.BusinessLogic.BusinessModels;
using Newtonsoft.Json;

namespace NMIBP.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction(nameof(SearchMovies));
        }

        public ActionResult MovieData(int id)
        {
            return View(new GetMovieQuery().Execute(id));
        }

        public ActionResult AddMovies()
        {
            return View(new AddMovie());
        }

        [HttpPost]
        public ActionResult AddMovies(AddMovie vm)
        {
            var mov = new movie();
            try { mov.Title = vm.Title; } catch { }
            try { mov.Summary = vm.Summary; } catch { }
            try { mov.Categories = vm.Categories; } catch { }
            try { mov.Description = vm.Description; } catch { }

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            if (new AddMovieCommand().Execute(mov))
            {
                return RedirectToAction(nameof(SearchMovies), new { title = mov.Title });
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult SearchMovies(string title = "")
        {
            return View(new SearchMovie { Patterns = title, Operator = "&", SearchType = "semantic" });
        }

        [HttpPost]
        public ActionResult SearchMovies(SearchMovie vm)
        {
            var patterns = vm.Patterns.Split('"')
                     .Select((element, index) => index % 2 == 0  // If even index
                                           ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)  // Split the item
                                           : new string[] { element })  // Keep the entire item
                     .SelectMany(element => element).ToList();

            string op;

            if (vm.Operator == "&")
                op = "&";
            else
                op = "||";

            response response = null;

            if ("semantic".Equals(vm.SearchType, StringComparison.InvariantCultureIgnoreCase))
            {
                response = new SearchMoviesSemanticQuery().Execute(patterns, op);
            }

            vm.SQLQuery = response.SQLQuery;

            vm.Movies = (from result in response.Results
                         select new Vmovie
                         {
                             ID = result.ID,
                             Title = result.Title,
                             Rank = result.Rank
                         }).ToList();

            return View(vm);
        }

        [HttpPost]
        public string AutoComplete(string Patterns)
        {
            response autocomplete = null;
            var patterns = Patterns.Split('"')
                        .Select((element, index) => index % 2 == 0
                                              ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                              : new string[] { element })
                        .SelectMany(element => element).ToList();

            autocomplete = new FuzzySearchQuery().Execute(patterns);
            string SQLQuery = autocomplete.SQLQuery;
            IEnumerable<Vmovie> movies = (from d in autocomplete.Results
                                          select new Vmovie
                                          {
                                              Title = d.Title
                                          }).ToList();

            List<string> titles = new List<string>();
            foreach(var movie in movies)
            {
                titles.Add(movie.Title);
            }

            return JsonConvert.SerializeObject(titles);
        }

        public ActionResult AnalyzeMovies()
        {
            return View(new AnalyzeMovie());
        }

        [HttpPost]
        public ActionResult AnalyzeMovies(AnalyzeMovie vm)
        {
            var gran = "Days".Equals(vm.Granulation, StringComparison.InvariantCultureIgnoreCase) ? "Days" : "Hours";
            vm.Response = new AnalyzeMovieQuery().Execute(vm.Start, vm.End, gran);
            return View(vm);
        }
    }
}
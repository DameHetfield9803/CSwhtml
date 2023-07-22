using Lesson13.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace Lesson13.Controllers;

public class MovieController : Controller
{


    [AllowAnonymous]
    public IActionResult About()
    {
        return View();
    }

    [Authorize(Roles = "manager, member")]
    public IActionResult ListMovies()
    {
        // Get a list of all movies from the database
        List<Movie> list = DBUtl.GetList<Movie>(
           @"SELECT * FROM Movie, Genre
                  WHERE Movie.GenreId = Genre.GenreId");
        return View(list);
    }

    [Authorize(Roles = "manager")]
    public IActionResult AddMovie()
    {
        ViewData["Genres"] = GetListGenres();
        return View();
    }

    [Authorize(Roles = "manager")]
    [HttpPost]
    public IActionResult AddMovie(Movie movie)
    {
        string errors = "";

        if (!ModelState.IsValid)
        {
            foreach (var state in ModelState.Values)
            {
                foreach (var error in state.Errors)
                {
                    errors += error;
                }
            }

            ViewData["Genres"] = GetListGenres();

            ViewData["Message"] = "Errors: " + errors;
            ViewData["MsgType"] = "warning";
            return View("AddMovie", movie);
        }
        else
        {
            string insert =
               @"INSERT INTO Movie(Title, ReleaseDate, Price, Duration, Rating, GenreId) 
                 VALUES('{0}', '{1:yyyy-MM-dd}', {2}, {3}, '{4}', {5})";
            int result =
               DBUtl.ExecSQL(insert, movie.Title, movie.ReleaseDate, movie.Price,
                                     movie.Duration, movie.Rating, movie.GenreId);

            if (result == 1)
            {
                TempData["Message"] = "Movie Created";
                TempData["MsgType"] = "success";
            }
            else
            {
                TempData["Message"] = DBUtl.DB_Message;
                TempData["MsgType"] = "danger";
            }
            return RedirectToAction("ListMovies");
        }
    }

    [Authorize(Roles = "manager")]
    [HttpGet]
    public IActionResult EditMovie(string id)
    {
        // Get the record from the database using the id
        string movieSql = @"SELECT MovieId, Title, ReleaseDate, Price, 
                                       Rating, Duration, Genre.GenreId
                                  FROM Movie, Genre
                                 WHERE Movie.GenreId = Genre.GenreId
                                   AND Movie.MovieId = '{0}'";
        List<Movie> lstMovie = DBUtl.GetList<Movie>(movieSql, id);

        // If the record is found, pass the model to the View
        if (lstMovie.Count == 1)
        {
            ViewData["Genres"] = GetListGenres();
            return View(lstMovie[0]); // This line needs modification
        }
        else
        // Otherwise redirect to the movie list page
        {
            TempData["Message"] = "Movie not found.";
            TempData["MsgType"] = "warning";
            return RedirectToAction("ListMovies"); // This line needs modification
        }
    }

    [Authorize(Roles = "manager")]
    [HttpPost]
    public IActionResult EditMovie(Movie movie)
    {
        //string errors = "";

        if (!ModelState.IsValid)
        {
            ViewData["Genres"] = GetListGenres();
            ViewData["Message"] = "Invalid Input";
            ViewData["MsgType"] = "Warning";

            //not sure what is the need of this foreach loop therefore, i commented it and simply just display a simple error message
            /*
            foreach (var state in ModelState.Values)
            {
                foreach (var error in state.Errors)
                {
                    errors += error;
                }
            }
            */

            return View("EditMovie");
        }

        string update =
           @"UPDATE Movie SET Title='{1}', ReleaseDate='{2:yyyy-MM-dd}', Price={3}, Duration={4}, Rating='{5}', GenreId={6} WHERE MovieId={0}"; 

        int result =
           DBUtl.ExecSQL(update, movie.MovieId, movie.Title, movie.ReleaseDate, movie.Price, movie.Duration, movie.Rating, movie.GenreId); 
        if (result == 1)
        {
            TempData["Message"] = "Movie Updated";
            TempData["MsgType"] = "success";
        }
        else
        {
            TempData["Message"] = DBUtl.DB_Message;
            TempData["MsgType"] = "danger";
        }
        return RedirectToAction("ListMovies");
    }


    [Authorize(Roles = "manager")]
    public IActionResult DeleteMovie(int id)
    {
        string select = @"SELECT * FROM Movie 
                              WHERE MovieId={0}";
        DataTable ds = DBUtl.GetTable(select, id);
        if (ds.Rows.Count != 1)
        {
            TempData["Message"] = "Movie record no longer exists.";
            TempData["MsgType"] = "warning";
        }
        else
        {
            string delete = "DELETE FROM Movie WHERE MovieId={0}";
            int res = DBUtl.ExecSQL(delete, id);
            if (res == 1)
            {
                TempData["Message"] = "Movie Deleted";
                TempData["MsgType"] = "success";
            }
            else
            {
                TempData["Message"] = DBUtl.DB_Message;
                TempData["MsgType"] = "danger";
            }
        }
        return RedirectToAction("ListMovies");
    }

    private static SelectList GetListGenres()
    {
        string genreSql = @"SELECT LTRIM(STR(GenreId)) as Value, GenreName as Text FROM Genre";
        List<SelectListItem> lstGenre = DBUtl.GetList<SelectListItem>(genreSql);
        return new SelectList(lstGenre, "Value", "Text");
    }
}
// Damien Foo 21011435 (for C#)
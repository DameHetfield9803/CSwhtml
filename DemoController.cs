using Lesson03.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lesson03.Controllers;

public class DemoController : Controller
{

    public IActionResult Number(int id)
    {
        // Select a different view based on id.
        // Only three views implemented: 0, 1, 2.
        return View("Number" + id);
    }

    public IActionResult Index()
    {
        return View();
    }

    #region "String Extension Methods"

    public IActionResult StringExtMethods()
    {
        return View();
    }

    public IActionResult Stretch()
    {
        string line = HttpContext.Request.Form["Line"];
        string result = line.Stretch();
        ViewData["Result"] = result;
        return View("StringExtMethods");
    }

    public IActionResult UpperLower()
    {
        string line = HttpContext.Request.Form["Line"];
        string result = line.UpperLower();
        ViewData["Result"] = result;
        return View("StringExtMethods");
    }

    #endregion

    public IActionResult ShowTriangle()
    {
        Triangle shape = new();
        Random rnd = new();
        shape.Side1 = rnd.Next(1, 5);
        shape.Side2 = rnd.Next(1, 5);
        shape.Side3 = rnd.Next(1, 5);
        return View("TriView", shape);
    }


    public IActionResult ShowRectangle()
    {
        Rectangle square = new();

        Random rnd = new();
        square.Side1 = rnd.Next(1, 5);
        square.Side2 = rnd.Next(1, 5);
        return View("RectView", square);
    }

    public IActionResult ShowDateTime()
    {
        DateTime dobMary = Convert.ToDateTime("28/2/1993");
        DateTime dobJohn = Convert.ToDateTime("31/7/1994");
        DateTime dobMike = Convert.ToDateTime("31/7/1994");
        DateTime dobPaul = Convert.ToDateTime("12/6/1996");
        AgeModel ageModel = new(dobMary, dobJohn, dobMike, dobPaul);
        return View("AgeView", ageModel);
    }
}
// 21011435 Damien Foo
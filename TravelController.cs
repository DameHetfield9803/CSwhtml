using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Lesson09.Controllers;

public class TravelController : Controller
{
    [AllowAnonymous]
    public IActionResult Index()
    {
        List<Trip> list = DBUtl.GetList<Trip>("SELECT * FROM TravelHighlight");
        return View("Index", list);
    }

    [AllowAnonymous]
    public IActionResult Details(int id)
    {
        string select =
           @"SELECT h.*, u.FullName AS SubmittedBy
             FROM TravelHighlight h, TravelUser u
             WHERE h.UserId = u.UserId
             AND Id={0}";

        //string sql = string.Format(select, id);
        //List<Trip> lstTrip = DBUtl.GetList<Trip>(sql);

        List<Trip> lstTrip = DBUtl.GetList<Trip>(select,id);
        if (lstTrip.Count == 1)
        {
            Trip trip = lstTrip[0];
            return View("Details", trip);
        }
        else
        {
            TempData["Message"] = "Trip Record does not exist";
            TempData["MsgType"] = "warning";
            return RedirectToAction("Index");
        }
    }
    [Authorize(Roles = "admin,user")]
    public IActionResult MyTrips()
    {
        string userid = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        string select = @"SELECT * FROM TravelHighlight 
                          WHERE UserId = '{0}'";
        List<Trip> list = DBUtl.GetList<Trip>(select, userid);
        return View("MyTrips", list);
    }

    [Authorize(Roles = "admin,user")]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize(Roles = "admin,user")]
    [HttpPost]
    public IActionResult Create(Trip trip, IFormFile photo)
    {
        ModelState.Remove("Picture");     // No Need to Validate "Picture" - derived from "Photo".
        ModelState.Remove("SubmittedBy"); // Ignore "SubmittedBy". See claim below.
        if (!ModelState.IsValid)
        {
            return View("Create");
        }
        else
        {
            string userid = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            string picfilename = DoPhotoUpload(trip.Photo);

            string insert = @"INSERT INTO TravelHighlight(Title, City, TripDate, 
                                   Duration, Spending, Story, Picture, UserId)
                              VALUES('{0}','{1}','{2:yyyy-MM-dd}',{3},{4},'{5}','{6}','{7}')";
            //string sql = string.Format(insert, trip.Title, trip.City, trip.TripDate,
            //                         trip.Duration, trip.Spending,
            //                       trip.Story, picfilename, userid);
            if (DBUtl.ExecSQL(insert, trip.Title, trip.City, trip.TripDate,
          trip.Duration, trip.Spending,trip.Story, picfilename, userid) == 1)
            {
                TempData["Message"] = "Trip Successfully Added.";
                TempData["MsgType"] = "success";
                return RedirectToAction("MyTrips");
            }
            else
            {
                ViewData["Message"] = DBUtl.DB_Message;
                ViewData["MsgType"] = "danger";
                return View("Create");
            }
        }
    }

    [Authorize(Roles = "admin,user")]
    public IActionResult Update(int id)
    {
        string userid = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        string select = @"SELECT * FROM TravelHighlight 
                         WHERE Id={0} AND UserId='{1}'";

        //string sql = string.Format(select, id, userid);
        //List<Trip> lstTrip = DBUtl.GetList<Trip>(sql);
        List<Trip> lstTrip = DBUtl.GetList<Trip>(select,userid);
        if (lstTrip.Count == 1)
        {
            Trip trip = lstTrip[0];
            return View(trip);
        }
        else
        {
            TempData["Message"] = "Trip Record does not exist";
            TempData["MsgType"] = "warning";
            return RedirectToAction("MyTrips");
        }
    }

    [Authorize(Roles = "admin,user")]
    [HttpPost]
    public IActionResult Update(Trip trip)
    {
        ModelState.Remove("Photo");       // No Need to Validate "Photo"
        ModelState.Remove("SubmittedBy"); // Ignore "SubmittedBy". See claim below.

        if (!ModelState.IsValid)
        {
            return View("Update", trip);
        }
        else
        {
            string userid = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            string update = @"UPDATE TravelHighlight  
                              SET Title='{2}', City='{3}', Story='{4}',
                                  TripDate='{5:yyyy-MM-dd}', Duration={6}, Spending={7} 
                              WHERE Id={0} AND UserId='{1}'";
           // string sql = string.Format(update, trip.Id, userid,
             //                         trip.Title, trip.City,
               //                       trip.Story, trip.TripDate,
                 //                     trip.Duration, trip.Spending);

            if (DBUtl.ExecSQL(update, trip.Id, userid,
                            trip.Title, trip.City,
                                    trip.Story, trip.TripDate,
                                  trip.Duration, trip.Spending) == 1)
            {
                TempData["Message"] = "Trip Updated";
                TempData["MsgType"] = "success";
            }
            else
            {
                TempData["Message"] = DBUtl.DB_Message;
                ViewData["ExecSQL"] = DBUtl.DB_SQL;
                TempData["MsgType"] = "danger";
            }
            return RedirectToAction("MyTrips");
        }
    }

    [Authorize(Roles = "admin,user")]
    public IActionResult Delete(int id)
    {
        string userid = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        string select = @"SELECT * FROM TravelHighlight 
                         WHERE id={0} AND UserId='{1}'";
        //string sql = string.Format(select, id, userid);
        //DataTable ds = DBUtl.GetTable(sql);
        DataTable ds = DBUtl.GetTable(select,id,userid);
        if (ds.Rows.Count != 1)
        {
            TempData["Message"] = "Trip Record does not exist";
            TempData["MsgType"] = "warning";
        }
        else
        {
            string photoFile = ds.Rows[0]["picture"]!.ToString()!;
            string fullpath = Path.Combine(_env.WebRootPath, "photos/" + photoFile);
            System.IO.File.Delete(fullpath);

            string delete = "DELETE FROM TravelHighlight WHERE id={0}";
            //string sql2 = string.Format(delete, id);
            //int res = DBUtl.ExecSQL(sql2);
            int res = DBUtl.ExecSQL(delete, id);
            if (res == 1)
            {
                TempData["Message"] = "Trip Record Deleted";
                TempData["MsgType"] = "success";
            }
            else
            {
                TempData["Message"] = DBUtl.DB_Message;
                TempData["ExecSQL"] = DBUtl.DB_SQL;
                TempData["MsgType"] = "danger";
            }
        }
        return RedirectToAction("MyTrips");
    }

    private string DoPhotoUpload(IFormFile photo)
    {
        string fext = Path.GetExtension(photo.FileName);
        string uname = Guid.NewGuid().ToString();
        string fname = uname + fext;
        string fullpath = Path.Combine(_env.WebRootPath, "photos/" + fname);
        using (FileStream fs = new(fullpath, FileMode.Create))
        {
            photo.CopyTo(fs);
        }
        return fname;
    }

    private readonly IWebHostEnvironment _env;
    public TravelController(IWebHostEnvironment environment)
    {
        _env = environment;
    }
}
// 21011435 Damien Foo for(C#)
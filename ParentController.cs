using Microsoft.AspNetCore.Mvc;

namespace Lesson03.Controllers;

public class ParentController : Controller
{
    public IActionResult Volunteer(string id)
    {
        return View("Volunteer" + id);
    }

    public IActionResult Submit()
    {
        // Use IFormCollection for shorter coding
        IFormCollection form = HttpContext.Request.Form;
        string name = form["Name"].ToString().Trim();
        string mobile = form["Mobile"].ToString().Trim();
        string postal = form["Postal"].ToString().Trim();
        string activity = form["Activity"].ToString();
        string title = form["Title"].ToString();
        string mon = form["Mon"].ToString();
        string wed = form["Wed"].ToString();
        string fri = form["Fri"].ToString();

        // Mandatory Fields must be entered
        // TODO Task 2: Uncomment the code below to ensure all fields are entered.
        if (ValidUtl.CheckIfEmpty(title, name, mobile, postal, activity))
        {
            ViewData["Message"] = "Please enter all fields.";
            return View("Volunteer");
        }

        // Mobile Phone must be eight digits
        // TODO Task 3: Add code to ensure the mobile phone number is 8 digits.
        //              Uncomment the 'if' and '{ }' pair first.
         if (!mobile.IsInteger() || mobile.Length != 8)
        {
            ViewData["Message"]="Please enter a valid phone number OR mobile phone number must be exactly eight digits. ";
            return View("Volunteer");
        }

        // Determine Number of Days Checked
        // TODO Task 4: Uncomment the code below to ensure at least one day is selcted.
        int days = 0;
        string daysSelected = "";
        if (mon.Equals("Mon"))
        {
            days++;
            daysSelected += "Monday, ";
        }
        if (wed.Equals("Wed"))
        {
            days++;
            daysSelected += "Wednesday, ";
        }
        if (fri.Equals("Fri"))
        {
            days++;
            daysSelected += "Friday, ";
        }

        // Validation - Check at least one day.
        if (days == 0)
        {
            ViewData["Message"] = "Check at least one day.";
            return View("Volunteer");
        }

        daysSelected = daysSelected.Substring(0, daysSelected.Length - 2);
        

        // Display Acknowledge View
        ViewData["FullName"] = title + " " + name;
        ViewData["Activity"] = activity;
        ViewData["Days"] = daysSelected;
        ViewData["Points"] = CalcCreditPoint(activity, days);
        ViewData["HP"] = mobile;
        return View("Submit");
    }

    private static int CalcCreditPoint(string activity, int days)
    {
        int credits = 0;
        if (activity.Equals("Story Telling") ||
            activity.Equals("Art and Craft"))
        {
            credits = days * 10;
        }
        else if (activity.Equals("Traffic Control"))
        {
            credits = days * 5;
        }
        else if (activity.Equals("Music Appreciation"))
        {
            credits = days * 15;
        }
        return credits;
    }
}
// 21011435 Damien Foo
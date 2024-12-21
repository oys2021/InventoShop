using System;
using Microsoft.AspNetCore.Mvc;

namespace ISMS.Controllers{

public class userController:Controller{

private readonly Databasehelper _databaseHelper;

public userController(Databasehelper databasehelper){
_databaseHelper=databasehelper;
}

[HttpGet]
public IActionResult userlogin(){
    return View();
}

[HttpPost]
public IActionResult userlogin(string username,string password){
    var user=_databaseHelper.getUserbyUsername(username);
    
     if (user == null || user[0].password != _databaseHelper.HashPassword(password) || !(user[0].isActive ?? false))
        {

            ViewData["Message"] = "Invalid username or password."; 
            // Console.WriteLine($"{_databaseHelper.HashPassword(password)},{user[0].password}");
            return View();
        }

    else if((user[0].RequirePasswordChange ?? false)){
        HttpContext.Session.SetString("Username", user[0].username);
    // Log the username (optional)
        Console.WriteLine($"User logged in: {user[0].username}");
        
        ViewData["Message"] = "Reguire password Change."; 
        Console.WriteLine($"{ViewData["Message"]}");
        return RedirectToAction("changepassword", "user"); 
    }
   HttpContext.Session.SetString("Username", user[0].username);
    // Log the username (optional)
    Console.WriteLine($"User logged in: {user[0].username}");
    Console.WriteLine($"User logged in: {user[0].role}");

    if (user[0].role == 2){
        return RedirectToAction("home", "assistant"); 
    }
    else{
         return RedirectToAction("home", "admin");
    }

}

public IActionResult logout()
{
    HttpContext.Session.Remove("Username");
    return RedirectToAction("userlogin", "user");
}



[HttpGet]
public IActionResult changepassword(){
    return View();
}

[HttpPost]
public IActionResult changepassword(string oldPassword, string newPassword, string confirmPassword)
{
    // Get the username from the session
    var username = HttpContext.Session.GetString("Username");
    Console.WriteLine($"this is the {username}");

    // Fetch the user from the database by username
    var user = _databaseHelper.getUserbyUsername(username);

    // Check if the user is null or empty
    if (user == null || user.Count == 0)
    {
        ViewData["Message"] = "User not found.";
        Console.WriteLine("User not found.");
        return View();
    }

    Console.WriteLine($"{user[0].password}");
    Console.WriteLine($"{_databaseHelper.HashPassword(oldPassword)}");

    // Check if the old password matches
    if (user[0].password != _databaseHelper.HashPassword(oldPassword))
    {
        ViewData["Message"] = "Old password is incorrect.";
        Console.WriteLine($"{ViewData["Message"]}");
        return View();
    }

    // Check if the new passwords match
    if (newPassword != confirmPassword)
    {
        ViewData["Message"] = "New passwords do not match.";
        Console.WriteLine($"{ViewData["Message"]}");
        return View();
    }

    // Update the password in the database
    Console.WriteLine($"this is my id {user[0].id}");
    var updateResult = _databaseHelper.UpdatePassword(user[0].id, _databaseHelper.HashPassword(newPassword));
    Console.WriteLine($"new update {updateResult}");

    if (updateResult)
    {
        ViewData["Message"] = "Password successfully changed.";
        _databaseHelper.alterPassword(user[0].id);
        
    if (user[0].role == 2){
        return RedirectToAction("home", "assistant"); 
    }
    else{
         return RedirectToAction("home", "admin");
    }
    }
    else
    {
        ViewData["Message"] = "Error occurred while updating the password.";
        Console.WriteLine($"{ViewData["Message"]}");
        return View();
    }
}
public IActionResult welcome(){
    return View();
}

}

}
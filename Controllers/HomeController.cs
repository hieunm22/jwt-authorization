﻿using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FormHTML.Models;
using System.Linq;
using FormHTML.Common;

namespace FormHTML.Controllers
{
  public class HomeController : Controller
  {
    readonly FormModel USER = new FormModel { Username = "hieunm", Password = "1" };

    [HttpGet]
    public IActionResult Index()
    {
      if (Request.Cookies.ContainsKey(Constant.Authorization))
      {
        var auth = Request.Cookies[Constant.Authorization];
        var jwtToken = Helper.DecodeJwtToken(auth);
        string username = jwtToken.Claims.FirstOrDefault(f => f.Type == "username")?.Value;
        FormModel model = new FormModel() { Username = username };
        return View("Welcome", model);
      }
      return View("Login");
    }

    [HttpPost]
    public IActionResult Index(FormModel model)
    {
      if (model.Username == USER.Username && model.Password == USER.Password)
      {
        Authorization();
        return Redirect("/");
      }
      return View("Login_Error");
    }

    [HttpGet]
    public IActionResult Logout()
    {
      DeleteCookie();
      return Redirect("/");
    }

    private void DeleteCookie()
    {
      var header = Request.Cookies.Keys;
      var cookies = Request.Cookies.Select(s => s.Key);
      foreach (var cookie in cookies)
      {
        Response.Cookies.Delete(cookie);
      }
    }

    private void Authorization()
    {
      if (Request.Cookies.ContainsKey(Constant.Authorization))
      {
        Response.Cookies.Delete(Constant.Authorization);
      }
      string token = Helper.Generate(USER);
      Response.Cookies.Append(Constant.Authorization, token);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}

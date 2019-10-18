using FormHTML.Common;
using FormHTML.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Linq;

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
        string exp = jwtToken.Claims.FirstOrDefault(f => f.Type == "exp")?.Value;
        long lExp = long.Parse(exp);
        if (DateTime.UnixEpoch.AddSeconds(lExp) < DateTime.Now) 
        {
          ClearAuthen();
          return View("Login");
        }
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
      ClearAuthen();
      return Redirect("/");
    }

    private void ClearAuthen()
    {
      if (Request.Cookies.ContainsKey(Constant.Authorization))
      {
        Response.Cookies.Delete(Constant.Authorization);
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

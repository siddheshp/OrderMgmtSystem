using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OrdersModelLibrary.Dtos;
using OrdersModelLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace OrdersWebApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly IConfiguration config;

        public AuthController(IConfiguration config)
        {
            this.config = config;
        }
        
        // GET: AuthController/Create
        public ActionResult Register()
        {
            return View();
        }

        // POST: AuthController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }
            user.Role = "Customer";
            //post, user api controller
            var baseUrl = config["ApiBaseUrl"];
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                //POST https://localhost:44357/api/users
                var response = client.PostAsJsonAsync("users", user).Result;
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    ModelState.AddModelError("", "Error while registering user");
                }
            }
            return View(user);
        }

        // GET: AuthController/Create
        public ActionResult Login()
        {
            return View();
        }

        // POST: AuthController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }
            var baseUrl = config["ApiBaseUrl"];
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                //POST https://localhost:44357/api/users/login
                var response = client.PostAsJsonAsync("users/login", user).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseString = response.Content.ReadAsStringAsync().Result;
                    var loginDto = JsonConvert.DeserializeObject<LoginDto>(responseString);
                    //todo
                    SessionHelper.SetObjectAsJson(HttpContext.Session, "loginDto", loginDto);
                    return RedirectToAction("Index", "Products");
                }
                else
                {
                    ModelState.AddModelError("", "Error while registering user");
                }
            }
            return View(user);
        }

        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }
    }
}

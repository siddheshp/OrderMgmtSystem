using Microsoft.AspNetCore.Authentication.JwtBearer;
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
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace OrdersWebApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IConfiguration config;

        public ProductsController(IConfiguration config)
        {
            this.config = config;
        }
        // GET: ProductsController
        public ActionResult Index()
        {
            var baseUrl = config["ApiBaseUrl"];
            List<Product> productList = new List<Product>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                //https://localhost:44357/api/products
                var response = client.GetAsync("products").Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseString = response.Content.ReadAsStringAsync().Result;
                    productList = JsonConvert.DeserializeObject<List<Product>>(responseString);
                    return View(productList);
                }
                else
                {
                    ModelState.AddModelError("", "Error while calling API");
                }
            }
            return View(productList);
        }

        // GET: ProductsController/Details/5
        public ActionResult Details(int id)
        {
            var product = GetProductById(id);
            if (product == null)
            {
                ModelState.AddModelError("", "Error calling API");
            }
            return View(product);
        }

        // GET: ProductsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product)
        {
            var baseUrl = config["ApiBaseUrl"];
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                //set token
                var loginDto = SessionHelper.GetObjectFromJson<LoginDto>(
                    HttpContext.Session, "loginDto");
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(
                        JwtBearerDefaults.AuthenticationScheme, loginDto.Token);

                //POST https://localhost:44357/api/products
                var response = client.PostAsJsonAsync("products", product).Result;
                if (response.IsSuccessStatusCode)
                {
                    //var responseString = response.Content.ReadAsStringAsync().Result;
                    //product = JsonConvert.DeserializeObject<Product>(responseString);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Error while calling API");
                }
            }
            return View(product);
        }

        // GET: ProductsController/Edit/5
        public ActionResult Edit(int id)
        {
            var product = GetProductById(id);
            if (product == null)
            {
                ModelState.AddModelError("", "Error calling API");
            }
            return View(product);
        }

        Product GetProductById(int id)
        {
            var baseUrl = config["ApiBaseUrl"];
            Product product = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                //GET: https://localhost:44357/api/products/{id}
                var response = client.GetAsync($"products/{id}").Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseString = response.Content.ReadAsStringAsync().Result;
                    product = JsonConvert.DeserializeObject<Product>(responseString);
                }
            }
            return product;
        }

        // POST: ProductsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Product product)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Validation failed");
                return View(product);
            }
            if (id != product.Id)
            {
                ModelState.AddModelError("", "Ids do not match");
                return View(product);
            }

            var baseUrl = config["ApiBaseUrl"];
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                //Put: https://localhost:44357/api/products/{id}
                var response = client.PutAsJsonAsync($"products/{id}", product).Result;
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(product);
        }

        // GET: ProductsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProductsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

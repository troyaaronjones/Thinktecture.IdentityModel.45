﻿using System;
using System.Net.Http;
using System.Web.Mvc;
using Thinktecture.IdentityModel.Clients;
using Thinktecture.Samples;

namespace OAuth2CodeFlow.Controllers
{
    public class CallbackController : Controller
    {
        static Uri _baseAddress = new Uri(Constants.WebHostBaseAddress);
        //static Uri _baseAddress = new Uri(Constants.SelfHostBaseAddress);

        public ActionResult Index()
        {
            var code = Request.QueryString["code"];

            if (!string.IsNullOrWhiteSpace(code))
            {
                ViewBag.Code = code;
            }
            else
            {
                ViewBag.Code = "none";
            }

            return View();
        }

        [HttpPost]
        [ActionName("Index")]
        public ActionResult Postback()
        {
            var client = new OAuth2Client(
                new Uri("https://idsrv.local/issue/oauth2/token"),
                "codeflowclient",
                "secret");

            var code = Request.QueryString["code"];

            var response = client.RequestAccessTokenCode(code);

            return View("Postback", response);
        }

        [HttpPost]
        [ActionName("CallService")]
        public ActionResult CallService(string token)
        {
            var client = new HttpClient
            {
                BaseAddress = _baseAddress
            };

            client.SetBearerToken(token);

            var response = client.GetAsync("identity").Result;
            response.EnsureSuccessStatusCode();

            var claims = response.Content.ReadAsAsync<ViewClaims>().Result;

            return View("Claims", claims);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using UrlShortner.Data;

namespace UrlShortner.Web.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            var repo = new UserRepo(Properties.Settings.Default.ConStr);
            ViewBag.User = repo.GetByEmail(User.Identity.Name);
            return View();
        }
        [Authorize]
        [Route("Histroy/{id}")]
        public ActionResult ViewHistory(int id)
        {
            ViewBag.Url = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, "");
            var repo = new UrlRepo(Properties.Settings.Default.ConStr);
            return View(repo.GetUrlsByUser(id));
        }
        [Authorize]
        [Route("ShortenUrl/{id}")]
        public ActionResult ShortenUrl(int id)
        {
            return View(id);
        }
        [HttpPost]
        public ActionResult NewShortUrl (int id, string url)
        {
            ViewBag.Url = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, "");
            var repo = new UrlRepo(Properties.Settings.Default.ConStr);
            var newUrl = repo.ShortenUrl(url, id);
            return Json(new
            {
                Id = newUrl.Id,
                ShortUrl = $"{ViewBag.Url}/{newUrl.ShortUrl}",
                RealUrl = newUrl.RealUrl,
            });
        }

        [Route("{shortUrl}")]
        public ActionResult LinkToTheWebSite(string shortUrl)
        {
            var repo = new UrlRepo(Properties.Settings.Default.ConStr);
            return Redirect(repo.GetRealUrl(shortUrl));
        }
        public ActionResult NewUser()
        {
            return View();
        }
        [HttpPost]
        public ActionResult NewUser(User user, string password)
        {
            var repo = new UserRepo(Properties.Settings.Default.ConStr);
            repo.NewUser(user,password);
            FormsAuthentication.SetAuthCookie(user.Email, true);
            return RedirectToAction("index");
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login (string email, string password)
        {
            var repo = new UserRepo(Properties.Settings.Default.ConStr);
            var user = repo.Login(email, password);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            FormsAuthentication.SetAuthCookie(user.Email, true);
            return RedirectToAction("index");
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}
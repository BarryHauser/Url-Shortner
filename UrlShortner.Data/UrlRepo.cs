using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using shortid;

namespace UrlShortner.Data
{
    public class UrlRepo
    {
        private string _connectionString;
        public UrlRepo(string connectionString)
        {
            _connectionString = connectionString;
        }
        public IEnumerable<Url> GetUrlsByUser(int id)
        {
            using(var context = new UrlsDataContext(_connectionString))
            {
                return context.Urls.Where(u => u.UserId == id).ToList();
            }
        }
        public void NewUrl()
        {
            var id = ShortId.Generate( true, false);
        }
        public Url ShortenUrl(string url, int id)
        {
            var newUrl = CheckIfUserShortenedThisUrl(url, id);
            if(newUrl != null)
            {
                return newUrl;
            }
            using (var context = new UrlsDataContext(_connectionString))
            {
                newUrl = new Url
                {
                    RealUrl = url,
                    ShortUrl = ShortId.Generate(true, false),
                    UserId = id,
                };
                context.Urls.InsertOnSubmit(newUrl);
                context.SubmitChanges();
                return newUrl;
            }

        }
        private Url CheckIfUserShortenedThisUrl(string url, int id)
        {
            using (var context = new UrlsDataContext(_connectionString))
            {
                return context.Urls.Where(u => u.UserId == id).FirstOrDefault(u => u.RealUrl == url);
            }
        }

        public string GetRealUrl (string url)
        {
            using (var context = new UrlsDataContext(_connectionString))
            {
                var currentUrl = context.Urls.FirstOrDefault(u => u.ShortUrl == url);
                if(currentUrl == null)
                {
                    return null;
                }
                context.ExecuteCommand("UPDATE Urls set TimesUsed = TimesUsed + 1 Where id = {0}", currentUrl.Id);
                return currentUrl.RealUrl;
            }
        }
    }
}

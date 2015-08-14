using System;
using System.Web.Mvc;
using Octokit;

namespace Cato.Controllers
{
    //http://www.saguiitay.com/2014/06/03/playing-with-github-api-octokit-net/
    public class HomeController : Controller
    {
        private string _clientId = "";
        private string _clientSecret = "";
        private GitHubClient _github = new GitHubClient(new ProductHeaderValue("Cato"), new Uri("https://github.com/"));

        public ActionResult Auth(string code)
        {
            code = Request.QueryString["code"];

            var oauthTokenRequest = new OauthTokenRequest(_clientId, _clientSecret, code)
            {
                RedirectUri = new Uri("http://localhost/Cato.Web/home/auth")
            };

            var oauthToken = _github.Oauth.CreateAccessToken(oauthTokenRequest).Result;
            //github.Credentials = new Credentials(oauthToken.AccessToken);

            Session["token"] = oauthToken.AccessToken;

            return RedirectToAction("Index");
        }

        public ActionResult Login()
        {
            var oauthLoginRequest = new OauthLoginRequest(_clientId);
            oauthLoginRequest.Scopes.Add("user");
            oauthLoginRequest.Scopes.Add("public_repo");
            //oauthLoginRequest.Scopes.Add("private_repo");

            var loginUrl = _github.Oauth.GetGitHubLoginUrl(oauthLoginRequest);

            return Redirect(loginUrl.ToString());
        }

        //
        // GET: /Home/
        public ActionResult Index()
        {

            //github.Credentials = new Credentials("f0f911b0cbccb5cadd20a969cd7c1239df771d5f");



            _github.Credentials = new Credentials((string)Session["token"]);



            var searchRepositoriesRequest = new SearchRepositoriesRequest("erm")
            {
                Language = Language.CSharp,
                Order = SortDirection.Descending,
                PerPage = 10,
            };

            var repo = _github.Repository.Get("ermtech", "erm.trunk").Result;

            var commits = _github.Repository.Commits.GetAll("ermtech", "erm.trunk").Result;



            SearchRepositoryResult searchRepositoryResult = _github.Search.SearchRepo(searchRepositoriesRequest).Result;

            return View(searchRepositoryResult.Items);
        }

    }
}

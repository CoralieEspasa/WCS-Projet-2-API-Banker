using Microsoft.AspNetCore.Mvc;

namespace API_Banker
{
    [Route("[controller]")]
    public class HomeController : Controller
    {
        // GET: Affiche la Page d'accueil
        [Route("Index")]
        public ActionResult Index()
        {
            return View();
        }
    }
}

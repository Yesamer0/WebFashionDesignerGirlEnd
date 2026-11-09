using Microsoft.AspNetCore.Mvc;

namespace WebFashionDesignerGirl.Controllers
{
    public class CommunityController : Controller
    {
        public IActionResult Index()
        {
            return View("Topluluk");
        }
    }
}
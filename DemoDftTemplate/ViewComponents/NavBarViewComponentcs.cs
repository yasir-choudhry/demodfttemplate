using Microsoft.AspNetCore.Mvc;

namespace RVSS.Web.ViewComponents
{
    public class NavBarViewComponent : ViewComponent
    {

        public NavBarViewComponent()
        {

        }
        public IViewComponentResult Invoke()
        {
            return View("NavBar");
        }
    }
}

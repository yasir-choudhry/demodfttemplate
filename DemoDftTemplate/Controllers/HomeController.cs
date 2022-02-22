using DemoDftTemplate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DemoDftTemplate.Controllers
{
    // Can set on whole controller by using here or individual ones (see Contact)
    //[Authorize(Policy = "HasAdminRights")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Disallowed()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        // Can set on individual ones
        //[Authorize(Policy = "HasDevRights")]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null)
        {
            ErrorViewModel vModel = new ErrorViewModel();

            if (statusCode == null)
            {
                var ex = HttpContext.Features.Get<IExceptionHandlerFeature>();

                vModel.RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
                vModel.Message = ex.Error.Message;
                vModel.StackTrace = ex.Error.StackTrace.Replace("\r\n", "<br /><br />");

                return View(vModel);
            }
            else
            {
                string reasonPhrase = ReasonPhrases.GetReasonPhrase((int)statusCode);
                vModel.Message = "Error code " + statusCode.ToString() + ": " + reasonPhrase;
                vModel.StatusCode = statusCode.ToString();
                return View(vModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Error(ErrorViewModel anError)
        {
            string Message = anError.Message;

            //TO DO: Need to add the code for sending the email to DS helpdesk once we have agreed way of sending mail

            return View();
        }
        
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                if (model.ReturnUrl != null)
                {
                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }
        }

    }
}

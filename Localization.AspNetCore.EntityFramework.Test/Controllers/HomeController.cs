using System.Diagnostics;
using Localization.AspNetCore.EntityFramework.Providers.Interfaces;
using Localization.AspNetCore.EntityFramework.Test.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Localization.AspNetCore.EntityFramework.Test.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILocalizationProvider _localizationManager;
        private readonly IStringLocalizer _localizer;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger,
            IStringLocalizer<HomeController> localizer,
            ILocalizationProvider localizationManager)
        {
            _logger = logger;
            _localizer = localizer;
            _localizationManager = localizationManager;
        }

        public IActionResult Index()
        {
            var z = _localizer["Test5"];
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}
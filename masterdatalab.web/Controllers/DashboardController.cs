using masterdatalab.domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace masterdatalab.web.Controllers
{
    public class DashboardController(DashboardService service) : Controller
    {
        public async Task<IActionResult> Index(DateTime? de, DateTime? ate)
            => View(await service.MontarAsync(de, ate));
    }
}

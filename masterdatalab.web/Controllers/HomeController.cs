using masterdatalab.domain.Models.DTOs.Cliente;
using masterdatalab.domain.Models.DTOs.Produtos;
using masterdatalab.domain.Models.DTOs.TabelaPreco;
using masterdatalab.domain.Repositories;
using masterdatalab.web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace masterdatalab.web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

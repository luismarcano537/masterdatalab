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

        public async Task<IActionResult> TesteTabelaPreco([FromServices] TabelaPrecoRepository repo)
        {
            var total = await repo.CountAsync();
            var lista = await repo.GetAllAsync(1, 10);
            var doCliente3 = await repo.GetByClienteAsync(3);
            var preco = await repo.GetPrecoAsync(3, 1006);        // esperado: 12,00
            var inexistente = await repo.GetPrecoAsync(3, 9999);  // esperado: null

            var novoId = await repo.InsertAsync(new TabelaPrecoRequest
            {
                CodCliente = 4,
                CodProduto = 1001,
                ValorUnit = 9.90m
            });

            var precoNovo = await repo.GetPrecoAsync(4, 1001);    // esperado: 9,90
            var apagou = await repo.DeleteAsync(novoId);          // esperado: true
            var apagouDeNovo = await repo.DeleteAsync(novoId);    // esperado: false

            return Json(new { total, lista, doCliente3, preco, inexistente, novoId, precoNovo, apagou, apagouDeNovo });
        }
    }
}

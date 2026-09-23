using JJMasterData.Web.Extensions;
using masterdatalab.domain.Models.ViewModels;
using masterdatalab.domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace masterdatalab.web.Controllers
{
    public class PedidoItensController(PedidoItemService itemService) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Create(int codPedido)
        {
            var panel = await itemService.SetupDataPanelItemAsync(codPedido);
            var result = await panel.GetResultAsync();

            if(result is IActionResult actionResult)
                return actionResult;

            ViewBag.Content = result.HtmlContent;
            return View(new PedidosViewModel { DataPanel = panel, CodPedido = codPedido });
        }

        [HttpPost]
        public async Task<IActionResult> Create(int codPedido, string? submitAction)
        {
            var panel = await itemService.SetupDataPanelItemAsync(codPedido);
            var result = await panel.GetResultAsync();

            if (result is IActionResult actionResult)
                return actionResult;

            var vm = await itemService.SalvarItemAsync(panel);

            if (!vm.TemErros)
                return RedirectToAction("Edit", "Pedidos", new { codPedido });

            vm.CodPedido = codPedido;
            ViewBag.Content = (await panel.GetResultAsync()).HtmlContent;
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (codPedido, erro) = await itemService.ExcluirItemAsync(id);

            if (codPedido is null)
                return RedirectToAction("Index", "Pedidos");

            if (erro is not null)
                TempData["Erro"] = erro;

            return RedirectToAction("Edit", "Pedidos", new { codPedido });
        }
    }
}

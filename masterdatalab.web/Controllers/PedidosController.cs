using JJMasterData.Web.Extensions;
using masterdatalab.domain.Models.ViewModels;
using masterdatalab.domain.Services;
using masterdatalab.web.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace masterdatalab.web.Controllers
{
    public class PedidosController(PedidoService service, PedidoItemService itemService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var result = await service.SetupFormViewAsync();

            if (result is IActionResult actionResult)
                return actionResult;

            ViewBag.Content = result.HtmlContent;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var panel = await service.SetupDataPanelPedidosAsync(null);
            var result = await panel.GetResultAsync();

            if (result is IActionResult actionResult)
                return actionResult;

            ViewBag.Content = result.HtmlContent;
            return View(new PedidosViewModel { DataPanel = panel });
        }

        [HttpPost]
        public async Task<IActionResult> Create(string? submitAction)
        {
            var panel = await service.SetupDataPanelPedidosAsync(null);
            var result = await panel.GetResultAsync();

            if (result is IActionResult actionResult)
                return actionResult;

            var vm = await service.SalvarAsync(panel);

            if (!vm.TemErros)
            {
                TempData["Mensagem"] = "Pedido criado. Inclua os itens.";
                return RedirectToAction("Edit", new { codPedido = vm.CodPedido });
            }

            if (vm.Erros.Count > 0)
                vm.MensagemHtml = Mensagens.Erro(vm.Erros);

            ViewBag.Content = (await panel.GetResultAsync()).HtmlContent;
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int codPedido)
        {
            return await MontarEditAsync(codPedido, false);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int codPedido, string? submitAction)
        {
            return await MontarEditAsync(codPedido, salvar: submitAction == "save");
        }

        private async Task<IActionResult> MontarEditAsync(int codPedido, bool salvar)
        {
            var panel = await service.SetupDataPanelPedidosAsync(codPedido);
            var result = await panel.GetResultAsync();
            if (result is IActionResult actionResult)
                return actionResult;

            var status = await service.ObterStatusAsync(codPedido) ?? 0;
            var itens = await itemService.SetupFormViewItemAsync(codPedido, status);
            var resultItens = await itens.GetResultAsync();

            if (resultItens is IActionResult ri)
                return ri;

            var vm = new PedidosViewModel { DataPanel = panel, CodPedido = codPedido };

            if (salvar)
            {
                vm = await service.SalvarAsync(panel);
                if (!vm.TemErros)
                {
                    TempData["Mensagem"] = "Pedido atualizado.";
                    return RedirectToAction("Edit", new { codPedido });
                }
                vm.CodPedido = codPedido;
                result = await panel.GetResultAsync();
            }

            if (TempData["Erro"] is string erroTemp)
                vm.Erros.Add(erroTemp);

            if (vm.Erros.Count > 0)
            {
                vm.MensagemHtml = Mensagens.Erro(vm.Erros);
            }
            else if (TempData["Mensagem"] is string msg)
            {
                vm.MensagemHtml = Mensagens.Sucesso(msg);
            }


            ViewBag.ContentHeader = result.HtmlContent;
            ViewBag.ContentItens = resultItens.HtmlContent;
            return View("Edit", vm);
        }
    }
}

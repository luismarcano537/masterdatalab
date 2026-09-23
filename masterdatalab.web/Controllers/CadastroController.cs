using JJMasterData.Web.Extensions;
using masterdatalab.domain.Models.DTOs.Produtos;
using masterdatalab.domain.Repositories;
using masterdatalab.domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace masterdatalab.web.Controllers
{
    public class CadastrosController(CadastroService service) : Controller
    {
        [Route("Cadastros/{elementName}")]
        public async Task<IActionResult> Index(string elementName)
        {
            if (!CadastroService.Elementos.ContainsKey(elementName))
                return NotFound();

            var result = await service.SetupFormViewAsync(elementName);

            if (result is IActionResult actionResult)
                return actionResult;

            ViewBag.Content = result.HtmlContent;
            ViewBag.Titulo = CadastroService.Elementos[elementName].Menu;
            return View();
        }
    }
}
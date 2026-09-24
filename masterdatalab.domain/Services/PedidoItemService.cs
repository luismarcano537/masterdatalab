using JJConsulting.Html.Bootstrap.Components;
using JJMasterData.Commons.Data.Entity.Repository.Abstractions;
using JJMasterData.Core.DataDictionary.Models;
using JJMasterData.Core.DataDictionary.Models.Actions;
using JJMasterData.Core.UI.Components;
using masterdatalab.domain.Models.Enums;
using masterdatalab.domain.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using JJConsulting.FontAwesome;
using masterdatalab.domain.Repositories;

namespace masterdatalab.domain.Services
{
    public class PedidoItemService(IComponentFactory factory,
        IHttpContextAccessor contextAccessor,
        LinkGenerator linkGen,
        IEntityRepository repository,
        PedidoService pedidoService,
        PedidoRepository pedidoRepo,
        ItemPedidoRepository itemRepo,
        TabelaPrecoRepository precoRepo)
    {
        public async Task<JJFormView> SetupFormViewItemAsync(int codPedido, int statusPedido)
        {
            var formView = await factory.FormView.CreateAsync("ItemPedidos");
            var http = contextAccessor.HttpContext!;
            var pendente = (int)SituacaoPedido.PendenteConfirmacao;

            formView.UserValues["pedido_status"] = statusPedido;

            // Toolbar Action Add item
            formView.GridView.AddToolbarAction(new UrlRedirectAction
            {
                Name = "addItem",
                Text = "Novo item",
                ShowAsButton = true,
                UrlRedirect = linkGen.GetPathByAction(http, "Create", "PedidoItens") + $"?codPedido={codPedido}",
                EnableExpression = $"exp:'{{pedido_status}}' = {pendente}",
                Order = 1
            });

            // Grid action remove item
            formView.GridView.AddGridAction(new ScriptAction
            {
                Name = "removeItem",
                Tooltip = "Remover item",
                Icon = FontAwesomeIcon.Trash,
                OnClientClick = "confirmarExclusao({id})",
                EnableExpression = $"exp:'{{pedido_status}}' = {pendente}",
                Order = 1
            });

            formView.GridView.FormElement.Fields["pedido_id"].VisibleExpression = "val:0";
            formView.GridView.SetCurrentFilter("pedido_id", codPedido);
            return formView;
        }

        public async Task<JJDataPanel> SetupDataPanelItemAsync(int codPedido)
        {
            var panel = await factory.DataPanel.CreateAsync("ItemPedidos");
            panel.PageState = PageState.Insert;
            panel.Values["pedido_id"] = codPedido;
            panel.FormElement.Fields["pedido_id"].ReadOnlyExpression = "val:1";
            panel.FormElement.Fields["valor_unit"].ReadOnlyExpression = "val:1";

            var clienteId = await pedidoRepo.GetClienteAsync(codPedido);
            panel.UserValues["cliente_id"] = clienteId ?? 0;

            return panel;
        }

        public async Task<PedidosViewModel> SalvarItemAsync(JJDataPanel panel)
        {
            var vm = new PedidosViewModel { DataPanel = panel };
            var values = await panel.GetFormValuesAsync();
            var errors = await panel.ValidateFieldsAsync(values);

            if (errors.Count > 0)
            {
                panel.Errors = errors;
                vm.ValidationSummaryHtml = new JJValidationSummary(errors).GetHtml();
                return vm;
            }

            var quantidade = Convert.ToInt32(values["quantidade"]);
            if (quantidade <= 0)
            {
                vm.Erros.Add("A quantidade deve ser maior que zero.");
                return vm;
            }

            var codPedido = Convert.ToInt32(values["pedido_id"]);

            var erro = await pedidoService.ValidarPendenteAsync(codPedido);
            if (erro is not null)
            {
                vm.Erros.Add(erro);
                return vm;
            }

            var codProduto = Convert.ToInt32(values["item_id"]);

            if (await itemRepo.ItemJaExisteAsync(codPedido, codProduto))
            {
                vm.Erros.Add("Este produto já está no pedido. Altere a quantidade do item existente.");
                return vm;
            }

            var clienteId = await pedidoRepo.GetClienteAsync(codPedido);
            var preco = await precoRepo.GetPrecoAsync(clienteId ?? 0, codProduto);

            if (preco is null)
            {
                vm.Erros.Add("Produto sem preço cadastrado para este cliente.");
                return vm;
            }

            values["valor_unit"] = preco;

            await repository.SetValuesAsync(panel.FormElement, values);
            vm.CodPedido = codPedido;
            return vm;
        }

        public async Task<(int? codPedido, string? erro)> ExcluirItemAsync(int id)
        {
            var item = await itemRepo.GetByIdAsync(id);

            if (item is null)
                return (null, "Item não encontrado.");

            var erro = await pedidoService.ValidarPendenteAsync(item.PedidoId);
            if (erro is not null)
                return (item.PedidoId, erro);

            await itemRepo.DeleteAsync(id);
            return (item.PedidoId, null);
        }
    }
}

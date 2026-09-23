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

namespace masterdatalab.domain.Services
{
    public class PedidoItemService(IComponentFactory factory,
        IHttpContextAccessor contextAccessor,
        LinkGenerator linkGen,
        IEntityRepository repository,
        PedidoService pedidoService)
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

            var clienteId = await pedidoService.ObterClienteAsync(codPedido);
            panel.UserValues["cliente_id"] = clienteId ?? 0;

            return panel;
        }
        private async Task<decimal?> ObterPrecoAsync(int clienteId, int codProduto)
        {
            var linha = await pedidoService.BuscarUmAsync("tabela_precos", new() { ["cod_cliente"] = clienteId, ["cod_produto"] = codProduto });
            return linha is null ? null : Convert.ToDecimal(linha["valor_unit"]);
        }

        private async Task<bool> ItemJaExisteAsync(int codPedido, int codProduto)
        {
            var linha = await pedidoService.BuscarUmAsync("ItemPedidos", new() { ["pedido_id"] = codPedido, ["item_id"] = codProduto });
            return linha is not null;
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

            if (await ItemJaExisteAsync(codPedido, codProduto))
            {
                vm.Erros.Add("Este produto já está no pedido. Altere a quantidade do item existente.");
                return vm;
            }

            var clienteId = await pedidoService.ObterClienteAsync(codPedido);
            var preco = await ObterPrecoAsync(clienteId ?? 0, codProduto);

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
            var element = (await factory.DataPanel.CreateAsync("ItemPedidos")).FormElement;
            var item = await repository.GetFieldsAsync(element, new Dictionary<string, object> { ["id"] = id });

            if (item is null)
                return (null, "Item não encontrado.");

            var codPedido = Convert.ToInt32(item["pedido_id"]);

            var erro = await pedidoService.ValidarPendenteAsync(codPedido);
            if (erro is not null)
                return (codPedido, erro);

            await repository.DeleteAsync(element, new Dictionary<string, object> { ["id"] = id });
            return (codPedido, null);
        }
    }
}

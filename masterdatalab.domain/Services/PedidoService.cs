using JJConsulting.FontAwesome;
using JJConsulting.Html.Bootstrap.Components;
using JJMasterData.Commons.Data.Entity.Models;
using JJMasterData.Commons.Data.Entity.Repository.Abstractions;
using JJMasterData.Core.DataDictionary.Models;
using JJMasterData.Core.DataDictionary.Models.Actions;
using JJMasterData.Core.DataDictionary.Repository.Abstractions;
using JJMasterData.Core.UI.Components;
using JJMasterData.Commons.Data.Entity.Repository;
using masterdatalab.domain.Models.Enums;
using masterdatalab.domain.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;

namespace masterdatalab.domain.Services
{
    public class PedidoService(
        IComponentFactory factory,
        IHttpContextAccessor contextAccessor,
        LinkGenerator linkGen,
        IEntityRepository repository,
        IDataDictionaryRepository dictionaryRepository)
    {
        public async Task<ComponentResult> SetupFormViewAsync()
        {
            var formView = await factory.FormView.CreateAsync("Pedidos");
            var http = contextAccessor.HttpContext!;


            // Toolbar Action Add pedido
            formView.GridView.AddToolbarAction(new UrlRedirectAction
            {
                Name = "addPedido",
                Text = "Novo pedido",
                ShowAsButton = true,
                UrlRedirect = linkGen.GetPathByAction(http, "Create", "Pedidos")!,
                Order = 1
            });

            // Toolbar Action edit
            formView.GridView.AddGridAction(new UrlRedirectAction
            {
                Name = "editPedido",
                Tooltip = "Editar pedido",
                Icon = FontAwesomeIcon.Pencil,
                UrlRedirect = linkGen.GetPathByAction(http, "Edit", "Pedidos") + "?codPedido={id}",
                EncryptParameters = false,
                EnableExpression = $"exp:'{{status}}' = {(int)SituacaoPedido.PendenteConfirmacao}",
                Order = 1
            });

            // Toolbar Action cancel
            formView.GridView.AddGridAction(new SqlCommandAction
            {
                Name = "cancelarPedido",
                Tooltip = "Cancelar pedido",
                Icon = FontAwesomeIcon.Ban,
                SqlCommand = $"UPDATE pedidos SET status = {(int)SituacaoPedido.Cancelado} WHERE id = {{id}};",
                ConfirmationMessage = "Tem certeza que deseja cancelar este pedido?",
                EnableExpression = $"exp:'{{status}}' = {(int)SituacaoPedido.PendenteConfirmacao}",
                Order = 2
            });

            return await formView.GetResultAsync();
        }

        public async Task<JJDataPanel> SetupDataPanelPedidosAsync(int? codPedido)
        {
            var panel = await factory.DataPanel.CreateAsync("Pedidos");
            var cliente = panel.FormElement.Fields["id_cliente"];
            cliente.VisibleExpression = "val:1";
            var status = panel.FormElement.Fields["status"];

            if (codPedido.HasValue)
            {
                panel.PageState = PageState.Update;
                await panel.LoadValuesFromPkAsync(codPedido);
                cliente.ReadOnlyExpression = "val:1";
            }
            else
            {
                panel.PageState = PageState.Insert;
                status.VisibleExpression = "val:0";
            }

            return panel;
        }

        public async Task<PedidosViewModel> SalvarAsync(JJDataPanel panel)
        {
            var vm = new PedidosViewModel { DataPanel = panel };
            var values = await panel.GetFormValuesAsync();

            if (panel.PageState == PageState.Insert)
                values["status"] = (int)SituacaoPedido.PendenteConfirmacao;

            var erros = await panel.ValidateFieldsAsync(values);

            if (erros.Count > 0)
            {
                panel.Errors = erros;
                vm.ValidationSummaryHtml = new JJValidationSummary(erros).GetHtml();
                return vm;
            }

            if (panel.PageState == PageState.Update)
            {
                var codPedido = Convert.ToInt32(values["id"]);

                var erro = await ValidarPendenteAsync(codPedido);
                if (erro is not null)
                {
                    vm.Erros.Add(erro);
                    return vm;
                }

                var novoStatus = Convert.ToInt32(values["status"]);

                if (novoStatus == (int)SituacaoPedido.PagamentoAprovado && !await PossuiItensAsync(codPedido))
                {
                    vm.Erros.Add("Não é possível aprovar o pagamento de um pedido sem itens.");
                    return vm;
                }
            }

            await repository.SetValuesAsync(panel.FormElement, values);
            vm.CodPedido = Convert.ToInt32(values["id"]);
            return vm;
        }

        public async Task<int?> ObterStatusAsync(int codPedido)
        {
            var element = await dictionaryRepository.GetFormElementAsync("Pedidos");
            var pedido = await repository.GetFieldsAsync(element, new Dictionary<string, object> { ["id"] = codPedido });
            return pedido is null ? null : Convert.ToInt32(pedido["status"]);
        }
        public async Task<int?> ObterClienteAsync(int codPedido)
        {
            var element = await dictionaryRepository.GetFormElementAsync("Pedidos");
            var pedido = await repository.GetFieldsAsync(element, new Dictionary<string, object> { ["id"] = codPedido });
            return pedido is null ? null : Convert.ToInt32(pedido["id_cliente"]);
        }

        public async Task<string?> ValidarPendenteAsync(int codPedido)
        {
            var status = await ObterStatusAsync(codPedido);
            if (status is null)
                return "Pedido não encontrado.";
            if (status != (int)SituacaoPedido.PendenteConfirmacao)
                return "Só é possível alterar pedidos pendentes de confirmação.";
            return null;
        }

        public async Task<Dictionary<string, object?>?> BuscarUmAsync(string elementName, Dictionary<string, object?> filtros)
        {
            var element = await dictionaryRepository.GetFormElementAsync(elementName);
            var parameters = new EntityParameters { Filters = filtros, RecordsPerPage = 1 };
            var result = await repository.GetDictionaryListAsync(element, parameters);
            return result.FirstOrDefault();
        }

        private async Task<bool> PossuiItensAsync(int codPedido)
        {
            var linha = await BuscarUmAsync("ItemPedidos", new() { ["pedido_id"] = codPedido });
            return linha is not null;
        }

    }
}

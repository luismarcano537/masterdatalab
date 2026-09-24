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
using masterdatalab.domain.Repositories;

namespace masterdatalab.domain.Services
{
    public class PedidoService(
        IComponentFactory factory,
        IHttpContextAccessor contextAccessor,
        LinkGenerator linkGen,
        IEntityRepository repository,
        PedidoRepository pedidoRepo)
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

        public Task<int?> ObterStatusAsync(int codPedido) => pedidoRepo.GetStatusAsync(codPedido);

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

                if (novoStatus == (int)SituacaoPedido.PagamentoAprovado && !await pedidoRepo.PossuiItensAsync(codPedido))
                {
                    vm.Erros.Add("Não é possível aprovar o pagamento de um pedido sem itens.");
                    return vm;
                }
            }

            await repository.SetValuesAsync(panel.FormElement, values);
            vm.CodPedido = Convert.ToInt32(values["id"]);
            return vm;
        }

        public async Task<string?> ValidarPendenteAsync(int codPedido)
        {
            var status = await pedidoRepo.GetStatusAsync(codPedido);
            if (status is null)
                return "Pedido não encontrado.";
            if (status != (int)SituacaoPedido.PendenteConfirmacao)
                return "Só é possível alterar pedidos pendentes de confirmação.";
            return null;
        }
    }
}

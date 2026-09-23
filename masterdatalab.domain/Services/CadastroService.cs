using JJConsulting.FontAwesome;
using JJMasterData.Core.UI.Components;

namespace masterdatalab.domain.Services
{
    public class CadastroService(IComponentFactory factory)
    {
        public static readonly Dictionary<string, (string Menu, string Singular)> Elementos = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Cliente"] = ("Clientes", "cliente"),
            ["Produtos"] = ("Produtos", "produto"),
            ["tabela_precos"] = ("Tabela de preços", "preço"),
            ["situacao"] = ("Situações", "situação")
        };

        public async Task<ComponentResult> SetupFormViewAsync(string elementName)
        {
            var formView = await factory.FormView.CreateAsync(elementName);
            var singular = Elementos[elementName].Singular;

            //Toolbar Action Add
            var insert = formView.GridView.ToolbarActions.InsertAction;
            insert.Text = $"Adicionar {singular}";
            insert.Tooltip = $"Adicionar um novo {singular}";
            insert.Icon = FontAwesomeIcon.Plus;
            insert.ShowAsButton = true;
            insert.Order = 1;

            // Grid Action View
            var view = formView.GridView.TableActions.ViewAction;
            view.Icon = FontAwesomeIcon.Eye;
            view.Tooltip = $"Visualizar {singular}";
            view.ShowAsButton = false;

            // Grid Action Edit
            var edit = formView.GridView.TableActions.EditAction;
            edit.Icon = FontAwesomeIcon.Pencil;
            edit.Tooltip = $"Editar {singular}";
            edit.ShowAsButton = false;

            // Grid Action Delete
            var delete = formView.GridView.TableActions.DeleteAction;
            delete.Icon = FontAwesomeIcon.Trash;
            delete.Tooltip = $"Excluir {singular}";
            delete.ShowAsButton = false;
            delete.ConfirmationMessage = $"Excluir {singular}? A operação não poderá ser desfeita.";

            return await formView.GetResultAsync();
        }
    }
}
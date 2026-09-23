using JJMasterData.Core.UI.Components;

namespace masterdatalab.domain.Services
{
    public class CadastroService(IComponentFactory factory)
    {
        public static readonly Dictionary<string, string> Elementos = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Cliente"] = "Clientes",
            ["Produtos"] = "Produtos",
            ["tabela_precos"] = "Tabela de preços",
            ["situacao"] = "Situações"
        };

        public async Task<ComponentResult> SetupFormViewAsync(string elementName)
        {
            var formView = await factory.FormView.CreateAsync(elementName);
            return await formView.GetResultAsync();
        }
    }
}
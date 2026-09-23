using JJConsulting.FontAwesome;
using JJConsulting.Html.Bootstrap.Components;
using JJConsulting.Html.Bootstrap.Models;

namespace masterdatalab.web.Helpers
{
    public static class Mensagens
    {
        public static string Erro(IEnumerable<string> erros)
        {
            var lista = erros.ToList();
            var box = new JJMessageBox
            {
                Title = lista.Count > 1 ? "Ocorreram os seguintes erros" : "Ocorreu o seguinte erro",
                Content = string.Join("<br/>", lista),
                Icon = MessageIcon.Error
            };

            return $"<script>{box.GetDomContentLoadedScript()}</script>";
        }

        public static string Sucesso(string texto)
        {
            var alert = new JJAlert
            {
                Color = BootstrapColor.Success,
                Icon = FontAwesomeIcon.Check,
                Title = texto
            };

            return alert.GetHtml();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using JJMasterData.Core.UI.Components;

namespace masterdatalab.domain.Models.ViewModels
{
    public class PedidosViewModel
    {
        public JJDataPanel? DataPanel { get; set; }
        public int? CodPedido { get; set; }
        public List<string> Erros { get; set; } = [];
        public string? ValidationSummaryHtml { get; set; }
        public bool TemErros => Erros.Count > 0 || ValidationSummaryHtml is not null;
        public string? MensagemHtml { get; set; }
    }
}

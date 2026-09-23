using System;
using System.Collections.Generic;
using System.Text;

namespace masterdatalab.domain.Models.Enums
{
    public enum SituacaoPedido
    {
        PendenteConfirmacao = 1,
        PagamentoAprovado = 2,
        Faturado = 3,
        Cancelado = 4
    }
}

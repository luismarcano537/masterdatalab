using System;
using System.Collections.Generic;
using System.Text;

namespace masterdatalab.domain.Models.DTOs.ItemPedido
{
    public class ItemPedidoResponse
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public int ItemId { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorUnit { get; set; }
    }
}

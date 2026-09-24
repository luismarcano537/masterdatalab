using System;
using System.Collections.Generic;
using System.Text;

namespace masterdatalab.domain.Models.DTOs.Pedido
{
    public class PedidoResponse
    {
        public int Id { get; set; }
        public int IdCliente { get; set; }
        public int Status { get; set; }
        public DateTime DataEmissao { get; set; }
        public decimal ValorTotal { get; set; }
        public int QtdProdutos { get; set; }
        public int QtdUnidades { get; set; }
    }
}

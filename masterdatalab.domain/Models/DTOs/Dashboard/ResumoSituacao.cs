using System;
using System.Collections.Generic;
using System.Text;

namespace masterdatalab.domain.Models.DTOs.Dashboard
{
    public class ResumoSituacao
    {
        public int Id { get; set; }
        public string Situacao { get; set; } = string.Empty;
        public int Qtd { get; set; }
        public decimal Total { get; set; }
    }
}

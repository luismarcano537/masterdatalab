using System;
using System.Collections.Generic;
using System.Text;

namespace masterdatalab.domain.Models.DTOs.Situacao
{
    public class SituacaoResponse
    {
        public int Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public int Icone { get; set; }
        public string? Cor { get; set; }
    }
}
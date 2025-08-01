using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DocAPI.Core.Models;

namespace DocAPI.Data.Dtos.ProntuarioDtos
{
    public class UpdateProntuarioDto
    {
        [Required(ErrorMessage = "Este campo é obrigatório")]
        public string? ID { get; set; }
        [Required]
        public DateOnly? DataConsulta { get; set; }
        public string? Tipo {get; set; }

        [Required]
        public DescricaoBasica? DescricaoBasica { get; set; }

        [Required]
        public AGO? AGO { get; set; }

        [Required]
        public Antecedentes? Antecedentes { get; set; }

        [Required]
        public AntecedentesFamiliares? AntecedentesFamiliares { get; set; }

        public List<AcoesCD>? CD { get; set; }

        public string InformacoesExtras { get; set; } = string.Empty;

        public List<Exame>? Exames { get; set; }

        public Internacao? SolicitacaoInternacao { get; set; }
        public PosOp? PosOperatorio { get; set; }
    }
}

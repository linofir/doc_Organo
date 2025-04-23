using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DocAPI.Models;

namespace DocAPI.Data.Dtos.ProntuarioDtos
{
    public class UpdateProntuarioDto
    {
        [Required(ErrorMessage = "A data é obrigatória")]
        public DateTime DataRequisicao { get; set; }

        // Continua referenciando o paciente já existente, caso queira permitir troca:
        [Required(ErrorMessage = "O paciente é obrigatório")]
        public string PacienteId { get; set; } = string.Empty;

        /* --- blocos aninhados --- */
        [Required]
        public DescricaoBasicaDto DescricaoBasica { get; set; } = new();

        [Required]
        public AgoDto AGO { get; set; } = new();

        [Required]
        public AntecedentesDto Antecedentes { get; set; } = new();

        [Required]
        public AntecedentesFamiliaresDto AntecedentesFamiliares { get; set; } = new();

        [Required(ErrorMessage = "Selecione ao menos uma ação em CD")]
        public List<AcoesCD> CD { get; set; } = new();

        public string InformacoesExtras { get; set; } = string.Empty;

        public List<ExameDto> Exames { get; set; } = new();

        public InternacaoDto? SolicitacaoInternacao { get; set; }
    }
}

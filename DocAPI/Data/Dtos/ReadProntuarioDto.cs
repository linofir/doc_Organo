using System;
using System.Collections.Generic;
using DocAPI.Models;

namespace DocAPI.Data.Dtos.ProntuarioDtos
{
    public class ReadProntuarioDto
    {
        public string ID { get; set; } = string.Empty;

        public DateTime DataRequisicao { get; set; }

        /* --- referência básica do paciente --- */
        public string PacienteId   { get; set; } = string.Empty;
        public string NomePaciente { get; set; } = string.Empty;

        /* --- blocos aninhados --- */
        public DescricaoBasicaDto        DescricaoBasica        { get; set; } = new();
        public AgoDto                    AGO                    { get; set; } = new();
        public AntecedentesDto           Antecedentes           { get; set; } = new();
        public AntecedentesFamiliaresDto AntecedentesFamiliares { get; set; } = new();

        public List<AcoesCD> CD { get; set; } = new();

        public string InformacoesExtras { get; set; } = string.Empty;

        public List<ExameDto> Exames { get; set; } = new();

        public InternacaoDto? SolicitacaoInternacao { get; set; }

        public DateTime ConsultadoEm { get; set; } = DateTime.Now; // carimbo de leitura
    }
}

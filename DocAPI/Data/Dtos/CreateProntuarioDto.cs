using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DocAPI.Models;

namespace DocAPI.Data.Dtos.ProntuarioDtos
{
    public class CreateProntuarioDto
    {
        /* ------------ Dados básicos ------------ */
        [Required(ErrorMessage = "A data é obrigatória")]
        public DateTime DataRequisicao { get; set; }

        [Required]                               // virá do Paciente pré‑selecionado (no controller/service)
        public string? PacienteId { get; set; }   // ID do paciente que está criando o prontuário

        /* --------- Descrição Básica --------- */
        public DescricaoBasicaDto DescricaoBasica { get; set; } = new();

        /* ------------- AGO ------------- */
        public AgoDto AGO { get; set; } = new();

        /* ------------- AP -------------- */
        public AntecedentesDto Antecedentes { get; set; } = new();

        /* ------------- AF -------------- */
        public AntecedentesFamiliaresDto AntecedentesFamiliares { get; set; } = new();

        /* ------------- CD -------------- */
        [Required(ErrorMessage = "Selecione ao menos uma ação em CD")]
        public List<AcoesCD> CD { get; set; } = new();

        /* ---------- Extras ---------- */
        public string InformacoesExtras { get; set; } = string.Empty;

        /* ---------- Exames ---------- */
        public List<ExameDto> Exames { get; set; } = new();

        /* ------- Solicitação de Internação ------- */
        public InternacaoDto? SolicitacaoInternacao { get; set; }
    }

    /* ============ Dtos aninhados ============ */

    public class DescricaoBasicaDto
    {
        [Required(ErrorMessage = "O nome do paciente é obrigatório")]
        public string NomePaciente { get; set; } = string.Empty;

        [Required(ErrorMessage = "A idade é obrigatória")]
        public int Idade { get; set; }

        [Required(ErrorMessage = "O campo profissão é obrigatório")]
        public string Profissao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo religião é obrigatório")]
        public string Religiao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo queixa/encaminhamento é obrigatório")]
        public string QD { get; set; } = string.Empty;
    }

    public class AgoDto
    {
        [Required(ErrorMessage = "O campo DUM é obrigatório")]
        public string DUM { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Paridade é obrigatório")]
        public string Paridade { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Desejo de Gestação é obrigatório")]
        public string DesejoGestacao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Vacina HPV é obrigatório")]
        public StatusVacinaHPV VacinaHPV { get; set; }

        [Required(ErrorMessage = "O campo CCO é obrigatório")]
        public string CCO { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo MAC/TRH é obrigatório")]
        public string MAC_TRH { get; set; } = string.Empty;

        public string Intercorrencias { get; set; } = string.Empty;
        public string Amamentacao   { get; set; } = string.Empty;
        public string VidaSexual    { get; set; } = string.Empty;
        public string Relacionamento { get; set; } = string.Empty;
        public string Parceiros      { get; set; } = string.Empty;
        public string Coitarca       { get; set; } = string.Empty;
        public string IST            { get; set; } = string.Empty;
    }

    public class AntecedentesDto
    {
        [Required(ErrorMessage = "O campo Comorbidades é obrigatório")]
        public string Comorbidades { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Medicação em uso é obrigatório")]
        public string Medicacao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Neoplasias é obrigatório")]
        public string Neoplasias { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Cirurgias é obrigatório")]
        public string Cirurgias { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Alergias é obrigatório")]
        public string Alergias { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Vícios é obrigatório")]
        public string Vicios { get; set; } = string.Empty;

        public string HabitoIntestinal { get; set; } = string.Empty;
        public string Vacinas          { get; set; } = string.Empty;
    }

    public class AntecedentesFamiliaresDto
    {
        [Required(ErrorMessage = "O campo Neoplasias (familiares) é obrigatório")]
        public string Neoplasias { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Comorbidades (familiares) é obrigatório")]
        public string Comorbidades { get; set; } = string.Empty;
    }

    public class ExameDto
    {
        [Required]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        public string Nome { get; set; } = string.Empty;
    }

    public class InternacaoDto
    {
        public List<string> Procedimentos { get; set; } = new();

        [Required(ErrorMessage = "A indicação clínica é obrigatória")]
        public string IndicacaoClinica { get; set; } = string.Empty;

        public string Observacao { get; set; } = string.Empty;

        public string CID { get; set; } = string.Empty;

        public DateTime? Data { get; set; }

        public string Regime { get; set; } = string.Empty;

        public string Carater { get; set; } = string.Empty;

        public bool UsaOPME { get; set; }

        public string Local { get; set; } = string.Empty;

        public long? Guia { get; set; }
    }
}

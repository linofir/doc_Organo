using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DocAPI.Models
{
    public class Prontuario
    {
        [Key]
        [Required(ErrorMessage = "Este campo é obrigatório")]
        public string ID { get; set; }
        public Prontuario(Paciente paciente)
        {
            ID = Guid.NewGuid().ToString();
            DescricaoBasica = new DescricaoBasica(paciente); 
        }

        [Required]
        public DescricaoBasica? DescricaoBasica { get; set; }

        [Required]
        public AGO? AGO { get; set; }

        [Required]
        public Antecedentes? Antecedentes { get; set; }

        [Required]
        public AntecedentesFamiliares? AntecedentesFamiliares { get; set; }

        public List<AcoesCD> CD { get; set; }   

        public string InformacoesExtras { get; set; } = string.Empty;

        public List<Exame> Exames { get; set; }
        public Internacao SolicitacaoInternacao { get; set;}
    }

    public class DescricaoBasica
    {
        public DescricaoBasica() {}

        public DescricaoBasica(Paciente paciente)
        {
            NomePaciente = paciente.Nome;
            Idade = paciente.Idade;
        } 
        [Required]
        public string? NomePaciente { get; set; } 

        [Required]
        public int Idade { get; set; } 

        [Required(ErrorMessage = "O campo profissão é obrigatório")]
        public string? Profissao { get; set; }

        [Required(ErrorMessage = "O campo religião é obrigatório")]
        public string? Religiao { get; set; }

        [Required(ErrorMessage = "O campo queixa/encaminhamento é obrigatório")]
        public string? QD { get; set; }
    }

    public class AGO
    {
        [Required(ErrorMessage = "O campo DUM é obrigatório")]
        public string? DUM { get; set; }

        [Required(ErrorMessage = "O campo Paridade é obrigatório")]
        public string? Paridade { get; set; }

        [Required(ErrorMessage = "O campo Desejo de Gestação é obrigatório")]
        public string? DesejoGestacao { get; set; }

        [Required(ErrorMessage = "O campo Vacina HPV é obrigatório")]
        public StatusVacinaHPV VacinaHPV { get; set; }

        [Required(ErrorMessage = "O campo CCO é obrigatório")]
        public string? CCO { get; set; }

        [Required(ErrorMessage = "O campo MAC/TRH é obrigatório")]
        public string? MAC_TRH { get; set; }

        public string Intercorrencias { get; set; } = string.Empty;
        public string Amamentacao { get; set; } = string.Empty;
        public string VidaSexual { get; set; } = string.Empty;
        public string Relacionamento { get; set; } = string.Empty;
        public string Parceiros { get; set; } = string.Empty;
        public string Coitarca { get; set; } = string.Empty;
        public string IST { get; set; } = string.Empty;
    }

    public class Antecedentes
    {

        [Required(ErrorMessage = "O campo Comorbidades é obrigatório")]
        public string? Comorbidades { get; set; }

        [Required(ErrorMessage = "O campo Medicação em uso é obrigatório")]
        public string? Medicacao { get; set; }

        [Required(ErrorMessage = "O campo Neoplasias é obrigatório")]
        public string? Neoplasias { get; set; }

        [Required(ErrorMessage = "O campo Cirurgias é obrigatório")]
        public string? Cirurgias { get; set; }

        [Required(ErrorMessage = "O campo Alergias é obrigatório")]
        public string? Alergias { get; set; }

        [Required(ErrorMessage = "O campo Vícios é obrigatório")]
        public string? Vicios { get; set; }

        public string HabitoIntestinal { get; set; } = string.Empty;
        public string Vacinas { get; set; } = string.Empty;
    }

    public class AntecedentesFamiliares
    {
        [Required]
        public string Neoplasias { get; set; } = string.Empty;

        [Required]
        public string Comorbidades { get; set; } = string.Empty;
    }
    public class Exame
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
    }
    public class Internacao
    {
        public List<string> Procedimentos { get; set; } = new();
        public string IndicaçãoClinica { get; set; } = string.Empty;
        public string Observacao { get; set; } = string.Empty;
        public string CID { get; set; } = string.Empty; 
        public DateTime Data { get; set; }
        public string Regime { get; set; } = string.Empty;
        public string Carater { get; set; } = string.Empty;
        public bool UsaOPME { get; set; }
        public string Local  { get; set; } = string.Empty; 
        public long Guia  { get; set; } 

    }

}

public enum StatusVacinaHPV
{
    [Display(Name = "Sim, 1 dose")]
    UmaDose,

    [Display(Name = "Sim, 2 doses")]
    DuasDoses,

    [Display(Name = "Sim, 3 doses")]
    TresDoses,

    [Display(Name = "Sem vacina")]
    SemVacina

    //Como usar no Display.Name, front
    // var displayName = pronto.AGO.VacinaHPV
    // .GetType()
    // .GetMember(pronto.AGO.VacinaHPV.ToString())
    // .First()
    // .GetCustomAttribute<DisplayAttribute>()
    // ?.Name;
}
public enum AcoesCD
{
    [Display(Name = "Pedido de internação")]
    PedidoInternacao,

    [Display(Name = "Pedido de exame")]
    PedidoExame,

    [Display(Name = "Indicação de Encaminhamentos")]
    IndicacaoEncaminhamentos,

    [Display(Name = "Informativos de instrumentadora")]
    InformativosInstrumentadora,

    [Display(Name = "Termo cirúrgico")]
    TermoCirurgico,

    [Display(Name = "Pasta Informativa")]
    PastaInformativa
   
}

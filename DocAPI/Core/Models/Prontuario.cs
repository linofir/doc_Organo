using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DocAPI.Core.Models;

public class Prontuario
{
    protected Prontuario() { }

    public Prontuario(Paciente paciente)
    {
        ID = Guid.NewGuid();
        Paciente = paciente;
        PacienteId = paciente.ID;
    }
   
    public Guid ID { get; private set; }
    public Paciente Paciente { get; private set; } = null!;
    public Guid PacienteId { get; private set; }

    public int Versao { get; private set; }
    public Guid? ProntuarioAnteriorId { get; private set; }
    public DateTime CriadoEm { get; private set; }
    public string CriadoPor { get; private set; } = string.Empty;
    
    public DateOnly DataConsulta {get; private set; } //NOT NULL NO EF
    public string Tipo {get; private set; } = string.Empty; //NOT NULL NO EF

    [Required]
    public DescricaoBasica? DescricaoBasica { get; set; } // como devo lidar com as value objects, devo deixar NOT NULL no EF?o q implica essa decisão

    [Required]
    public AGO? AGO { get; set; }

    [Required]
    public Antecedentes? Antecedentes { get; set; }

    [Required]
    public AntecedentesFamiliares? AntecedentesFamiliares { get; set; }

    public List<AcoesCD>? CD { get; set; }   

    public string? InformacoesExtras { get; private set; }//não é obrigatório

    public List<Exame>? Exames { get; set; }// nem sempre será passado exames
    public Internacao? SolicitacaoInternacao { get; set;}//nem sempre terá procedimento
    public PosOp? PosOperatorio  { get; set;}//só terá essa classe se o tipo for posOp
}        

public class DescricaoBasica
{
    public DescricaoBasica() {}
    public DescricaoBasica(Paciente paciente)// A primeira dúvida aqui é se preciso de um constructor
    {
        NomePaciente = paciente.Nome;
        Idade = paciente.Idade;
        Cpf = paciente.CPF;
    }
    [Required]
    public Guid? PacienteId { get; set; } // já que minha classe prontuario já é condicionada á uma instância de Paciente poderia somente usar para alimentar essas props? ou ainda será que é necessário criar uma coluna no EF já que terei acesso as pops de Paciente para referenciar diretamente no dominio?
    [Required]
    public string? NomePaciente { get; set; } 
    [Required]
    public string? Cpf { get; set; } 
    [Required]
    public int Idade { get; set; } 

    [Required(ErrorMessage = "O campo profissão é obrigatório")]
    public string? Profissao { get; set; }//ainda não vou criarum Enum, mas poderia ser no furuto, teria algum problema?

    [Required(ErrorMessage = "O campo religião é obrigatório")]
    public string? Religiao { get; set; }// pensei em colocar como NOT NULL, para forçar o front a ter uma lista de seleção com a opção não declarada por exemplo. QUal a melhor forma de abordar isso? 

    [Required(ErrorMessage = "O campo queixa/encaminhamento é obrigatório")]
    public string? QD { get; set; }//Será um campo de muitos caracteres no EF 
    public string? AtividadeFisica { get; set; } = string.Empty; ////////////////////
}
    
    public class AGO
    {
        public string Menarca { get; set; } = string.Empty; 
        [Required(ErrorMessage = "O campo DUM é obrigatório")]
        public string? DUM { get; set; }//campo de muitos caracteres

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
    public class Exame//aqui ainda tenho algumas dúvidas, será um VO, porém como seria uma lista de exames(de acrodo com a classe Exame) dentro da tabela Prontuario
    {
        public string Codigo { get; set; } = string.Empty;//esse código é o código fornecido por uma tabela padrão de exames
        public string Nome { get; set; } = string.Empty;
    
    }
    public class Internacao
    {
        public List<string> Procedimentos { get; set; } = new();
        public DateOnly Data { get; set; }
        public string IndicacaoClinica { get; set; } = string.Empty;
        public string Observacao { get; set; } = string.Empty;
        public string CID { get; set; } = string.Empty; // sim essa tabela de CID que irá alimentar esse campo, qual a  melhro forma de faze-lo?
        public string TempoDoenca { get; set; } = string.Empty;
        public string Diarias { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Regime { get; set; } = string.Empty;
        public string Carater { get; set; } = string.Empty;
        public bool UsaOPME { get; set; } = false;
        public string Local  { get; set; } = string.Empty; 
        public string? Guia  { get; set; } 

    }   

public class PosOp
{
    public string? PeriodoSeguimento { get; set; }
    public string? Conclusao { get; set; }
    public string? ExameMacro { get; set; }
}



public enum StatusVacinaHPV
{
    [Display(Name = "Sim, 1 dose")]
    UmaDose = 1,

    [Display(Name = "Sim, 2 doses")]
    DuasDoses = 2,

    [Display(Name = "Sim, 3 doses")]
    TresDoses = 3,

    [Display(Name = "Sem Vacina")]
    SemVacina = 4,
    [Display(Name = "Sem Informação")] 
    SemInfo = 5

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
    PedidoInternacao = 0,

    [Display(Name = "Pedido de exame")]
    PedidoExame = 1,

    [Display(Name = "Indicação de encaminhamentos")]
    IndicacaoEncaminhamentos = 2,

    [Display(Name = "Informativos de instrumentadora")]
    InformativosInstrumentadora = 3,

    [Display(Name = "Termo cirúrgico")]
    TermoCirurgico = 4,

    [Display(Name = "Pasta Informativa")]
    PastaInformativa = 5,
    [Display(Name = "Sem Info")]
    SemInformacao = 6
   
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DocAPI.Core.Entities;

public class Prontuario
{
    protected Prontuario() { }

    public Prontuario(Guid pacienteId, Guid atendimentoId)
    {
        ID = Guid.NewGuid();
        PacienteId = pacienteId;
        AtendimentoId = atendimentoId;
        CriadoEm = DateTime.UtcNow;
        Versao = 1;
    }
   
    public Guid ID { get; private set; }
    public Guid PacienteId { get; private set; }
    public Paciente Paciente { get; private set; } = null!;
    public Guid AtendimentoId { get; private set; }
    public Atendimento Atendimento { get; private set; } = null!;

    public int Versao { get; private set; }
    public Guid? ProntuarioAnteriorId { get; private set; }
    public Prontuario? ProntuarioAnterior { get; private set; }

    public DateTime CriadoEm { get; private set; }
    public DateTime? AtualizadoEm { get; private set; }
    public string? AtualizadoPor { get; private set; }
    public bool Deletado { get; private set; }
    public DateTime? DeletadoEm { get; private set; }
    
    public DateOnly DataConsulta {get; private set; } //NOT NULL NO EF
    public int Tipo {get; private set; }//
    public string? InformacoesExtras { get; private set; }

    public DescricaoBasica DescricaoBasica { get; private set; } = null!;
    public AGO AGO { get; private set; } = null!;
    public Antecedentes Antecedentes { get; private set; } = null!;
    public AntecedentesFamiliares AntecedentesFamiliares { get; private set; } = null!;
    public PosOp? PosOperatorio { get; private set; }

    public ICollection<ProntuarioAcaoCD> AcoesCD { get; private set; }
    = new List<ProntuarioAcaoCD>(); 


    public ICollection<Exame> Exames { get; private set; } = new List<Exame>();

    public Internacao? Internacao { get; private set; }
}        

public class DescricaoBasica
{
    protected DescricaoBasica() { } // EF

    public DescricaoBasica(
        string nomePaciente,
        string cpf,
        int idade,
        string profissao,
        string religiao,
        string qd,
        string? atividadeFisica
    )
    {
        NomePaciente = nomePaciente;
        Cpf = cpf;
        Idade = idade;
        Profissao = profissao;
        Religiao = religiao;
        QD = qd;
        AtividadeFisica = atividadeFisica;
    }

    public string NomePaciente { get; private set; } = string.Empty;

    public string Cpf { get; private set; } = string.Empty;

    public int Idade { get; private set; }

    public string Profissao { get; private set; } = string.Empty;

    public string Religiao { get; private set; } = string.Empty;

    public string QD { get; private set; } = string.Empty;

    public string? AtividadeFisica { get; private set; }
}
    
public class AGO
{
    protected AGO(){}

    public AGO(
        string menarca,
        string dum,
        string paridade,
        string desejoGestacao,
        StatusVacinaHPV vacinaHPV,
        string cco,
        string macTrh
    )
    {
        Menarca = menarca;
        DUM = dum;
        Paridade = paridade;
        DesejoGestacao = desejoGestacao;
        VacinaHPV = vacinaHPV;
        CCO = cco;
        MAC_TRH = macTrh;
    }

    public string Menarca { get; private set; } = string.Empty;

    public string DUM { get; private set; } = string.Empty;

    public string Paridade { get; private set; } = string.Empty;

    public string DesejoGestacao { get; private set; } = string.Empty;

    public StatusVacinaHPV VacinaHPV { get; private set; }

    public string CCO { get; private set; } = string.Empty;

    public string MAC_TRH { get; private set; } = string.Empty;

    public string Intercorrencias { get; private set; } = string.Empty;

    public string Amamentacao { get; private set; } = string.Empty;

    public string VidaSexual { get; private set; } = string.Empty;

    public string Relacionamento { get; private set; } = string.Empty;

    public string Parceiros { get; private set; } = string.Empty;

    public string Coitarca { get; private set; } = string.Empty;

    public string IST { get; private set; } = string.Empty;
}

public class Antecedentes
{
    protected Antecedentes(){}

    public Antecedentes(
        string comorbidades,
        string medicacao,
        string neoplasias,
        string cirurgias,
        string alergias,
        string vicios
    )
    {
        Comorbidades = comorbidades;
        Medicacao = medicacao;
        Neoplasias = neoplasias;
        Cirurgias = cirurgias;
        Alergias = alergias;
        Vicios = vicios;
    }

    public string Comorbidades { get; private set; } = string.Empty;

    public string Medicacao { get; private set; } = string.Empty;

    public string Neoplasias { get; private set; } = string.Empty;

    public string Cirurgias { get; private set; } = string.Empty;

    public string Alergias { get; private set; } = string.Empty;

    public string Vicios { get; private set; } = string.Empty;

    public string HabitoIntestinal { get; private set; } = string.Empty;

    public string Vacinas { get; private set; } = string.Empty;
}

public class AntecedentesFamiliares
{
    protected AntecedentesFamiliares(){}

    public AntecedentesFamiliares(string neoplasias, string comorbidades)
    {
        Neoplasias = neoplasias;
        Comorbidades = comorbidades;
    }

    public string Neoplasias { get; private set; } = string.Empty;

    public string Comorbidades { get; private set; } = string.Empty;
}

public class PosOp
{
    protected PosOp(){}

    public PosOp(string? periodoSeguimento, string? conclusao, string? exameMacro)
    {
        PeriodoSeguimento = periodoSeguimento;
        Conclusao = conclusao;
        ExameMacro = exameMacro;
    }

    public string? PeriodoSeguimento { get; private set; }

    public string? Conclusao { get; private set; }

    public string? ExameMacro { get; private set; }
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
}


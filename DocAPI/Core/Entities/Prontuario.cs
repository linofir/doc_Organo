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

    // ── Identity ───────────────────────────────────────────
    public Guid ID { get; private set; }
    public Guid PacienteId { get; private set; }
    public Paciente Paciente { get; private set; } = null!;
    public Guid AtendimentoId { get; private set; }
    public Atendimento Atendimento { get; private set; } = null!;

    // ── Versioning ─────────────────────────────────────────
    public int Versao { get; private set; }
    public Guid? ProntuarioAnteriorId { get; private set; }
    public Prontuario? ProntuarioAnterior { get; private set; }

    // ── Audit ──────────────────────────────────────────────
    public DateTime CriadoEm { get; private set; }
    public DateTime? AtualizadoEm { get; private set; }
    public string? AtualizadoPor { get; private set; }
    public bool Deletado { get; private set; }
    public DateTime? DeletadoEm { get; private set; }

    // ── Clinical scalars ───────────────────────────────────
    public DateOnly DataConsulta { get; private set; }
    public int Tipo { get; private set; }
    public string? InformacoesExtras { get; private set; }

    // ── Owned value objects ────────────────────────────────
    public DescricaoBasica DescricaoBasica { get; private set; } = null!;
    public AGO AGO { get; private set; } = null!;
    public Antecedentes Antecedentes { get; private set; } = null!;
    public AntecedentesFamiliares AntecedentesFamiliares { get; private set; } = null!;
    public PosOp? PosOperatorio { get; private set; }

    // ── Child collections ──────────────────────────────────
    public ICollection<ProntuarioAcaoCD> AcoesCD { get; private set; }
        = new List<ProntuarioAcaoCD>();

    public ICollection<Exame> Exames { get; private set; } = new List<Exame>();

    public Internacao? Internacao { get; private set; }

    // ── Domain methods ─────────────────────────────────────

    /// <summary>Populates clinical payload after v1 creation.</summary>
    public void AplicarCriacao(
        DateOnly dataConsulta,
        int tipo,
        string? informacoesExtras,
        DescricaoBasica descricaoBasica,
        AGO ago,
        Antecedentes antecedentes,
        AntecedentesFamiliares antecedentesFamiliares,
        PosOp? posOperatorio,
        List<ProntuarioAcaoCD>? acoesCD,
        List<Exame>? exames,
        Internacao? internacao)
    {
        DataConsulta = dataConsulta;
        Tipo = tipo;
        InformacoesExtras = informacoesExtras;
        DescricaoBasica = descricaoBasica ?? throw new ArgumentNullException(nameof(descricaoBasica));
        AGO = ago ?? throw new ArgumentNullException(nameof(ago));
        Antecedentes = antecedentes ?? throw new ArgumentNullException(nameof(antecedentes));
        AntecedentesFamiliares = antecedentesFamiliares ?? throw new ArgumentNullException(nameof(antecedentesFamiliares));
        PosOperatorio = posOperatorio;

        if (acoesCD != null)
            AcoesCD = acoesCD;
        if (exames != null)
            Exames = exames;
        Internacao = internacao;
    }

    /// <summary>Applies correction-safe scalar updates in place.
    /// Does NOT mutate child collections per design D-01.</summary>
    public void AplicarCorrecao(
        string? informacoesExtras,
        string? profissao,
        string? religiao,
        string? atividadeFisica)
    {
        if (informacoesExtras != null)
            InformacoesExtras = informacoesExtras;

        DescricaoBasica = DescricaoBasica.AplicarCorrecao(profissao, religiao, atividadeFisica);

        AtualizadoEm = DateTime.UtcNow;
    }

    /// <summary>Creates a clinical-evolution successor version.
    /// Validates the assigned version number per D-02 and ADR-006:
    /// the aggregate <b>validates</b> the rule; it does <b>not</b> query persistence.</summary>
    public Prontuario CriarNovaVersao(
        int nextVersao,
        DateOnly dataConsulta,
        int tipo,
        string? informacoesExtras,
        DescricaoBasica descricaoBasica,
        AGO ago,
        Antecedentes antecedentes,
        AntecedentesFamiliares antecedentesFamiliares,
        PosOp? posOperatorio,
        List<ProntuarioAcaoCD>? acoesCD,
        List<Exame>? exames,
        Internacao? internacao)
    {
        if (nextVersao <= Versao)
            throw new InvalidOperationException(
                $"Version evolution requires nextVersao > {Versao}, received {nextVersao}.");

        var novaVersao = new Prontuario(PacienteId, AtendimentoId)
        {
            Versao = nextVersao,
            ProntuarioAnteriorId = ID
        };

        novaVersao.AplicarCriacao(
            dataConsulta, tipo, informacoesExtras,
            descricaoBasica, ago, antecedentes, antecedentesFamiliares,
            posOperatorio, acoesCD, exames, internacao);

        return novaVersao;
    }

    /// <summary>Soft-deletes this single version per ADR-001.</summary>
    public void MarcarComoExcluido()
    {
        Deletado = true;
        DeletadoEm = DateTime.UtcNow;
    }
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

    /// <summary>Returns a new DescricaoBasica with correction-safe fields updated.
    /// Identity snapshot fields (NomePaciente, Cpf, Idade) and clinical fields (QD)
    /// are immutable — they never change via correction.</summary>
    public DescricaoBasica AplicarCorrecao(
        string? profissao,
        string? religiao,
        string? atividadeFisica)
    {
        return new DescricaoBasica(
            NomePaciente,
            Cpf,
            Idade,
            profissao ?? Profissao,
            religiao ?? Religiao,
            QD,
            atividadeFisica ?? AtividadeFisica);
    }
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
        string macTrh,
        string intercorrencias = "",
        string amamentacao = "",
        string vidaSexual = "",
        string relacionamento = "",
        string parceiros = "",
        string coitarca = "",
        string ist = ""
    )
    {
        Menarca = menarca;
        DUM = dum;
        Paridade = paridade;
        DesejoGestacao = desejoGestacao;
        VacinaHPV = vacinaHPV;
        CCO = cco;
        MAC_TRH = macTrh;
        Intercorrencias = intercorrencias;
        Amamentacao = amamentacao;
        VidaSexual = vidaSexual;
        Relacionamento = relacionamento;
        Parceiros = parceiros;
        Coitarca = coitarca;
        IST = ist;
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
        string vicios,
        string habitoIntestinal = "",
        string vacinas = ""
    )
    {
        Comorbidades = comorbidades;
        Medicacao = medicacao;
        Neoplasias = neoplasias;
        Cirurgias = cirurgias;
        Alergias = alergias;
        Vicios = vicios;
        HabitoIntestinal = habitoIntestinal;
        Vacinas = vacinas;
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


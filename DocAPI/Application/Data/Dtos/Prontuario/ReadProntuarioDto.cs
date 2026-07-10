using System;
using System.Collections.Generic;

namespace DocAPI.Data.Dtos.ProntuarioDtos;

public class ReadProntuarioDto
{
    public Guid Id { get; set; }
    public Guid PacienteId { get; set; }
    public Guid AtendimentoId { get; set; }

    public int Versao { get; set; }
    public Guid? ProntuarioAnteriorId { get; set; }

    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
    public bool Deletado { get; set; }

    public DateOnly DataConsulta { get; set; }
    public int Tipo { get; set; }
    public string? InformacoesExtras { get; set; }

    public DescricaoBasicaDto? DescricaoBasica { get; set; }
    public AGODto? AGO { get; set; }
    public AntecedentesDto? Antecedentes { get; set; }
    public AntecedentesFamiliaresDto? AntecedentesFamiliares { get; set; }
    public PosOpDto? PosOperatorio { get; set; }

    public List<AcoesCDDto>? CD { get; set; }
    public List<ExameDto>? Exames { get; set; }
    public SolicitacaoInternacaoDto? SolicitacaoInternacao { get; set; }
}
using DocFront.Models.ViewModels;
using DocFront.Models.Dtos;

namespace DocFront.Mappers;
public static class ProntuarioMapper
{
    public static ProntuarioApiDto ToApi(ProntuarioViewModel model)
    {
        return new ProntuarioApiDto
        {
            ID = model.Id,
            DataConsulta = model.DataConsulta,
            Tipo = model.Tipo,

            DescricaoBasica = MapDescricaoBasica(model.DescricaoBasica),
            AGO = MapAGO(model.AGO),
            Antecedentes = MapAntecedentes(model.Antecedentes),
            AntecedentesFamiliares = MapAntecedentesFamiliares(model.AntecedentesFamiliares),

            CD = model.AcoesCD?.ToList(),
            Exames = MapExames(model.Exames),
            SolicitacaoInternacao = MapInternacao(model.SolicitacaoInternacao),
            PosOperatorio = MapPosOp(model.PosOperatorio),

            InformacoesExtras = model.InformacoesExtras
        };
    }
    private static DescricaoBasicaDto MapDescricaoBasica(DescricaoBasicaViewModel vm)
    {
        return new DescricaoBasicaDto
        {
            PacienteId = vm.PacienteId,
            NomePaciente = vm.NomePaciente,
            Cpf = vm.Cpf,
            Idade = vm.Idade,
            Profissao = vm.Profissao,
            Religiao = vm.Religiao,
            QD = vm.QD,
            AtividadeFisica = vm.AtividadeFisica
        };
    }
    private static AGODto MapAGO(AGOViewModel vm)
    {
        return new AGODto
        {
            Menarca = vm.Menarca,
            DUM = vm.DUM,
            Paridade = vm.Paridade,
            DesejoGestacao = vm.DesejoGestacao,
            VacinaHPV = vm.VacinaHPV,
            CCO = vm.CCO,
            MAC_TRH = vm.MAC_TRH,
            Intercorrencias = vm.Intercorrencias,
            Amamentacao = vm.Amamentacao,
            VidaSexual = vm.VidaSexual,
            Relacionamento = vm.Relacionamento,
            Parceiros = vm.Parceiros,
            Coitarca = vm.Coitarca,
            IST = vm.IST
        };
    }
    private static AntecedentesDto MapAntecedentes(AntecedentesViewModel vm)
    {
        return new AntecedentesDto
        {
            Comorbidades = vm.Comorbidades,
            Medicacao = vm.Medicacao,
            Neoplasias = vm.Neoplasias,
            Cirurgias = vm.Cirurgias,
            Alergias = vm.Alergias,
            Vicios = vm.Vicios,
            HabitoIntestinal = vm.HabitoIntestinal,
            Vacinas = vm.Vacinas
        };
    }
    private static AntecedentesFamiliaresDto MapAntecedentesFamiliares(
        AntecedentesFamiliaresViewModel vm)
    {
        return new AntecedentesFamiliaresDto
        {
            Neoplasias = vm.Neoplasias,
            Comorbidades = vm.Comorbidades
        };
    }
    private static List<ExameDto>? MapExames(List<ExameViewModel>? exames)
    {
        if (exames is null || exames.Count == 0)
            return null;

        return exames.Select(e => new ExameDto
        {
            Codigo = e.Codigo,
            Nome = e.Nome
        }).ToList();
    }
    private static InternacaoDto? MapInternacao(InternacaoViewModel? vm)
    {
        if (vm is null) return null;

        return new InternacaoDto
        {
            Procedimentos = vm.Procedimentos?.ToList() ?? new(),
            Data = vm.Data,
            IndicacaoClinica = vm.IndicacaoClinica,
            Observacao = vm.Observacao,
            CID = vm.CID,
            TempoDoenca = vm.TempoDoenca,
            Diarias = vm.Diarias,
            Tipo = vm.Tipo,
            Regime = vm.Regime,
            Carater = vm.Carater,
            UsaOPME = vm.UsaOPME,
            Local = vm.Local,
            Guia = vm.Guia
        };
    }
    private static PosOpDto? MapPosOp(PosOpViewModel? vm)
    {
        if (vm is null) return null;

        return new PosOpDto
        {
            PeriodoSeguimento = vm.PeriodoSeguimento,
            Conclusao = vm.Conclusao,
            ExameMacro = vm.ExameMacro
        };
    }

    //Dto to ViewModel
    public static ProntuarioViewModel? ToViewModel(ProntuarioApiDto dto)
    {
        if (dto == null) return null;

        return new ProntuarioViewModel
        {
            Id = dto.ID,
            DataConsulta = dto.DataConsulta,
            Tipo = dto.Tipo,
            DescricaoBasica = DescricaoToViewModel(dto.DescricaoBasica!)!,
            AGO = AgoToViewModel(dto.AGO!)!,
            Antecedentes = AntecedentesToViewModel(dto.Antecedentes!)!,
            AntecedentesFamiliares = FamiliatesToViewModel(dto.AntecedentesFamiliares!)!,
            AcoesCD = dto.CD?.ToList()!,
            InformacoesExtras = dto.InformacoesExtras,
            Exames = ExameToViewModel(dto.Exames)!,
            SolicitacaoInternacao = InternacaoToViewModel(dto.SolicitacaoInternacao)!,
            PosOperatorio = PosOpToViewModel(dto.PosOperatorio)!
        };
    }
  
    private static DescricaoBasicaViewModel? DescricaoToViewModel(DescricaoBasicaDto dto)
    {
        if (dto == null) return null;

        return new DescricaoBasicaViewModel
        {
            PacienteId = dto.PacienteId,
            NomePaciente = dto.NomePaciente,
            Cpf = dto.Cpf,
            Idade = dto.Idade,
            Profissao = dto.Profissao,
            Religiao = dto.Religiao,
            QD = dto.QD,
            AtividadeFisica = dto.AtividadeFisica
        };
    }

    private static AGOViewModel? AgoToViewModel(AGODto dto)
    {
        if (dto == null) return null;

        return new AGOViewModel
        {
            Menarca = dto.Menarca,
            DUM = dto.DUM,
            Paridade = dto.Paridade,
            DesejoGestacao = dto.DesejoGestacao,
            VacinaHPV = dto.VacinaHPV,
            CCO = dto.CCO,
            MAC_TRH = dto.MAC_TRH,
            Intercorrencias = dto.Intercorrencias,
            Amamentacao = dto.Amamentacao,
            VidaSexual = dto.VidaSexual,
            Relacionamento = dto.Relacionamento,
            Parceiros = dto.Parceiros,
            Coitarca = dto.Coitarca,
            IST = dto.IST
        };
    }
    private static AntecedentesViewModel? AntecedentesToViewModel(AntecedentesDto dto)
    {
        if (dto == null) return null;

        return new AntecedentesViewModel
        {
            Comorbidades = dto.Comorbidades,
            Medicacao = dto.Medicacao,
            Neoplasias = dto.Neoplasias,
            Cirurgias = dto.Cirurgias,
            Alergias = dto.Alergias,
            Vicios = dto.Vicios,
            HabitoIntestinal = dto.HabitoIntestinal,
            Vacinas = dto.Vacinas
        };
    }
    private static AntecedentesFamiliaresViewModel? FamiliatesToViewModel(
    AntecedentesFamiliaresDto dto)
    {
        if (dto == null) return null;

        return new AntecedentesFamiliaresViewModel
        {
            Neoplasias = dto.Neoplasias,
            Comorbidades = dto.Comorbidades
        };
    }
    private static List<ExameViewModel>? ExameToViewModel(List<ExameDto>? dtos)
    {
        if (dtos is null || dtos.Count == 0)
            return null;

        return dtos.Select(dto => new ExameViewModel
        {
            Codigo = dto.Codigo,
            Nome = dto.Nome
        }).ToList();
    }
    private static InternacaoViewModel? InternacaoToViewModel(InternacaoDto? dto)
    {
        if (dto is null) return null;

        return new InternacaoViewModel
        {
            Procedimentos = dto.Procedimentos?.ToList() ?? new(),
            Data = dto.Data,
            IndicacaoClinica = dto.IndicacaoClinica,
            Observacao = dto.Observacao,
            CID = dto.CID,
            TempoDoenca = dto.TempoDoenca,
            Diarias = dto.Diarias,
            Tipo = dto.Tipo,
            Regime = dto.Regime,
            Carater = dto.Carater,
            UsaOPME = dto.UsaOPME,
            Local = dto.Local,
            Guia = dto.Guia
        };
    }
    private static PosOpViewModel? PosOpToViewModel(PosOpDto? dto)
    {
        if (dto is null) return null;

        return new PosOpViewModel
        {
            PeriodoSeguimento = dto.PeriodoSeguimento,
            Conclusao = dto.Conclusao,
            ExameMacro = dto.ExameMacro
        };
    }

    // ProntuarioCardDto

    public static List<ProntuarioCardDto>? ToProntuarioCard(List<ReadProntuarioDto> dtoList)
    {
        Console.WriteLine("teste mapper inicio");
        if (dtoList is null) return new List<ProntuarioCardDto>();;
        Console.WriteLine("teste mapper depois de null");
        var list = dtoList.Select(p => new ProntuarioCardDto
        {
            Id = p.Id,
            Nome = p.DescricaoBasica?.NomePaciente ?? "Sem nome",
            Data = p.DataConsulta,
            Tipo = p.Tipo ?? "Sem tipo"
        }).ToList();
        // var testingList = new List<ProntuarioCardDto>(){};
        // {
        //     new ProntuarioCardDto
        //     {
        //         Id = "1234",
        //         Nome = "nome",
        //         Data = DateOnly.MaxValue,
        //         Tipo = "teste"
        //     };
        //     new ProntuarioCardDto
        //     {
        //         Id = "1234",
        //         Nome = "nome",
        //         Data = DateOnly.MaxValue,
        //         Tipo = "teste"
        //     };
            
        // }
    
        Console.WriteLine("teste mapper fim");
        return list;
    }
}

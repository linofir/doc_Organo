using System;
using System.Collections.Generic;
using DocAPI.Core.Models;

namespace DocAPI.Data.Dtos.ProntuarioDtos
{
    public class ReadProntuarioDto
{
    public string ID { get; set; }

    public DateTime DataRequisicao { get; set; }

    public DescricaoBasica DescricaoBasica { get; set; }

    public AGO AGO { get; set; }

    public Antecedentes Antecedentes { get; set; }

    public AntecedentesFamiliares AntecedentesFamiliares { get; set; }

    public List<AcoesCD> CD { get; set; }

    public string InformacoesExtras { get; set; }

    public List<Exame> Exames { get; set; }

    public Internacao SolicitacaoInternacao { get; set; }
}
}

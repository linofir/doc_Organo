using System.ComponentModel.DataAnnotations;

namespace DocAPI.Core.Entities;

public class ProntuarioAcaoCD
{
    protected ProntuarioAcaoCD(){}

    public ProntuarioAcaoCD(Guid prontuarioId, AcoesCD tipo)
    {
        ID = Guid.NewGuid();
        ProntuarioId = prontuarioId;
        Tipo = tipo;
    }

    public Guid ID { get; private set; }

    public Guid ProntuarioId { get; private set; }

    public Prontuario Prontuario { get; private set; } = null!;

    public AcoesCD Tipo { get; private set; }
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
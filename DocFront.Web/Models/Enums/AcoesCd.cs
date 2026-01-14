using System.ComponentModel.DataAnnotations;

namespace DocFront.Models.Enums;
public enum AcoesCd
{
    [Display(Name = "Pedido de Internação")]
    PedidoInternacao = 0,

    [Display(Name = "Pedido de Exame")]
    PedidoExame = 1,

    [Display(Name = "Encaminhamentos")]
    IndicacaoEncaminhamentos = 2,

    [Display(Name = "Informações à Instrumentadora")]
    InformativosInstrumentadora = 3,

    [Display(Name = "Termo Cirúrgico")]
    TermoCirurgico = 4,

    [Display(Name = "Pasta Informativa")]
    PastaInformativa = 5,

    [Display(Name = "Sem Informação")]
    SemInformacao = 6
   
}
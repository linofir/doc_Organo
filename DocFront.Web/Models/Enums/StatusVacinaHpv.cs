using System.ComponentModel.DataAnnotations;
namespace DocFront.Models.Enums;

public enum StatusVacinaHPV
{
    [Display(Name = "Uma dose")]
    UmaDose = 1,

    [Display(Name = "Duas doses")]
    DuasDoses = 2,

    [Display(Name = "Três doses")]
    TresDoses = 3,

    [Display(Name = "Não vacinada")]
    SemVacina = 4,

    [Display(Name = "Sem informação")]
    SemInfo = 5
}

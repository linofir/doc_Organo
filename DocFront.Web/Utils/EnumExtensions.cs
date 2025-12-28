using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace DocFront.Utils;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        return value
            .GetType()
            .GetMember(value.ToString())
            .FirstOrDefault()?
            .GetCustomAttribute<DisplayAttribute>()?
            .Name
            ?? value.ToString();
    }
}

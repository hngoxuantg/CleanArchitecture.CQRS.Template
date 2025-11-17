using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Project.API.Helpers
{
    public static class EnumHelper
    {
        public static List<object> ToList<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(e => new
                {
                    Id = Convert.ToInt32(e),
                    Name = e.GetType()
                            .GetMember(e.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()?
                            .GetName() ?? e.ToString()
                })
                .ToList<object>();
        }
    }
}

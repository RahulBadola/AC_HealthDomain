using time_track_dshp.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace time_track_dshp.Extensions
{
    public static class ObjectExtensions
    {
        public static List<string> UpdatedFields<T>(this T original, T comparison)
        {
            var type = typeof(T);

            var propInfos = type.GetProperties();
            var valueTypes = propInfos.Where(x => x.PropertyType.IsValueType || x.PropertyType.IsPrimitive || x.PropertyType.IsEnum || x.PropertyType.Name.ToLower() == "string");
            var nonCustomAttrTypes = valueTypes.Where(x => !x.CustomAttributes.Any(x => x.AttributeType == typeof(NonSqlPropertyAttribute)));

            var result = new List<string>();
            foreach(var field in nonCustomAttrTypes)
            {
                var originalValue = field.GetValue(original);
                var comparisonValue = field.GetValue(comparison);
                if (originalValue != null)
                {
                    if (!originalValue.Equals(comparisonValue)) result.Add(field.Name);
                } 
                else if (originalValue != comparisonValue)
                {
                    result.Add(field.Name);
                }
            }
            return result;
        }
    }
}

using System.Collections.Generic;
using System.Linq;

namespace SharpMC.Generator.Prismarine.Data
{
    internal static class DataPriCommon
    {
        internal static List<OneField> SortByName(this IEnumerable<OneField> fields)
        {
            return fields.OrderBy(x => x.Name).ToList();
        }

        internal static void AddAllField(this ICollection<OneField> fields, string fieldType)
        {
            var allNames = fields.Select(e => e.Name);
            fields.Add(new OneField
            {
                Name = "All", TypeName = $"readonly {fieldType}[]",
                Constant = $" = {{ {string.Join(", ", allNames)} }}"
            });
        }
    }
}
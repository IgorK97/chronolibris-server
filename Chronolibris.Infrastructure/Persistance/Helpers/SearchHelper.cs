using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Infrastructure.DataAccess.Persistance.Helpers
{
    public static class SearchHelper
    {
        public static string ToLikePattern(string input)
        {
            var escaped = input.Replace("\\", "\\\\")
                .Replace("%", "\\%")
                .Replace("_", "\\_");

            return $"%{escaped}%";
        }
    }
}

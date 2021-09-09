using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CadastramentoPerformace.Core
{
    public static class Ajuda
    {
        public static bool ValidateNumbers(string value)
        {
            var regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
            var parsingOk = !regex.IsMatch(value);
            if (!parsingOk)
            {
                return false;
            }
            return true;
        }
    }
}

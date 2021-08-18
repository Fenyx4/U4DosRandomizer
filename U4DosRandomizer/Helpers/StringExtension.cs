using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace U4DosRandomizer.Helpers
{
    public static class StringExtension
    {
        public static string CapitalizeFirstLetter(this String input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return input.Substring(0, 1).ToUpper(CultureInfo.CurrentCulture) +
                input.Substring(1, input.Length - 1);
        }
    }
}

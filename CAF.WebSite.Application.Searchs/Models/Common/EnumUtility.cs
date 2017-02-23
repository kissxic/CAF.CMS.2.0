﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.WebSite.Application.Searchs.Models.Common
{
    public static class EnumUtility
    {
        public static T SafeParse<T>(string value, T defaultValue)
            where T : struct
        {
            T result;

            if (!Enum.TryParse(value, out result))
                result = defaultValue;

            return result;
        }
    }
}

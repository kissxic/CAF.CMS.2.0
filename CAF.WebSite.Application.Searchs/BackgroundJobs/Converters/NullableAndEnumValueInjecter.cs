using System;
using System.Reflection;
using Omu.ValueInjecter;
using System.Collections.Generic;

namespace CAF.WebSite.Application.Searchs.Converters
{
    public class NullableAndEnumValueInjecter: ConventionInjection
    {
   
        protected override bool Match(ConventionInfo c)
        {
            return PropertyMatch(c.SourceProp, c.TargetProp);
        }

        protected override object SetValue(ConventionInfo c)
        {
            if (c.TargetProp.Type.IsEnum)
            {
                if (c.SourceProp.Value != null)
                {
                    var value = Enum.Parse(c.TargetProp.Type, c.SourceProp.Value.ToString(), true);
                    c.TargetProp.Value = value;
                }
            }

            if (c.TargetProp.Type == typeof(string) && c.SourceProp.Type.IsEnum)
            {
                c.TargetProp.Value = c.SourceProp.Value.ToString(); 
            }
            else
            {
                if (!c.TargetProp.Type.IsEnum)
                {
                    c.TargetProp.Value = c.SourceProp.Value;
                }
            }
            return c.TargetProp.Value;
        }
    
 

        private static bool PropertyMatch(ConventionInfo.PropInfo source, ConventionInfo.PropInfo target)
        {
            var result = string.Equals(source.Name, target.Name);
            if (result)
            {
                result = !source.Type.IsArray && !target.Type.IsArray;
            }
            if (result)
            {
                result = !target.Type.IsArray && !target.Type.IsArray;
            }

            if (result)
            {
                result = source.Type == target.Type || source.Type == Nullable.GetUnderlyingType(target.Type)
                        || Nullable.GetUnderlyingType(source.Type) == target.Type;

                if (!result)
                {
                    result = source.Type.IsEnum || target.Type.IsEnum;
                }
            }

            return result;
        }
    }
}
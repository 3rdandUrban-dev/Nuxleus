using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Globalization;

namespace Newtonsoft.Json.Utilities
{
  internal delegate T Creator<T>();

  internal static class MiscellaneousUtils
  {
    public static bool TryAction<T>(Creator<T> creator, out T output)
    {
      ValidationUtils.ArgumentNotNull(creator, "creator");

      try
      {
        output = creator();
        return true;
      }
      catch
      {
        output = default(T);
        return false;
      }
    }

    public static bool TryGetDescription(object value, out string description)
    {
      return TryAction<string>(delegate { return GetDescription(value); }, out description);
    }

    public static string GetDescription(object o)
    {
      ValidationUtils.ArgumentNotNull(o, "o");

      ICustomAttributeProvider attributeProvider = o as ICustomAttributeProvider;

      // object passed in isn't an attribute provider
      // if value is an enum value, get value field member, otherwise get values type
      if (attributeProvider == null)
      {
        Type valueType = o.GetType();

        if (valueType.IsEnum)
          attributeProvider = valueType.GetField(o.ToString());
        else
          attributeProvider = valueType;
      }

      DescriptionAttribute descriptionAttribute = ReflectionUtils.GetAttribute<DescriptionAttribute>(attributeProvider);

      if (descriptionAttribute != null)
        return descriptionAttribute.Description;
      else
        throw new Exception("No DescriptionAttribute on '{0}'.".FormatWith(CultureInfo.InvariantCulture, o.GetType()));
    }

    public static IList<string> GetDescriptions(IList values)
    {
      ValidationUtils.ArgumentNotNull(values, "values");

      string[] descriptions = new string[values.Count];

      for (int i = 0; i < values.Count; i++)
      {
        descriptions[i] = GetDescription(values[i]);
      }

      return descriptions;
    }

    public static string ToString(object value)
    {
      if (value == null)
        return "{null}";

      return (value is string) ? @"""" + value.ToString() + @"""" : value.ToString();
    }
  }
}

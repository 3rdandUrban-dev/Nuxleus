﻿#region License
// Copyright (c) 2007 James Newton-King
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json.Utilities;
using System.Globalization;

namespace Newtonsoft.Json.Linq
{
  /// <summary>
  /// Represents a JSON object.
  /// </summary>
  public class JObject : JContainer
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="JObject"/> class.
    /// </summary>
    public JObject()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JObject"/> class from another <see cref="JObject"/> object.
    /// </summary>
    /// <param name="other">A <see cref="JObject"/> object to copy from.</param>
    public JObject(JObject other)
      : base(other)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JObject"/> class with the specified content.
    /// </summary>
    /// <param name="content">The contents of the object.</param>
    public JObject(params object[] content)
      : this((object)content)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JObject"/> class with the specified content.
    /// </summary>
    /// <param name="content">The contents of the object.</param>
    public JObject(object content)
    {
      Add(content);
    }

    internal override bool DeepEquals(JToken node)
    {
      JObject t = node as JObject;
      return (t != null && ContentsEqual(t));
    }

    internal override void ValidateToken(JToken o)
    {
      ValidationUtils.ArgumentNotNull(o, "o");

      if (o.Type != JsonTokenType.Property)
        throw new Exception("Can not add {0} to {1}".FormatWith(CultureInfo.InvariantCulture, o.GetType(), GetType()));
    }

    internal override JToken CloneNode()
    {
      return new JObject(this);
    }

    /// <summary>
    /// Gets the node type for this <see cref="JToken"/>.
    /// </summary>
    /// <value>The type.</value>
    public override JsonTokenType Type
    {
      get { return JsonTokenType.Object; }
    }

    /// <summary>
    /// Gets an <see cref="IEnumerable{JProperty}"/> of this object's properties.
    /// </summary>
    /// <returns>An <see cref="IEnumerable{JProperty}"/> of this object's properties.</returns>
    public IEnumerable<JProperty> Properties()
    {
      return Children().Cast<JProperty>();
    }

    /// <summary>
    /// Gets a <see cref="JProperty"/> the specified name.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <returns>A <see cref="JProperty"/> with the specified name or null.</returns>
    public JProperty Property(string name)
    {
      return Properties()
        .Where(p => string.Compare(p.Name, name, StringComparison.Ordinal) == 0)
        .SingleOrDefault();
    }

    /// <summary>
    /// Gets an <see cref="JEnumerable{JToken}"/> of this object's property values.
    /// </summary>
    /// <returns>An <see cref="JEnumerable{JToken}"/> of this object's property values.</returns>
    public JEnumerable<JToken> PropertyValues()
    {
      return new JEnumerable<JToken>(Properties().Select(p => p.Value));
    }

    /// <summary>
    /// Gets the <see cref="JToken"/> with the specified key.
    /// </summary>
    /// <value>The <see cref="JToken"/> with the specified key.</value>
    public override JToken this[object key]
    {
      get
      {
        ValidationUtils.ArgumentNotNull(key, "o");

        string propertyName = key as string;
        if (propertyName == null)
          throw new ArgumentException("Accessed JObject values with invalid key value: {0}. Object property name expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));

        JProperty property = Property(propertyName);

        return (property != null) ? property.Value : null;
      }
    }

    /// <summary>
    /// Loads an <see cref="JObject"/> from a <see cref="JsonReader"/>. 
    /// </summary>
    /// <param name="reader">A <see cref="JsonReader"/> that will be read for the content of the <see cref="JObject"/>.</param>
    /// <returns>A <see cref="JObject"/> that contains the XML that was read from the specified <see cref="JsonReader"/>.</returns>
    public static JObject Load(JsonReader reader)
    {
      ValidationUtils.ArgumentNotNull(reader, "reader");

      if (reader.TokenType == JsonToken.None)
      {
        if (!reader.Read())
          throw new Exception("Error reading JObject from JsonReader.");
      }
      if (reader.TokenType != JsonToken.StartObject)
      {
        throw new Exception("Current JsonReader item is not an object.");
      }
      else
      {
        if (!reader.Read())
          throw new Exception("Error reading JObject from JsonReader.");
      }

      JObject o = new JObject();
      o.ReadContentFrom(reader);

      return o;
    }

    /// <summary>
    /// Load a <see cref="JObject"/> from a string that contains JSON.
    /// </summary>
    /// <param name="json">A <see cref="String"/> that contains JSON.</param>
    /// <returns>A <see cref="JObject"/> populated from the string that contains JSON.</returns>
    public static JObject Parse(string json)
    {
      JsonReader jsonReader = new JsonTextReader(new StringReader(json));

      return Load(jsonReader);
    }

    /// <summary>
    /// Creates a <see cref="JObject"/> from an object.
    /// </summary>
    /// <param name="o">The object that will be used to create <see cref="JObject"/>.</param>
    /// <returns>A <see cref="JObject"/> with the values of the specified object</returns>
    public static JObject FromObject(object o)
    {
      JToken token = FromObjectInternal(o);

      if (token.Type != JsonTokenType.Object)
        throw new ArgumentException("Object serialized to {0}. JObject instance expected.".FormatWith(CultureInfo.InvariantCulture, token.Type));

      return (JObject)token;
    }

    internal override void ValidateObject(JToken o, JToken previous)
    {
      ValidationUtils.ArgumentNotNull(o, "o");

      if (o.Type != JsonTokenType.Property)
        throw new ArgumentException("An item of type {0} cannot be added to content.".FormatWith(CultureInfo.InvariantCulture, o.Type));
    }

    /// <summary>
    /// Writes this token to a <see cref="JsonWriter"/>.
    /// </summary>
    /// <param name="writer">A <see cref="JsonWriter"/> into which this method will write.</param>
    /// <param name="converters">A collection of <see cref="JsonConverter"/> which will be used when writing the token.</param>
    public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
    {
      writer.WriteStartObject();

      foreach (JProperty property in Properties())
      {
        property.WriteTo(writer, converters);
      }

      writer.WriteEndObject();
    }
  }
}
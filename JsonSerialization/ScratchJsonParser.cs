using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace JsonSerialization
{
    public class ScratchJsonParser
    {
        private string GetObjectStrValueByKey(string jsonLine)
        {
            jsonLine = jsonLine.TrimStart();
            int countInnerObject = 0;
            string innerObjectLine = "";
            foreach (char ch in jsonLine)
            {
                if (ch == '{') countInnerObject++;
                if (ch == '}') countInnerObject--;
                innerObjectLine += ch;
                if (countInnerObject == 0) break;
            }

            return innerObjectLine;
        }

        private string GetStrValueByKey(string jsonLine, string property)
        {
            var index = jsonLine.IndexOf(property, StringComparison.CurrentCultureIgnoreCase);
            if (index == -1) return null;
            var valuePart = jsonLine.Substring(index).Split(":")[1].TrimStart();
            if (Regex.IsMatch(valuePart, @"^[\{]"))
                return GetObjectStrValueByKey(String.Join(":",
                    jsonLine.Substring(index).Split(":")[new Range(1, Index.End)]));
            if (Regex.IsMatch(valuePart, @"^[\[]"))
                return "array";
            string value = "";
            if (Regex.IsMatch(valuePart, @"^['‘""]"))
            {
                var quote = valuePart[0] == '‘' ? '’' : valuePart[0];
                value = valuePart.Split(quote)[0].Substring(1);
            }
            else
            {
                value = valuePart.Split(',')[0];
            }

            return value;
        }


        public IDictionary<string, object> ParseToDictionary(string jsonLine, Type type)
        {
            var newObject = new Dictionary<string, object>();
            PropertyInfo[] properties = type.GetProperties();
            foreach (var property in properties)
            {
                var value = GetStrValueByKey(jsonLine, property.Name);

                if (property.PropertyType == typeof(string))
                    newObject.Add(property.Name, value);
                else if (property.PropertyType == typeof(long))
                {
                    if (!string.IsNullOrEmpty(value))
                        newObject.Add(property.Name, long.Parse(value));
                }
                else if (property.PropertyType == typeof(long?))
                {
                    if (string.IsNullOrEmpty(value))
                        newObject.Add(property.Name, null);
                    else
                        newObject.Add(property.Name, long.Parse(value));
                }
                else if (!property.PropertyType.IsPrimitive && property.PropertyType != typeof(string))
                {
                   var obj = ParseToDictionary(value, property.PropertyType);

                   newObject.Add(property.Name, obj);
                }
            }

            return newObject;
        }
    }
}

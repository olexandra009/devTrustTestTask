using System;
using System.Collections;
using System.Text.RegularExpressions;
using TestTaskApi.Data.Entity;
using static System.String;

namespace TestTaskApi
{
    public interface IJsonParser
    {
        string Serialize(object input);
        T Deserialize<T>(string jsonLine) where T : new();
    }

    public class SimpleJsonParser: IJsonParser
    {
        public string Serialize(object input)
        {
            if (input is IEnumerable || input.GetType().IsArray)
            {
                return SerializeList((IEnumerable)input);
            }
            var result = "{" + Environment.NewLine;
            foreach (var property in input.GetType().GetProperties())
            {
                var name = char.ToLower(property.Name[0])+property.Name[1..];
                var value = property.GetValue(input)?.ToString();
                if (property.PropertyType == typeof(Address))
                {
                    value = Serialize(property.GetValue(input)); 
                    result += name + ": " + value;
                }
                else if (input.GetType() == typeof(Address) && property.Name == "AddressLine")//for unquoted case in address line 
                    result += name + ": " + value + ",";
                else if(property.PropertyType == typeof(string))
                    result += name + ": ‘" + value + "’,";
                else
                    result += name + ": " + value + ",";

                result += Environment.NewLine;
            }

            result += "}";
            return result;
        }

        private string SerializeList(IEnumerable list)
        {
            var result = "[" + Environment.NewLine;
            foreach (var obj in list)
            {
                result += Serialize(obj) + "," + Environment.NewLine;
            }

            result += "]";
            return result;
        }


        public T Deserialize<T>(string jsonLine) where T : new()
        {
            return (T) DeserializeObject(jsonLine, typeof(T));
        }

        private object DeserializeObject(string json, Type type)
        {
            object instance = Activator.CreateInstance(type);
            foreach (var property in type.GetProperties())
            {
                var value = GetStrValueByKey(json, property.Name);
                if (property.PropertyType == typeof(string))
                    property.SetValue(instance, value);
                else if (property.PropertyType == typeof(long))
                    property.SetValue(instance, IsNullOrEmpty(value) ? 0 : long.Parse(value));
                else if (property.PropertyType == typeof(long?))
                    property.SetValue(instance, IsNullOrEmpty(value) ? null : long.Parse(value));
                else //case for Address
                {
                    property.SetValue(instance, DeserializeObject(value, property.PropertyType));
                }
            }
            return instance;
    }

        #region Methods for parsing JSON
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
            var index = jsonLine.IndexOf(property+":", StringComparison.CurrentCultureIgnoreCase);
            if (index == -1) return null;

            var valuePart = jsonLine.Substring(index).Split(":")[1].TrimStart();
            var line = Join(":", jsonLine.Substring(index).Split(":")[1..]);

            if (Regex.IsMatch(valuePart, @"^[\{]"))
                return GetObjectStrValueByKey(line);
            if (Regex.IsMatch(valuePart, @"^[\[]")) //out of scope
                throw new NotImplementedException();
           
            string value;
            if (Regex.IsMatch(valuePart, @"^['‘""]"))
            {
                var quote = valuePart[0] == '‘' ? '’' : valuePart[0];
                value = valuePart.Split(quote)[0].Substring(1);
            }
            else
                value = valuePart.Split(',')[0];
            
            return value;
        }



        #endregion
    }
}

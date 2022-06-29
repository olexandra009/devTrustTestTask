using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TestTaskApi.Data.Entity;
using static System.String;

namespace TestTaskApi
{
    public interface IJsonParser
    {
        string Serialize(List<Person> persons);
        string Serialize(Person person);
        Person Deserialize(string jsonLine);
    }
    public class SimpleJsonParser: IJsonParser
    {

        public string Serialize(List<Person> persons)
        {
            var result = "["+ Environment.NewLine;
            foreach (var person in persons)
            {
                result += Serialize(person)+"," + Environment.NewLine; 
            }

            result += "]";
            return result;
        }
        public string Serialize(Person person)
        {
            var result = "{" + Environment.NewLine;
            foreach (var property in typeof(Person).GetProperties())
            {
                var name = char.ToLower(property.Name[0])+property.Name[1..];
                var value = property.GetValue(person)?.ToString();
                if (property.PropertyType == typeof(Address))
                {
                    value = Serialize(person.Address);
                    result += name + ": " + value;
                }
                else if(property.PropertyType == typeof(string))
                    result += name + ": ‘" + value + "’,";
                else
                    result += name + ": " + value + ",";

                result += Environment.NewLine;
            }

            result += "}";
            return result;
        }

        private string Serialize(Address address)
        {
            string result = "{" + Environment.NewLine;
            foreach (var property in typeof(Address).GetProperties())
            {
                var name = char.ToLower(property.Name[0]) + property.Name[1..];
                var value = property.GetValue(address)?.ToString();
                if (property.Name == "AddressLine")
                    result += name + ": " + value + ",";
                else if (property.PropertyType == typeof(string))
                    result += name + ": ‘" + value + "’,";
                else
                    result += name + ": " + value + ",";
                result += Environment.NewLine;
            }
            result += "}";
            return result;
        }

        public Person Deserialize(string jsonLine)
        {
            Person newPerson = new Person();
            foreach (var property in typeof(Person).GetProperties())
            {
                var value = GetStrValueByKey(jsonLine, property.Name);
                if (property.PropertyType == typeof(string))
                    property.SetValue(newPerson, value);
                else if(property.PropertyType == typeof(long))
                    property.SetValue(newPerson, IsNullOrEmpty(value) ? 0 : long.Parse(value));
                else if(property.PropertyType==typeof(long?))
                    property.SetValue(newPerson, IsNullOrEmpty(value) ? null : long.Parse(value));
                else //case for Address
                {
                    Address address = new Address
                    {
                        City = GetStrValueByKey(value, "City"), 
                        AddressLine = GetStrValueByKey(value, "AddressLine")
                    };
                    var id = GetStrValueByKey(value, "Id");
                    address.Id = IsNullOrEmpty(id) ? 0 : long.Parse(id);
                    newPerson.Address = address;
                }

            }
            return newPerson;
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
            if (Regex.IsMatch(valuePart, @"^[\[]"))
                return "array";
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

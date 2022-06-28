using System;
using System.Text.RegularExpressions;
using static System.String;

namespace JsonSerialization
{
    public class SimpleJsonParser
    {

        public string Serialize(Person person)
        {

            return "";
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
            var index = jsonLine.IndexOf(property, StringComparison.CurrentCultureIgnoreCase);
            if (index == -1) return null;
            var valuePart = jsonLine.Substring(index).Split(":")[1].TrimStart();
            if (Regex.IsMatch(valuePart, @"^[\{]"))
                return GetObjectStrValueByKey(Join(":",
                    jsonLine.Substring(index).Split(":")[new Range(1, Index.End)]));
            if (Regex.IsMatch(valuePart, @"^[\[]"))
                return "array";
            string value;
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



        #endregion
    }
}

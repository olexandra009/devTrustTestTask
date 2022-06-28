namespace JsonSerialization
{
    class Program
    {
        static void Main()
        {
            ScratchJsonParser scratch = new ScratchJsonParser();
            var a = scratch.ParseToDictionary("{city: ‘Kiev’, addressLine: prospect “Peremogy” 28/7,}", typeof(Address));
            a.ToObject<Address>();

            var pa = scratch.ParseToDictionary("{firstName: ‘Ivan’, lastName: ‘Petrov’, address: {city: ‘Kiev’, addressLine: prospect “Peremogy” 28/7,}", typeof(Person));
            //pa.ToObject<Person>();

            SimpleJsonParser parser = new SimpleJsonParser();
            var pe = parser.Deserialize("{firstName: ‘Ivan’, lastName: ‘Petrov’, address: {city: ‘Kiev’, addressLine: prospect “Peremogy” 28/7,}");

        }

    }
}

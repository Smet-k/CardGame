using System;

namespace Durak 
{
    public static class Serializer
    {
        readonly static currentGameId = 0;

        public static string Serialize<T>(T input)
        { 
            var output = new List<string>();
            output.Add("<root>\n");
            foreach (var property in input.GetType().GetProperties())
            {
                output.Add($"<{property.Name}>{property.GetValue(input)}</{property.Name}>\n");
            }
            //<root>
            // <FirstName>Name1</FirstName>
            // <LastName></LastName>
            //<root/>

            //return input.ConvertToXml();
            output.Add("</root>");

            var sOutput = "";
            output.ForEach(x => sOutput += x);
            return sOutput;
        }

        public static void WriteToFile(string input)
        {
            var fileID = 1;
            while (File.Exists($"{fileID}.xml"))
            {
                fileID++;
                currentGameId = fileID;
            }
            File.WriteAllText($"{fileID}.xml", input);
        }
    }
}

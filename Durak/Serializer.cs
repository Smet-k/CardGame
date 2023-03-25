using Newtonsoft.Json;
using System;

namespace Durak 
{
    public static class Serializer
    {
        public static string currentGameId = "0";


        public static void CreateSave()
        {
            
            var dirID = 1;
            while (Directory.Exists($"{dirID}")) 
            {
                dirID++;
                currentGameId = Convert.ToString(dirID);
            }
            Directory.CreateDirectory($"{dirID}");
        }

        public static void CreateSave(string name)
        {
            var path = Path.Combine("..", "..", "..", $"{name}");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory($"{name}");
                currentGameId = name;
            }
            
        }

        public static void WriteToFile<T>(string input,T type)
        {
            if (!Directory.Exists(Path.Combine("..", "..", "..", $"{currentGameId}"))) 
            {
                Directory.CreateDirectory(Path.Combine("..", "..", "..", $"{currentGameId}"));
            }
            var path = Path.Combine("..", "..", "..", $"{currentGameId}" ,$"{type.GetType().Name}.json");
            File.WriteAllText(path,input);
        }
    }
}

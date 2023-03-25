using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durak
{
    public class LogClass
    {
        List<string> log = new List<string>();
        private readonly string path = Path.Combine("..", "..", "..", "log.txt");

        public void Log(string input)
        {
            if (!File.Exists(path))
            {
                File.Create(path);
            }
              
            
            File.AppendAllText(path, $"{DateTime.UtcNow.ToLocalTime().ToString()} {input} \n");
        }
    }
}

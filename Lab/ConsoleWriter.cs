using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Writing
{
    class ConsoleWriter : IWriter
    {
        public void Write(string str)
        {
            Console.WriteLine(str);
        }
    }
}

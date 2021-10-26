using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serialization
{
    interface ISerializer
    {
        public string Serialize(object obj);

    }
}

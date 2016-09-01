using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndexTest
{
    //some delegate for index
    class Delegate<K, V>
    {
        public delegate int CompareT(K t1, K t2);
        public delegate K GetDefaultKey();
        public delegate V GetDefaultValue();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndexTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(DateTime.Now.ToString("hh:mm:ss fff"));
            TrieIndex trieIndex = new TrieIndex();
            for (int i = 0; i < 100000000; i += 2)
            {
                string key = GenerateRandomString(64);
                IntPtr value = new IntPtr(i);
                trieIndex.Insert(key, value);
            }

            string key1 = "a3b0e9e7cddbbe78270fa4182a7675ff00b92872d8df7d14265a2b1e379a9d33";
            IntPtr value1 = new IntPtr(999999999);
            trieIndex.Insert(key1, value1);

            for (int i = 100000000; i < 200000000; i += 2)
            {
                string key = GenerateRandomString(64);
                IntPtr value = new IntPtr(i);
                trieIndex.Insert(key, value);
            }

            Console.WriteLine(DateTime.Now.ToString("hh:mm:ss fff"));
            string readKey = Console.ReadLine();
            Console.WriteLine(DateTime.Now.ToString("hh:mm:ss fff"));
            Console.WriteLine(trieIndex.Search(readKey));
            Console.WriteLine(DateTime.Now.ToString("hh:mm:ss fff"));
            Console.ReadLine();

            //Console.WriteLine(DateTime.Now.ToString("hh:mm:ss fff"));
            //Index<string> hashTree = new Index<string>(stringCompare, stringGetDefault);

            //for (int i = 0; i < 100000000; i += 2)
            //{
            //    string key = GenerateRandomString(64);
            //    IntPtr value = new IntPtr(i);
            //    hashTree.BTInsert(ref hashTree.root, key, value);
            //}

            //string key1 = "a3b0e9e7cddbbe78270fa4182a7675ff00b92872d8df7d14265a2b1e379a9d33";
            //IntPtr value1 = new IntPtr(999999999);
            //hashTree.BTInsert(ref hashTree.root, key1, value1);

            //for (int i = 100000000; i < 200000000; i += 2)
            //{
            //    string key = GenerateRandomString(64);
            //    IntPtr value = new IntPtr(i);
            //    hashTree.BTInsert(ref hashTree.root, key, value);
            //}

            //Console.WriteLine(DateTime.Now.ToString("hh:mm:ss fff"));

            //string readKey = Console.ReadLine();
            //IntPtr result = new IntPtr(0);
            //Console.WriteLine(DateTime.Now.ToString("hh:mm:ss fff"));
            //hashTree.BTSearch(hashTree.root, readKey, ref result);
            //Console.WriteLine(result);
            //Console.WriteLine(DateTime.Now.ToString("hh:mm:ss fff"));
            //Console.ReadLine();
        }
        static int stringCompare(string val1, string val2)
        {
            return string.CompareOrdinal(val1, val2);
        }
        static string stringGetDefault()
        {
            return "";
        }

        private static char[] constant = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        public static string GenerateRandomString(int length)
        {
            string checkCode = String.Empty;
            Random rd = new Random();
            for (int i = 0; i < length; i++)
            {
                checkCode += constant[rd.Next(36)].ToString();
            }
            return checkCode;
        }

    }
}

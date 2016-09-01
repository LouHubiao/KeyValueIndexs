using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARTStrSpace;
using ARTInt64Space;

namespace IndexTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine(DateTime.Now.ToString("hh:mm:ss fff"));
            //TrieIndex trieIndex = new TrieIndex();
            //for (int i = 0; i < 100000000; i += 2)
            //{
            //    string key = GenerateRandomString(64);
            //    IntPtr value = new IntPtr(i);
            //    trieIndex.Insert(key, value);
            //}

            //string key1 = "a3b0e9e7cddbbe78270fa4182a7675ff00b92872d8df7d14265a2b1e379a9d33";
            //IntPtr value1 = new IntPtr(999999999);
            //trieIndex.Insert(key1, value1);

            //for (int i = 100000000; i < 200000000; i += 2)
            //{
            //    string key = GenerateRandomString(64);
            //    IntPtr value = new IntPtr(i);
            //    trieIndex.Insert(key, value);
            //}

            //Console.WriteLine(DateTime.Now.ToString("hh:mm:ss fff"));
            //string readKey = Console.ReadLine();
            //Console.WriteLine(DateTime.Now.ToString("hh:mm:ss fff"));
            //Console.WriteLine(trieIndex.Search(readKey));
            //Console.WriteLine(DateTime.Now.ToString("hh:mm:ss fff"));
            //Console.ReadLine();

            //Console.WriteLine("BTreeTest:");
            //BTreeTest();

            //Console.WriteLine("ARTTest:");
            //ARTTest();

            //Console.WriteLine("BTreeTest:");
            //BTreeGenericsTest();

            Console.WriteLine("ARTInt64Test:");
            ARTInt64Test();

            Console.ReadLine();
            Console.ReadLine();
        }

        static void BTreeGenericsTest()
        {
            B_Tree<string, IntPtr> hashTree = new B_Tree<string, IntPtr>();

            Console.WriteLine(DateTime.Now.ToString("hh:mm:ss fff"));
            List<string> tests = new List<string>();
            Random rd = new Random();
            for (int i = 0; i < 100000000; i += 2)
            {
                string key = GenerateRandomString(64, rd);
                IntPtr value = new IntPtr(i);
                B_Tree<string, IntPtr>.Insert(ref hashTree.root, key, value, stringCompare);

                if (i % 10000 == 0)
                    tests.Add(key);
            }

            Console.WriteLine(DateTime.Now.ToString("hh:mm:ss fff"));
            IntPtr result;
            foreach (string readKey in tests)
            {
                B_Tree<string, IntPtr>.Search(hashTree.root, readKey, stringCompare, out result);
            }
            Console.WriteLine(DateTime.Now.ToString("hh:mm:ss fff"));
        }

        static void BTreeTest()
        {
            BTree<string> hashTree = new BTree<string>(stringCompare, stringGetDefaultKey);

            Console.WriteLine(DateTime.Now.ToString("hh:mm:ss fff"));
            List<string> tests = new List<string>();
            Random rd = new Random();
            for (int i = 0; i < 10000000; i += 2)
            {
                string key = GenerateRandomString(64, rd);
                IntPtr value = new IntPtr(i);
                hashTree.Insert(ref hashTree.root, key, value);

                if (i % 10000 == 0)
                    tests.Add(key);
            }

            Console.WriteLine(DateTime.Now.ToString("hh:mm:ss fff"));
            foreach (string readKey in tests)
            {
                hashTree.Search(hashTree.root, readKey);
            }
            Console.WriteLine(DateTime.Now.ToString("hh:mm:ss fff"));
        }

        static void ARTTest()
        {
            //init ART
            ARTStr artTree = new ARTStr();

            Console.WriteLine(DateTime.Now.ToString("hh:mm:ss fff"));
            List<string> tests = new List<string>();
            Random rd = new Random();
            for (int i = 0; i < 100000000; i += 2)
            {
                string key = GenerateRandomString(64, rd);
                IntPtr value = new IntPtr(i);
                artTree.Insert(artTree.tree, key.ToArray(), value);

                if (i % 10000 == 0)
                    tests.Add(key);
            }

            Console.WriteLine(DateTime.Now.ToString("hh:mm:ss fff"));
            foreach (string readKey in tests)
            {
                artTree.Search(artTree.tree, readKey.ToArray());
            }
            Console.WriteLine(DateTime.Now.ToString("hh:mm:ss fff"));
        }

        static void ARTInt64Test()
        {
            //init ART
            ARTInt64 artTree = new ARTInt64();

            Console.WriteLine(DateTime.Now.ToString("hh:mm:ss fff"));
            List<Int64> tests = new List<Int64>();
            Random rd = new Random();
            for (int i = 0; i < 100000000; i += 2)
            {
                Int64 key = rd.Next(1000000,Int32.MaxValue);
                IntPtr value = new IntPtr(i);
                artTree.Insert(artTree.tree, key, value);

                if (i % 10000 == 0)
                    tests.Add(key);
            }

            Console.WriteLine(DateTime.Now.ToString("hh:mm:ss fff"));
            foreach (Int64 readKey in tests)
            {
                IntPtr result = artTree.Search(artTree.tree, readKey);
                Console.WriteLine(result.ToInt32());
            }
            Console.WriteLine(DateTime.Now.ToString("hh:mm:ss fff"));
        }

        static int stringCompare(string val1, string val2)
        {
            return string.CompareOrdinal(val1, val2);
        }
        static string stringGetDefaultKey()
        {
            return "";
        }

        static IntPtr intPtrGetDefaultValue()
        {
            return IntPtr.Zero;
        }

        private static char[] constant = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        public static string GenerateRandomString(int length, Random rd)
        {
            string checkCode = String.Empty;
            for (int i = 0; i < length; i++)
            {
                checkCode += constant[rd.Next(36)].ToString();
            }
            return checkCode;
        }

    }
}

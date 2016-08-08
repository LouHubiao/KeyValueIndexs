using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndexTest
{
    //MAX_KEYS = 1024;

    class Node<K, V>
    {
        public int isLeaf;  //is this a leaf node?
        public List<K> keys = new List<K>();
        public List<V> values = new List<V>();
        public List<Node<K, V>> kids = new List<Node<K, V>>();
    }

    class B_Tree<K, V>
    {
        const int MAX_KEYS = 1024;

        //t1>t2, retuen 1; 
        //t1==t2, return 0;
        //t1<t2, return -1;
        public delegate int CompareT(K t1, K t2);
        CompareT compare;

        public delegate K GetDefaultKey();
        GetDefaultKey getDefaultKey;

        public delegate V GetDefaultValue();
        GetDefaultValue getDefaultVaule;

        public Node<K, V> root;

        /// <summary>
        /// init BTree
        /// </summary>
        /// <returns>BTree root</returns>
        public B_Tree(CompareT _compare, GetDefaultKey _getDefaultKey, GetDefaultValue _getDefaultVaule)
        {
            root = new Node<K, V>();
            root.isLeaf = 1;
            this.compare = _compare;
            this.getDefaultKey = _getDefaultKey;
            this.getDefaultVaule = _getDefaultVaule;
        }

        /// <summary>
        /// search key in full BTree's subtree
        /// </summary>
        /// <param name="b">root node of subtree</param>
        /// <param name="key">target key</param>
        /// <returns>the value of key</returns>
        public V Search(Node<K, V> b, K key)
        {
            int pos;
            V result;

            // have to check for empty tree
            if (b.keys.Count == 0)
            {
                return getDefaultVaule();
            }

            // look for smallest position that key fits below
            pos = SearchKeyInNode(b.keys, key);

            //return the value of key
            if (pos < b.keys.Count && compare(b.keys[pos], key) == 0)
            {
                result = b.values[pos];
                return result;
            }
            else
            {
                if (b.isLeaf == 0)
                {
                    //not found and not leaf, find kid
                    return Search(b.kids[pos], key);
                }
                else
                {
                    return getDefaultVaule();
                }
            }
        }

        //insert one node into BTree
        public void Insert(ref Node<K, V> b, K key, V value)
        {
            Node<K, V> b1;   //new left child
            Node<K, V> b2;   //new right child
            K medianKey = getDefaultKey();
            V medianValue;

            b2 = BTInsertInternal(b, key, value, ref medianKey, out medianValue);

            // split
            if (b2 != null)
            {
                // root to be child
                b1 = b;

                // make root point to b1 and b2
                b = new Node<K, V>();
                b.isLeaf = 0;
                b.keys.Add(medianKey);
                b.values.Add(medianValue);
                b.kids.Add(b1);
                b.kids.Add(b2);
            }
        }

        /// <summary>
        /// search in a node
        /// </summary>
        /// <param name="keys">node's keys</param>
        /// <param name="key">target key</param>
        /// <returns>pos in this node</returns>
        int SearchKeyInNode(List<K> keys, K key)
        {
            int lo = -1;
            int hi = keys.Count;
            int mid = -1;
            while (lo + 1 < hi)
            {
                mid = (lo + hi) / 2;
                if (compare(keys[mid], key) == 0)
                {
                    return mid;
                }
                else if (compare(keys[mid], key) < 0)
                {
                    lo = mid;
                }
                else
                {
                    hi = mid;
                }
            }
            return hi;
        }

        /// <summary>
        /// insert core function
        /// </summary>
        /// <param name="b">root node of subtree</param>
        /// <param name="key">insert key</param>
        /// <param name="value">insert value</param>
        /// <param name="medianKey">splie out the mid key</param>
        /// <param name="medianValue">splie out the mid value</param>
        /// <returns>if inserted return null, if splited return right child</returns>
        Node<K, V> BTInsertInternal(Node<K, V> b, K key, V value, ref K medianKey, out V medianValue)
        {
            int pos;    //insert pos
            int mid;    //splite pos
            K midKey = getDefaultKey();      //splited mid key
            V midValue;    //splited mid value
            Node<K, V> b2;

            pos = SearchKeyInNode(b.keys, key);

            if (pos < b.keys.Count && compare(b.keys[pos], key) == 0)
            {
                //find nothing to do
                medianValue = getDefaultVaule();
                return null;
            }

            if (b.isLeaf == 1)
            {
                /* everybody above pos moves up one space */
                b.keys.Insert(pos, key);
                b.values.Insert(pos, value);
            }
            else
            {
                /* insert in child */
                b2 = BTInsertInternal(b.kids[pos], key, value, ref midKey, out midValue);

                /* maybe insert a new key in b */
                if (b2 != null)
                {
                    b.keys.Insert(pos, midKey);
                    b.values.Insert(pos, midValue);
                    b.kids.Insert(pos + 1, b2);
                }
            }

            if (b.keys.Count > MAX_KEYS)
            {
                mid = b.keys.Count / 2;

                medianKey = b.keys[mid];
                medianValue = b.values[mid];

                b2 = new Node<K, V>();

                b2.isLeaf = b.isLeaf;

                //shallow copy but safe
                int movLen = b.keys.Count - mid - 1;
                b2.keys = b.keys.GetRange(mid + 1, movLen);
                b2.values = b.values.GetRange(mid + 1, movLen);
                b.keys.RemoveRange(mid, movLen + 1);
                b.values.RemoveRange(mid, movLen + 1);
                if (b.isLeaf == 0)
                {
                    //Console.WriteLine(b.kids.Count);
                    b2.kids = b.kids.GetRange(mid + 1, movLen + 1);
                    b.kids.RemoveRange(mid + 1, movLen + 1);
                }

                return b2;
            }

            medianValue = getDefaultVaule();
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndexTest
{
    class TrieNode
    {
        public IntPtr addr = new IntPtr(0);
        public TrieNode[] childrenIndexs = new TrieNode[62];
    }

    class TrieIndex
    {
        public TrieNode root;

        public TrieIndex()
        {
            root = new TrieNode();
        }

        public void Insert(string key, IntPtr value)
        {
            if (key == null)
                return;
            TrieNode node = root;
            TrieNode nodeCopy;
            for (int i = 0; i < key.Length; i++)
            {
                int offset = GetOffset(key[i]);
                nodeCopy = node.childrenIndexs[offset];
                if (nodeCopy == null)
                {
                    nodeCopy = new TrieNode();
                    node.childrenIndexs[offset] = nodeCopy;
                }
                node = nodeCopy;
            }
            node.addr = value;
        }

        public IntPtr Search(string key)
        {
            TrieNode node = root;
            for (int i = 0; i < key.Length; i++)
            {
                //search next
                int offset = GetOffset(key[i]);
                node = node.childrenIndexs[offset];
                if (node == null)
                {
                    return new IntPtr(0);
                }

                //if the last, return addr
                if (i == key.Length - 1)
                {
                    return node.addr;
                }
            }
            return new IntPtr(0);
        }

        public int GetOffset(char ch)
        {
            short assic = (short)ch;
            if (assic < 58)
                return assic - 48;
            else if (assic < 91)
                return assic - 65 + 10;
            else if (assic < 123)
                return assic - 97 + 36;
            return 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARTInt64Space
{
    class ARTInt64Node
    {
        public IntPtr value = IntPtr.Zero;
        public Int64 prefix;
        public ARTInt64Node leftChild;
        public ARTInt64Node rightChild;
    }

    class ARTInt64Tree
    {
        public ARTInt64Node root;
    }

    class ArtLeaf
    {
        public IntPtr value;

        public ArtLeaf(IntPtr _value)
        {
            this.value = _value;
        }
    }

    class ARTInt64
    {
        //tree
        public ARTInt64Tree tree;

        public ARTInt64()
        {
            tree = new ARTInt64Tree();
            tree.root = new ARTInt64Node();
        }

        //find from inode to last key item, and than search leaf
        public IntPtr Search(ARTInt64Tree t, Int64 key)
        {
            ARTInt64Node node = t.root;
            if (key == 0)
            {
                node = node.leftChild;
            }
            while (node != null && key != 0)
            {
                if (key == node.prefix)
                {
                    return node.value;
                }
                else if (key % 2 == 0)
                {
                    node = node.leftChild;
                }
                else
                {
                    node = node.rightChild;
                }
                key = key >> 1;
            }
            if (node == null)
                return IntPtr.Zero;
            else
                return node.value;
        }

        public void Insert(ARTInt64Tree t, Int64 key, IntPtr value)
        {
            ARTInt64Node node = t.root;
            if (key == 0)
            {
                node = node.leftChild;
            }
            while (key != 0)
            {
                if (key == node.prefix)
                {
                    node.value = value;
                }
                else if (key % 2 == 0)
                {
                    if (node.leftChild == null)
                    {
                        node.leftChild = new ARTInt64Node();
                        node.prefix = 
                    } 
                    else{
                        node = node.leftChild;
                    }
                }
                else
                {
                    if (node.rightChild == null)
                        node.rightChild = new ARTInt64Node();
                    node = node.rightChild;
                }
                key = key >> 1;
            }
            node.value = value;
        }
    }
}

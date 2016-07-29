using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IndexTest
{
    class ArtNode
    {
        public byte type;
        public byte num_children;
        public byte prefix_len;
        public char[] prefix;   //max prefix = 10
    }

    class ArtNode4 : ArtNode
    {
        public char[] keys = new char[4];
        public object[] children = new object[4];
    }

    class ArtNode16 : ArtNode
    {
        public char[] keys = new char[16];
        public object[] children = new object[16];
    }

    class ArtNode48 : ArtNode
    {
        public char[] keys = new char[256];
        public object[] children = new object[48];
    }

    class ArtNode256 : ArtNode
    {
        public object[] children = new object[256];
    }

    class ArtTree
    {
        public object root;
        public int size;
    }

    class ArtLeaf
    {
        public IntPtr value;

        public ArtLeaf(IntPtr _value)
        {
            this.value = _value;
        }
    }

    class ART
    {
        byte NODE4 = 1;
        byte NODE16 = 2;
        byte NODE48 = 3;
        byte NODE256 = 4;

        ArtTree tree;

        public ART()
        {
            tree.root = AllocNode(NODE4);
            tree.size = 0;
        }

        public IntPtr ArtSearch(ArtTree t, char[] key)
        {
            object node = t.root;
            int offset = 0;  //offset
            while (node != null)
            {
                //if is leaf, return value id key is equal 
                if (node is ArtLeaf)
                {
                    ArtLeaf leaf = node as ArtLeaf;
                    return leaf.value;
                }

                ArtNode artNode = (ArtNode)node;

                //if has prefix, must match
                if (artNode.prefix_len > 0)
                {
                    if (MatchPrefix(artNode, key, offset) == false)
                        return IntPtr.Zero;
                    offset += artNode.prefix_len;
                }

                // Recursively search
                node = FindNextChild(artNode, key[offset]);
                if (node == null)
                {
                    return IntPtr.Zero;
                }
                offset++;
            }
            return IntPtr.Zero;
        }

        public void Insert(ArtTree t, char[] key, IntPtr value)
        {

        }

        void Recursive_Insert(ArtNode node, char[] key, int offset, IntPtr value, ArtNode preNode, int preOffset)
        {
            //move prefix
            if (node.prefix_len > 0)
            {
                //change offset
            }

            //
            if (offset == key.Length - 1)
            {
                //jump to 0 child, insert or update
                //InsertLeaf(node, value, preNode, preOffset);
                //return;
            }

            int nodeOffset;
            object child = FindNextChild(node, key[offset], out nodeOffset);
            if (child == null)
            {
                if (offset == key.Length - 1)
                {
                    ArtLeaf newLeaf = new ArtLeaf(value);
                    //AddChild(inNode, key[offset], newLeaf);
                }
                else
                {
                    ArtNode newNode = AllocNode(NODE4);

                    if (offset != key.Length - 2)
                    {
                        //prefix add, copy from key to prefix [offset+1,length-2]
                        Array.Copy(key, offset + 1, newNode.prefix, 0, key.Length - offset - 3);
                        newNode.prefix_len = (byte)(key.Length - offset - 2);

                    }
                    //AddChild(inNode, key[offset], newNode);

                    ArtLeaf grandChild = new ArtLeaf(value);
                    //AddChild(newNode, key[length-1], grandChild);

                }
            }
            else
            {
                Recursive_Insert(child, key, offset + 1, value, node, );
            }
        }

        //find the child node, nodeOri must inNode
        object FindNextChild(ArtNode nodeOri, char c, out int nodeOffset)
        {
            switch (nodeOri.type)
            {
                case 1:     //node4
                    ArtNode4 node4 = (ArtNode4)nodeOri;
                    for (int i = 0; i < node4.num_children; i++)
                    {
                        if (node4.keys[i] == c)
                        {
                            nodeOffset = i;
                            return node4.children[i];
                        }
                    }
                    break;
                case 2:     //node16
                    ArtNode16 node16 = (ArtNode16)nodeOri;
                    int index16 = FindBinary(node16.keys, 0, node16.num_children - 1, c);
                    if (index16 != -1)
                    {
                        nodeOffset = index16;
                        return node16.children[index16];
                    }
                    break;
                case 3:     //node48, index-1 for 0 is meaningful
                    ArtNode48 node48 = (ArtNode48)nodeOri;
                    int index48 = node48.keys[c];
                    if (index48 != 0)
                    {
                        nodeOffset = index48;
                        return node48.children[index48 - 1];
                    }
                    break;
                case 4:     //ndoe256
                    ArtNode256 node256 = (ArtNode256)nodeOri;
                    if (node256.children[c] != null)
                    {
                        nodeOffset = (int)c;
                        return node256.children[c];
                    }
                    break;
                default:
                    break;
            }
            nodeOffset = -1;
            return null;
        }

        //binaary find
        int FindBinary(char[] array, int begin, int end, char target)
        {
            int mid = 0;
            while (begin < end)
            {
                mid = (begin + end) / 2;
                if (array[mid] == target)
                    return mid;
                else if (array[mid] < target)
                    return FindBinary(array, mid + 1, end, target);
                else
                    return FindBinary(array, begin, mid - 1, target);
            }
            return -1;
        }

        //match the longest prefix
        bool MatchPrefix(ArtNode node, char[] key, int offset)
        {
            int cmpLen = Math.Min(node.prefix_len, key.Length);
            for (int i = 0; i < cmpLen; i++)
            {
                if (node.prefix[i] != key[offset + i])
                    return false;
            }
            return true;
        }

        //alloc a node by diffent type
        ArtNode AllocNode(byte type)
        {
            ArtNode result;
            switch (type)
            {
                case 1:     //node4
                    result = new ArtNode4();
                    break;
                case 2:     //node16
                    result = new ArtNode16();
                    break;
                case 3:     //node48
                    result = new ArtNode48();
                    break;
                case 4:     //node256
                    result = new ArtNode256();
                    break;
                default:
                    return null;
            }
            result.type = type;
            return result;
        }
    }
}

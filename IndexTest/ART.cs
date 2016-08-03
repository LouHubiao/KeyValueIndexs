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
        public char[] prefix;
        public object[] children;
    }

    class ArtNode4 : ArtNode
    {
        public char[] keys;
    }

    class ArtNode16 : ArtNode
    {
        public char[] keys;
    }

    class ArtNode48 : ArtNode
    {
        public char[] keys;
    }

    class ArtNode256 : ArtNode
    {

    }

    class ArtTree
    {
        public ArtNode root;
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
        public enum NodeType
        {
            NODE4 = 1,
            NODE16 = 2,
            NODE48 = 3,
            NODE256 = 4,
        };


        public ArtTree tree;

        public ART()
        {
            tree = new ArtTree();
            tree.root = AllocNode(NodeType.NODE4);
            tree.size = 0;
        }

        //find from inode to last key item, and than search leaf
        public IntPtr Search(ArtTree t, char[] key)
        {
            ArtNode node = t.root;
            int offset = 0;  //offset
            int findOffset;
            while (offset != key.Length)
            {
                //if has prefix, must match
                if (node.prefix_len > 0)
                {
                    if (MatchPrefix(node, key, offset) == false)
                        return IntPtr.Zero;
                    offset += node.prefix_len;
                    if (offset == key.Length)
                        break;
                }

                // Recursively search
                node = FindNextChild(node, key[offset], out findOffset) as ArtNode;
                if (findOffset == -1)
                {
                    return IntPtr.Zero;
                }
                offset++;
            }

            //get leaf
            ArtLeaf leaf = FindLeaf(node);
            if (leaf != null)
                return leaf.value;

            return IntPtr.Zero;
        }

        //insert a key-value pair
        public void Insert(ArtTree t, char[] key, IntPtr value)
        {
            Recursive_Insert(t.root, key, 0, value, null, 0);
        }

        //insert value into node Recursive
        void Recursive_Insert(ArtNode node, char[] key, int offset, IntPtr value, object[] preChildren, int preIndex)
        {
            //try move prefix first
            if (node.prefix_len > 0)
            {
                //move offset
                int matchLength = PrefixMatch(node, key, offset);
                offset += matchLength;
                if (matchLength == node.prefix_len)
                {
                    //key length is more than same prefix, so add a node4(with prefix) with leaf in node
                    if (offset < key.Length)
                    {
                        //init node4 with prefix
                        ArtNode4 artnode4 = AllocNode(NodeType.NODE4) as ArtNode4;
                        artnode4.prefix_len = (byte)(key.Length - offset - 1);
                        artnode4.prefix = new char[artnode4.prefix_len];
                        Array.Copy(key, offset + 1, artnode4.prefix, 0, artnode4.prefix_len);
                        //add node4
                        AddChild(node, key[offset], artnode4, preChildren, preIndex);
                        //addd leaf into node4
                        Add_UpdateLeaf(artnode4, value);
                        return;
                    }
                    else if (offset == key.Length)
                    {
                        //update the value
                        Add_UpdateLeaf(node, value);
                    }
                }
                else
                {
                    //key length is more than same prefix, and prefix is more than same prefix too, so need splite prefix, nodeUpdate replace node, and node as a child, also add a leaf
                    //init nodeUpdate
                    ArtNode4 nodeUpdate = AllocNode(NodeType.NODE4) as ArtNode4;
                    nodeUpdate.prefix_len = (byte)matchLength;
                    nodeUpdate.prefix = new char[nodeUpdate.prefix_len];
                    Array.Copy(node.prefix, nodeUpdate.prefix, nodeUpdate.prefix_len);
                    //replace node
                    if (preChildren != null)
                        preChildren[preIndex] = nodeUpdate;
                    //add node into nodeUpdate
                    AddChild(nodeUpdate, node.prefix[matchLength], node, preChildren, preIndex);
                    //edit node prefix
                    for (int i = matchLength + 1; i < node.prefix_len; i++)
                    {
                        node.prefix[i - matchLength - 1] = node.prefix[i];
                    }
                    node.prefix_len = (byte)(node.prefix_len - matchLength - 1);
                    //key is full match
                    if (offset == key.Length)
                    {
                        //addd leaf into node4
                        Add_UpdateLeaf(nodeUpdate, value);
                        return;
                    }
                    else
                    {
                        //add node4 with leaf into nodeUpdate
                        ArtNode4 artnode4 = AllocNode(NodeType.NODE4) as ArtNode4;
                        artnode4.prefix_len = (byte)(key.Length - offset - 1);
                        artnode4.prefix = new char[artnode4.prefix_len];
                        Array.Copy(key, offset + 1, artnode4.prefix, 0, artnode4.prefix_len);
                        //add new node
                        AddChild(nodeUpdate, key[offset], artnode4, preChildren, preIndex);
                        //addd leaf into node4
                        Add_UpdateLeaf(artnode4, value);
                        return;
                    }
                }
            }

            //add(update) leaf
            if (offset == key.Length)
            {
                Add_UpdateLeaf(node, value);
                return;
            }

            int nodeOffset;
            ArtNode child = FindNextChild(node, key[offset], out nodeOffset) as ArtNode;
            //if no child, need add new node4 and new leaf
            if (child == null)
            {
                //add node
                ArtNode4 newNode = AllocNode(NodeType.NODE4) as ArtNode4;
                AddChild(node, key[offset], newNode, preChildren, preIndex);

                //add prefix
                newNode.prefix_len = (byte)(key.Length - 1 - offset);
                newNode.prefix = new char[newNode.prefix_len];
                Array.Copy(key, offset + 1, newNode.prefix, 0, newNode.prefix_len);

                //add leaf
                Add_UpdateLeaf(newNode, value);
            }
            else
            {
                Recursive_Insert(child, key, offset + 1, value, node.children, nodeOffset);
            }
        }

        //returen the length of matched prefix, with node.prefix and key from offset
        int PrefixMatch(ArtNode node, char[] key, int offset)
        {
            int minLen = Math.Min(node.prefix_len, key.Length - offset);
            for (int i = 0; i < minLen; i++)
            {
                if (key[offset + i] != node.prefix[i])
                    return i;
            }
            return minLen;
        }

        //add an child index
        void AddChild(ArtNode node, char keyItem, object child, object[] preChildren, int preIndex)
        {
            switch (node.type)
            {
                case 1:     //node4
                    AddChild4(node as ArtNode4, keyItem, child, preChildren, preIndex);
                    break;
                case 2:     //node16
                    AddChild16(node as ArtNode16, keyItem, child, preChildren, preIndex);
                    break;
                case 3:     //node48, index-1 for 0 is meaningful
                    AddChild48(node as ArtNode48, keyItem, child, preChildren, preIndex);
                    break;
                case 4:     //ndoe256
                    AddChild256(node as ArtNode256, keyItem, child, preChildren, preIndex);
                    break;
                default:
                    break;
            }
        }

        void AddChild4(ArtNode4 node, char keyItem, object child, object[] preChildren, int preIndex)
        {
            if (node.num_children < 4)
            {
                node.keys[node.num_children] = keyItem;
                node.children[node.num_children] = child;
                node.num_children++;
            }
            else
            {
                //replace artNode4 by artNode16
                ArtNode16 artNode16 = AllocNode(NodeType.NODE16) as ArtNode16;
                CopyHeader(node, artNode16);
                for (int i = 1; i < node.num_children; i++)
                {
                    for (int j = 0; j < i; j++)
                    {
                        if (node.keys[j] > node.keys[i])
                        {
                            char temp = node.keys[j];
                            node.keys[j] = node.keys[i];
                            node.keys[i] = temp;
                            object tempObj = node.children[j];
                            node.children[j] = node.children[i];
                            node.children[i] = tempObj;
                        }
                    }
                }
                Array.Copy(node.keys, artNode16.keys, node.num_children);
                Array.Copy(node.children, artNode16.children, node.num_children);
                if (preChildren != null)
                    preChildren[preIndex] = artNode16;
                //add child into artNode16
                AddChild16(artNode16, keyItem, child, preChildren, preIndex);
                if (preChildren == null)
                    tree.root = artNode16;
            }
        }

        void AddChild16(ArtNode16 node, char keyItem, object child, object[] preChildren, int preIndex)
        {
            if (node.num_children < 16)
            {
                int index = node.num_children;
                while (index > 0 && node.keys[index - 1] > keyItem)
                {
                    node.keys[index] = node.keys[index - 1];
                    node.children[index] = node.children[index - 1];
                    index--;
                }
                node.keys[index] = keyItem;
                node.children[index] = child;
                node.num_children++;
            }
            else
            {
                //replace artNode16 by artNode48
                ArtNode48 artNode48 = AllocNode(NodeType.NODE48) as ArtNode48;
                CopyHeader(node, artNode48);
                for (int i = 0; i < node.num_children; i++)
                {
                    if (node.keys[i] != 0)
                    {
                        artNode48.keys[node.keys[i]] = (char)(i + 1);
                        artNode48.children[i + 1] = node.children[i];
                    }
                }
                if (preChildren != null)
                    preChildren[preIndex] = artNode48;
                //add child into artNode48
                AddChild48(artNode48, keyItem, child, preChildren, preIndex);
                if (preChildren == null)
                    tree.root = artNode48;
            }
        }

        void AddChild48(ArtNode48 node, char keyItem, object child, object[] preChildren, int preIndex)
        {
            if (node.num_children < 47)
            {
                node.keys[keyItem] = (char)(node.num_children + 1);
                node.children[node.num_children + 1] = child;
                node.num_children++;
            }
            else
            {
                //replace artNode16 by artNode48
                ArtNode256 artNode256 = AllocNode(NodeType.NODE256) as ArtNode256;
                CopyHeader(node, artNode256);
                for (int i = 0; i < 256; i++)
                {
                    artNode256.children[i] = (node.keys[i] == 0) ? null : node.children[node.keys[i]];
                }
                if (preChildren != null)
                    preChildren[preIndex] = artNode256;
                //add child into artNode48
                AddChild256(artNode256, keyItem, child, preChildren, preIndex);
                if (preChildren == null)
                    tree.root = artNode256;
            }
        }

        void AddChild256(ArtNode256 node, char keyItem, object child, object[] preChildren, int preIndex)
        {
            node.children[keyItem] = child;
            node.num_children++;
        }

        //copy the header of ArtNode
        void CopyHeader(ArtNode source, ArtNode target)
        {
            target.num_children = source.num_children;
            if (source.prefix_len > 0)
            {
                if (target.prefix_len < source.prefix_len)
                    target.prefix = new char[target.prefix_len];
                target.prefix_len = source.prefix_len;
                Array.Copy(source.prefix, target.prefix, source.prefix_len);
            }

        }

        //add if not exit, update if exit
        void Add_UpdateLeaf(ArtNode node, IntPtr value)
        {
            ArtLeaf oriLeaf = FindLeaf(node);
            if (oriLeaf != null)
            {
                //update leaf
                oriLeaf.value = value;
            }
            else
            {
                //add leaf, leaf key = 1
                ArtLeaf newLeaf = new ArtLeaf(value);
                AddChild(node, (char)1, newLeaf, node.children, 0);
            }
        }

        //find leaf of node, the leaf must be the child of node
        ArtLeaf FindLeaf(ArtNode node)
        {
            if (node.num_children > 0)
            {
                switch (node.type)
                {
                    case 1:     //node4
                        ArtNode4 node4 = node as ArtNode4;
                        for (int i = 0; i < node4.num_children; i++)
                        {
                            if (node4.keys[i] == 1)
                                return node4.children[i] as ArtLeaf;
                        }
                        break;
                    case 2:     //node16
                        ArtNode16 node16 = node as ArtNode16;
                        if (node16.keys[0] == 1)
                            return node16.children[0] as ArtLeaf;
                        break;
                    case 3:     //node48
                        ArtNode48 node48 = node as ArtNode48;
                        if (node48.keys[1] != 0)
                            return node48.children[node48.keys[1]] as ArtLeaf;
                        break;
                    case 4:     //node256
                        ArtNode256 node256 = node as ArtNode256;
                        if (node256.children[1] != null)
                            return node256.children[1] as ArtLeaf;
                        break;
                    default:
                        break;
                }
            }
            return null;
        }

        //find the child node, nodeOri must inNode
        object FindNextChild(ArtNode node, char c, out int nodeOffset)
        {
            switch (node.type)
            {
                case 1:     //node4
                    ArtNode4 node4 = node as ArtNode4;
                    for (int i = 0; i < node4.num_children; i++)
                    {
                        if (node4.keys[i] == c)
                        {
                            nodeOffset = i;
                            return node4.children[i];
                        }
                    }
                    break;
                case 2:     //node16, can use binary search
                    ArtNode16 node16 = node as ArtNode16;
                    int index16 = FindBinary(node16.keys, 0, node16.num_children - 1, c);
                    if (index16 != -1)
                    {
                        nodeOffset = index16;
                        return node16.children[index16];
                    }
                    break;
                case 3:     //node48
                    ArtNode48 node48 = node as ArtNode48;
                    int index48 = node48.keys[c];
                    if (index48 != 0)
                    {
                        nodeOffset = index48;
                        return node48.children[nodeOffset];
                    }
                    break;
                case 4:     //ndoe256
                    ArtNode256 node256 = node as ArtNode256;
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
            while (begin <= end)
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

        //match the longest prefix of the key and node's prefix
        bool MatchPrefix(ArtNode node, char[] key, int offset)
        {
            int minLen = Math.Min(node.prefix_len, key.Length);
            for (int i = 0; i < minLen; i++)
            {
                if (node.prefix[i] != key[offset + i])
                    return false;
            }
            return true;
        }

        //alloc a node by diffent type
        ArtNode AllocNode(NodeType type)
        {
            ArtNode result;
            switch (type)
            {
                case NodeType.NODE4:
                    ArtNode4 artNode4 = new ArtNode4();
                    artNode4.keys = new char[4];
                    artNode4.children = new object[4];
                    result = artNode4;
                    break;
                case NodeType.NODE16:
                    ArtNode16 artNode16 = new ArtNode16();
                    artNode16.keys = new char[16];
                    artNode16.children = new object[16];
                    result = artNode16;
                    break;
                case NodeType.NODE48:
                    ArtNode48 artNode48 = new ArtNode48();
                    artNode48.keys = new char[256];
                    artNode48.children = new object[48];
                    result = artNode48;
                    break;
                case NodeType.NODE256:
                    ArtNode256 artNode256 = new ArtNode256();
                    artNode256.children = new object[256];
                    result = artNode256;
                    break;
                default:
                    return null;
            }
            result.type = (byte)type;
            return result;
        }
    }
}

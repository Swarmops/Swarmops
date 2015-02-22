using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Common.Interfaces;

namespace Swarmops.Common.Generics
{
    public class Tree<TNode>
        where TNode: IHasIdentity, IHasParentIdentity
    {
        public Tree()
        {
            RootNodes = new List<TreeNode<TNode>>();
            this._nodeLookup = new Dictionary<int, TreeNode<TNode>>();
        }

        public List<TreeNode<TNode>> RootNodes { get; private set; }
        private readonly Dictionary<int, TreeNode<TNode>> _nodeLookup;

        public TreeNode<TNode> this [int identity]
        {
            get { return this._nodeLookup[identity]; }
        }

        public static Tree<TNode> FromCollection (List<TNode> collection)
        {
            Dictionary<int, List<TreeNode<TNode>>> parentLookup = new Dictionary<int, List<TreeNode<TNode>>>();
            Tree<TNode> result = new Tree<TNode>();

            // To construct a tree from a random collection, start by keying all parent identities into a dictionary

            foreach (TNode item in collection)
            {
                TreeNode<TNode> node = new TreeNode<TNode>(item);

                if (!parentLookup.ContainsKey ((item.ParentIdentity)))
                {
                    parentLookup[item.ParentIdentity] = new List<TreeNode<TNode>>();
                }
                parentLookup[item.ParentIdentity].Add (node);
                result._nodeLookup[item.Identity] = node;
            }

            // Then, recurse into that dictionary, starting at parentId zero which becomes the root level

            result.RootNodes = ConstructRecurse (null, parentLookup);

            return result;
        }

        private static List<TreeNode<TNode>> ConstructRecurse (TreeNode<TNode> parent, Dictionary<int, List<TreeNode<TNode>>> parentLookup)
        {
            List<TreeNode<TNode>> result = new List<TreeNode<TNode>>();
            int nodeId = parent == null ? 0 : parent.Data.Identity;

            if (!parentLookup.ContainsKey (nodeId))
            {
                return result; // no nodes with requested parent - exit condition hit, return empty list
            }

            foreach (TreeNode<TNode> node in parentLookup[nodeId])
            {
                node.Parent = parent;
                node.Children = ConstructRecurse (node, parentLookup);
                result.Add (node);
            }

            return result;
        }
    }

    public class TreeNode<TNode>
        where TNode: IHasIdentity, IHasParentIdentity
    {
        public TreeNode()
        {
            Children = new List<TreeNode<TNode>>();
        }

        public TreeNode (TNode data)
        {
            Data = data;
        }

        public TNode Data;
        public List<TreeNode<TNode>> Children;
        public TreeNode<TNode> Parent;
    }


}

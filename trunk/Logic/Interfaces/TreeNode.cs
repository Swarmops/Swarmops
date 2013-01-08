using System.Collections.Generic;
namespace Swarmops.Logic.Interfaces
{
    internal interface ITreeNode
    {
        int Identity { get; }

        int ParentIdentity { get; }

        string Name { get; }

        int[] ChildrenIdentities { get; }
    }

    public interface ITreeNodeObject
    {
        int Identity { get; }

        int ParentIdentity { get; }

        string Name { get; }

        int[] ChildrenIdentities { get; }

        ITreeNodeObject ParentObject { get; }

        List<ITreeNodeObject> ChildObjects { get; }
        
    }

}
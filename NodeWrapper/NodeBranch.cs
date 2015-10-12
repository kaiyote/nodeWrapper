using LibGit2Sharp;

namespace NodeWrapper
{
    public class NodeBranch
    {
        internal Branch Branch;

        public NodeBranch(Branch branch)
        {
            Branch = branch;
        }

        public string Name()
        {
            return Branch.Name;
        }

        public bool IsHead()
        {
            return Branch.IsCurrentRepositoryHead;
        }

        public bool IsRemote()
        {
            return Branch.IsRemote;
        }

        public bool IsTracking()
        {
            return Branch.IsTracking;
        }

        public NodeBranch Remote()
        {
            return new NodeBranch(Branch.TrackedBranch);
        }
    }
}

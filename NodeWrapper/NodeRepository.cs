using System.Collections.Generic;
using LibGit2Sharp;

namespace NodeWrapper
{
    public class NodeRepository
    {
        internal Repository Repository;

        public NodeRepository(string path)
        {
            Repository = new Repository(path);
        }

        public NodeRepository(Repository repo)
        {
            Repository = repo;
        }

        public BlameHunkCollection Blame(string path, BlameOptions options = null)
        {
            return Repository.Blame(path, options ?? new BlameOptions());
        }

        public NodeBranch Checkout(Branch branch, CheckoutOptions options = null, Signature signature = null)
        {
            return new NodeBranch(Repository.Checkout(branch, options ?? new CheckoutOptions(), signature));
        }

        public NodeBranch Checkout(Commit commit, CheckoutOptions options = null, Signature signature = null)
        {
            return new NodeBranch(Repository.Checkout(commit, options ?? new CheckoutOptions(), signature));
        }

        public NodeBranch Checkout(string commitishOrBranchSpec, CheckoutOptions options = null, Signature signature = null)
        {
            return new NodeBranch(Repository.Checkout(commitishOrBranchSpec, options ?? new CheckoutOptions(), signature));
        }

        public void CheckoutPaths(string commitishOrBranchSpec, IEnumerable<string> paths, CheckoutOptions options = null)
        {
            Repository.CheckoutPaths(commitishOrBranchSpec, paths, options ?? new CheckoutOptions());
        }

        public CherryPickResult CherryPick(Commit commit, Signature signature, CherryPickOptions options = null)
        {
            return Repository.CherryPick(commit, signature, options ?? new CherryPickOptions());
        }
    }
}

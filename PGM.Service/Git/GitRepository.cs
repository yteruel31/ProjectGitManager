using System;
using LibGit2Sharp;
using NLog;
using PGM.Model;

namespace PGM.Service.Git
{
    public class GitRepository : IGitRepository
    {
        private string FullName => Settings.FullName;
        private string RepositoryPath => "D:\\git\\testforpgm";
        private Branch MasterBranch => _repository.Branches["master"];
        private Remote OriginRemote => _repository.Network.Remotes["origin"];
        private string Email => Settings.Email;
        private IPgmSettingManagerService _pgmSettingManagerService;
        private PGMSetting Settings => _pgmSettingManagerService.CurrentSettings;
        private readonly Repository _repository;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();


        public GitRepository(IPgmSettingManagerService pgmSettingManagerService)
        {
            _pgmSettingManagerService = pgmSettingManagerService;
            _repository = new Repository(RepositoryPath);
        }

        public GitResult<Branch> CheckoutMaster()
        {
            try
            {
                Branch resultBranch = Commands.Checkout(_repository, MasterBranch);

                return new GitResult<Branch>(true, resultBranch);
            }
            catch (LibGit2SharpException e)
            {
                return new GitResult<Branch>(false, e);
            }
        }

        private Credentials GetCredentials(string username, string password)
        {
            return new UsernamePasswordCredentials
            {
                Username = username,
                Password = password
            };
        }

        public GitResult<Branch> CheckoutIssueBranch(string issueId)
        {
            try
            {
                _repository.CreateBranch($"issue/{issueId}");
                Branch branch = _repository.Branches[$"issue/{issueId}"];
                _repository.Branches.Update(branch,
                    b => b.Remote = OriginRemote.Name, b => b.UpstreamBranch = branch.CanonicalName);
                Branch resultBranch = Commands.Checkout(_repository, branch);
                PushOnOriginBranch(branch);

                return new GitResult<Branch>(true, resultBranch);
            }
            catch (LibGit2SharpException e)
            {
                return new GitResult<Branch>(false, e);
            }
        }

        public GitResult<MergeStatus> PullOnRepository()
        {
            try
            {
                MergeResult mergeResult = Commands.Pull(_repository, GetSignature(), null);
                return new GitResult<MergeStatus>(true, mergeResult.Status);
            }
            catch (LibGit2SharpException e)
            {
                return new GitResult<MergeStatus>(false, e);
            }
        }

        public GitResult<RebaseStatus> RebaseOntoMaster(Branch actualBranch)
        {
            try
            {
                RebaseResult rebaseResult = _repository.Rebase.Start(actualBranch, actualBranch, MasterBranch, GetIdentity(), null);
                return new GitResult<RebaseStatus>(true, rebaseResult.Status);
            }
            catch (LibGit2SharpException e)
            {
                return new GitResult<RebaseStatus>(false, e);
            }
        }

        public GitResult PushOnOriginMaster()
        {
            try
            {
                _repository.Network.Push(MasterBranch);

                return new GitResult(true, "OK");
            }
            catch (LibGit2SharpException e)
            {
                return new GitResult(false, e);
            }
        }

        public GitResult PushOnOriginBranch(Branch branch)
        {
            try
            {
                _repository.Network.Push(OriginRemote, branch.UpstreamBranchCanonicalName, new PushOptions
                {
                    CredentialsProvider = (url, fromUrl, types) => GetCredentials(Settings.Credential.Username, Settings.Credential.Password)
                });
                return new GitResult(true, "OK");
            }
            catch (LibGit2SharpException e)
            {
                return new GitResult(false, e);
            }
        }

        public GitResult DeleteLocalBranch(Branch branch)
        {
            try
            {
                _repository.Branches.Remove(branch);
                return new GitResult(true, "OK");
            }
            catch (LibGit2SharpException e)
            {
                return new GitResult(false, e);
            }
        }

        private Signature GetSignature()
        {
            return new Signature(GetIdentity(), DateTimeOffset.Now);
        }

        private Identity GetIdentity()
        {
            return new Identity(FullName, Email);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using LibGit2Sharp;
using NLog;
using PGM.Model;

namespace PGM.Service.Git
{
    public class GitRepository : IGitRepository
    {
        private string FullName => Settings.FullName;
        private Branch MasterBranch => _repository.Branches["master"];
        private Branch UpStreamMasterBranch => _repository.Branches["origin/master"];
        private Remote OriginRemote => _repository.Network.Remotes["origin"];
        private string Email => Settings.Email;
        private readonly IPgmSettingManagerService _pgmSettingManagerService;
        private PGMSetting Settings => _pgmSettingManagerService.CurrentSettings;
        private Repository _repository;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();


        public GitRepository(IPgmSettingManagerService pgmSettingManagerService)
        {
            _pgmSettingManagerService = pgmSettingManagerService;
        }

        public void SetupRepository(GitlabProject currentProject)
        {
            if (currentProject.RepositoryPath == null)
            {
                return;
            }

            Settings.CurrentGitlabProject = currentProject;
            _repository = new Repository(currentProject.RepositoryPath);
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
                if (!IssueBranchExist(issueId))
                {
                    Branch createdBranch = CreateNewIssueBranch(issueId);
                    Commands.Checkout(_repository, createdBranch);

                    return new GitResult<Branch>(true, createdBranch);
                }

                Branch branch = GetIssueBranch(issueId, true);
                Branch resultBranch = Commands.Checkout(_repository, branch);

                return new GitResult<Branch>(true, resultBranch);
            }
            catch (LibGit2SharpException e)
            {
                return new GitResult<Branch>(false, e);
            }
        }

        private Branch CreateNewIssueBranch(string issueId)
        {
            Branch createdBranch = _repository.CreateBranch($"issue/{issueId}");
            _repository.Branches.Update(createdBranch,
                b => b.Remote = OriginRemote.Name, b => b.UpstreamBranch = createdBranch.CanonicalName);

            return createdBranch;
        }

        private Branch GetIssueBranch(string issueId, bool isOnRemote)
        {
            if (!isOnRemote)
            {
                return _repository.Branches[$"issue/{issueId}"];
            }

            List<Branch> remoteBranches = _repository.Branches.Where(b => b.IsRemote).ToList();
            return remoteBranches.Single(rb => rb.FriendlyName.EndsWith($"issue/{issueId}"));
        }

        private bool IssueBranchExist(string issueId)
        {
            return _repository.Branches.Any(b => b.FriendlyName.Contains($"issue/{issueId}"));
        }

        public GitResult<Branch> GetActualBranch(string issueId)
        {
            try
            {
                Branch branch = _repository.Branches[$"issue/{issueId}"];

                return new GitResult<Branch>(true, branch);
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
                MergeResult mergeResult = Commands.Pull(_repository, GetSignature(), new PullOptions
                {
                    FetchOptions = new FetchOptions
                    {
                        CredentialsProvider = (url, fromUrl, types) => GetCredentials(Settings.Credential.Username, Settings.Credential.Password)
                    },
                    MergeOptions = new MergeOptions
                    {
                        FailOnConflict = true
                    }
                });
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
                RebaseResult rebaseResult = _repository.Rebase.Start(actualBranch, UpStreamMasterBranch, MasterBranch, GetIdentity(), null);
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
                _repository.Network.Push(MasterBranch, new PushOptions
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

        public GitResult PushOnOriginBranch(Branch branch, bool force = false)
        {
            try
            {
                if (!force)
                {
                    _repository.Network.Push(OriginRemote, branch.UpstreamBranchCanonicalName, new PushOptions
                    {
                        CredentialsProvider = (url, fromUrl, types) => GetCredentials(Settings.Credential.Username, Settings.Credential.Password)
                    });
                }
                else
                {
                    GetGitProcess($"push origin {branch.FriendlyName} --force").Start();
                }
               
                return new GitResult(true, "OK");
            }
            catch (LibGit2SharpException e)
            {
                return new GitResult(false, e);
            }
        }

        private Process GetGitProcess(string args)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("git.exe")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = Settings.CurrentGitlabProject.RepositoryPath,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = args,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            return new Process
            {
                StartInfo = startInfo
            };
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

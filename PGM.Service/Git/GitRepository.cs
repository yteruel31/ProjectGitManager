﻿using System;
using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp;
using NLog;
using PGM.Service.Utilities;

namespace PGM.Service.Git
{
    public class GitRepository : IGitRepository
    {
        private string RepositoryPath => _settings.RepositoryPath;
        private string UserName => _settings.FullName;
        private Branch MasterBranch => _repository.Branches["master"];
        private string Email => _settings.Email;
        private readonly IPGMSettings _settings;
        private Repository _repository;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();


        public GitRepository(IPGMSettings settings)
        {
            _settings = settings;
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

        public GitResult FetchRemote()
        {
            string logMessage = string.Empty;
            try
            {
                Remote remote = _repository.Network.Remotes["origin"];
                IEnumerable<string> refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
                Commands.Fetch(_repository, remote.Name, refSpecs, null, logMessage);
                Logger.Info(logMessage);

                return new GitResult(true, logMessage);
            }
            catch (LibGit2SharpException e)
            {
                return new GitResult(false, e);
            }
        }

        public GitResult<Branch> CheckoutIssueBranch(string issueId)
        {
            try
            {
                FetchRemote();
                CheckoutMaster();
                _repository.CreateBranch($"issue/{issueId}");
                Branch branch = _repository.Branches[$"issue/{issueId}"];
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
                _repository.Network.Push(branch);
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
            return new Identity(UserName, Email);
        }
    }
}

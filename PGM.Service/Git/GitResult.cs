using LibGit2Sharp;

namespace PGM.Service.Git
{
    public class GitResult<T>
    {
        public GitResult(bool hasSucceeded, T response)
        {
            HasSucceeded = hasSucceeded;
            Response = response;
        }

        public GitResult(bool hasSucceeded, LibGit2SharpException messageException)
        {
            HasSucceeded = hasSucceeded;
            MessageException = messageException;
        }

        public bool HasSucceeded { get; }

        public T Response { get; }

        public LibGit2SharpException MessageException { get; }
    }

    public class GitResult
    {
        public GitResult(bool hasSucceeded, string response)
        {
            HasSucceeded = hasSucceeded;
            Response = response;
        }

        public GitResult(bool hasSucceeded, LibGit2SharpException messageException)
        {
            HasSucceeded = hasSucceeded;
            MessageException = messageException;
        }

        public bool HasSucceeded { get; }

        public string Response { get; }

        public LibGit2SharpException MessageException { get; }
    }
}
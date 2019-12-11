namespace PGM.Model
{
    public interface ISettings
    {
        string GitLabApiKey { get; }

        string RepoPath { get; }

        string GitLabUserName { get; }
    }
}

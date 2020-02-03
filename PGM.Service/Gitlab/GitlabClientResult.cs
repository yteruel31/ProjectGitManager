namespace PGM.Service.Gitlab
{
    public class GitlabClientResult
    {
        public GitlabClientResult(bool hasSucceeded, string response)
        {
            HasSucceeded = hasSucceeded;
            Response = response;
        }

        public bool HasSucceeded { get; }

        public string Response { get; }
    }
}
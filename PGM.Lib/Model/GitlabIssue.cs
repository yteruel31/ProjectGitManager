namespace PGM.Lib.Model
{
    public class GitlabIssue
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public Sprint Sprint { get; set; }
    }

    public class Sprint
    {
        public int Id { get; set; }

        public string Title { get; set; }
    }
}
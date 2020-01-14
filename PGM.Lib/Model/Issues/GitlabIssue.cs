using System.Collections.Generic;

namespace PGM.Lib.Model.Issues
{
    public class GitlabIssue
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public GitlabMilestone GitlabMilestone { get; set; }
        
        public List<GitlabLabel> Labels { get; set; }

        public StepType StepType { get; set; }
    }

}
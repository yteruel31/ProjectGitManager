using System;
using System.Collections.Generic;

namespace PGM.Model
{
    public class GitlabIssue
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public GitlabMilestone GitlabMilestone { get; set; }
        
        public List<GitlabLabel> Labels { get; set; }

        public List<GitlabAssignee> Assignees { get; set; }

        public StepType StepType { get; set; }
    }

    public class GitlabAssignee
    {
        public int Id { get; set; }

        public string State { get; set; }

        public string AvatarUrl { get; set; }

        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Username { get; set; }
    }
}
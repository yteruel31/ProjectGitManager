using GitLabApiClient.Models;
using PGM.Model;

namespace PGM.Service.Gitlab
{
    public static class GitlabExtension
    {
        public static GitlabAssignee ToGitLabAssignee(this Assignee assignee)
        {
            return new GitlabAssignee
            {
                Id = assignee.Id,
                Name = assignee.Name,
                Username = assignee.Username,
                State = assignee.State,
                AvatarUrl = assignee.AvatarUrl,
                CreatedAt = assignee.CreatedAt
            };
        }
    }
}
using System;

namespace PGM.Model
{
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
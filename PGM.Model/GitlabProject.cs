using System.Collections.Generic;
using Newtonsoft.Json;

namespace PGM.Model
{
    public class GitlabProject
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string RepositoryPath { get; set; }
        
        [JsonIgnore]
        public List<GitlabIssue> Issues { get; set; }
    }
}
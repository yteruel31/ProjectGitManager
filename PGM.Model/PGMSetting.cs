using System.Collections.Generic;
using Newtonsoft.Json;

namespace PGM.Model
{
    public class PGMSetting
    {
        public string GitLabApiKey { get; set; }
        
        public string FullName { get; set; }
        
        public string Email { get; set; }

        public Credential Credential { get; set; }

        [JsonIgnore]
        public GitlabProject CurrentGitlabProject { get; set; }

        public List<GitlabProject> Projects { get; set; }

        [JsonIgnore]
        public bool PgmHasSetup { get; set; }
    }
}
using System.Collections.Generic;

namespace PGM.Model
{
    public class PGMSetting
    {
        public string GitLabApiKey { get; set; }
        
        public string FullName { get; set; }
        
        public string Email { get; set; }

        public Credidential Credidential { get; set; }

        public Project CurrentProject { get; set; }

        public List<Project> Projects { get; set; }

        public bool SettingsIsSetup { get; set; }
    }
}
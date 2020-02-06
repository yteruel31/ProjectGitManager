using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace PGM.GUI.ViewModel
{
    public class PGMSettingVO : ObservableObject
    {
        private string _gitLabApiKey;
        private string _email;
        private string _fullName;
        private bool _settingsIsSetup;

        public string GitLabApiKey
        {
            get { return _gitLabApiKey; }
            set
            {
                if (_gitLabApiKey != value)
                {
                    Set(nameof(GitLabApiKey), ref _gitLabApiKey, value);
                }
            }
        }

        public string FullName
        {
            get { return _fullName; }
            set
            {
                if (_fullName != value)
                {
                    Set(nameof(FullName), ref _fullName, value);
                }
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                if (_email != value)
                {
                    Set(nameof(Email), ref _email, value);
                }
            }
        }

        public bool SettingsIsSetup
        {
            get { return _settingsIsSetup; }
            set
            {
                if (_settingsIsSetup != value)
                {
                    Set(nameof(SettingsIsSetup), ref _settingsIsSetup, value);
                }
            }
        }

        public List<ProjectVO> Projects { get; set; }
    }
}
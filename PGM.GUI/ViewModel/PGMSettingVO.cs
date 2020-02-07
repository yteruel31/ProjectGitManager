using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using GalaSoft.MvvmLight;

namespace PGM.GUI.ViewModel
{
    public class PGMSettingVO : ObservableObject
    {
        private string _gitLabApiKey;
        private string _email;
        private string _fullName;
        private bool _pgmHasSetup;
        private ICollectionView _groupedProjects;
        private CredentialVO _credential;

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

        public CredentialVO Credential
        {
            get { return _credential; }
            set
            {
                if (_credential == null)
                {
                    _credential = new CredentialVO();
                }

                if (_credential != value)
                {
                    Set(nameof(Credential), ref _credential, value);
                }
            }
        }

        public bool PgmHasSetup
        {
            get { return _pgmHasSetup; }
            set

            {
                if (_pgmHasSetup != value)
                {
                    Set(nameof(PgmHasSetup), ref _pgmHasSetup, value);
                }
            }
        }

        public ObservableCollection<ProjectVO> Projects { get; set; } = new ObservableCollection<ProjectVO>();
    }

    public class CredentialVO : ObservableObject
    {
        private string _userName;
        private string _password;

        public string Username
        {
            get { return _userName; }
            set
            {
                if (_userName != value)
                {
                    Set(nameof(Username), ref _userName, value);
                }
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    Set(nameof(Password), ref _password, value);
                }
            }
        }
    }
}
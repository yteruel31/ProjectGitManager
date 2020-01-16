using GalaSoft.MvvmLight;

namespace PGM.GUI.ViewModel
{
    public class PGMSettingsVO : ObservableObject
    {
        private string _gitApiKey;
        private string _projectId;
        private string _email;
        private string _repositoryPath;
        private string _fullName;

        public string GitApiKey
        {
            get { return _gitApiKey; }
            set
            {
                if (_gitApiKey != value)
                {
                    Set(nameof(GitApiKey), ref _gitApiKey, value);
                }
            }
        }

        public string RepositoryPath
        {
            get { return _repositoryPath; }
            set
            {
                if (_repositoryPath != value)
                {
                    Set(nameof(RepositoryPath), ref _repositoryPath, value);
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

        public string ProjectId
        {
            get { return _projectId; }
            set
            {
                if (_projectId != value)
                {
                    Set(nameof(ProjectId), ref _projectId, value);
                }
            }
        }
    }
}
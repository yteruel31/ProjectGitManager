using System;
using System.IO;
using Newtonsoft.Json;

namespace PGM.Service.Utilities
{
    public class PGMSettings : IPGMSettings
    {
        private string _gitApiKey;
        private string _repositoryPath;
        private string _fullName;
        private string _projetId;
        private string _email;
        private bool _isRead;

        public string GitApiKey
        {
            get
            {
                if (string.IsNullOrEmpty(_gitApiKey))
                {
                    Read();
                }

                return _gitApiKey;
            }
            set
            {
                _gitApiKey = value;
                Write();
            }
        }

        public string RepositoryPath
        {
            get
            {
                if (string.IsNullOrEmpty(_repositoryPath))
                {
                    Read();
                }

                return _repositoryPath;
            }
            set
            {
                _repositoryPath = value;
                Write();
            }
        }

        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(_fullName))
                {
                    Read();
                }

                return _fullName;
            }
            set
            {
                _fullName = value;
                Write();
            }
        }

        public string Email
        {
            get
            {
                if (string.IsNullOrEmpty(_email))
                {
                    Read();
                }

                return _email;
            }
            set
            {
                _email = value;
                Write();
            }
        }

        public string ProjectId {
            get
            {
                if (string.IsNullOrEmpty(_projetId))
                {
                    Read();
                }

                return _projetId;
            }
            set
            {
                _projetId = value;
                Write();
            }
        }

        private string GetSettingsPath()
        {
            return Path.Combine(GetFolderPath(), "settings.json");
        }

        private string GetFolderPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PGM");
        }

        private void Read()
        {
            string path = GetSettingsPath();
            if (!File.Exists(path))
            {
                _gitApiKey = "NA";
                _repositoryPath = "NA";
                _fullName = "NA";
                _email = "NA";
                _projetId = "NA";
                Write();
                _isRead = true;
                return;
            }

            using (StreamReader sr = new StreamReader(path))
            {
                string str = sr.ReadToEnd();
                PGMSettings settings = JsonConvert.DeserializeObject<PGMSettings>(str);
                _gitApiKey = settings._gitApiKey;
                _repositoryPath = settings._repositoryPath;
                _fullName = settings._fullName;
                _email = settings._email;
                _projetId = settings._projetId;
            }

            _isRead = true;
        }

        public PGMSettings GetPGMSettings => this;

        private void Write()
        {
            if (!_isRead)
            {
                return;
            }

            if (!Directory.Exists(GetFolderPath()))
            {
                Directory.CreateDirectory(GetFolderPath());
            }

            string str = JsonConvert.SerializeObject(this);

            using (StreamWriter sw = new StreamWriter(GetSettingsPath()))
            {
                sw.Write(str);
            }
        }
    }
}
using System;
using System.IO;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using PGM.GUI.Properties;
using PGM.Lib.Model;

namespace PGM.GUI.ViewModel
{
    public class SettingsViewModel : ObservableObject, IPGMSettings
    {
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

        public string Accronyme
        {
            get
            {
                if (string.IsNullOrEmpty(_accronyme))
                {
                    Read();
                }

                return _accronyme;
            }
            set
            {
                _accronyme = value;
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

        private bool _isRead;

        private void Read()
        {
            string path = GetSettingsPath();
            if (!File.Exists(path))
            {
                _gitApiKey = Settings.Default.GitApiKey;
                _repositoryPath = Settings.Default.Repertoire;
                _accronyme = Settings.Default.Accronyme;
                _projetId = Settings.Default.ProjectId;
                Write();
                _isRead = true;
                return;
            }

            using (StreamReader sr = new StreamReader(path))
            {
                var str = sr.ReadToEnd();
                SettingsViewModel settings = JsonConvert.DeserializeObject<SettingsViewModel>(str);
                _gitApiKey = settings._gitApiKey;
                _repositoryPath = settings._repositoryPath;
                _accronyme = settings._accronyme;
                _projetId = settings._projetId;
            }

            _isRead = true;
        }

        private string _gitApiKey;
        private string _repositoryPath;
        private string _accronyme;
        private string _projetId;

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
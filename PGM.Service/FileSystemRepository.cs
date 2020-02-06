using System;
using System.IO;
using Newtonsoft.Json;

namespace PGM.Service
{
    public class FileSystemRepository : IFileSystemRepository
    {
        private bool _isRead;

        private string GetDataPath()
        {
            return Path.Combine(GetFolderPath(), "data.json");
        }

        private string GetFolderPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PGM");
        }

        public bool FileExist(string filePath)
        {
            return File.Exists(filePath);
        }

        public void WriteOnFileData(object objectToJson)
        {
            if (!_isRead)
            {
                return;
            }

            if (!Directory.Exists(GetFolderPath()))
            {
                Directory.CreateDirectory(GetFolderPath());
            }

            string str = JsonConvert.SerializeObject(objectToJson);

            using (StreamWriter sw = new StreamWriter(GetDataPath()))
            {
                sw.Write(str);
            }
        }

        public FileSystemResult<T> ReadOnFileData<T>()
        {
            if (!FileExist(GetDataPath()))
            {
                return new FileSystemResult<T>(true, "Fichier non trouvé");
            }

            string dataPath = GetDataPath();
            T jsonToObject;

            using (StreamReader sr = new StreamReader(dataPath))
            {
                string str = sr.ReadToEnd();
                jsonToObject = JsonConvert.DeserializeObject<T>(str);
            }

            _isRead = true;

            return new FileSystemResult<T>(true, jsonToObject);
        }
    }


    public class FileSystemResult<T>
    {
        public FileSystemResult(bool hasSucceeded, T type)
        {
            HasSucceeded = hasSucceeded;
            Type = type;
        }

        public FileSystemResult(bool hasSucceeded, string response)
        {
            HasSucceeded = hasSucceeded;
            Response = response;
        }

        public bool HasSucceeded { get; set; }

        public string Response { get; set; }

        public T Type { get; set; }
    }
}
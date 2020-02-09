namespace PGM.Service
{
    public interface IFileSystemRepository
    {
        void WriteOnFileData(object objectToJson);

        FileSystemResult<T> ReadOnFileData<T>();

        bool DirectoryExist(string directoryPath);

        bool DataFileExist();
    }
}
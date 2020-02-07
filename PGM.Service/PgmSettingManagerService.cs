using PGM.Model;

namespace PGM.Service
{
    public class PgmSettingManagerService : IPgmSettingManagerService
    {
        private readonly IFileSystemRepository _fileSystemRepository;

        public PGMSetting CurrentSettings { get; set; }

        public PgmSettingManagerService(IFileSystemRepository fileSystemRepository)
        {
            _fileSystemRepository = fileSystemRepository;
            SetCurrentSettings();
        }

        private void SetCurrentSettings()
        {
            FileSystemResult<PGMSetting> result = _fileSystemRepository.ReadOnFileData<PGMSetting>();

            if (result.HasSucceeded)
            {
                CurrentSettings = result.Type;
            }
        }
    }

    public interface IPgmSettingManagerService
    {
        PGMSetting CurrentSettings { get; set; }
    }
}
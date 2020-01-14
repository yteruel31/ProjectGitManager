// ReSharper disable InconsistentNaming
namespace PGM.Lib.Model
{
    public class PGMSettings : IPGMSettings
    {
        private readonly IPGMSettings _pgmSettings;

        public PGMSettings(IPGMSettings pgmSettings)
        {
            _pgmSettings = pgmSettings;
        }

        public string GitApiKey => _pgmSettings.GitApiKey;
        public string RepositoryPath => _pgmSettings.RepositoryPath;
        public string Accronyme => _pgmSettings.Accronyme;
        public string ProjectId => _pgmSettings.ProjectId;
    }
}
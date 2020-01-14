// ReSharper disable InconsistentNaming
namespace PGM.Lib.Model
{
    public interface IPGMSettings
    {
        string GitApiKey { get; }

        string RepositoryPath { get; }

        string Accronyme { get; }

        string ProjectId { get; }
    }
}
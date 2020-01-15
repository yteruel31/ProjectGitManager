// ReSharper disable InconsistentNaming
namespace PGM.Lib.Model
{
    public interface IPGMSettings
    {
        string GitApiKey { get; }

        string RepositoryPath { get; }

        string FullName { get; }

        string Email { get; }

        string ProjectId { get; }
    }
}
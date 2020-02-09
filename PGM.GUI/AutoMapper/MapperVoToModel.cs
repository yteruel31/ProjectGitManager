using AutoMapper;
using PGM.GUI.ViewModel;
using PGM.Model;

namespace PGM.GUI.AutoMapper
{
    public class MapperVoToModel : IMapperVoToModel
    {
        public IMapper Mapper { get; set; }

        public MapperVoToModel()
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PGMSetting, PGMSettingVO>();
                cfg.CreateMap<PGMSettingVO, PGMSetting>();
                cfg.CreateMap<ProjectVO, GitlabProject>();
                cfg.CreateMap<GitlabProject, ProjectVO>();
                cfg.CreateMap<GitlabIssueVO, GitlabIssue>();
                cfg.CreateMap<GitlabIssue, GitlabIssueVO>();
                cfg.CreateMap<CredentialVO, Credential>();
                cfg.CreateMap<Credential, CredentialVO>();
                cfg.CreateMap<GitlabAssigneeVO, GitlabAssignee>();
                cfg.CreateMap<GitlabAssignee, GitlabAssigneeVO>();
            });
            Mapper = config.CreateMapper();
        }
    }

    public interface IMapperVoToModel
    {
        IMapper Mapper { get; set; }
    }
}

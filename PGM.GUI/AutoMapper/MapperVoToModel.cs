using AutoMapper;
using PGM.GUI.ViewModel;
using PGM.Service.Utilities;

namespace PGM.GUI.AutoMapper
{
    public class MapperVoToModel : IMapperVoToModel
    {
        private readonly IMapper _mapper;

        public MapperVoToModel()
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PGMSettings, PGMSettingsVO>();
                cfg.CreateMap<PGMSettingsVO, PGMSettings>()
                    .AfterMap((vo, settings) =>
                    {
                        settings.Email = vo.Email;
                        settings.FullName = vo.FullName;
                        settings.GitApiKey = vo.GitApiKey;
                        settings.ProjectId = vo.ProjectId;
                        settings.RepositoryPath = vo.RepositoryPath;
                    });
            });
            _mapper = config.CreateMapper();
        }

        public PGMSettingsVO GetPgmSettingsVo(PGMSettings pgmSettings)
        {
            return _mapper.Map<PGMSettingsVO>(pgmSettings);
        }
    }

    public interface IMapperVoToModel
    {
        PGMSettingsVO GetPgmSettingsVo(PGMSettings pgmSettings);
    }
}

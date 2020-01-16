using AutoMapper;
using PGM.GUI.ViewModel;
using PGM.Lib.Utilities;

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

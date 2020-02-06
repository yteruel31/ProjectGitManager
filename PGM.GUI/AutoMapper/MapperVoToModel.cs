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
                cfg.CreateMap<PGMSetting, PGMSettingVO>()
                    .ForMember(vo => vo.Projects, opt => opt.Ignore());
                cfg.CreateMap<PGMSettingVO, PGMSetting>();
            });
            Mapper = config.CreateMapper();
        }
    }

    public interface IMapperVoToModel
    {
        IMapper Mapper { get; set; }
    }
}

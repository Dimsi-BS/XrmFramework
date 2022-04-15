using AutoMapper;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Configuration
{
    public class AutoMapperLocalToLocalProfile : Profile
    {
        public AutoMapperLocalToLocalProfile()
        {
            CreateMap<PluginAssembly, PluginAssembly>();
            CreateMap<Plugin, Plugin>()
                .ForMember(dest => dest.Children, opt => opt.Ignore());
            CreateMap<Step, Step>();
            CreateMap<StepImage, StepImage>();
            CreateMap<CustomApi, CustomApi>()
                .ForMember(dest => dest.Children, opt => opt.Ignore());

            CreateMap<CustomApiRequestParameter, CustomApiRequestParameter>();
            CreateMap<CustomApiResponseProperty, CustomApiResponseProperty>();
            //CreateMap<AssemblyContext, AssemblyContext>();

            ShouldMapField = fi => true;
            CreateMap<StepCollection, StepCollection>();

            //CreateMap<ICustomApiComponent, ICrmComponent>();

            //CreateMap<ICrmComponent, ICrmComponent>();
            CreateMap<IAssemblyContext, IAssemblyContext>()
                .ConstructUsing(_ => new AssemblyContext())
                .ForMember(dest => dest.Assembly, opt => opt.MapFrom(src => src.Assembly));

        }
    }

    public class AutoMapperRemoteToLocalProfile : Profile
    {
        public AutoMapperRemoteToLocalProfile()
        {
            CreateMap<Deploy.PluginAssembly, PluginAssembly>();
            CreateMap<Deploy.CustomApi, CustomApi>()
                .ForMember(p => p.ParentId, opt => opt.MapFrom(d => d.PluginTypeId.Id));
            CreateMap<Deploy.CustomApiRequestParameter, CustomApiRequestParameter>()
                .ForMember(p => p.ParentId, opt => opt.MapFrom(d => d.CustomApiId.Id));

            CreateMap<Deploy.CustomApiResponseProperty, CustomApiResponseProperty>()
                .ForMember(p => p.ParentId, opt => opt.MapFrom(d => d.CustomApiId.Id));

        }
    }

    public class AutoMapperLocalToRemoteProfile : Profile
    {
        public AutoMapperLocalToRemoteProfile()
        {
        }
    }
}

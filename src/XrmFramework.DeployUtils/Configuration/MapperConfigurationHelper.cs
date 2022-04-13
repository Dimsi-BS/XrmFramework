using AutoMapper;
using Deploy;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Configuration
{
    internal class MapperConfigurationHelper
    {
        public static void ConfigureMapperExpression(IMapperConfigurationExpression expression)
        {
            expression.CreateMap<PluginAssembly, PluginAssembly>();
            expression.CreateMap<Plugin, Plugin>()
                .ForMember(p => p.Children, opt => opt.Ignore());
            expression.CreateMap<Step, Step>()
                .ForMember(p => p.Children, opt => opt.Ignore());
            expression.CreateMap<StepCollection, StepCollection>();
            expression.CreateMap<StepImage, StepImage>();
            expression.CreateMap<CustomApi, CustomApi>()
                .ForMember(p => p.ParentId, opt => opt.Ignore())
                .ForMember(p => p.Children, opt => opt.Ignore());
            expression.CreateMap<CustomApiRequestParameter, CustomApiRequestParameter>()
                .ForMember(p => p.ParentId, opt => opt.Ignore());
            expression.CreateMap<CustomApiResponseProperty, CustomApiResponseProperty>()
                .ForMember(p => p.ParentId, opt => opt.Ignore());
            expression.CreateMap<ICrmComponent, ICrmComponent>();
            expression.CreateMap<AssemblyContext, AssemblyContext>()
                .ForMember(p => p.ComponentsOrderedPool, opt => opt.Ignore());
            expression.CreateMap<IAssemblyContext, AssemblyContext>()
                .ForMember(p => p.ComponentsOrderedPool, opt => opt.Ignore());
        }
    }
}

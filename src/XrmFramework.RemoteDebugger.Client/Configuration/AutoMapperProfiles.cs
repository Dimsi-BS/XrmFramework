using AutoMapper;
using System;
using System.Linq;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using XrmFramework.RemoteDebugger.Model.CrmComponentInfos;

namespace XrmFramework.DeployUtils.Configuration
{
    /// <summary>
    /// Profile to indicate to AutoMapper how to map objects from the <see cref="Model"/> namespace
    /// to the <see cref="RemoteDebugger.Model.CrmComponentInfos"/> namespace
    /// </summary>
    public class AutoMapperAssemblyToInfo : Profile
    {
        public AutoMapperAssemblyToInfo()
        {
            CreateMap<IAssemblyContext, AssemblyContextInfo>()
                .ForMember(dest => dest.AssemblyName,
                    opt => opt.MapFrom(src => src.UniqueName))
                .ForMember(dest => dest.Culture,
                    opt => opt.MapFrom(src => src.AssemblyInfo.Culture))
                .ForMember(dest => dest.Version,
                    opt => opt.MapFrom(src => src.AssemblyInfo.Version))
                .ForMember(dest => dest.PublicKeyToken,
                    opt => opt.MapFrom(src => src.AssemblyInfo.PublicKeyToken));

            CreateMap<Plugin, PluginInfo>()
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom(src => src.FullName));

            CreateMap<Plugin, WorkflowsInfo>()
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom(src => src.FullName));


            CreateMap<Step, StepInfo>()
                .ForMember(dest => dest.PreImage,
                    opt => opt.Condition(src => src.PreImage.IsUsed))
                .ForMember(dest => dest.PostImage,
                    opt => opt.Condition(src => src.PostImage.IsUsed))
                .ForMember(dest => dest.Message,
                    opt => opt.MapFrom(src => src.Message.ToString()))
                .ForMember(dest => dest.Stage,
                    opt => opt.MapFrom(src => src.Stage.ToString()))
                .ForMember(dest => dest.Mode,
                    opt => opt.MapFrom(src => src.Mode.ToString()));

            CreateMap<StepImage, StepImageInfo>();

            CreateMap<CustomApi, CustomApiInfo>()
                .ForMember(dest => dest.UniqueName,
                    opt => opt.MapFrom(src => src.UniqueName))
                .ForMember(dest => dest.RequestParameters,
                    opt => opt.MapFrom(src =>
                        src.Children
                            .Where(c => c is CustomApiRequestParameter)
                            .ToList()))
                .ForMember(dest => dest.ResponseProperties,
                    opt => opt.MapFrom(src =>
                        src.Children
                            .Where(c => c is CustomApiResponseProperty)
                            .ToList()));


            CreateMap<CustomApiRequestParameter, CustomApiRequestParameterInfo>()
                .ForMember(dest => dest.Type,
                    opt => opt.MapFrom(src => Enum.ToObject(typeof(Deploy.CustomApiFieldType), src.Type.Value).ToString()));
            CreateMap<CustomApiResponseProperty, CustomApiResponsePropertyInfo>()
                .ForMember(dest => dest.Type,
                    opt => opt.MapFrom(src => Enum.ToObject(typeof(Deploy.CustomApiFieldType), src.Type.Value).ToString()));

        }
    }
}


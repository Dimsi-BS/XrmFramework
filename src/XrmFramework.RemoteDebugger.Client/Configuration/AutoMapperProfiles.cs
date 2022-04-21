using AutoMapper;
using Deploy;
using System;
using System.Linq;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using XrmFramework.RemoteDebugger.Model.CrmComponentInfos;
using CustomApi = XrmFramework.DeployUtils.Model.CustomApi;
using CustomApiRequestParameter = XrmFramework.DeployUtils.Model.CustomApiRequestParameter;
using CustomApiResponseProperty = XrmFramework.DeployUtils.Model.CustomApiResponseProperty;

namespace XrmFramework.DeployUtils.Configuration
{
    public class AutoMapperAssemblyToInfo : Profile
    {
        public AutoMapperAssemblyToInfo()
        {
            CreateMap<IAssemblyContext, AssemblyContextInfo>()
                .ForMember(dest => dest.AssemblyName,
                    opt => opt.MapFrom(src => src.UniqueName));

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


            ;



            CreateMap<StepImage, StepImageInfo>();

            CreateMap<CustomApi, CustomApiInfo>()
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom(src => src.Name))
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
                    opt => opt.MapFrom(src => Enum.ToObject(typeof(CustomApiFieldType), src.Type.Value).ToString()));
            CreateMap<CustomApiResponseProperty, CustomApiResponsePropertyInfo>()
                .ForMember(dest => dest.Type,
                    opt => opt.MapFrom(src => Enum.ToObject(typeof(CustomApiFieldType), src.Type.Value).ToString()));

        }
    }
}


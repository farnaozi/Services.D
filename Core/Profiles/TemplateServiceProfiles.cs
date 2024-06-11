using AutoMapper;
using Services.D.Core.Dtos;
using Services.D.Core.Models;

namespace Services.D.Core.Profiles
{
    public class TemplateServiceProfiles : Profile
    {
        public TemplateServiceProfiles()
        {
            // source --> target
            //CreateMap<ServiceTemplateShortInfo, ServiceTemplateRead>();
            //CreateMap<ServiceTemplateFullInfo, ServiceTemplateByIdRead>();
            //CreateMap<ServiceTemplateCreate, ServiceTemplateFullInfo>();
        }
    }
}
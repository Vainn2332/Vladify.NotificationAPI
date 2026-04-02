using AutoMapper;
using Vladify.BuisnessLogic.Models;
using Vladify.DataAccess.Entities;

namespace Vladify.BuisnessLogic.MapperProfiles;

public class NotificationProfile : Profile
{
    public NotificationProfile()
    {
        CreateMap<NotificationModel, NotificationInfo>().ReverseMap();

        CreateMap<NotificationSubscriptionModel, NotificationSubscription>().ReverseMap();

        CreateMap<NotificationRequestModel, NotificationInfo>()
           .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}

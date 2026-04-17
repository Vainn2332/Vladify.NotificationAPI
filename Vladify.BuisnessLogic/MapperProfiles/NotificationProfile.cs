using AutoMapper;
using Vladify.BuisnessLogic.Models;
using Vladify.DataAccess.Entities;

namespace Vladify.BuisnessLogic.MapperProfiles;

public class NotificationProfile : Profile
{
    public NotificationProfile()
    {
        CreateMap<UserNotificationSettingsModel, UserNotificationSettings>().ReverseMap();

        CreateMap<NotificationSubscriptionModel, NotificationSubscription>().ReverseMap();

        CreateMap<UserNotificationSettingsRequestModel, UserNotificationSettings>()
           .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}

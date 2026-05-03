using AutoMapper;
using Vladify.BusinessLogic.Messages;
using Vladify.BusinessLogic.Models;
using Vladify.DataAccess.Entities;

namespace Vladify.BusinessLogic.MapperProfiles;

public class NotificationProfile : Profile
{
    public NotificationProfile()
    {
        CreateMap<UserNotificationSettingsModel, UserNotificationSettings>().ReverseMap();

        CreateMap<NotificationSubscriptionModel, NotificationSubscription>().ReverseMap();

        CreateMap<UserNotificationSettingsRequestModel, UserNotificationSettings>()
           .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<UserCreatedMessage, UserNotificationSettingsRequestModel>()
            .ForMember(dest => dest.NotificationSubscription, opt => opt.MapFrom(src => new NotificationSubscriptionModel()));

    }
}

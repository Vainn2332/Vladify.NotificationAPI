using MassTransit;
using Vladify.BuisnessLogic.Models.Messages;

namespace Vladify.BuisnessLogic;

public class TESTProducer(ITopicProducer<SongMessage> _producer)
{
    public Task CreateMessageAsync()
    {
        return _producer.Produce(new SongMessage() { Album = "bla", Author = "testAuthor", Title = "testTitle" });
    }
}

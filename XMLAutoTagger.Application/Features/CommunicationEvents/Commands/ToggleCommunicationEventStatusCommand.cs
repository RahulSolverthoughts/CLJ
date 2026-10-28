using MediatR;

namespace XMLAutoTagger.Application.Features.CommunicationEvents.Commands
{
    public class ToggleCommunicationEventStatusCommand : IRequest<bool>
    {
        public int EventId { get; set; }

        public ToggleCommunicationEventStatusCommand(int eventId)
        {
            EventId = eventId;
        }
    }
}

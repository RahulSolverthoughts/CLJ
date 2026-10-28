using System.Collections.Generic;
using MediatR;
using XMLAutoTagger.Application.Features.CommunicationEvents.DTOs;

namespace XMLAutoTagger.Application.Features.CommunicationEvents.Queries
{
    public class GetCommunicationEventsQuery : IRequest<List<CommunicationEventDto>>
    {
    }
}

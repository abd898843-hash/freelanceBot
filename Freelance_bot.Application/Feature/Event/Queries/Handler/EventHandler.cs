


using AutoMapper;
using Freelance_bot.Application.Feature.Event.Queries.Models;
using Freelance_bot.Application.Feature.Event.Responses;
using Freelance_bot.Application.Feature.Projects.DTOs;
using Freelance_bot.Application.Feature.Projects.Queries.Models;
using Freelance_bot.Application.Feature.Projects.Responses;
using Freelance_Bot.Domain.IRepository;
using MediatR;

namespace Freelance_bot.Application.Feature.Event.Queries.Handler
{
    public class EventHandler
        : IRequestHandler<GetEventQueries, IEnumerable<UnprocessedEventsResponse>>
    {
        #region Fields

        private readonly IEventRepository _eventRepo;
        private readonly IMapper _mapper;

        #endregion

        #region Ctor

        public EventHandler(
            IEventRepository eventRepo,
            IMapper mapper)
        {
            _eventRepo = eventRepo;
            _mapper = mapper;
        }

        #endregion

        #region Handle

        public async Task<IEnumerable<UnprocessedEventsResponse>> Handle(
            GetEventQueries request,
            CancellationToken cancellationToken)
        {
            var eventList = await _eventRepo.GetAllAsync();

            var eventDtoList =
                _mapper.Map<IEnumerable<UnprocessedEventsResponse>>(eventList);

            return eventDtoList;
        }

        #endregion
    }
}
//using AutoMapper;
//using Freelance_bot.Application.Feature.Event.Responses;
//using Freelance_bot.Application.Feature.Projects.Queries.Models;
//using Freelance_bot.Application.Feature.Reports.Queries.Models;
//using Freelance_bot.Application.Feature.Reports.Response;
//using Freelance_Bot.Domain.IRepository;
//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Freelance_bot.Application.Feature.Reports.Queries.Handler
//{
//    public class ReportHandler
//       : IRequestHandler<GetReportQueries, IEnumerable<ReportResponse>>
//    {
//        #region Fields

//        //private readonly IReportRepository _reportRepo;
//        private readonly IMapper _mapper;

//        #endregion

//        #region Ctor

//        public ReportHandler(
//            IReportRepository reportRepo,
//            IMapper mapper)
//        {
//            _reportRepo = reportRepo;
//            _mapper = mapper;
//        }

//        #endregion

//        #region Handle

//        public async Task<IEnumerable<ReportResponse>> Handle(
//            GetReportQueries request,
//            CancellationToken cancellationToken)
//        {
//            var reportList = await _reportRepo.GetAllAsync();

//            var reportDtoList =
//                _mapper.Map<IEnumerable<ReportResponse>>(reportList);

//            return reportDtoList;
//        }

//        #endregion
//    }
//}

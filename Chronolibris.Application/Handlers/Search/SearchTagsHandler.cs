//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Chronolibris.Application.Requests.Search;
//using Chronolibris.Domain.Interfaces;
//using Chronolibris.Domain.Models;
//using MediatR;

//namespace Chronolibris.Application.Handlers.Search
//{
//    public class SearchTagsHandler : IRequestHandler<SearchTagsQuery, List<TagDetails>>
//    {
//        private readonly IContentRepository _repository;

//        public SearchTagsHandler(IContentRepository repository)
//        {
//            _repository = repository;
//        }

//        public async Task<List<TagDetails>> Handle(SearchTagsQuery request, CancellationToken ct)
//        {
//            return await _repository.SearchTagsAsync(
//                request.SearchTerm,
//                request.TagTypeId,
//                request.Limit,
//                ct
//            );
//        }
//    }
//}

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Chronolibris.Application.Requests.Books;
//using Chronolibris.Domain.Interfaces;
//using Chronolibris.Domain.Models;
//using MediatR;

//namespace Chronolibris.Application.Handlers.Books
//{
//    public class GetReadingProgressHandler(IUnitOfWork uow):IRequestHandler<GetReadingProgressQuery, ReadingProgressDto?>
//    {
//        public async Task<ReadingProgressDto?> Handle(GetReadingProgressQuery request, CancellationToken ct)
//        {
//            var entity = await uow.ReadingProgresses.GetForBookUser(request.BookFileId, request.UserId, ct);
//            //Что такое вар узнать и как правильно это называется 
//            if (entity is null) return null;

//            return new ReadingProgressDto(
//                entity.BookFileId, entity.Percentage, entity.ParaIndex, entity.ReadingDate);
//        }
//    }
//}

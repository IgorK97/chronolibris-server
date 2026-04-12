//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Chronolibris.Application.Requests.Books;
//using Chronolibris.Domain.Entities;
//using Chronolibris.Domain.Interfaces;
//using Chronolibris.Domain.Models;
//using MediatR;

//namespace Chronolibris.Application.Handlers.Books
//{
//    public class UpdatereadingProgressHandler(IUnitOfWork uow) : IRequestHandler<UpdateReadingProgressCommand, ReadingProgressDto>
//    {
//        public async Task<ReadingProgressDto> Handle(UpdateReadingProgressCommand command, CancellationToken token)
//        {
//            var readingProgress = await uow.ReadingProgresses.GetForBookUser(command.BookFileId, command.UserId);
//            ReadingProgress entity;

//            if(readingProgress is null)
//            {
//                entity = new ReadingProgress
//                {
//                    Id = 0,
//                    BookFileId = command.BookFileId,
//                    UserId = command.UserId,
//                    Percentage = command.Percentage,
//                    ReadingDate = DateTime.UtcNow,
//                    ParaIndex = command.ParaIndex,
//                };
//                await uow.ReadingProgresses.AddAsync(entity);

//            }
//            else
//            {
//                if(command.Percentage > readingProgress.Percentage)
//                {
//                    readingProgress.Percentage = command.Percentage;
//                    readingProgress.ParaIndex = command.ParaIndex;
//                    readingProgress.ReadingDate = DateTime.UtcNow;
//                    uow.ReadingProgresses.Update(readingProgress);
//                }
//                entity = readingProgress;
//            }

//            await uow.SaveChangesAsync(token);

//            return new ReadingProgressDto(entity.BookFileId, entity.Percentage,
//                entity.ParaIndex, entity.ReadingDate);

//            //if (readingProgress.Percentage < command.ReadingProgress)
//            //{
//            //    readingProgress.Percentage = command.ReadingProgress;

//            //}
//            //readingProgress.ReadingDate = DateTime.UtcNow;
//            //uow.ReadingProgresses.Update(readingProgress);
//            //var res = await uow.SaveChangesAsync();
//            //if (res > 0)
//            //    return true;
//            //return false;
//        }
//    }
//}

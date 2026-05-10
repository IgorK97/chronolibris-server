using MediatR;
using Chronolibris.Application.Models;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Exceptions;

namespace Chronolibris.Application.Handlers.Contents
{
    public class UpdateContentHandler : IRequestHandler<UpdateContentRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateContentHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateContentRequest request, CancellationToken cancellationToken)
        {
            var content = await _unitOfWork.Contents.GetByIdAsync(request.Id, cancellationToken);
            if (content == null) 
                throw new ChronolibrisException("Такого контента нет", ErrorType.NotFound);

            if(request.Title!=null)
                content.Title = request.Title;
            
            if(request.Description!=null)
                content.Description = request.Description;

            if(request.CountryId!=null)
                content.CountryId = (long)request.CountryId;
            
            if(request.ContentTypeId!=null)
                content.ContentTypeId = (long) request.ContentTypeId;
            
            if(request.LanguageId!=null)
                content.LanguageId = (long)request.LanguageId;
            
            if(request.YearFromProvided)
                content.YearFrom = request.YearFrom;

            if(request.YearToProvided)
                content.YearTo = request.YearTo;

            if (request.ThemeIds != null)
                _unitOfWork.Contents.SyncThemes(content, request.ThemeIds);

            if (request.PersonFilters != null)
                _unitOfWork.Contents.SyncParticipations(content, request.PersonFilters);
            //if(request.TagIds!=null)
            //    await _contentRepository.SyncTagsAsync(content.Id, request.TagIds, cancellationToken);

            //_contentRepository.Update(content);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
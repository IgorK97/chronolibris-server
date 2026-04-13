using MediatR;
using Chronolibris.Application.Models;
using Chronolibris.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chronolibris.Domain.Models;
using Chronolibris.Application.Requests.Contents;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Exceptions;

namespace Chronolibris.Application.Handlers.Contents
{
    public class GetContentsHandler : IRequestHandler<GetContentsQuery, PagedResult<ContentDto>>
    {
        private readonly IContentRepository _contentRepository;

        public GetContentsHandler(IContentRepository contentRepository)
        {
            _contentRepository = contentRepository;
        }

        public async Task<PagedResult<ContentDto>> Handle(GetContentsQuery request, CancellationToken cancellationToken)
        {
            return await _contentRepository.GetWithFilterAsync(
                request.Filter, cancellationToken);

        }
    }

    public class GetContentByIdHandler : IRequestHandler<GetContentByIdQuery, ContentDto?>
    {
        private readonly IContentRepository _contentRepository;

        public GetContentByIdHandler(IContentRepository contentRepository)
        {
            _contentRepository = contentRepository;
        }

        public async Task<ContentDto?> Handle(GetContentByIdQuery request, CancellationToken cancellationToken)
        {
            var contentDto = await _contentRepository.GetDtoByIdAsync(request.Id, cancellationToken);

            return contentDto;
        }
    }

    public class GetContentBooksHandler : IRequestHandler<GetContentBooksQuery, List<BookDto>>
    {
        private readonly IContentRepository _contentRepository;

        public GetContentBooksHandler(
            IContentRepository contentRepository)
        {
            _contentRepository = contentRepository;
        }

        public async Task<List<BookDto>> Handle(GetContentBooksQuery request, CancellationToken cancellationToken)
        {
            return await _contentRepository.GetBooksDtoByContentIdAsync(request.ContentId, cancellationToken);
        }
    }

    public class CreateContentHandler : IRequestHandler<CreateContentCommand, long>
    {
        private readonly IContentRepository _contentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateContentHandler(IContentRepository contentRepository, IUnitOfWork unitOfWork)
        {
            _contentRepository = contentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> Handle(CreateContentCommand request, CancellationToken cancellationToken)
        {
            var content = new Content
            {
                Id=0,
                Title = request.Title,
                Description = request.Description,
                CountryId = request.CountryId,
                ContentTypeId = request.ContentTypeId,
                LanguageId = request.LanguageId,
                Year = request.Year,
                CreatedAt = DateTime.UtcNow,
                Participations = new List<ContentParticipation>(),
                Themes = new List<Theme>()
            };

            //if (request.PersonFilters != null)
            //{
            //    foreach (var filter in request.PersonFilters)
            //    {
            //        foreach (var personId in filter.PersonIds)
            //        {
            //            content.Participations.Add(new ContentParticipation
            //            {
            //                PersonId = personId,
            //                PersonRoleId = filter.RoleId
            //            });
            //        }
            //    }
            //}

            if (request.PersonFilters != null)
                _contentRepository.SyncParticipations(content, request.PersonFilters);

            if (request.ThemeIds != null)
            {
                _contentRepository.SyncThemes(content, request.ThemeIds);
            }


            await _contentRepository.AddAsync(content, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return content.Id;
        }
    }

    public class UpdateContentHandler : IRequestHandler<UpdateContentRequest, Unit>
    {
        private readonly IContentRepository _contentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateContentHandler(IContentRepository contentRepository, IUnitOfWork unitOfWork)
        {
            _contentRepository = contentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateContentRequest request, CancellationToken cancellationToken)
        {
            var content = await _contentRepository.GetByIdAsync(request.Id, cancellationToken);
            if (content == null) 
                throw new ChronolibrisException($"Такого контента нет", ErrorType.NotFound);

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
            
            if(request.YearProvided)
                content.Year = request.Year;

            if (request.ThemeIds != null)
                _contentRepository.SyncThemes(content, request.ThemeIds);

            if (request.PersonFilters != null)
                _contentRepository.SyncParticipations(content, request.PersonFilters);
            //if(request.TagIds!=null)
            //    await _contentRepository.SyncTagsAsync(content.Id, request.TagIds, cancellationToken);

            //_contentRepository.Update(content);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }

    public class DeleteContentHandler : IRequestHandler<DeleteContentCommand, Unit>
    {
        private readonly IContentRepository _contentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteContentHandler(IContentRepository contentRepository, IUnitOfWork unitOfWork)
        {
            _contentRepository = contentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteContentCommand request, CancellationToken cancellationToken)
        {
            var content = await _contentRepository.GetByIdAsync(request.Id, cancellationToken);
            if (content == null)
                return Unit.Value;

            _contentRepository.Delete(content);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }

    public class LinkBookToContentHandler : IRequestHandler<LinkBookToContentCommand, Unit>
    {
        private readonly IContentRepository _contentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LinkBookToContentHandler(IContentRepository contentRepository, IUnitOfWork unitOfWork)
        {
            _contentRepository = contentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(LinkBookToContentCommand request, CancellationToken cancellationToken)
        {
            await _contentRepository.LinkContentToBookAsync(
                request.ContentId, request.BookId, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }

    public class UnlinkBookFromContentHandler : IRequestHandler<UnlinkBookFromContentCommand, Unit>
    {
        private readonly IContentRepository _contentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UnlinkBookFromContentHandler(IContentRepository contentRepository, IUnitOfWork unitOfWork)
        {
            _contentRepository = contentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UnlinkBookFromContentCommand request, CancellationToken cancellationToken)
        {
            await _contentRepository.UnlinkContentFromBookAsync(
                request.ContentId, request.BookId, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
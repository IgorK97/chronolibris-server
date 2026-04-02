// File: Chronolibris.Application.Handlers.ContentHandlers.cs
using MediatR;
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chronolibris.Domain.Models;

namespace Chronolibris.Application.Handlers
{
    public class GetContentsHandler : IRequestHandler<GetContentsQuery, ContentListResponse>
    {
        private readonly IContentRepository _contentRepository;

        public GetContentsHandler(IContentRepository contentRepository)
        {
            _contentRepository = contentRepository;
        }

        public async Task<ContentListResponse> Handle(GetContentsQuery request, CancellationToken cancellationToken)
        {
            var (items, totalCount, nextCursor, prevCursor) = await _contentRepository.GetWithFilterAsync(
                request.Filter, cancellationToken);

            var contentDtos = new List<ContentDto>();
            foreach (var content in items)
            {
                var authors = await _contentRepository.GetAuthorNamesByContentIdAsync(content.Id, cancellationToken);
                var themes = await _contentRepository.GetThemesByContentIdAsync(content.Id, cancellationToken);
                var booksCount = await _contentRepository.GetBooksCountAsync(content.Id, cancellationToken);

                contentDtos.Add(new ContentDto
                {
                    Id = content.Id,
                    Title = content.Title,
                    Description = content.Description,
                    CountryId = content.CountryId,
                    CountryName = content.Country?.Name,
                    ContentTypeId = content.ContentTypeId,
                    ContentType = content.ContentType?.Name,
                    LanguageId = content.LanguageId,
                    LanguageName = content.Language?.Name,
                    Year = content.Year,
                    //ParentContentId = content.ParentContentId,
                    //Position = content.Position,
                    CreatedAt = content.CreatedAt,
                    //UpdatedAt = content.UpdatedAt,
                    Authors = authors,
                    Themes = themes.Select(t => new ThemeDto
                    {
                        Id = t.Id,
                        Name = t.Name
                    }).ToList(),
                    BooksCount = booksCount
                });
            }

            return new ContentListResponse
            {
                Items = contentDtos,
                NextCursor = nextCursor,
                PrevCursor = prevCursor,
                TotalCount = totalCount,
                HasMore = nextCursor != null
            };
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
            var content = await _contentRepository.GetByIdAsync(request.Id, cancellationToken);
            if (content == null) return null;

            var authors = await _contentRepository.GetAuthorNamesByContentIdAsync(content.Id, cancellationToken);
            var themes = await _contentRepository.GetThemesByContentIdAsync(content.Id, cancellationToken);
            var booksCount = await _contentRepository.GetBooksCountAsync(content.Id, cancellationToken);

            var participants = content.Participations.GroupBy(p => p.PersonRoleId)
                .Select(g => new PersonRoleFilter
                {
                    RoleId = g.Key,
                    PersonIds = g.Select(p => p.PersonId).ToList()
                })
                .ToList();

            return new ContentDto
            {
                Id = content.Id,
                Title = content.Title,
                Description = content.Description,
                CountryId = content.CountryId,
                CountryName = content.Country?.Name,
                ContentTypeId = content.ContentTypeId,
                ContentType = content.ContentType?.Name,
                LanguageId = content.LanguageId,
                LanguageName = content.Language?.Name,
                Year = content.Year,
                //ParentContentId = content.ParentContentId,
                //Position = content.Position,
                CreatedAt = content.CreatedAt,
                //UpdatedAt = content.UpdatedAt,
                Authors = authors,
                Themes = themes.Select(t => new ThemeDto
                {
                    Id = t.Id,
                    Name = t.Name
                }).ToList(),
                BooksCount = booksCount,
                Participants = participants
            };
        }
    }

    // File: Chronolibris.Application.Handlers.ContentHandlers.cs
    public class GetContentBooksHandler : IRequestHandler<GetContentBooksQuery, List<BookDto>>
    {
        private readonly IContentRepository _contentRepository;
        private readonly IBookRepository _bookRepository;

        public GetContentBooksHandler(
            IContentRepository contentRepository,
            IBookRepository bookRepository)
        {
            _contentRepository = contentRepository;
            _bookRepository = bookRepository;
        }

        public async Task<List<BookDto>> Handle(GetContentBooksQuery request, CancellationToken cancellationToken)
        {
            var books = await _contentRepository.GetBooksByContentIdAsync(request.ContentId, cancellationToken);

            var bookDtos = new List<BookDto>();
            foreach (var book in books)
            {
                // Авторы книги берутся через BookParticipation → Person
                var authors = await _bookRepository.GetAuthorNamesByBookIdAsync(book.Id, cancellationToken);

                // Темы книги берутся через BookContent → Content → Themes
                // Поскольку книга связана с этим контентом, темы будут включать темы этого контента
                var themes = await _bookRepository.GetThemesByBookIdAsync(book.Id, cancellationToken);

                bookDtos.Add(new BookDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    Description = book.Description,
                    CountryId = book.CountryId,
                    CountryName = book.Country?.Name,
                    LanguageId = book.LanguageId,
                    LanguageName = book.Language?.Name,
                    Year = book.Year,
                    ISBN = book.ISBN,
                    CoverPath = book.CoverPath,
                    IsAvailable = book.IsAvailable,
                    IsReviewable = book.IsReviewable,
                    PublisherId = book.PublisherId,
                    PublisherName = book.Publisher?.Name,
                    //SeriesId = book.SeriesId,
                    //SeriesName = book.Series?.Name,
                    CreatedAt = book.CreatedAt,
                    UpdatedAt = book.UpdatedAt,
                    Authors = authors,
                    Themes = themes.Select(t => new ThemeDto
                    {
                        Id = t.Id,
                        Name = t.Name
                    }).ToList()
                });
            }

            return bookDtos;
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
                //ParentContentId = request.ParentContentId,
                //Position = request.Position,
                CreatedAt = DateTime.UtcNow,
                //UpdatedAt = null
            };

            await _contentRepository.AddAsync(content, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Здесь можно добавить связь с персонами и темами через отдельные репозитории
            // Для краткости опущено

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
            if (content == null) throw new KeyNotFoundException($"Content with ID {request.Id} not found");

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

            if(request.PersonFilters!=null)
                await _contentRepository.SyncPersonsAsync(content.Id, request.PersonFilters, cancellationToken);
            if(request.ThemeIds!=null)
                await _contentRepository.SyncThemesAsync(content.Id, request.ThemeIds, cancellationToken);
            if(request.TagIds!=null)
                await _contentRepository.SyncTagsAsync(content.Id, request.TagIds, cancellationToken);

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
            if (content == null) throw new KeyNotFoundException($"Content with ID {request.Id} not found");

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
                request.ContentId, request.BookId, request.Order, cancellationToken);

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
// File: Chronolibris.Application.Handlers.BookHandlers.cs
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

namespace Chronolibris.Application.Handlers
{
    public class GetBooksHandler : IRequestHandler<GetBooksQuery, BookListResponse>
    {
        private readonly IBookRepository _bookRepository;

        public GetBooksHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<BookListResponse> Handle(GetBooksQuery request, CancellationToken cancellationToken)
        {
            var (items, totalCount, nextCursor, prevCursor) = await _bookRepository.GetWithFilterAsync(
                request.Filter, cancellationToken);

            var bookDtos = new List<BookDto>();
            foreach (var book in items)
            {
                var authors = await _bookRepository.GetAuthorNamesByBookIdAsync(book.Id, cancellationToken);
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

            return new BookListResponse
            {
                Items = bookDtos,
                NextCursor = nextCursor,
                PrevCursor = prevCursor,
                TotalCount = totalCount,
                HasMore = nextCursor != null
            };
        }
    }

    public class GetBookByIdHandler : IRequestHandler<GetBookByIdQuery, BookDto?>
    {
        private readonly IBookRepository _bookRepository;

        public GetBookByIdHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<BookDto?> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(request.Id, cancellationToken);
            if (book == null) return null;

            var authors = await _bookRepository.GetAuthorNamesByBookIdAsync(book.Id, cancellationToken);
            var themes = await _bookRepository.GetThemesByBookIdAsync(book.Id, cancellationToken);

            return new BookDto
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
            };
        }
    }

    //public class CreateBookHandler : IRequestHandler<CreateBookCommand, long>
    //{
    //    private readonly IBookRepository _bookRepository;
    //    private readonly IUnitOfWork _unitOfWork;

    //    public CreateBookHandler(IBookRepository bookRepository, IUnitOfWork unitOfWork)
    //    {
    //        _bookRepository = bookRepository;
    //        _unitOfWork = unitOfWork;
    //    }

    //    public async Task<long> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    //    {
    //        var book = new Book
    //        {
    //            Id=0,
    //            Title = request.Title,
    //            Description = request.Description,
    //            CountryId = request.CountryId,
    //            LanguageId = request.LanguageId,
    //            Year = request.Year,
    //            ISBN = request.ISBN,
    //            //FilePath = request.FilePath ?? string.Empty,
    //            CoverPath = request.CoverPath ?? string.Empty,
    //            IsAvailable = request.IsAvailable,
    //            IsReviewable = request.IsReviewable,
    //            PublisherId = request.PublisherId,
    //            //SeriesId = request.SeriesId,
    //            CreatedAt = DateTime.UtcNow,
    //            UpdatedAt = null,
    //            //IsFragment=false,
    //        };

    //        await _bookRepository.AddAsync(book, cancellationToken);
    //        await _unitOfWork.SaveChangesAsync(cancellationToken);

    //        return book.Id;
    //    }
    //}

    //public class UpdateBookHandler : IRequestHandler<UpdateBookCommand, Unit>
    //{
    //    private readonly IBookRepository _bookRepository;
    //    private readonly IUnitOfWork _unitOfWork;

    //    public UpdateBookHandler(IBookRepository bookRepository, IUnitOfWork unitOfWork)
    //    {
    //        _bookRepository = bookRepository;
    //        _unitOfWork = unitOfWork;
    //    }

    //    public async Task<Unit> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    //    {
    //        var book = await _bookRepository.GetByIdAsync(request.Id, cancellationToken);
    //        if (book == null) throw new KeyNotFoundException($"Book with ID {request.Id} not found");

    //        book.Title = request.Title;
    //        book.Description = request.Description;
    //        book.CountryId = request.CountryId;
    //        book.LanguageId = request.LanguageId;
    //        book.Year = request.Year;
    //        book.ISBN = request.ISBN;
    //        //book.FilePath = request.FilePath ?? book.FilePath;
    //        book.CoverPath = request.CoverPath ?? book.CoverPath;
    //        book.IsAvailable = request.IsAvailable;
    //        book.IsReviewable = request.IsReviewable;
    //        book.PublisherId = request.PublisherId;
    //        //book.SeriesId = request.SeriesId;
    //        book.UpdatedAt = DateTime.UtcNow;

    //        _bookRepository.Update(book);
    //        await _unitOfWork.SaveChangesAsync(cancellationToken);

    //        return Unit.Value;
    //    }
    //}

    public class DeleteBookHandler : IRequestHandler<DeleteBookCommand, Unit>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteBookHandler(IBookRepository bookRepository, IUnitOfWork unitOfWork)
        {
            _bookRepository = bookRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(request.Id, cancellationToken);
            if (book == null) throw new KeyNotFoundException($"Book with ID {request.Id} not found");

            _bookRepository.Delete(book);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }

    public class GetBookContentsHandler : IRequestHandler<GetBookContentsQuery, List<ContentDto>>
    {
        private readonly IBookRepository _bookRepository;

        public GetBookContentsHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<List<ContentDto>> Handle(GetBookContentsQuery request, CancellationToken cancellationToken)
        {
            var contents = await _bookRepository.GetContentsByBookIdAsync(request.BookId, cancellationToken);

            var contentDtos = new List<ContentDto>();
            foreach (var content in contents)
            {
                //var authors = await _bookRepository.GetAuthorNamesByBookIdAsync(request.BookId, cancellationToken);
                var authors = content.Participations.Select(a => a.Person.Name).ToList();
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
                    Themes = content.Themes.Select(t => new ThemeDto
                    {
                        Id = t.Id,
                        Name = t.Name
                    }).ToList()
                });
            }

            return contentDtos;
        }
    }

    public class LinkContentToBookHandler : IRequestHandler<LinkContentToBookCommand, Unit>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LinkContentToBookHandler(IBookRepository bookRepository, IUnitOfWork unitOfWork)
        {
            _bookRepository = bookRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(LinkContentToBookCommand request, CancellationToken cancellationToken)
        {
            await _bookRepository.LinkContentToBookAsync(
                request.BookId, request.ContentId, request.Order, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }

    public class UnlinkContentFromBookHandler : IRequestHandler<UnlinkContentFromBookCommand, Unit>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UnlinkContentFromBookHandler(IBookRepository bookRepository, IUnitOfWork unitOfWork)
        {
            _bookRepository = bookRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UnlinkContentFromBookCommand request, CancellationToken cancellationToken)
        {
            await _bookRepository.UnlinkContentFromBookAsync(
                request.BookId, request.ContentId, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }

}
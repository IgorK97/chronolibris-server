using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;

namespace Chronolibris.Domain.Interfaces
{
    /// <summary>
    /// Определяет контракт для Шаблона Единица Работы (Unit of Work). 
    /// Он инкапсулирует контекст базы данных и предоставляет доступ ко всем 
    /// репозиториям, а также управляет транзакциями и сохранением изменений.
    /// Наследует <see cref="System.IDisposable"/> для управления ресурсами контекста.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {

        /// <summary>
        /// Получает репозиторий для управления сущностями <see cref="Chronolibris.Domain.Interfaces.IBookRepository"/>.
        /// </summary>
        IBookRepository Books { get; }

        /// <summary>
        /// Получает репозиторий для управления сущностями <see cref="Chronolibris.Domain.Interfaces.IBookmarkRepository"/>.
        /// </summary>
        IBookmarkRepository Bookmarks { get; }

        /// <summary>
        /// Получает репозиторий для управления сущностями <see cref="Chronolibris.Domain.Interfaces.IReviewReactionsRepository"/> (оценки отзывов).
        /// </summary>
        IReviewReactionsRepository ReviewReactions { get; }
        ICommentReactionsRepository CommentReactions { get; }



        /// <summary>
        /// Получает репозиторий для управления сущностями <see cref="IReviewRepository"/> (отзывы).
        /// </summary>
        IReviewRepository Reviews { get; }

        /// <summary>
        /// Получает репозиторий для управления сущностями <see cref="Chronolibris.Domain.Interfaces.ISelectionsRepository"/> (подборки книг).
        /// </summary>
        ISelectionsRepository Selections { get; }

        /// <summary>
        /// Получает репозиторий для управления сущностями <see cref="Chronolibris.Domain.Interfaces.IShelfRepository"/> (полки пользователя).
        /// </summary>
        IShelfRepository Shelves { get; }

        ICommentRepository Comments { get; }

        /// <summary>
        /// Получает обобщенный репозиторий для управления сущностями <see cref="Person"/>.
        /// </summary>
        IGenericRepository<Person> Persons { get; }
        IGenericRepository<Country> Countries { get; }
        IGenericRepository<Format> Formats { get; }
        ILanguageRepository Languages { get; }

        /// <summary>
        /// Получает обобщенный репозиторий для управления сущностями <see cref="Content"/> (файлы/содержимое).
        /// </summary>
        IGenericRepository<Content> Contents { get; }

        /// <summary>
        /// Получает обобщенный репозиторий для управления сущностями <see cref="Publisher"/>.
        /// </summary>
        IGenericRepository<Publisher> Publishers { get; }

        IGenericRepository<PersonRole> PersonRoles { get; }
        IReadingProgressRepository ReadingProgresses { get; }
        IGenericRepository<Series> Series { get; }

        /// <summary>
        /// Асинхронно сохраняет все изменения, накопленные в контексте отслеживания 
        /// изменений во всех репозиториях, в базе данных как единую транзакцию.
        /// </summary>
        /// <param name="token">Токен отмены для прерывания операции. По умолчанию — <c>default</c>.</param>
        /// <returns>Задача, представляющая асинхронную операцию. Результат задачи — количество успешно сохраненных записей.</returns>
        Task<int> SaveChangesAsync(CancellationToken token = default);
    }
}

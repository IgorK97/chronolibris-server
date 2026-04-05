using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests;
using Chronolibris.Domain.Interfaces;
using MediatR;

namespace Chronolibris.Application.Handlers
{
    /// <summary>
    /// Обработчик запроса для получения списка активных подборок (коллекций книг).
    /// Использует первичный конструктор для внедрения зависимости <see cref="ISelectionsRepository"/>.
    /// Реализует интерфейс <see cref="IRequestHandler{TRequest, TResponse}"/>
    /// для обработки <see cref="GetSelectionsQuery"/> и возврата коллекции <see cref="SelectionDetails"/>.
    /// </summary>
    public class GetSelectionsQueryHandler(ISelectionsRepository selectionsRepository)
    : IRequestHandler<GetSelectionsQuery, PagedResult<SelectionDetails>>
    {
        // Примечание: Внедрение зависимости через первичный конструктор (Primary Constructor)
        // автоматически создает приватное поле только для чтения `selectionsRepository`.

        /// <summary>
        /// Обрабатывает запрос на получение списка активных подборок.
        /// </summary>
        /// <remarks>
        /// 1. Вызывает репозиторий для получения всех активных подборок.
        /// 2. Преобразует полученные сущности подборок в <see cref="SelectionDetails"/> DTO.
        /// </remarks>
        /// <param name="request">Объект запроса, в данном случае используется как маркер.</param>
        /// <param name="ct">Токен отмены для асинхронной операции.</param>
        /// <returns>
        /// Задача, представляющая асинхронную операцию.
        /// Результат задачи — коллекция <see cref="IEnumerable{T}"/> объектов <see cref="SelectionDetails"/>.
        /// </returns>
        public async Task<PagedResult<SelectionDetails>> Handle(GetSelectionsQuery request, CancellationToken ct)
        {
            var selections = await selectionsRepository.GetSelectionsAsync(request.LastId,
                request.Limit+1, request.OnlyActive, ct);

            var hasMore = selections.Count > request.Limit;
            var pageItems = hasMore ? selections.Take(request.Limit) : selections;
            //var nextCursor = hasMore ? (pageItems.ToList())[^1].Id : (long?)null;

            return new PagedResult<SelectionDetails>
            {
                HasNext = hasMore,
                Items = pageItems,
                LastId = request.LastId,
                Limit = request.Limit,
            };
        }
    }

}

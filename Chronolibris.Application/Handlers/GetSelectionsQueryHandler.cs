using System;
using System.Collections.Generic;
using System.Linq;
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
    : IRequestHandler<GetSelectionsQuery, IEnumerable<SelectionDetails>>
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
        public async Task<IEnumerable<SelectionDetails>> Handle(GetSelectionsQuery request, CancellationToken ct)
        {
            var selections = await selectionsRepository.GetActiveSelectionsAsync(ct);

            return selections.Select(s => new SelectionDetails
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                //SelectionTypeId = s.SelectionTypeId
                //IsActive = s.IsActive
            });
        }
    }

}

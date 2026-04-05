using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using MediatR;

namespace Chronolibris.Application.Requests
{
    /// <summary>
    /// Запрос на асинхронное получение списка всех доступных подборок (<see cref="SelectionDetails"/>).
    /// <para>
    /// Этот класс является <c>record</c> без входных параметров, 
    /// что обеспечивает неизменяемость (immutability).
    /// </para>
    /// </summary>
    /// <returns>Возвращает <see cref="System.Collections.Generic.IEnumerable{T}"/> 
    /// объектов <see cref="SelectionDetails"/>, содержащий все подборки.</returns>
    public record GetSelectionsQuery(long? LastId = null, int Limit = 20, bool? OnlyActive=true) : IRequest<PagedResult<SelectionDetails>>;

    public record GetAllSelectionsQuery() : IRequest<IEnumerable<SelectionDetails>>;

}

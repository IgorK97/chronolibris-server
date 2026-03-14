using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Fb2Converter.Interfaces;

namespace Chronolibris.Infrastructure.DataAccess.Fb2Converter
{
    /// <summary>
    /// Хранилище в памяти — используется в тестах и локальной разработке.
    /// </summary>
    public sealed class InMemoryBookStorage : IBookStorage
    {
        private readonly ConcurrentDictionary<string, string> _store = new();

        public Task SaveAsync(string bookId, string fileName, string content,
            CancellationToken cancellationToken = default)
        {
            _store[$"{bookId}/{fileName}"] = content;
            return Task.CompletedTask;
        }

        public Task<string?> ReadAsync(string bookId, string fileName,
            CancellationToken cancellationToken = default)
        {
            _store.TryGetValue($"{bookId}/{fileName}", out var value);
            return Task.FromResult(value);
        }

        public Task<bool> ExistsAsync(string bookId, string fileName,
            CancellationToken cancellationToken = default)
            => Task.FromResult(_store.ContainsKey($"{bookId}/{fileName}"));

        public IReadOnlyDictionary<string, string> GetAll() => _store;
    }

}

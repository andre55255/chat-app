using Chat.Core.Entities.NoSql;
using FluentResults;

namespace Chat.Core.RepositoriesInterface.NoSql
{
    public interface ILogRepository
    {
        public Task<Result> WriteAsync(Log log);
        public Result WriteSync(Log log);
    }
}

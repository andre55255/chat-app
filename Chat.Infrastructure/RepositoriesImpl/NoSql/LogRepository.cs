using Chat.Communication.CustomExceptions;
using Chat.Core.Entities.NoSql;
using Chat.Core.RepositoriesInterface.NoSql;
using Chat.Infrastructure.Data.NoSql;
using FluentResults;

namespace Chat.Infrastructure.RepositoriesImpl.NoSql
{
    public class LogRepository : ILogRepository
    {
        private readonly MongoDbContext _dbMongo;

        public LogRepository(MongoDbContext dbMongo)
        {
            _dbMongo = dbMongo;
        }

        public async Task<Result> WriteAsync(Log log)
        {
            try
            {
                log.Id = null;
                await _dbMongo.Logs.InsertOneAsync(log);
                return Result.Ok().WithSuccess(log.Id);
            }
            catch (DbConnectException ex)
            {
                throw new RepositoryException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        public Result WriteSync(Log log)
        {
            try
            {
                log.Id = null;
                _dbMongo.Logs.InsertOne(log);
                return Result.Ok().WithSuccess(log.Id);
            }
            catch (DbConnectException ex)
            {
                throw new RepositoryException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
    }
}

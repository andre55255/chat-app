using Chat.Core.Entities.NoSql;
using Chat.Core.RepositoriesInterface.NoSql;
using Chat.Core.ServicesInterface;

namespace Chat.Infrastructure.ServicesImpl
{
    public class LogService : ILogService
    {
        private readonly ILogRepository _logRepo;

        public LogService(ILogRepository logRepo)
        {
            _logRepo = logRepo;
        }

        public async Task WriteAsync(Exception exception, string message, string place)
        {
            try
            {
                Log log = new Log
                {
                    Exception = exception.ToString(),
                    Message = message,
                    Place = place
                };
                await _logRepo.WriteAsync(log);
            }
            catch (Exception)
            {
            }
        }

        public void WriteSync(Exception exception, string message, string place)
        {
            try
            {
                Log log = new Log
                {
                    Exception = exception.ToString(),
                    Message = message,
                    Place = place
                };
                _logRepo.WriteSync(log);
            }
            catch (Exception)
            {
            }
        }
    }
}

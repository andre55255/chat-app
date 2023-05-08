using Chat.Core.Entities.NoSql;
using Chat.Core.RepositoriesInterface.NoSql;

namespace Chat.Core.ServicesInterface
{
    public interface ILogService
    {
        public Task WriteAsync(Exception exception, string message, string place);
        public void WriteSync(Exception exception, string message, string place);
    }
}

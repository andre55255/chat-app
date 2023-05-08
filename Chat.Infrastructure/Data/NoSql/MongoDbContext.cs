using Chat.Communication.CustomExceptions;
using Chat.Communication.ViewObjects.APISettings;
using Chat.Core.Entities.NoSql;
using Chat.Core.ServicesInterface;
using Chat.Infrastructure.ServicesImpl;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Chat.Infrastructure.Data.NoSql
{
    public class MongoDbContext
    {
        private IMongoDatabase _db { get; }

        public MongoDbContext(IConfiguration configuration)
        {
            try
            {
                IAPISettingsService apiSettingsService = new APISettingsService(configuration, null);
                AppSettingsVO appSettings = apiSettingsService.GetInfoAppSettings();

                MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(appSettings!.MongoDB!.Uri));
                MongoClient client = new MongoClient(settings);
                _db = client.GetDatabase(appSettings!.MongoDB!.Database);
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new DbConnectException($"Falha ao realizar conexão com a base de dados", ex);
            }
        }

        public IMongoCollection<EndPointMap> Endpoints
        {
            get
            {
                return _db.GetCollection<EndPointMap>("endpoints");
            }
        }
        public IMongoCollection<Log> Logs
        {
            get
            {
                return _db.GetCollection<Log>("logs");
            }
        }

        public IMongoCollection<Role> Roles
        {
            get
            {
                return _db.GetCollection<Role>("roles");
            }
        }

        public IMongoCollection<ApplicationUser> Users
        {
            get
            {
                return _db.GetCollection<ApplicationUser>("users");
            }
        }
    }
}

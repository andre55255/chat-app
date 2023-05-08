using AutoMapper;
using Chat.Communication.CustomExceptions;
using Chat.Communication.ViewObjects.EndPoints;
using Chat.Communication.ViewObjects.User;
using Chat.Communication.ViewObjects.Utils;
using Chat.Core.Entities.NoSql;
using Chat.Core.RepositoriesInterface.NoSql;
using Chat.Core.ServicesInterface;
using Chat.Helpers;
using Chat.Infrastructure.Data.NoSql;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Chat.Infrastructure.RepositoriesImpl.NoSql
{
    public class EndPointRepository : IEndPointRepository
    {
        private readonly MongoDbContext _dbMongo;
        private readonly ILogService _logService;
        private readonly IMapper _mapper;

        public EndPointRepository(MongoDbContext dbMongo, ILogService logService, IMapper mapper)
        {
            _dbMongo = dbMongo;
            _logService = logService;
            _mapper = mapper;
        }

        public async Task<EndPointSaveVO> CreateAsync(EndPointSaveVO model)
        {
            try
            {
                EndPointMap entity = _mapper.Map<EndPointMap>(model);

                entity.Id = null;
                await _dbMongo.Endpoints.InsertOneAsync(entity);

                EndPointSaveVO response = _mapper.Map<EndPointSaveVO>(entity);
                return response;
            }
            catch (DbConnectException ex)
            {
                throw new RepositoryException(ex.Message, ex);
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao criar endpoint: {StaticMethods.SerializeObject(model)}",
                    this.GetType().ToString());

                throw new RepositoryException($"Falha inesperada ao criar endpoint na base de dados");
            }
        }

        public async Task<EndPointSaveVO> DeleteAsync(string id)
        {
            try
            {
                StaticMethods.IsObjectIdValid(id);

                EndPointMap save =
                    await _dbMongo.Endpoints
                                  .Find(x => x.Id == id)
                                  .FirstOrDefaultAsync();

                if (save == null)
                    throw new NotFoundException($"Endpoint não encontrado");

                save.Id = id;
                save.DisabledAt = DateTime.Now;

                ReplaceOneResult result =
                    await _dbMongo.Endpoints.ReplaceOneAsync(x => x.Id == id, save);

                if (result == null || result.MatchedCount <= 0)
                    throw new RepositoryException($"Não foi possível deletar endpoint na base de dados");

                EndPointSaveVO response = _mapper.Map<EndPointSaveVO>(save);
                return response;
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (DbConnectException ex)
            {
                throw new RepositoryException(ex.Message, ex);
            }
            catch (NotFoundException ex)
            {
                throw ex;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao deletar endpoint: {id}",
                    this.GetType().ToString());

                throw new RepositoryException($"Falha inesperada ao deletar endpoint na base de dados");
            }
        }

        public async Task<EndPointSaveVO> EditAsync(EndPointSaveVO model)
        {
            try
            {
                StaticMethods.IsObjectIdValid(model.Id);

                EndPointMap save =
                    await _dbMongo.Endpoints
                                  .Find(x => x.Id == model.Id)
                                  .FirstOrDefaultAsync();

                if (save == null)
                    throw new NotFoundException($"Endpoint não encontrado");

                save.Id = model.Id;
                save.Route = model.Route;
                save.Verb = model.Verb;
                save.Roles = model.Roles;

                ReplaceOneResult result =
                    await _dbMongo.Endpoints.ReplaceOneAsync(x => x.Id == model.Id, save);

                if (result == null || result.MatchedCount <= 0)
                    throw new RepositoryException($"Não foi possível editar endpoint na base de dados");

                EndPointSaveVO response = _mapper.Map<EndPointSaveVO>(save);
                return response;
            }
            catch (DbConnectException ex)
            {
                throw new RepositoryException(ex.Message, ex);
            }
            catch (NotFoundException ex)
            {
                throw ex;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao editar endpoint: {StaticMethods.SerializeObject(model)}",
                    this.GetType().ToString());

                throw new RepositoryException($"Falha inesperada ao editar endpoint na base de dados");
            }
        }

        public async Task<ListAllEntityVO<EndPointSaveVO>> GetAllAsync(FilterEndpointVO filter)
        {
            try
            {
                Expression<Func<EndPointMap, bool>> conditionWhere =
                    x => x.DisabledAt == null &&
                         (filter.Route == null || x.Route!.Contains(filter.Route)) &&
                         (filter.Verb == null || x.Verb!.Contains(filter.Verb)) &&
                         (filter.Roles == null || x.Roles!.Intersect(filter.Roles).Any());

                ListAllEntityVO<EndPointSaveVO> response = new ListAllEntityVO<EndPointSaveVO>();
                response.TotalItems =
                    await _dbMongo.Endpoints
                                  .CountDocumentsAsync(conditionWhere);

                if (response.TotalItems.Value <= 0)
                    return null;

                int? limit = filter.Limit;
                int? page = filter.Page;
                StaticMethods.GetPaginationItems(ref response, ref limit, ref page);

                List<EndPointMap> save =
                    await _dbMongo.Endpoints
                                  .Find(conditionWhere)
                                  .Skip(limit!.Value * page!.Value)
                                  .Limit(limit)
                                  .ToListAsync();

                if (save == null)
                    return null;

                response.Items = _mapper.Map<List<EndPointSaveVO>>(save);
                return response;
            }
            catch (DbConnectException ex)
            {
                throw new RepositoryException(ex.Message, ex);
            }
            catch (NotFoundException ex)
            {
                throw ex;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao listar endpoints - {StaticMethods.SerializeObject(filter)}",
                    this.GetType().ToString());

                throw new RepositoryException($"Falha inesperada ao listar endpoints da base de dados");
            }
        }

        public async Task<EndPointSaveVO> GetByIdAsync(string id)
        {
            try
            {
                StaticMethods.IsObjectIdValid(id);

                EndPointMap save =
                    await _dbMongo.Endpoints
                                  .Find(x => x.DisabledAt == null &&
                                             x.Id == id)
                                  .FirstOrDefaultAsync();

                if (save == null)
                    return null;

                EndPointSaveVO response = _mapper.Map<EndPointSaveVO>(save);
                return response;
            }
            catch (DbConnectException ex)
            {
                throw new RepositoryException(ex.Message, ex);
            }
            catch (NotFoundException ex)
            {
                throw ex;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao listar endpoint por id: {id}",
                    this.GetType().ToString());

                throw new RepositoryException($"Falha inesperada ao listar endpoint pelo id {id}");
            }
        }

        public async Task<EndPointSaveVO> GetByRouteByVerbAsync(string route, string verb)
        {
            try
            {
                EndPointMap save =
                    await _dbMongo.Endpoints
                                  .Find(x => x.DisabledAt == null &&
                                             x.Route == route &&
                                             x.Verb == verb)
                                  .FirstOrDefaultAsync();

                if (save == null)
                    return null;

                EndPointSaveVO response = _mapper.Map<EndPointSaveVO>(save);
                return response;
            }
            catch (DbConnectException ex)
            {
                throw new RepositoryException(ex.Message, ex);
            }
            catch (NotFoundException ex)
            {
                throw ex;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao listar endpoint pela rota {route} e verbo {verb}",
                    this.GetType().ToString());

                throw new RepositoryException($"Falha inesperada ao listar endpoint pela rota {route} e verbo {verb}");
            }
        }

        public EndPointSaveVO GetByRouteByVerbSync(string route, string verb)
        {
            try
            {
                EndPointMap save =
                    _dbMongo.Endpoints
                                  .Find(x => x.DisabledAt == null &&
                                             x.Route == route &&
                                             x.Verb == verb)
                                  .FirstOrDefault();

                if (save == null)
                    return null;

                EndPointSaveVO response = _mapper.Map<EndPointSaveVO>(save);
                return response;
            }
            catch (DbConnectException ex)
            {
                throw new RepositoryException(ex.Message, ex);
            }
            catch (NotFoundException ex)
            {
                throw ex;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                _logService.WriteSync(ex,
                    $"Falha inesperada ao listar endpoint pela rota {route} e verbo {verb}",
                    this.GetType().ToString());

                throw new RepositoryException($"Falha inesperada ao listar endpoint pela rota {route} e verbo {verb}");
            }
        }
    }
}

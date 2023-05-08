using AutoMapper;
using Chat.Communication.CustomExceptions;
using Chat.Communication.ViewObjects.Role;
using Chat.Core.Entities.NoSql;
using Chat.Core.RepositoriesInterface.NoSql;
using Chat.Core.ServicesInterface;
using Chat.Helpers;
using Chat.Infrastructure.Data.NoSql;
using MongoDB.Driver;

namespace Chat.Infrastructure.RepositoriesImpl.NoSql
{
    public class RoleRepository : IRoleRepository
    {
        private readonly MongoDbContext _dbMongo;
        private readonly ILogService _logService;
        private readonly IMapper _mapper;

        public RoleRepository(MongoDbContext dbMongo, ILogService logService, IMapper mapper)
        {
            _dbMongo = dbMongo;
            _logService = logService;
            _mapper = mapper;
        }

        public async Task<RoleReturnVO> CreateAsync(RoleSaveVO role)
        {
            try
            {
                Role roleEntity = _mapper.Map<Role>(role);
                await _dbMongo.Roles.InsertOneAsync(roleEntity);
                
                RoleReturnVO response = _mapper.Map<RoleReturnVO>(roleEntity);
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
                    $"Falha inesperada ao criar o perfil: " + StaticMethods.SerializeObject(role),
                    this.GetType().ToString());

                throw new RepositoryException($"Falha inesperada ao criar perfil {role.Name}");
            }
        }

        public async Task<RoleReturnVO> DeleteAsync(string roleId)
        {
            try
            {
                StaticMethods.IsObjectIdValid(roleId);

                Role role =
                    await _dbMongo.Roles
                                  .Find(x => x.Id == roleId)
                                  .FirstOrDefaultAsync();

                if (role == null)
                    throw new NotFoundException($"Perfil não encontrado");

                DeleteResult result =
                    await _dbMongo.Roles
                                  .DeleteOneAsync(x => x.Id == roleId);

                if (result == null || result.DeletedCount <= 0)
                    throw new RepositoryException($"Não foi possível deletar o perfil na base de dados, ele não foi encontrado");

                RoleReturnVO response = _mapper.Map<RoleReturnVO>(role);
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
                    $"Falha inesperada ao deletar o perfil: " + roleId,
                    this.GetType().ToString());

                throw new RepositoryException($"Falha inesperada ao deletar perfil na base de dados");
            }
        }

        public async Task<RoleReturnVO> EditAsync(string roleId, RoleSaveVO role)
        {
            try
            {
                StaticMethods.IsObjectIdValid(roleId);

                Role save =
                    await _dbMongo.Roles
                                  .Find(x => x.Id == roleId)
                                  .FirstOrDefaultAsync();

                if (role == null)
                    throw new NotFoundException($"Perfil não encontrado");

                role.Id = roleId;
                save = _mapper.Map<Role>(role);

                ReplaceOneResult result =
                    await _dbMongo.Roles
                                  .ReplaceOneAsync(x => x.Id == roleId, save);

                if (result == null || result.MatchedCount <= 0)
                    throw new RepositoryException($"Não foi possível editar perfil na base de dados");

                RoleReturnVO response = _mapper.Map<RoleReturnVO>(save);
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
                    $"Falha inesperada ao editar o perfil: {roleId} - " + StaticMethods.SerializeObject(role),
                    this.GetType().ToString());

                throw new RepositoryException($"Falha inesperada ao editar perfil {role.Name}");
            }
        }

        public async Task<List<RoleReturnVO>> GetAllAsync()
        {
            try
            {
               List<Role> roles = 
                    await _dbMongo.Roles
                                  .Find(x => true)
                                  .ToListAsync();

                if (roles == null)
                    return null;

                List<RoleReturnVO> response = _mapper.Map<List<RoleReturnVO>>(roles);
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
                    $"Falha inesperada ao listar perfis",
                    this.GetType().ToString());

                throw new RepositoryException($"Falha inesperada ao listar perfis da base de dados");
            }
        }

        public async Task<RoleReturnVO> GetByIdAsync(string roleId)
        {
            try
            {
                StaticMethods.IsObjectIdValid(roleId);

                Role save =
                    await _dbMongo.Roles
                                  .Find(x => x.Id == roleId)
                                  .FirstOrDefaultAsync();

                if (save == null)
                    return null;

                RoleReturnVO response = _mapper.Map<RoleReturnVO>(save);
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
                    $"Falha inesperada ao listar o perfil: " + roleId,
                    this.GetType().ToString());

                throw new RepositoryException($"Falha inesperada ao listar perfil da base de dados");
            }
        }

        public async Task<RoleReturnVO> GetByNameAsync(string roleName)
        {
            try
            {
                Role save =
                    await _dbMongo.Roles
                                  .Find(x => x.NormalizedName == roleName.ToUpper())
                                  .FirstOrDefaultAsync();

                if (save == null)
                    return null;

                RoleReturnVO response = _mapper.Map<RoleReturnVO>(save);
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
                    $"Falha inesperada ao listar o perfil: " + roleName,
                    this.GetType().ToString());

                throw new RepositoryException($"Falha inesperada ao listar perfil da base de dados");
            }
        }
    }
}

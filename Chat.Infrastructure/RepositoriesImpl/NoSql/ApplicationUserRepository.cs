using AutoMapper;
using Chat.Communication.CustomExceptions;
using Chat.Communication.ViewObjects.Role;
using Chat.Communication.ViewObjects.User;
using Chat.Communication.ViewObjects.Utils;
using Chat.Core.Entities.NoSql;
using Chat.Core.RepositoriesInterface.NoSql;
using Chat.Core.ServicesInterface;
using Chat.Helpers;
using Chat.Infrastructure.Data.NoSql;
using FluentResults;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Chat.Infrastructure.RepositoriesImpl.NoSql
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly MongoDbContext _dbMongo;
        private readonly ILogService _logService;
        private readonly IMapper _mapper;

        public ApplicationUserRepository(MongoDbContext dbMongo, ILogService logService, IMapper mapper)
        {
            _dbMongo = dbMongo;
            _logService = logService;
            _mapper = mapper;
        }

        public async Task<UserReturnVO> CreateAsync(UserCreateVO applicationUser, List<RoleReturnVO> roles)
        {
            try
            {
                ApplicationUser save = _mapper.Map<ApplicationUser>(applicationUser);
                
                save.Id = null;
                save.Roles = _mapper.Map<List<Role>>(roles);
                await _dbMongo.Users.InsertOneAsync(save);

                UserReturnVO response = _mapper.Map<UserReturnVO>(save);
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
                    $"Falha inesperada ao criar usuário: {StaticMethods.SerializeObject(applicationUser)}",
                    this.GetType().ToString());

                throw new RepositoryException($"Falha inesperada ao criar usuário na base de dados");
            }
        }

        public async Task<UserReturnVO> DeleteAsync(string userId)
        {
            try
            {
                StaticMethods.IsObjectIdValid(userId);

                ApplicationUser save =
                    await _dbMongo.Users
                                  .Find(x => x.Id == userId)
                                  .FirstOrDefaultAsync();

                if (save == null)
                    throw new NotFoundException($"Usuário não encontrado");

                save.Id = userId;
                save.DisabledAt = DateTime.Now;

                ReplaceOneResult result =
                    await _dbMongo.Users.ReplaceOneAsync(x => x.Id == userId, save);

                if (result == null || result.MatchedCount <= 0)
                    throw new RepositoryException($"Não foi possível deletar usuário na base de dados");

                UserReturnVO response = _mapper.Map<UserReturnVO>(save);
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
                    $"Falha inesperada ao deletar usuário: {userId}",
                    this.GetType().ToString());

                throw new RepositoryException($"Falha inesperada ao deletar usuário na base de dados");
            }
        }

        public async Task<UserReturnVO> EditAsync(string userId, UserEditVO applicationUser, List<RoleReturnVO> roles)
        {
            try
            {
                StaticMethods.IsObjectIdValid(userId);

                ApplicationUser save =
                    await _dbMongo.Users
                                  .Find(x => x.Id == userId)
                                  .FirstOrDefaultAsync();

                if (save == null)
                    throw new NotFoundException($"Usuário não encontrado");

                save.Id = userId;
                save.FirstName = applicationUser.FirstName;
                save.LastName = applicationUser.LastName;
                save.Email = applicationUser.Email;
                save.Username = applicationUser.Username;
                save.Roles = _mapper.Map<List<Role>>(roles);

                ReplaceOneResult result =
                    await _dbMongo.Users.ReplaceOneAsync(x => x.Id == userId, save);

                if (result == null || result.MatchedCount <= 0)
                    throw new RepositoryException($"Não foi possível editar usuário na base de dados");

                UserReturnVO response = _mapper.Map<UserReturnVO>(save);
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
                    $"Falha inesperada ao editar usuário: {userId} - {StaticMethods.SerializeObject(applicationUser)}",
                    this.GetType().ToString());

                throw new RepositoryException($"Falha inesperada ao editar usuário na base de dados");
            }
        }

        public async Task<ListAllEntityVO<UserReturnVO>> GetAllAsync(FilterUserVO filter)
        {
            try
            {
                Expression<Func<ApplicationUser, bool>> conditionWhere =
                    x => x.DisabledAt == null &&
                         (filter.Username == null || x.Username!.Contains(filter.Username)) &&
                         (filter.Email == null || x.Email!.Contains(filter.Email)) &&
                         (filter.Name == null || x.FirstName!.Contains(filter.Name) ||
                                                 x.LastName!.Contains(filter.Name) &&
                         (filter.Roles == null || x.Roles!.Where(x => filter.Roles.Contains(x.Name!)).Any()));

                ListAllEntityVO<UserReturnVO> response = new ListAllEntityVO<UserReturnVO>();
                response.TotalItems =
                    await _dbMongo.Users
                                  .CountDocumentsAsync(conditionWhere);

                if (response.TotalItems.Value <= 0)
                    return null;

                int? limit = filter.Limit;
                int? page = filter.Page;
                StaticMethods.GetPaginationItems(ref response, ref limit, ref page);

                List<ApplicationUser> save =
                    await _dbMongo.Users
                                  .Find(conditionWhere)
                                  .Skip(limit!.Value * page!.Value)
                                  .Limit(limit)
                                  .ToListAsync();

                if (save == null)
                    return null;

                response.Items = _mapper.Map<List<UserReturnVO>>(save);
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
                    $"Falha inesperada ao listar usuários",
                    this.GetType().ToString());

                throw new RepositoryException($"Falha inesperada ao listar usuários da base de dados");
            }
        }

        public async Task<UserReturnVO> GetByEmailAsync(string email)
        {
            try
            {
                ApplicationUser save =
                    await _dbMongo.Users
                                  .Find(x => x.DisabledAt == null &&
                                             x.Email == email)
                                  .FirstOrDefaultAsync();

                if (save == null)
                    return null;

                UserReturnVO response = _mapper.Map<UserReturnVO>(save);
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
                    $"Falha inesperada ao listar usuário pelo email: {email}",
                    this.GetType().ToString());

                throw new RepositoryException($"Falha inesperada ao listar usuário pelo email {email}");
            }
        }

        public async Task<UserReturnVO> GetByIdAsync(string userId)
        {
            try
            {
                StaticMethods.IsObjectIdValid(userId);

                ApplicationUser save =
                    await _dbMongo.Users
                                  .Find(x => x.DisabledAt == null &&
                                             x.Id == userId)
                                  .FirstOrDefaultAsync();

                if (save == null)
                    return null;

                UserReturnVO response = _mapper.Map<UserReturnVO>(save);
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
                    $"Falha inesperada ao listar usuário por id: {userId}",
                    this.GetType().ToString());

                throw new RepositoryException($"Falha inesperada ao listar usuário pelo id {userId}");
            }
        }

        public async Task<UserReturnVO> GetByUsernameAsync(string username)
        {
            try
            {
                ApplicationUser save =
                    await _dbMongo.Users
                                  .Find(x => x.DisabledAt == null &&
                                             x.Username == username)
                                  .FirstOrDefaultAsync();

                if (save == null)
                    return null;

                UserReturnVO response = _mapper.Map<UserReturnVO>(save);
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
                    $"Falha inesperada ao listar usuário pelo nome de usuário: {username}",
                    this.GetType().ToString());

                throw new RepositoryException($"Falha inesperada ao listar usuário pelo nome de usuário {username}");
            }
        }

        public async Task<Result> SetAttemptLoginAsync(string userId, bool isIncrement = false)
        {
            try
            {
                StaticMethods.IsObjectIdValid(userId);

                ApplicationUser save =
                    await _dbMongo.Users
                                  .Find(x => x.Id == userId)
                                  .FirstOrDefaultAsync();

                if (save == null)
                    throw new NotFoundException($"Usuário não encontrado");

                save.AttemptAccessLogin = isIncrement ? save.AttemptAccessLogin + 1 : 0;
                save.UpdatedAt = DateTime.Now;

                ReplaceOneResult result =
                    await _dbMongo.Users.ReplaceOneAsync(x => x.Id == userId, save);

                if (result == null || result.MatchedCount <= 0)
                    throw new RepositoryException($"Não foi possível incrementar login errado no usuário na base de dados");

                return Result.Ok();
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
                    $"Falha inesperada ao incrementar tentativas de login de usuário: {userId} - {isIncrement}",
                    this.GetType().ToString());

                throw new RepositoryException($"Falha inesperada ao alterar usuário na base de dados");
            }
        }

        public async Task<Result> SetBlockUnblockUserAsync(string userId, bool isUnblock, DateTime? lockouDateEnd = null)
        {
            try
            {
                StaticMethods.IsObjectIdValid(userId);

                ApplicationUser save =
                    await _dbMongo.Users
                                  .Find(x => x.Id == userId)
                                  .FirstOrDefaultAsync();

                if (save == null)
                    throw new NotFoundException($"Usuário não encontrado");

                if (isUnblock)
                {
                    save.AttemptAccessLogin = 0;
                    save.LockoutDate = null;
                    save.LockoutDateEnd = null;
                }
                else
                {
                    save.LockoutDate = DateTime.Now;
                    save.LockoutDateEnd = lockouDateEnd;
                }
                save.UpdatedAt = DateTime.Now;

                ReplaceOneResult result =
                    await _dbMongo.Users.ReplaceOneAsync(x => x.Id == userId, save);

                if (result == null || result.MatchedCount <= 0)
                    throw new RepositoryException($"Não foi possível bloquear/desbloquear usuário na base de dados");

                return Result.Ok();
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
                    $"Falha inesperada ao setar status de usuário: {userId} - {isUnblock} - {lockouDateEnd}",
                    this.GetType().ToString());

                throw new RepositoryException($"Falha inesperada ao alterar status de usuário na base de dados");
            }
        }

        public async Task<Result> SetNewPasswordAsync(string userId, string newPassword)
        {
            try
            {
                StaticMethods.IsObjectIdValid(userId);

                ApplicationUser save =
                    await _dbMongo.Users
                                  .Find(x => x.Id == userId)
                                  .FirstOrDefaultAsync();

                if (save == null)
                    throw new NotFoundException($"Usuário não encontrado");

                save.PasswordHash = newPassword;
                save.UpdatedAt = DateTime.Now;

                ReplaceOneResult result =
                    await _dbMongo.Users.ReplaceOneAsync(x => x.Id == userId, save);

                if (result == null || result.MatchedCount <= 0)
                    throw new RepositoryException($"Não foi possível setar senha de usuário na base de dados");

                return Result.Ok();
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
                    $"Falha inesperada ao setar nova senha de usuário: {userId} - {newPassword}",
                    this.GetType().ToString());

                throw new RepositoryException($"Falha inesperada ao alterar senha de usuário na base de dados");
            }
        }

        public async Task<Result> SetRefreshTokenAsync(string userId, string refreshToken)
        {
            try
            {
                StaticMethods.IsObjectIdValid(userId);

                ApplicationUser save =
                    await _dbMongo.Users
                                  .Find(x => x.Id == userId)
                                  .FirstOrDefaultAsync();

                if (save == null)
                    throw new NotFoundException($"Usuário não encontrado");

                save.RefreshToken = refreshToken;
                save.UpdatedAt = DateTime.Now;

                ReplaceOneResult result =
                    await _dbMongo.Users.ReplaceOneAsync(x => x.Id == userId, save);

                if (result == null || result.MatchedCount <= 0)
                    throw new RepositoryException($"Não foi possível setar token de usuário na base de dados");

                return Result.Ok();
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
                    $"Falha inesperada ao setar token de usuário: {userId} - {refreshToken}",
                    this.GetType().ToString());

                throw new RepositoryException($"Falha inesperada ao alterar token de usuário na base de dados");
            }
        }

        public async Task<Result> SetTokenPassAsync(string userId, string tokenPass)
        {
            try
            {
                StaticMethods.IsObjectIdValid(userId);

                ApplicationUser save =
                    await _dbMongo.Users
                                  .Find(x => x.Id == userId)
                                  .FirstOrDefaultAsync();

                if (save == null)
                    throw new NotFoundException($"Usuário não encontrado");

                save.TokenPass = tokenPass;
                save.UpdatedAt = DateTime.Now;

                ReplaceOneResult result =
                    await _dbMongo.Users.ReplaceOneAsync(x => x.Id == userId, save);

                if (result == null || result.MatchedCount <= 0)
                    throw new RepositoryException($"Não foi possível setar token de usuário na base de dados");

                return Result.Ok();
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
                    $"Falha inesperada ao setar token de usuário: {userId} - {tokenPass}",
                    this.GetType().ToString());

                throw new RepositoryException($"Falha inesperada ao alterar token de usuário na base de dados");
            }
        }
    }
}

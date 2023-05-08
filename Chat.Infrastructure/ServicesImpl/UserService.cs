using AutoMapper;
using Chat.Communication.CustomExceptions;
using Chat.Communication.ViewObjects.Role;
using Chat.Communication.ViewObjects.User;
using Chat.Communication.ViewObjects.Utils;
using Chat.Core.Entities.NoSql;
using Chat.Core.RepositoriesInterface.NoSql;
using Chat.Core.ServicesInterface;
using Chat.Helpers;
using FluentResults;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Chat.Infrastructure.ServicesImpl
{
    public class UserService : IUserService
    {
        private readonly IApplicationUserRepository _userRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly ILogService _logService;
        private readonly IMapper _mapper;

        public UserService(IApplicationUserRepository userRepo, ILogService logService, IMapper mapper, IRoleRepository roleRepo)
        {
            _userRepo = userRepo;
            _logService = logService;
            _mapper = mapper;
            _roleRepo = roleRepo;
        }

        public async Task<UserReturnVO> CreateAsync(UserCreateVO model)
        {
            try
            {
                await ValidConsistenceDataCreateAsync(model);
                List<RoleReturnVO> roles = await GetRolesObjectAsync(model.Roles);

                string salt = StaticMethods.GetSaltGenerateHashPasswordUser(model, model.Password);
                model.Password = StaticMethods.CryptPasswordMD5(model.Password, salt);

                UserReturnVO user = await _userRepo.CreateAsync(model, roles);
                return user;
            }
            catch (RepositoryException ex)
            {
                throw new ValidException(ex.Message, ex);
            }
            catch (EmailException ex)
            {
                throw new ValidException(ex.Message, ex);
            }
            catch (NotFoundException ex)
            {
                throw ex;
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao criar usuário: ${StaticMethods.SerializeObject(model)}",
                    this.GetType().ToString());

                throw new ValidException("Falha inesperada ao criar usuário");
            }
        }

        public async Task<UserReturnVO> DeleteAsync(string userId)
        {
            try
            {
                StaticMethods.IsObjectIdValid(userId);
                UserReturnVO result = await _userRepo.DeleteAsync(userId);
                return result;
            }
            catch (RepositoryException ex)
            {
                throw new ValidException(ex.Message, ex);
            }
            catch (EmailException ex)
            {
                throw new ValidException(ex.Message, ex);
            }
            catch (NotFoundException ex)
            {
                throw ex;
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao deletar usuário: ${userId}",
                    this.GetType().ToString());

                throw new ValidException("Falha inesperada ao deletar usuário");
            }
        }

        public async Task<UserReturnVO> EditAsync(string userId, UserEditVO model)
        {
            try
            {
                StaticMethods.IsObjectIdValid(userId);

                await ValidConsistenceDataEditAsync(userId, model);
                List<RoleReturnVO> roles = await GetRolesObjectAsync(model.Roles);
                UserReturnVO response = await _userRepo.EditAsync(userId, model, roles);
                return response;
            }
            catch (RepositoryException ex)
            {
                throw new ValidException(ex.Message, ex);
            }
            catch (EmailException ex)
            {
                throw new ValidException(ex.Message, ex);
            }
            catch (NotFoundException ex)
            {
                throw ex;
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao editar usuário: {userId} - {StaticMethods.SerializeObject(model)}",
                    this.GetType().ToString());

                throw new ValidException("Falha inesperada ao editar usuário");
            }
        }

        public async Task<ListAllEntityVO<UserReturnVO>> GetAllAsync(FilterUserVO filter)
        {
            try
            {
                filter = filter == null ? new FilterUserVO() : filter;
                ListAllEntityVO<UserReturnVO> response = await _userRepo.GetAllAsync(filter);
                if (response == null)
                    throw new NotFoundException($"Nenhum registro encontrado");

                return response;
            }
            catch (RepositoryException ex)
            {
                throw new ValidException(ex.Message, ex);
            }
            catch (EmailException ex)
            {
                throw new ValidException(ex.Message, ex);
            }
            catch (NotFoundException ex)
            {
                throw ex;
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao listar usuários {StaticMethods.SerializeObject(filter)}",
                    this.GetType().ToString());

                throw new ValidException($"Falha inesperada ao listar usuários");
            }
        }

        public async Task<UserReturnVO> GetByIdAsync(string userId)
        {
            try
            {
                UserReturnVO user = await _userRepo.GetByIdAsync(userId);
                if (user == null)
                    throw new NotFoundException($"Usuário não encontrado");

                return user;
            }
            catch (RepositoryException ex)
            {
                throw new ValidException(ex.Message, ex);
            }
            catch (EmailException ex)
            {
                throw new ValidException(ex.Message, ex);
            }
            catch (NotFoundException ex)
            {
                throw ex;
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao listar usuário pelo id {userId}",
                    this.GetType().ToString());

                throw new ValidException($"Falha inesperada ao usuário pelo id {userId}");
            }
        }

        private async Task ValidConsistenceDataCreateAsync(UserCreateVO model)
        {
            try
            {
                UserReturnVO usernameExist = await _userRepo.GetByUsernameAsync(model.Username);
                if (usernameExist != null)
                    throw new ValidException($"Já existe um usuário com o nome de usuário {model.Username}");

                UserReturnVO emailExist = await _userRepo.GetByEmailAsync(model.Email);
                if (emailExist != null)
                    throw new ValidException($"Já existe um usuário com o email {model.Email}");
            }
            catch (RepositoryException ex)
            {
                throw new ValidException(ex.Message, ex);
            }
            catch (NotFoundException ex)
            {
                throw ex;
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao validar dados para salvar usuário: ${StaticMethods.SerializeObject(model)}",
                    this.GetType().ToString());

                throw new ValidException("Falha inesperada ao validar dados para salvar usuário");
            }
        }

        private async Task ValidConsistenceDataEditAsync(string userId, UserEditVO model)
        {
            try
            {
                UserReturnVO usernameExist = await _userRepo.GetByUsernameAsync(model.Username);
                if (usernameExist != null && usernameExist.Id != userId)
                    throw new ValidException($"Já existe um usuário com o nome de usuário {model.Username}");

                UserReturnVO emailExist = await _userRepo.GetByEmailAsync(model.Email);
                if (emailExist != null && emailExist.Id != userId)
                    throw new ValidException($"Já existe um usuário com o email {model.Email}");
            }
            catch (RepositoryException ex)
            {
                throw new ValidException(ex.Message, ex);
            }
            catch (NotFoundException ex)
            {
                throw ex;
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao validar dados para salvar usuário: ${StaticMethods.SerializeObject(model)}",
                    this.GetType().ToString());

                throw new ValidException("Falha inesperada ao validar dados para salvar usuário");
            }
        }

        private async Task<List<RoleReturnVO>> GetRolesObjectAsync(List<string> roles)
        {
            try
            {
                List<RoleReturnVO> response = new List<RoleReturnVO>();
                foreach (string role in roles)
                {
                    StaticMethods.IsObjectIdValid(role);
                    RoleReturnVO aux = await _roleRepo.GetByIdAsync(role);
                    if (aux == null)
                        throw new NotFoundException($"Perfis informados não encontrado: {role}");

                    response.Add(aux);
                }
                return response;
            }
            catch (RepositoryException ex)
            {
                throw new ValidException(ex.Message, ex);
            }
            catch (NotFoundException ex)
            {
                throw ex;
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao validar perfis enviados: ${StaticMethods.SerializeObject(roles)}",
                    this.GetType().ToString());

                throw new ValidException("Falha inesperada ao validar perfis enviados");
            }
        }
    }
}

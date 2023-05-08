using Chat.Communication.CustomExceptions;
using Chat.Communication.ViewObjects.Role;
using Chat.Core.RepositoriesInterface.NoSql;
using Chat.Core.ServicesInterface;
using Chat.Helpers;

namespace Chat.Infrastructure.ServicesImpl
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepo;
        private readonly ILogService _logService;

        public RoleService(IRoleRepository roleRepo, ILogService logService)
        {
            _roleRepo = roleRepo;
            _logService = logService;
        }

        public async Task<RoleReturnVO> CreateAsync(RoleSaveVO roleSaveVO)
        {
            try
            {
                RoleReturnVO roleExist = await _roleRepo.GetByNameAsync(roleSaveVO.Name);
                if (roleExist != null)
                    throw new ValidException($"Já existe uma role com o nome {roleSaveVO.Name}");

                RoleReturnVO result = await _roleRepo.CreateAsync(roleSaveVO);
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
                    $"Falha inesperada ao criar perfil: ${StaticMethods.SerializeObject(roleSaveVO)}",
                    this.GetType().ToString());

                throw new ValidException("Falha inesperada ao criar perfil");
            }
        }

        public async Task<RoleReturnVO> DeleteAsync(string roleId)
        {
            try
            {
                StaticMethods.IsObjectIdValid(roleId);

                RoleReturnVO result = await _roleRepo.DeleteAsync(roleId);
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
                    $"Falha inesperada ao deletar perfil: {roleId}",
                    this.GetType().ToString());

                throw new ValidException("Falha inesperada ao deletar perfil");
            }
        }

        public async Task<RoleReturnVO> EditAsync(string roleId, RoleSaveVO roleSaveVO)
        {
            try
            {
                StaticMethods.IsObjectIdValid(roleId);

                RoleReturnVO roleExist = await _roleRepo.GetByNameAsync(roleSaveVO.Name);
                if (roleExist != null && roleExist.Id != roleId)
                    throw new ValidException($"Já existe uma role com o nome {roleSaveVO.Name}");

                RoleReturnVO result = await _roleRepo.EditAsync(roleId, roleSaveVO);
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
                    $"Falha inesperada ao editar perfil: ${StaticMethods.SerializeObject(roleSaveVO)}",
                    this.GetType().ToString());

                throw new ValidException("Falha inesperada ao editar perfil");
            }
        }

        public async Task<List<RoleReturnVO>> GetAllAsync()
        {
            try
            {
                List<RoleReturnVO> result = await _roleRepo.GetAllAsync();
                if (result == null)
                    throw new NotFoundException($"Perfil não encontrado");

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
                    $"Falha inesperada ao listar perfis",
                this.GetType().ToString());

                throw new ValidException($"Falha inesperada ao listar perfis");
            }
        }

        public async Task<RoleReturnVO> GetByIdAsync(string roleId)
        {
            try
            {
                StaticMethods.IsObjectIdValid(roleId);

                RoleReturnVO result = await _roleRepo.GetByIdAsync(roleId);
                if (result == null)
                    throw new NotFoundException($"Perfil não encontrado");

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
                    $"Falha inesperada ao listar perfil pelo id: {roleId}",
                    this.GetType().ToString());

                throw new ValidException($"Falha inesperada ao listar perfil pelo id {roleId}");
            }
        }
    }
}

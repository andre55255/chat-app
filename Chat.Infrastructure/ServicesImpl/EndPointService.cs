using AutoMapper;
using Chat.Communication.CustomExceptions;
using Chat.Communication.ViewObjects.EndPoints;
using Chat.Communication.ViewObjects.Role;
using Chat.Communication.ViewObjects.Utils;
using Chat.Core.RepositoriesInterface.NoSql;
using Chat.Core.ServicesInterface;
using Chat.Helpers;

namespace Chat.Infrastructure.ServicesImpl
{
    public class EndPointService : IEndPointService
    {
        private readonly IEndPointRepository _endPointRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly ILogService _logService;

        public EndPointService(IEndPointRepository endPointRepo, IRoleRepository roleRepo, ILogService logService)
        {
            _endPointRepo = endPointRepo;
            _roleRepo = roleRepo;
            _logService = logService;
        }

        public async Task<EndPointSaveVO> CreateAsync(EndPointSaveVO model)
        {
            try
            {
                await ValidConsistenceDataAsync(model);

                EndPointSaveVO response = await _endPointRepo.CreateAsync(model);
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
                    $"Falha inesperada ao criar ednpoint: ${StaticMethods.SerializeObject(model)}",
                    this.GetType().ToString());

                throw new ValidException("Falha inesperada ao criar endpoint");
            }
        }

        public async Task<EndPointSaveVO> DeleteAsync(string id)
        {
            try
            {
                EndPointSaveVO result = await _endPointRepo.DeleteAsync(id);
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
                    $"Falha inesperada ao deletar endpoint: ${id}",
                    this.GetType().ToString());

                throw new ValidException("Falha inesperada ao deletar endpoint");
            }
        }

        public async Task<EndPointSaveVO> EditAsync(EndPointSaveVO model)
        {
            try
            {
                StaticMethods.IsObjectIdValid(model.Id);
                await ValidConsistenceDataAsync(model, true);

                EndPointSaveVO response = await _endPointRepo.EditAsync(model);
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
                    $"Falha inesperada ao editar endpoint: ${StaticMethods.SerializeObject(model)}",
                    this.GetType().ToString());

                throw new ValidException("Falha inesperada ao editar endpoint");
            }
        }

        public async Task<ListAllEntityVO<EndPointSaveVO>> GetAllAsync(FilterEndpointVO filter)
        {
            try
            {
                filter = filter == null ? new FilterEndpointVO() : filter;
                ListAllEntityVO<EndPointSaveVO> response = await _endPointRepo.GetAllAsync(filter);
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
                    $"Falha inesperada ao listar endpoints {StaticMethods.SerializeObject(filter)}",
                    this.GetType().ToString());

                throw new ValidException($"Falha inesperada ao listar endpoints");
            }
        }

        public async Task<EndPointSaveVO> GetByIdAsync(string id)
        {
            try
            {
                StaticMethods.IsObjectIdValid(id);

                EndPointSaveVO save = await _endPointRepo.GetByIdAsync(id);
                if (save == null)
                    throw new NotFoundException($"Endpoint não encontrado");

                return save;
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
                    $"Falha inesperada ao listar endpoint pelo id {id}",
                    this.GetType().ToString());

                throw new ValidException($"Falha inesperada ao endpoint pelo id {id}");
            }
        }

        private async Task ValidConsistenceDataAsync(EndPointSaveVO model, bool isEdit = false)
        {
            try
            {
                EndPointSaveVO exist = await _endPointRepo.GetByRouteByVerbAsync(model.Route, model.Verb);
                if (!isEdit && exist != null)
                    throw new ValidException($"Já existe um endpoint cadastrado com a rota {model.Route} e com o verbo {model.Verb}");

                if (isEdit && exist.Id != model.Id)
                    throw new ValidException($"Já existe um endpoint cadastrado com a rota {model.Route} e com o verbo {model.Verb}");

                foreach (string role in model.Roles)
                {
                    RoleReturnVO roleExist = await _roleRepo.GetByNameAsync(role);
                    if (roleExist == null)
                        throw new ValidException($"Perfil {role} não encontrado para vincular");
                }
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
                    $"Falha inesperada ao validar dados para salvar endpoint: ${StaticMethods.SerializeObject(model)}",
                    this.GetType().ToString());

                throw new ValidException("Falha inesperada ao validar dados para salvar endpoint");
            }
        }
    }
}

using Chat.Communication.ViewObjects.Role;

namespace Chat.Core.ServicesInterface
{
    public interface IRoleService
    {
        public Task<RoleReturnVO> CreateAsync(RoleSaveVO roleSaveVO);
        public Task<RoleReturnVO> EditAsync(string roleId, RoleSaveVO roleSaveVO);
        public Task<RoleReturnVO> DeleteAsync(string roleId);
        public Task<RoleReturnVO> GetByIdAsync(string roleId);
        public Task<List<RoleReturnVO>> GetAllAsync();
    }
}

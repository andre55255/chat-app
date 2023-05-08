using Chat.Communication.ViewObjects.Role;

namespace Chat.Core.RepositoriesInterface.NoSql
{
    public interface IRoleRepository
    {
        public Task<RoleReturnVO> CreateAsync(RoleSaveVO role);
        public Task<RoleReturnVO> EditAsync(string roleId, RoleSaveVO role);
        public Task<RoleReturnVO> DeleteAsync(string roleId);
        public Task<RoleReturnVO> GetByIdAsync(string roleId);
        public Task<RoleReturnVO> GetByNameAsync(string roleName);
        public Task<List<RoleReturnVO>> GetAllAsync();
    }
}

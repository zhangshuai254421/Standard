using JM.IServices.Base;
using JM.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JM.IServices
{
    /// <summary>
    /// RoleServices
    /// </summary>	
    public interface IRoleServices : IBaseServices<Role>
    {
        Task<Role> SaveRole(string roleName);
        Task<string> GetRoleNameByRid(int rid);

    }
}

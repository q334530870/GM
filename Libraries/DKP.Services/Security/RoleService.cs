using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Dynamic;
using DKP.Core;
using DKP.Core.Data;
using DKP.Core.Extensions;
using DKP.Core.Models;
using Permission = DKP.Data.Permission;
using DKP.Data;

namespace DKP.Services.Security
{
    public interface IRoleService
    {
        IPagedList<Role> GetAllRoles(NameValueCollection forms, PageParam pageParam);
        IList<Role> GetAllRoles();

        IList<Role> GetAllRoles(string name);

        Role GetRoleById(int id);

        void InsterRole(Role role, string[] permissions);

        void UpdateRole(Role role, string[] permissions);

        void DeleteRole(int id);
    }

    public class RoleService : IRoleService
    {
        #region Fields

        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<Permission> _permissionRepository;

        #endregion

        #region Ctor

        public RoleService(IRepository<Role> roleRepository, IRepository<Permission> permissionRepository)
        {
            this._roleRepository = roleRepository;
            this._permissionRepository = permissionRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <returns></returns>
        public IPagedList<Role> GetAllRoles(NameValueCollection forms, PageParam pageParam)
        {
            var query = _roleRepository.Table.Where(forms.ResolveToLinq()).OrderBy(pageParam.ToString());
            return new PagedList<Role>(query, pageParam);
        }

        public IList<Role> GetAllRoles()
        {
            return _roleRepository.Table.ToList();
        }

        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <param name="name">角色名称</param>
        /// <returns></returns>
        public IList<Role> GetAllRoles(string name)
        {
            var query = _roleRepository.Table;
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(t => t.Name.Contains(name));
            }
            query = query.OrderByDescending(t => t.Id);
            return query.ToList();
        }

        /// <summary>
        /// 根据id获取角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Role GetRoleById(int id)
        {
            return _roleRepository.GetById(id);
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="role"></param>
        /// <param name="permissions"></param>
        public void InsterRole(Role role, string[] permissions)
        {
            _roleRepository.Insert(role);


            role.Permissions.Clear();

            if (permissions != null && permissions.Any())
            {
                var permIds = (from permission in permissions
                               where !string.IsNullOrEmpty(permission)
                               select Convert.ToInt32(permission)).ToList();
                var allPerms = _permissionRepository.Table.ToList();

                foreach (int pid in permIds)
                {
                    var perm = allPerms.FirstOrDefault(t => t.Id == pid);
                    role.Permissions.Add(perm);
                }
            }

            _roleRepository.Save();
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="role"></param>
        /// <param name="permissions"></param>
        public void UpdateRole(Role role, string[] permissions)
        {
            _roleRepository.Update(role);
            role.Permissions.Clear();
            if (permissions != null && permissions.Any())
            {
                var permIds = (from permission in permissions
                               where !string.IsNullOrEmpty(permission)
                               select Convert.ToInt32(permission)).ToList();
                var allPerms = _permissionRepository.Table.ToList();
                foreach (int pid in permIds)
                {
                    var perm = allPerms.FirstOrDefault(t => t.Id == pid);
                    role.Permissions.Add(perm);
                }
            }
            _roleRepository.Save();
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id"></param>
        public void DeleteRole(int id)
        {
            var role = _roleRepository.GetById(id);
            _roleRepository.Delete(role);
            role.Permissions.Clear();
            _roleRepository.Save();
        }

        #endregion
    }
}

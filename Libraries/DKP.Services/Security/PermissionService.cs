using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DKP.Core.Caching;
using DKP.Core.Data;
using DKP.Data;

namespace DKP.Services.Security
{
    public interface IPermissionService
    {
        IList<Permission> GetAllPermissions();

        Permission GetPermissionById(int id);

        void ClearCache();

        void AddPermission(Permission permission);

        void updatePermission(Permission permission);

        void deletePermission(int id);

    }
    public class PermissionService : IPermissionService
    {
        #region Constants

        private const string PERMISSION_ALLOWED_KEY = "DKP.permission.allowed-{0}";

        private const string PERMISSION_PATTERN_KEY = "DKP.permission.";

        #endregion

        #region Fields

        private readonly IRepository<Permission> _permissionRepository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        public PermissionService(IRepository<Permission> permissionRepository, ICacheManager cacheManager)
        {
            _permissionRepository = permissionRepository;
            _cacheManager = cacheManager;
        }

        #endregion

        #region Utilities

        public virtual IList<Permission> GetAllPermissions()
        {
            var key = string.Format(PERMISSION_ALLOWED_KEY, "all");
            //return _permissionRepository.Table.OrderBy(t => t.Order).ToList();
            return _cacheManager.Get(key,
                () => _permissionRepository.Table.Include(t => t.Children).OrderBy(t => t.Order).ToList());
        }

        public Permission GetPermissionById(int id)
        {
            return _permissionRepository.GetById(id);
        }

        public void ClearCache()
        {
            _cacheManager.RemoveByPattern(PERMISSION_PATTERN_KEY);
        }

        public void AddPermission(Permission permission)
        {
            _permissionRepository.Insert(permission);
            _permissionRepository.Save();
        }

        public void updatePermission(Permission permission)
        {
            _permissionRepository.Update(permission);
            _permissionRepository.Save();
        }

        public void deletePermission(int id)
        {
            _permissionRepository.Delete(id);
            _permissionRepository.Save();
        }

        #endregion
    }
}

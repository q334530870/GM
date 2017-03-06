using System;
using AutoMapper;
using Permission = DKP.Core.Models.Permission;
using DKP.Core.Infrastructure;
using DKP.Data;
using DKP.Core.Models;

namespace DKP.Web.Infrastructure
{
    public class AutoMapperStartupTask : IStartupTask
    {
        public void Execute()
        {
            //automapper
            Mapper.CreateMap<DKP.Data.Permission, Permission>();
            Mapper.CreateMap<Permission, DKP.Data.Permission>();

            Mapper.CreateMap<Role, RoleModel>().ForMember(dest => dest.PermissionIds, mo => mo.Ignore());
            Mapper.CreateMap<RoleModel, Role>()
                  .ForMember(dest => dest.Permissions, mo => mo.Ignore())
                  .ForMember(dest => dest.Users, mo => mo.Ignore());


        }

        public int Order
        {
            get { return 0; }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using DKP.Core.Infrastructure;
using DKP.Core.Infrastructure.DependencyManagement;

namespace Papaya.WebApi.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
        }

        public int Order
        {
            get { return 3; }
        }
    }
}
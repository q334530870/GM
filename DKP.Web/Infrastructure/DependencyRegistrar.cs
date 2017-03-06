using Autofac;
using DKP.Core.Infrastructure;
using DKP.Core.Infrastructure.DependencyManagement;

namespace DKP.Web.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            //builder.RegisterType<SubscriptionController>()
            //    .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("papaya_cache_static"));
        }

        public int Order
        {
            get { return 2; }
        }
    }
}
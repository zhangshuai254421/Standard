using Autofac.Core;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Autofac.Extensions.DependencyInjection;
using System.Linq;
using System.Web.UI;

namespace AutoFac.Unit
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            ContainerBuilder containerBuilder = new ContainerBuilder();
            ServiceCollection services = new ServiceCollection();
        
            // containerBuilder.RegisterType<Role>();
            services.AddOptions();

            var ServiceProvider = services.BuildServiceProvider();

            //  containerBuilder.RegisterModule(new AutofacModuleRegister());
            // containerBuilder.RegisterType<BaseRepository<Role>>().As<IBaseRepository<Role>>();

            containerBuilder.RegisterType<B>();
            containerBuilder.RegisterType <A>().PropertiesAutowired();
            containerBuilder.Populate(services);
            var build = containerBuilder.Build();


          //  var IBaseRepository = build.Resolve<IBaseRepository<Role>>();
            try
            {
                var service =
                build.ComponentRegistry.Registrations.SelectMany(x => x.Services)
                    .OfType<IServiceWithType>()
                    .Select(x => x.ServiceType);
                  var IBaseServices = build.Resolve<A>();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public class A
        {
            public string Name = "A";
            public B b { get; set; }
        }
        public class B
        {
            public string Name = "B";
        }
    }
}

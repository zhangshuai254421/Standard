using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SocketModule;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using JM.LogModule.IOC;
using System.Reflection;
using System.Threading;

namespace SocketModuleUnit
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {

            ContainerBuilder containerBuilder = new ContainerBuilder();

            var config = new ConfigurationBuilder().AddJsonFile("SocketClientOptions.json", true, true).Build();//读取bin目录下的json文件

            var ee = config.GetSection("SocketClientOptionsItem:SocketClient01").Get<SocketClientOptions>();
            ServiceCollection services = new ServiceCollection();


           

            containerBuilder.Populate(services);
            services.AddOptions();        
            

            services.Configure<SocketClientOptions>(SocketClientOptions.SocketClient01, config.GetSection("SocketClientOptionsItem:SocketClient01"));//将json的参数绑定到选型的对象

            services.PostConfigure<SocketClientOptions>("Key", p =>
            {
                var s = p;
            });
            
            var ServiceProvider = services.BuildServiceProvider();
            SocketClientOptions socketClientOptions = ServiceProvider.GetService<IOptionsMonitor<SocketClientOptions>>().Get(SocketClientOptions.SocketClient01);
        
            var currentvalue = socketClientOptions.IpAddress;
      
            //socketClientOptions.IpPort=123;
            containerBuilder.Register(c => new Log("JMModifyLogger") { Name = "测试" }).Keyed<Log>("Modify");
            containerBuilder.Register(c => new Log("JMGlobalLogger") { Name = "测试" }).Keyed<Log>("Global");

            containerBuilder.Register(c => new SocketClient(socketClientOptions,c.ResolveKeyed<Log>("Modify")));
             var build= containerBuilder.Build();
            var SocketClient = build.Resolve<SocketClient>();
            SocketClient.Initialize();

            ServiceProvider.GetService<IPostConfigureOptions<SocketClientOptions>>().PostConfigure("Key", socketClientOptions);
            while (true)
            {
                Thread.Sleep(2000);
            }

        }
        public class Person
        {
            public SocketClientOptions optionsMonitor1 { get; set; }
            public string Name { get; set; }
            public Person(SocketClientOptions optionsMonitor)
            {
                optionsMonitor1=optionsMonitor;
            }
        }
    }
}

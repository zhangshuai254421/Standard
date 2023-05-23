using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace JM.LogModule.IOC.Unit
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var builder = new ContainerBuilder();

            // build.RegisterType<log>().As<ILog>().UsingConstructor(typeof(string), "JMModifyLogger");

          
           builder.Register(c => new log("JMModifyLogger") { Name="测试"});
           //     builder.RegisterType<log>()
           //.As<ILog>()
           //.WithParameter("LoggerName", "JMModifyLogger");
            var container = builder.Build();
            var log=container.Resolve<log>();
            log.Debug("测试写入");
            string str = "";
        }
    }
}

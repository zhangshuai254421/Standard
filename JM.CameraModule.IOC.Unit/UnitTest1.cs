using Autofac;
using Autofac.Extensions.DependencyInjection;
using JM.LogModule.IOC;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Microsoft.Extensions.Configuration;
using log4net.Core;
using log4net.Repository.Hierarchy;
using Microsoft.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using log4net;

namespace JM.CameraModule.IOC.Unit
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            ContainerBuilder containerBuilder = new ContainerBuilder();

            
            var config = new ConfigurationBuilder().AddJsonFile("HKAreaCameraOptions.json", true, true).Build();//读取bin目录下的json文件
            

            var ee = config.GetSection("HKAreaCameraOptionsItem:Camera01").Get<HKAreaCameraOptions>();
            
            ServiceCollection services = new ServiceCollection();

           // LogManager.GetLogger()
            containerBuilder.Populate(services);

            services.AddOptions();

            services.Configure<HKAreaCameraOptions>(HKAreaCameraOptions.Camera01, config.GetSection("HKAreaCameraOptionsItem:Camera01"));//将json的参数绑定到选型的对象

      
            services.PostConfigure<HKAreaCameraOptions>(HKAreaCameraOptions.Camera01, p =>
            {
                var s = p;
            });

            //services.PostConfigureAll<HKAreaCameraOptions>( p =>
            //{
            //    var s = p;
            //});

            var ServiceProvider = services.BuildServiceProvider();
            HKAreaCameraOptions socketClientOptions = ServiceProvider.GetService<IOptionsMonitor<HKAreaCameraOptions>>().Get(HKAreaCameraOptions.Camera01);


            containerBuilder.Register(c => new Log("JMModifyLogger") { Name = "测试" }).Keyed<Log>("Modify");
            containerBuilder.Register(c => new Log("JMGlobalLogger") { Name = "测试" }).Keyed<Log>("Global");

            containerBuilder.Register(c => new HKAreaCamera(socketClientOptions, c.ResolveKeyed<Log>("Modify")));
            var build = containerBuilder.Build();
            var HKAreaCamera = build.Resolve<HKAreaCamera>();
            HKAreaCamera.OpenCamera();
            HKAreaCamera.HKAreaCameraOptions.IP = "111";
            ServiceProvider.GetService<IPostConfigureOptions<HKAreaCameraOptions>>().PostConfigure(HKAreaCameraOptions.Camera01, socketClientOptions);
        }

       
        public enum ProgramNameEnum
        {
            Station01_行列判断 = 0,
            Station01_双相机抓取角度纠偏 = 1,
            Station01_双相机抓取平移纠偏 = 2
        }

        [TestMethod]
        public void TestMethod2()
        {
  

            var ss2=(int)ProgramNameEnum.Station01_双相机抓取角度纠偏;
           string ss=ProgramNameEnum.Station01_双相机抓取角度纠偏.ToString();
        }
    }
}

using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using JM.DataBase.Unit;
using JM.IServices;
using JM.IServices.Base;
using JM.LogModule.IOC;
using JM.Model.Models;
using JM.Repository;
using JM.Services;
using JM.Services.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text.Json;

namespace JM.DataBase.Unit
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {

            ContainerBuilder containerBuilder = new ContainerBuilder();


            ServiceCollection services = new ServiceCollection();

            ConnectionConfig connectionConfig = new ConnectionConfig()
            {
                ConnectionString = @"DataSource=E:\04_开发项目\JM_Simple\demo\bin\Debug\Data\test.db",
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = true,
            };

            SqlSugarScope sqlSugarScope = new SqlSugarScope(connectionConfig);
            try
            {
                sqlSugarScope.DbMaintenance.CreateDatabase();
            }
            catch (Exception ex)
            {

                throw;
            }


            services.AddSingleton<ISqlSugarClient>(o =>
            {


                return new SqlSugarScope(connectionConfig);
            });


            // LogManager.GetLogger()
            containerBuilder.Populate(services);

            services.AddOptions();




            //services.PostConfigureAll<HKAreaCameraOptions>( p =>
            //{
            //    var s = p;
            //});

            var ServiceProvider = services.BuildServiceProvider();

            var build = containerBuilder.Build();

            var db = ServiceProvider.GetService<ISqlSugarClient>();
            var result = db.Queryable<User>().ToList();


            //If no exist create datebase 
            db.DbMaintenance.CreateDatabase();

            //Use db query
            var dt = db.Ado.GetDataTable("select 1");

            //Create tables
            db.CodeFirst.InitTables(typeof(OrderItem), typeof(Order));
            var id = db.Insertable(new Order() { Name = "order1", CustomId = 1, Price = 0, CreateTime = DateTime.Now }).ExecuteReturnIdentity();
        }


        [TestMethod]
        public  void TestMethod2()
        {
            ContainerBuilder containerBuilder = new ContainerBuilder();
            ServiceCollection services = new ServiceCollection();
            ConnectionConfig connectionConfig = new ConnectionConfig()
            {
                ConnectionString = @"DataSource=E:\04_开发项目\JM_Simple\demo\bin\Debug\Data\test.db",
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = true,
            };
            services.AddSingleton<ISqlSugarClient>(o =>
            {
                return new SqlSugarScope(connectionConfig);
            });
            // containerBuilder.RegisterType<Role>();
            services.AddOptions();

            var ServiceProvider = services.BuildServiceProvider();

            containerBuilder.RegisterModule(new AutofacModuleRegister());
           // containerBuilder.RegisterType<BaseRepository<Role>>().As<IBaseRepository<Role>>();


            containerBuilder.Populate(services);
            var build = containerBuilder.Build();
          

            var IBaseRepository = build.Resolve<IBaseRepository<Role>>();
            try
            {
                var service =
                build.ComponentRegistry.Registrations.SelectMany(x => x.Services)
                    .OfType<IServiceWithType>()
                    .Select(x => x.ServiceType);
                var IBaseServices = build.Resolve<IBaseServices<Role>>();
                
                var IRoleServices = build.Resolve<IRoleServices>();

                var listresult = ( IRoleServices.Query(p=>p.Id==10)).Result.FirstOrDefault();

                listresult.Name = "修改测试";
                  IRoleServices.Update(listresult);


            }
            catch (Exception ex)
            {

                throw;
            }


            var serviceRole = build.Resolve<IRoleServices>();
        }

        public class AutofacModuleRegister : Autofac.Module
        {
            protected override void Load(ContainerBuilder builder)
            {

                builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IBaseRepository<>)).InstancePerDependency();//注册仓储
                builder.RegisterGeneric(typeof(BaseServices<>)).As(typeof(IBaseServices<>)).InstancePerDependency();//注册服务

                builder.RegisterType<RoleServices>().As<BaseServices<Role>>().PropertiesAutowired();
                builder.RegisterType<RoleServices>().As<IRoleServices>().PropertiesAutowired();

                // 获取 Service.dll 程序集服务，并注册
                var assemblysServices = Assembly.LoadFrom("E:\\04_开发项目\\07_标准Dll\\Standard\\JM.Services\\bin\\Debug\\JM.Services.dll");
                //builder.RegisterAssemblyTypes(assemblysServices).Where(
                //        p=>
                //        p.Name.EndsWith("Services")
                //    )
                //    .AsImplementedInterfaces()
                //    .InstancePerDependency()
                //    .PropertiesAutowired();
                //引用Autofac.Extras.DynamicProxy;

                // 获取 Repository.dll 程序集服务，并注册
                var assemblysRepository = Assembly.LoadFrom("E:\\04_开发项目\\07_标准Dll\\Standard\\JM.Repository\\bin\\Debug\\JM.Repository.dll");
                //builder.RegisterAssemblyTypes(assemblysRepository)
                //    .AsImplementedInterfaces()
                //    .InstancePerDependency()
                //    .PropertiesAutowired();
            }
        }

        #region Json
        [TestMethod]
        public void TestMethod3()
        {
            JsonPoleData data = new JsonPoleData()
            {
                Mark1 = new PolePoint() { X = 1, Y = 2 },
                Mark2 = new PolePoint() { Y = 3, X = 4 },
                Mark3 = new PolePoint() { Y = 3, X = 4 },
                Mark4 = new PolePoint() { Y = 3, X = 4 },
                polePoints = new List<PolePoint>
                {
                    new PolePoint() { X=1, Y=2 },
                    new PolePoint(){ X=3, Y=4 },
                      new PolePoint(){ X=3, Y=4 },
                        new PolePoint(){ X=3, Y=4 },
                          new PolePoint(){ X=3, Y=4 },
                            new PolePoint(){ X=3, Y=4 },
                              new PolePoint(){ X=3, Y=4 },
                                new PolePoint(){ X=3, Y=4 },
                }
            };

            string jsonString = JsonSerializer.Serialize(data);
            var ss = JsonSerializer.Deserialize<JsonPoleData>(jsonString);

           
        }
    }
    public class JsonPoleData
    {
        public  string SN { get;set; }
        public PolePoint Mark1 { get; set; }
        public PolePoint Mark2 { get; set; }

        public PolePoint Mark3 { get; set; }

        public PolePoint Mark4 { get; set; }
        public List<PolePoint> polePoints { get; set; }
    }

    public class PolePoint
    {
        public double X { get; set; }
        public double Y { get; set; }

        public double Thi { get; set; } = 0;
    }
    #endregion


    [SqlSugar.SugarTable("OrderDetail")]
    public class OrderItem
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ItemId { get; set; }
        public int OrderId { get; set; }
        public decimal? Price { get; set; }
        [SqlSugar.SugarColumn(IsNullable = true)]
        public DateTime? CreateTime { get; set; }
    }




    public class Order
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        public string Name { get; set; }
        public decimal Price { get; set; }
        [SugarColumn(IsNullable = true)]
        public DateTime CreateTime { get; set; }
        [SugarColumn(IsNullable = true)]
        public int CustomId { get; set; }
        [SugarColumn(IsIgnore = true)]
        public List<OrderItem> Items { get; set; }
    }
    public class User
    {

        //登入ID
        public int LoginID { get; set; }
        //登入名称
        public string LoginName { get; set; }
        //登入密码
        public string LoginPWD { get; set; }
        //登入权限
        public int Role { get; set; }
    }
}


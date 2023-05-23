using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using JM.LogModule;
using LogModule;

namespace JM.LogModule.Unit
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Log.RegisterLog();
            try
            {
                Log.Warn("测试文件");
            }
            catch (Exception ex)
            {

                var ss = ex;
            }
          
         
          
            string str = "测试文件";
        }
    }
}

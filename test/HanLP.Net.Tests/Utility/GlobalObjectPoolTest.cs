using System;
using System.Collections.Generic;
using System.Text;
using HanLP.Net.Utility;
using Xunit;

namespace HanLP.Net.Tests.Utility
{
    public class GlobalObjectPoolTest
    {

        [Fact]
        public void AddObjectTest() {
            var obj = new object();
            var addObj = GlobalObjectPool.Add("testObj", obj);

            Assert.NotNull(addObj);
        }

        [Fact]
        public void GetObjectTest() {
            var obj = new List<string> { "1", "2" };
            GlobalObjectPool.Add("testObj", obj);

            var obj2 = GlobalObjectPool.Get<object>("testObj");

            Assert.Same(obj, obj2);
        }
    }
}

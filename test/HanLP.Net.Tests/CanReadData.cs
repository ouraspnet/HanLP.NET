using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace HanLP.Net.Tests
{
    public class CanReadData : TestBase
    {

        [Fact]
        public void CanReadDataDirectory() {
            Assert.NotNull(Config);
            Assert.Equal(Path.Combine(Config.DataRootPath, "data/dictionary/CoreNatureDictionary.txt"),
                Config.FileConfig.CoreDictionaryPath);
        }
    }
}

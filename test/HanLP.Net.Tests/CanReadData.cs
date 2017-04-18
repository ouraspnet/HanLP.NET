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
    public class CanReadData
    {

        [Fact]
        public void CanReadDataDirectory() {

            var loggerFactory = new LoggerFactory().AddConsole();
            var configRoot = new ConfigurationBuilder()
                .SetBasePath(Path.GetFullPath("../../../../HanLP.Net.Tests"))
                .AddJsonFile("appsettings.json").Build();

            var config = new HanLPConfiguration(configRoot, loggerFactory);

            Assert.NotNull(config);
            Assert.NotNull(config.FileConfig);
            Assert.Equal(Path.Combine(config.DataRootPath, "data/dictionary/CoreNatureDictionary.txt"), config.FileConfig.CoreDictionaryPath);
        }
    }
}

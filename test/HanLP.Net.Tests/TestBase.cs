using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HanLP.Net.Tests
{
    public class TestBase
    {
        protected HanLPConfiguration Config { get; set; }

        public TestBase() {

            var loggerFactory = new LoggerFactory().AddConsole();
            var configRoot = new ConfigurationBuilder()
                .SetBasePath(Path.GetFullPath("../../../../HanLP.Net.Tests"))
                .AddJsonFile("appsettings.json").Build();

            var config = new HanLPConfiguration(configRoot, loggerFactory);
            Config = config;
        }
    }
}

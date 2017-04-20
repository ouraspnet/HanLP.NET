using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HanLP.Net.Collection.Trie;
using HanLP.Net.Utility;
using Xunit;

namespace HanLP.Net.Tests.Collection
{
    public class DoubleArrayTrieTest : TestBase
    {
        string _testFile = "data/test/buildtest" + Predefine.TRIE_EXT;

        Dictionary<string, string> _mockData = new Dictionary<string, string> {
                    { "测试key","测试value1" },
                    { "测试key3","测试value2" },
                    { "测试key4","测试value3" },
        };


        [Fact]
        public void BuildTest() {
            DoubleArrayTrie<string> trie = new DoubleArrayTrie<string>();

            var errorCount = trie.Build(_mockData.Keys.ToList(),_mockData.Values.ToList());

            Assert.Equal(errorCount, 0);

            trie.Save(Path.Combine(Config.DataRootPath, _testFile));
        }

        [Fact]
        public void LoadTest() {
            var path = Path.Combine(Config.DataRootPath, _testFile);
            if (!File.Exists(path)) {
                BuildTest();
            }

            DoubleArrayTrie<string> trie = new DoubleArrayTrie<string>();

            trie.Load(path);

            Assert.Equal(trie.Size, 3);
        }


        [Fact]
        public void BuildTest2() {
            var trie = new DoubleArrayTrieBuilder<string>(System.Threading.Thread.CurrentThread.ManagedThreadId);

            var res = trie.Build(_mockData);

            Assert.True(res);

            trie.Save(Path.Combine(Config.DataRootPath, _testFile));
        }

        [Fact]
        public void LoadTest2() {
            var path = Path.Combine(Config.DataRootPath, _testFile);
            if (!File.Exists(path)) {
                BuildTest2();
            }
             
            var trie = new DoubleArrayTrieBuilder<string>(System.Threading.Thread.CurrentThread.ManagedThreadId);

            var search = new DoubleArrayTrieSearch<string>();

            search.Load(path, _mockData.Values.ToList());

            var res = search.Get("测试key3");

            Assert.Equal(res, "测试value2");
        }




    }
}

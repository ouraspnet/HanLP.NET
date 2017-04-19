using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;

namespace HanLP.Net.Utility
{
    public class GlobalObjectPool
    {
        private static ConcurrentDictionary<string, object> pool = 
            new ConcurrentDictionary<string, object>();

        public static T Get<T>(string objId) {
            if (!pool.ContainsKey(objId)) {
                return default(T);
            }
            return (T)pool[objId];
        }

        public static T Add<T>(string objId, T value) {
            var newValue = pool.AddOrUpdate(objId, value, (key, oldValue) => { return value; });
            return newValue == null ? default(T) : (T)newValue;
        }

        public static void Clear() {
            pool.Clear();
        }
    }
}

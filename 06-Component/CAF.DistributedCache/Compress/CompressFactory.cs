using BSF.BaseService.DistributedCache.SystemRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BSF.BaseService.DistributedCache.Compress
{
    public static class CompressFactory
    {
        private static ICompressProvider GetProvider(string compresstype)
        {
            if (string.Equals("gzip", compresstype, StringComparison.CurrentCultureIgnoreCase))
            {
                return new GZipCompressProvider();
            }
            throw new DistributedCacheException("找不到相应的字符串压缩类型:" + compresstype);
        }
        public static string CompressString(string compresstype,string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            var provider = GetProvider(compresstype);
            return provider.CompressString(text);
        }

        public static string DecompressString(string compresstype, string compressedText)
        {
            if (string.IsNullOrEmpty(compressedText))
                return compressedText;
            var provider = GetProvider(compresstype);
            return provider.DecompressString(compressedText);
        }

    }
}

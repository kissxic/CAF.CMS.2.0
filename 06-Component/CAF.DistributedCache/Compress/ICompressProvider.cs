using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BSF.BaseService.DistributedCache.Compress
{
    /// <summary>
    /// 字符串压缩
    /// </summary>
    public interface ICompressProvider
    {
         string CompressString(string text);

         string DecompressString(string compressedText);
    }
}

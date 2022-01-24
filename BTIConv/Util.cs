using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTIConv
{
    internal static class Util
    {
        public static string NameWithoutExt(this FileInfo file) => Path.GetFileNameWithoutExtension(file.Name);
    }
}

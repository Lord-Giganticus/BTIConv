namespace BTIConv
{
    internal static class Util
    {
        public static string NameWithoutExt(this FileInfo file) => Path.GetFileNameWithoutExtension(file.Name);
    }
}

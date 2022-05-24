global using BTIConv;
global using SuperBMDLib.Materials;
global using GameFormatReader.Common;
global using SixLabors.ImageSharp;
global using SixLabors.ImageSharp.PixelFormats;
global using static System.Console;

string[] FileArgs = args.Where(x => new FileInfo(x).Exists).ToArray();

if (FileArgs.Length <= 0)
{
    Error.WriteLine("There are no files specified.");
    return;
}
    
string[] PngArgs = args.Where(x => new FileInfo(x).Extension is ".png").ToArray();

string[] BtiArgs = args.Where(x => new FileInfo(x).Extension is ".bti").ToArray();

if (PngArgs.Length > 0)
{
    for (int i = 0; i < PngArgs.Length; i++)
    {
        FileInfo file = new(PngArgs[i]);
        using FileStream BtiStream = new($"{file.Directory?.FullName}\\{file.NameWithoutExt()}.bti", FileMode.Create);
        BinaryTextureImage bti = Methods.FromImage(file);
        var buf = Methods.ToBytes(bti);
        BtiStream.Write(buf);
    }
}

if (BtiArgs.Length > 0)
{
    for (int i = 0; i < BtiArgs.Length; i++)
    {
        FileInfo file = new(BtiArgs[i]);
        BinaryTextureImage image = Methods.LoadBti(file, Endian.Big);
        using Image<Bgra32> res = Methods.GetImage(image);
        res.Save($"{file.Directory?.FullName}\\{file.NameWithoutExt()}.png");
    }
}

return;
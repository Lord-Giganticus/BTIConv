global using BTIConv;
global using SuperBMDLib.Materials;
global using GameFormatReader.Common;
global using SixLabors.ImageSharp;
global using SixLabors.ImageSharp.PixelFormats;
global using static System.Console;
global using Newtonsoft.Json;
global using Newtonsoft.Json.Linq;

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
        JObject obj = new();
        if (File.Exists($"{file.Directory?.FullName}\\{file.NameWithoutExt()}.json"))
            obj = JObject.Parse(File.ReadAllText($"{file.Directory?.FullName}\\{file.NameWithoutExt()}.json"));
        using FileStream BtiStream = new($"{file.Directory?.FullName}\\{file.NameWithoutExt()}.bti", FileMode.Create);
        BinaryTextureImage bti = Methods.FromImage(file, obj);
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
        string json = JsonConvert.SerializeObject(image, Formatting.Indented);
        using Image<Bgra32> res = Methods.GetImage(image);
        res.Save($"{file.Directory?.FullName}\\{file.NameWithoutExt()}.png");
        File.WriteAllText($"{file.Directory?.FullName}\\{file.NameWithoutExt()}.json", json);
    }
}

return;
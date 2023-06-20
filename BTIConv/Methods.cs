namespace BTIConv;

public static class Methods
{
    public static BinaryTextureImage LoadBti(FileInfo file, Endian order)
    {
        if (file.Extension is not ".bti")
            throw new Exception("Extension must be bti.");
        using FileStream stream = file.OpenRead();
        using EndianBinaryReader reader = new(stream, order);
        BinaryTextureImage image = new(file.Name);
        image.Load(reader, 0);
        return image;
    }

    public static BinaryTextureImage FromImage(FileInfo file, JObject obj)
    {
        if (file.Extension is ".bti")
            throw new Exception("Extension must not be bti.");
        byte[] buf = File.ReadAllBytes(file.FullName);
        using Image<Bgra32> image = Image.Load<Bgra32>(buf);
        BinaryTextureImage res = new(file.Name);
        res.Load(image);
        PopulateHeader(ref res, obj);
        return res;
    }

    public static Image<Bgra32> GetImage(BinaryTextureImage bti)
    {
        return bti.CreateImage();
    }

    public static byte[] ToBytes(BinaryTextureImage bti)
    {
        using MemoryStream stream = new();
        using EndianBinaryWriter writer = new(stream, Endian.Big);
        bti.WriteHeader(writer);
        var tup = bti.EncodeData();
        writer.Write(tup.Item1);
        writer.Seek(12, SeekOrigin.Begin);
        writer.Write(32);
        writer.Seek(28, SeekOrigin.Begin);
        writer.Write(32 + bti.PaletteCount);
        SuperBMDLib.Util.StreamUtility.PadStreamWithByte(writer, 32, 0x40);
        return stream.ToArray();
    }

    static void PopulateHeader(ref BinaryTextureImage bti, JObject obj)
    {
        var props = bti.GetType().GetProperties();
        var fields = bti.GetType().GetFields();
        var vprops = props.Where(x => obj.ContainsKey(x.Name)).ToArray();
        var vfields = fields.Where(x => obj.ContainsKey(x.Name)).ToArray();
        foreach (var prop in vprops)
        {
            if (obj[prop.Name] is JToken tok)
            {
                var value = tok.ToObject(prop.PropertyType);
                prop.SetValue(bti, value);
            }
        }
        foreach (var field in vfields)
        {
            if (obj[field.Name] is JToken tok)
            {
                var value = tok.ToObject(field.FieldType);
                field.SetValue(bti, value);
            }
        }
    }
}
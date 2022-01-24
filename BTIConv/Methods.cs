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

    public static BinaryTextureImage FromImage(FileInfo file)
    {
        if (file.Extension is ".bti")
            throw new Exception("Extension must not be bti.");
        byte[] buf = File.ReadAllBytes(file.FullName);
        using Image<Bgra32> image = Image.Load<Bgra32>(buf);
        BinaryTextureImage res = new(file.Name);
        res.Load(image);
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
        return stream.ToArray();
    }
}
namespace TrackYourLife.SharedLib.Application.Extensions;

public static class ByteArrayExtensions
{
    public static Stream ToStream(this byte[] byteArray)
    {
        return new MemoryStream(byteArray);
    }
}

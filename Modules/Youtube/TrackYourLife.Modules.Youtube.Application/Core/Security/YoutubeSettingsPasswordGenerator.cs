using System.Security.Cryptography;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Core.Security;

internal static class YoutubeSettingsPasswordGenerator
{
    private const string Lower = "abcdefghijklmnopqrstuvwxyz";
    private const string Upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Digits = "0123456789";
    private const string Special = "@#$%^&+=!.";
    private const int DefaultLength = 16;

    public static Result<string> Generate(int length = DefaultLength)
    {
        var size = Math.Max(length, YoutubeSettingsPassword.MinLength);
        var chars = new char[size];
        chars[0] = RandomChar(Lower);
        chars[1] = RandomChar(Upper);
        chars[2] = RandomChar(Digits);
        chars[3] = RandomChar(Special);

        var all = Lower + Upper + Digits + Special;
        for (var i = 4; i < size; i++)
        {
            chars[i] = RandomChar(all);
        }

        Shuffle(chars);

        var password = new string(chars);
        var validation = YoutubeSettingsPassword.Create(password);
        return validation.IsFailure
            ? Result.Failure<string>(validation.Error)
            : Result.Success(password);
    }

    private static char RandomChar(string charset)
    {
        var index = RandomNumberGenerator.GetInt32(charset.Length);
        return charset[index];
    }

    private static void Shuffle(char[] items)
    {
        for (var i = items.Length - 1; i > 0; i--)
        {
            var j = RandomNumberGenerator.GetInt32(i + 1);
            (items[i], items[j]) = (items[j], items[i]);
        }
    }
}

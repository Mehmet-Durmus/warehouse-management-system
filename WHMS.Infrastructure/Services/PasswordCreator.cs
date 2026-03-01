using System.Security.Cryptography;
using WHMS.Application.Abstractions.Infrastructure;

namespace WHMS.Infrastructure.Services;

public class PasswordCreator : IPasswordCreator
{
    private const string Lowercase = "abcdefghijklmnopqrstuvwxyz";
    private const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Digits = "0123456789";

    private static readonly string AllChars = Lowercase + Uppercase + Digits;

    public async Task<string> CreateTempPassword()
    {
        var password = new char[6];

        password[0] = Lowercase[RandomNumberGenerator.GetInt32(Lowercase.Length)];
        password[1] = Uppercase[RandomNumberGenerator.GetInt32(Uppercase.Length)];
        password[2] = Digits[RandomNumberGenerator.GetInt32(Digits.Length)];

        for (int i = 3; i < 6; i++)
        {
            password[i] = AllChars[RandomNumberGenerator.GetInt32(AllChars.Length)];
        }

        return new string(password.OrderBy(_ => RandomNumberGenerator.GetInt32(int.MaxValue)).ToArray());
    }
}
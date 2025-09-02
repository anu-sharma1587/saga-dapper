using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace HotelManagement.BuildingBlocks.Security.Hashing;

public static class PasswordHasher
{
    private const int IterationCount = 10000;
    private const int SubkeyLength = 256 / 8; // 256 bits
    private const int SaltSize = 128 / 8; // 128 bits

    public static string HashPassword(string password)
    {
        // Generate a random salt
        byte[] salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Hash the password with PBKDF2
        byte[] hash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: IterationCount,
            numBytesRequested: SubkeyLength);

        // Combine salt and hash
        byte[] hashBytes = new byte[SaltSize + SubkeyLength];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, SubkeyLength);

        return Convert.ToBase64String(hashBytes);
    }

    public static bool VerifyPassword(string hashedPassword, string password)
    {
        byte[] hashBytes = Convert.FromBase64String(hashedPassword);

        // Extract salt and hash from combined bytes
        byte[] salt = new byte[SaltSize];
        byte[] hash = new byte[SubkeyLength];
        Array.Copy(hashBytes, 0, salt, 0, SaltSize);
        Array.Copy(hashBytes, SaltSize, hash, 0, SubkeyLength);

        // Hash the input password with the same salt
        byte[] computedHash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: IterationCount,
            numBytesRequested: SubkeyLength);

        // Compare the computed hash with the stored hash
        return CryptographicOperations.FixedTimeEquals(hash, computedHash);
    }
}

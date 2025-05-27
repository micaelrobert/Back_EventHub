using System.Security.Cryptography;
using System.Text;
using GestaoEventos.API.Helpers;

namespace GestaoEventos.API.Helpers // Certifique-se de que o namespace reflete a pasta
{
    public static class PasswordHasher
    {
        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("A senha não pode ser vazia ou nula.", nameof(password));

            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("A senha não pode ser vazia ou nula.", nameof(password));
            if (storedHash.Length != 64) // HMACSHA512 produz um hash de 64 bytes
                throw new ArgumentException("Comprimento inválido do hash da senha.", nameof(storedHash));
            if (storedSalt.Length != 128) // HMACSHA512 usa uma chave de 128 bytes como salt
                throw new ArgumentException("Comprimento inválido do salt da senha.", nameof(storedSalt));

            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(storedHash);
            }
        }
    }
}
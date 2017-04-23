using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace IdentityServerTest.Identity
{
    public interface ICryptography
    {
        bool Verify(string hashedPassword, string password);
        string Hash(string password);
    }

    public class Cryptography : ICryptography
    {
        private int _saltSize = 32;
        private int _keyLength = 64;
        private int _iterations = 10000;
        private int _hashSize;

        public Cryptography()
        {
            _hashSize = _saltSize + _keyLength + sizeof(int);
        }

        public string Hash(string password)
        {
            // PBKDF2: Password-Based Key Derivation Functionality
            using (var PBKDF2 = new Rfc2898DeriveBytes(password, _saltSize, _iterations))
            {
                var salt = PBKDF2.Salt;
                var keyBytes = PBKDF2.GetBytes(_keyLength);
                var iterationBytes =
                    BitConverter.IsLittleEndian
                    ? BitConverter.GetBytes(_iterations)
                    : BitConverter.GetBytes(_iterations)
                        .Reverse()
                        .ToArray();

                var hashedPassword = new byte[_hashSize];

                Buffer.BlockCopy(salt, 0, hashedPassword, 0, _saltSize);
                Buffer.BlockCopy(keyBytes, 0, hashedPassword, _saltSize, _keyLength);
                Buffer.BlockCopy(iterationBytes, 0, hashedPassword, _saltSize + _keyLength, sizeof(int));

                return Convert.ToBase64String(hashedPassword);
            }
        }

        public bool Verify(string hashedPassword, string password)
        {
            var hashedPasswordBytes = Convert.FromBase64String(hashedPassword);

            if (hashedPasswordBytes.Length != _hashSize)
            {
                return false;
            }
            else
            {
                var salt = new byte[_saltSize];
                var keyBytes = new byte[_keyLength];
                var iterationBytes = new byte[sizeof(int)];

                Buffer.BlockCopy(hashedPasswordBytes, 0, salt, 0, _saltSize);
                Buffer.BlockCopy(hashedPasswordBytes, _saltSize, keyBytes, 0, _keyLength);
                Buffer.BlockCopy(hashedPasswordBytes, _saltSize + _keyLength, iterationBytes, 0, sizeof(int));

                var iterations = BitConverter.ToInt32(BitConverter.IsLittleEndian ? iterationBytes : iterationBytes.Reverse().ToArray(), 0);

                // PBKDF2: Password-Based Key Derivation Functionality
                using (var PBKDF2 = new Rfc2898DeriveBytes(password, salt, iterations))
                {
                    var challengeBytes = PBKDF2.GetBytes(_keyLength);

                    for (int i = 0; i < keyBytes.Length; i++)
                    {
                        if (keyBytes[i] != challengeBytes[i])
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }
    }
}

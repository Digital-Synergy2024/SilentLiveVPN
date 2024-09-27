namespace SilentLiveVPN.Tools
{
    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.Engines;
    using Org.BouncyCastle.Crypto.Generators;
    using Org.BouncyCastle.Crypto.Modes;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Security;
    using System.Text;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Org.BouncyCastle.Crypto.Paddings;

    /// <summary>
    /// Provides methods for simple encryption and decryption of data using AES encryption.
    /// This class utilizes the BouncyCastle library for cryptographic operations.
    /// </summary>
    public class SimpleEncryption
    {
        /// <summary>
        /// Encrypts the specified message using the provided password.
        /// This method generates a random salt, derives a key and initialization vector (IV)
        /// from the password and salt, and then encrypts the message using AES in CBC mode
        /// with PKCS7 padding.
        /// </summary>
        /// <param name="message">The plaintext message to be encrypted.</param>
        /// <param name="password">The password used for key derivation.</param>
        /// <returns>A byte array containing the encrypted message.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the message or password is null.</exception>
        /// <exception cref="CryptographicException">Thrown when encryption fails.</exception>
        /// 

        public static byte[] EncryptData(string message, string password)
        {
            // Step 1: Generate a random salt
            var salt = new byte[8];
            new SecureRandom().NextBytes(salt);

            // Step 2: Derive key and IV from the password and salt
            Pkcs5S2ParametersGenerator generator = new Pkcs5S2ParametersGenerator();
            generator.Init(PbeParametersGenerator.Pkcs5PasswordToBytes(password.ToCharArray()), salt, 1000);
            ParametersWithIV keyParam = (ParametersWithIV)generator.GenerateDerivedParameters("AES-256/CBC/PKCS7", 256, 128);

            // Step 3: Create AES cipher in CBC mode with PKCS7 padding
            var cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new AesEngine()));
            cipher.Init(true, keyParam);

            // Step 4: Convert message to byte array and encrypt
            byte[] inputBytes = Encoding.UTF8.GetBytes(message);
            byte[] outputBytes = new byte[cipher.GetOutputSize(inputBytes.Length)];
            int length = cipher.ProcessBytes(inputBytes, 0, inputBytes.Length, outputBytes, 0);
            length += cipher.DoFinal(outputBytes, length);

            // Combine salt and encrypted data for storage or transmission
            byte[] result = new byte[salt.Length + length];
            Array.Copy(salt, 0, result, 0, salt.Length);
            Array.Copy(outputBytes, 0, result, salt.Length, length);

            return result;
        }

        /// <summary>
        /// Demonstrates the encryption of a test message using a predefined password.
        /// This method is intended for testing purposes and outputs the original and
        /// encrypted messages to the console.
        /// </summary>
        public static void Bouncy()
        {
            string message = "Hello, this is a test message!";
            string password = "StrongPassword123";
            byte[] encryptedMessage = SimpleEncryption.EncryptData(message, password);
            Console.WriteLine("Original Message: " + message);
            Console.WriteLine("Encrypted Message: " + BitConverter.ToString(encryptedMessage));
        }
    }
    
}

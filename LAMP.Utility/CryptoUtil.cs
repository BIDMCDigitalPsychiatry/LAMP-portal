using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;
using System.Web.Security;

namespace LAMP.Utility
{
    public sealed class CryptoUtil
    {
        private static MachineKeySection machineKeyConfig = (MachineKeySection)ConfigurationManager.GetSection("system.web/machineKey");
        /// <summary>
        /// CryptoUtil
        /// </summary>
        static CryptoUtil()
        {
            string configKey = machineKeyConfig.DecryptionKey;
            if (configKey.Contains("AutoGenerate"))
            {
                throw new ConfigurationErrorsException(
                    "Explicit encryption key required.");
            }
        }

        static string key = machineKeyConfig.DecryptionKey;

        /// <summary>
        /// Encrypts the string provided using MD5 Encryption
        /// </summary>
        /// <param name="inputString">The string value to be encrypted</param>
        /// <returns>string: Encrypted string value</returns>
        public static string EncryptString(string originalText)
        {
            string retValue = string.Empty;
            try
            {
                ASCIIEncoding asciiEncoding = new ASCIIEncoding();
                // Converts the string to byte array using ASCII Encoder
                byte[] inputStringBytes = asciiEncoding.GetBytes(originalText);

                //Gets the hash byte array using MD5CryptoServiceProvider
                MD5CryptoServiceProvider md5CryproProvider = new MD5CryptoServiceProvider();
                byte[] hashedBytes = md5CryproProvider.ComputeHash(inputStringBytes);

                //Converting the hashed byte array to string  
                StringBuilder hashString = new StringBuilder();
                foreach (byte hashByte in hashedBytes)
                {
                    hashString.Append(hashByte.ToString("x2"));
                }

                retValue = hashString.ToString();
            }
            catch (Exception)
            {

            }
            return retValue;
        }
        /// <summary>
        /// EncryptStringWithKey
        /// </summary>
        /// <param name="originalText">The string value to be encrypted</param>
        /// <returns>string:  string value Encrypted with key</returns>
        public static string EncryptStringWithKey(string originalText)
        {
            // First we need to turn the input string into a byte array. 

            byte[] clearBytes =
              System.Text.Encoding.Unicode.GetBytes(originalText);

            byte[] encryptedData = Encrypt(clearBytes);

            // Now we need to turn the resulting byte array into a string. 
            // A common mistake would be to use an Encoding class for that.
            //It does not work because not all byte values can be
            // represented by characters. 
            // We are going to be using Base64 encoding that is designed
            //exactly for what we are trying to do. 
            return Convert.ToBase64String(encryptedData);

        }
        /// <summary>
        /// DecryptStringWithKey
        /// </summary>
        /// <param name="encryptedText">The string value to be decrypted</param>
        /// <returns>string:  string value with out encryption</returns>
        public static string DecryptStringWithKey(string encryptedText)
        {
            try
            {
                encryptedText = encryptedText.Replace(" ", "+");
                // First we need to turn the input string into a byte array. 
                // We presume that Base64 encoding was used 
                byte[] cipherBytes = Convert.FromBase64String(encryptedText);


                byte[] decryptedData = Decrypt(cipherBytes);

                // Now we need to turn the resulting byte array into a string. 
                // A common mistake would be to use an Encoding class for that.
                // It does not work 
                // because not all byte values can be represented by characters. 
                // We are going to be using Base64 encoding that is 
                // designed exactly for what we are trying to do. 
                return System.Text.Encoding.Unicode.GetString(decryptedData);
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return encryptedText;
            }
        }

        // Encrypt a byte array into a byte array using a key and an IV 
        /// <summary>
        /// Encrypt a byte array into a byte array using a key and an IV 
        /// </summary>
        /// <param name="inputBuffer">inputBuffer</param>
        /// <returns>outputBuffer</returns>
        private static byte[] Encrypt(byte[] inputBuffer)
        {
            SymmetricAlgorithm algorithm;
            byte[] outputBuffer;

            if (inputBuffer == null)
            {
                throw new ArgumentNullException("inputBuffer");
            }

            algorithm = GetCryptAlgorithm();

            using (var ms = new MemoryStream())
            {
                algorithm.GenerateIV();
                ms.Write(algorithm.IV, 0, algorithm.IV.Length);

                using (var cs = new CryptoStream(
                     ms,
                     algorithm.CreateEncryptor(),
                     CryptoStreamMode.Write))
                {
                    cs.Write(inputBuffer, 0, inputBuffer.Length);
                    cs.FlushFinalBlock();
                }

                outputBuffer = ms.ToArray();
            }

            return outputBuffer;
        }
        /// <summary>
        /// GetCryptAlgorithm
        /// </summary>
        /// <returns></returns>
        private static SymmetricAlgorithm GetCryptAlgorithm()
        {
            SymmetricAlgorithm algorithm;
            string algorithmName;

            algorithmName = machineKeyConfig.Decryption;
            if (algorithmName == "Auto")
            {
                throw new ConfigurationErrorsException(
                     "Explicit encryption key required.");
            }

            switch (algorithmName)
            {
                case "AES":
                    algorithm = new RijndaelManaged();
                    break;
                case "3DES":
                    algorithm = new TripleDESCryptoServiceProvider();
                    break;
                case "DES":
                    algorithm = new DESCryptoServiceProvider();
                    break;
                default:
                    throw new ConfigurationErrorsException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Unrecognized algorithm",
                            algorithmName));
            }

            algorithm.Key = HexStringToByteArray(key);

            return algorithm;
        }
        /// <summary>
        /// HexStringToByteArray
        /// </summary>
        /// <param name="str">str</param>
        /// <returns>byte array</returns>
        private static byte[] HexStringToByteArray(string str)
        {
            byte[] buffer;

            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            if (str.Length % 2 == 1)
            {
                str = '0' + str;
            }

            buffer = new byte[str.Length / 2];

            for (int i = 0; i < buffer.Length; ++i)
            {
                buffer[i] = byte.Parse(
                    str.Substring(i * 2, 2),
                    NumberStyles.HexNumber,
                    CultureInfo.InvariantCulture);
            }

            return buffer;
        }

        /// <summary>
        /// Decrypt a byte array into a byte array using a key and an IV 
        /// </summary>
        /// <param name="inputBuffer">encrypted byte array</param>
        /// <returns>decrypeted  byte array</returns>
        private static byte[] Decrypt(byte[] inputBuffer)
        {
            SymmetricAlgorithm algorithm;
            byte[] inputVectorBuffer, outputBuffer;

            algorithm = GetCryptAlgorithm();
            outputBuffer = null;

            try
            {

                inputVectorBuffer = new byte[algorithm.IV.Length];
                Array.Copy(
                     inputBuffer,
                     inputVectorBuffer,
                     inputVectorBuffer.Length);
                algorithm.IV = inputVectorBuffer;

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(
                        ms,
                        algorithm.CreateDecryptor(),
                        CryptoStreamMode.Write))
                    {
                        cs.Write(
                            inputBuffer,
                            inputVectorBuffer.Length,
                            inputBuffer.Length - inputVectorBuffer.Length);
                        cs.FlushFinalBlock();
                    }

                    outputBuffer = ms.ToArray();
                }
            }
            catch (FormatException e)
            {
                throw new CryptographicException(
                    "The string could not be decoded.", e);
            }

            return outputBuffer;
        }

        #region AES Encrytion Decryption

        private static AesManaged aes = null;

        /// <summary>
        /// Getting 32 byte key for AES
        /// </summary>
        /// <param name="credentialKey">value passed to get key</param>
        /// <returns></returns>
        private static byte[] GetKeyBytes(string credentialKey)
        {
            int keySizeInBytes = aes.KeySize / 8; //setting the keysize
            byte[] keyBytes = new byte[keySizeInBytes];//Getting bytes
            UTF8Encoding.UTF8.GetBytes(credentialKey, 0, Math.Min(credentialKey.Length, keySizeInBytes), keyBytes, 0);

            return keyBytes;
        }

        /// <summary>
        /// Getting  initialization vector for AES
        /// </summary>
        /// <param name="credentialIV">value passed to get initialization vector</param>
        /// <returns></returns>
        private static byte[] GetIVBytes(string credentialIV)
        {
            //setting the block size
            int blockSizeInBytes = aes.BlockSize / 8;
            byte[] ivBytes = new byte[blockSizeInBytes];
            UTF8Encoding.UTF8.GetBytes(credentialIV, 0, Math.Min(credentialIV.Length, blockSizeInBytes), ivBytes, 0);

            return ivBytes;
        }

        /// <summary>
        /// Getting encrypted string
        /// </summary>
        /// <param name="inputString">string to be encrypted</param>
        /// <param name="credential">key value to encrypt</param>
        /// <returns></returns>
        public static string EncryptAESToBase64(string inputString)
        {
            string credential = key; //key value to encrypt

            aes = new AesManaged();

            //setting key
            aes.Key = GetKeyBytes(credential);
            //setting intialization vector
            aes.IV = GetIVBytes(credential);
            //padding applied when the message data block is shorter than the full number of bytes needed for a cryptographic operation
            aes.Padding = PaddingMode.PKCS7;
            //Specifies the block cipher mode to use for encryption.
            aes.Mode = CipherMode.ECB;
            //Creates a memory stream 
            MemoryStream ms = new MemoryStream();
            //Defines a stream for cryptographic transformations.
            CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);  // Encrypting string using key, and IV.

            byte[] inputBytes = UTF8Encoding.UTF8.GetBytes(inputString);
            cs.Write(inputBytes, 0, inputBytes.Length);
            cs.Close();

            byte[] encryptedBytes = ms.ToArray();
            string encryptedTextInBase64 = Convert.ToBase64String(encryptedBytes);

            ms.Dispose();
            cs.Dispose();

            return encryptedTextInBase64;
        }

        /// <summary>
        /// Decrypt the picture bytes
        /// </summary>
        /// <param name="encryptedString">encryptedString</param>
        /// <returns>string</returns>
        public static string DecryptAESFromBase64(string encryptedString)
        {
            encryptedString = encryptedString.Replace(" ", "+");
            aes = new AesManaged();

            //Decrypt the picture bytes
            aes.Key = GetKeyBytes(key);
            aes.IV = GetIVBytes(key);
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.ECB;
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
            byte[] encryptedBytes = Convert.FromBase64String(encryptedString);
            cs.Write(encryptedBytes, 0, encryptedBytes.Length);
            cs.Dispose();
            cs.Close();
            byte[] decryptedBytes = ms.ToArray();

            return UTF8Encoding.UTF8.GetString(decryptedBytes);
        }

        #endregion

        /// <summary>
        /// GenerateToken
        /// </summary>
        /// <returns>token</returns>
        public static string GenerateToken()
        {
            RandomNumberGenerator rng = new RNGCryptoServiceProvider();
            byte[] tokenData = new byte[32];
            rng.GetBytes(tokenData);

            string token = Convert.ToBase64String(tokenData);
            return token;
        }
        /// <summary>
        /// random
        /// </summary>
        private static readonly Random random = new Random();

        /// <summary>
        /// GeneratePassword
        /// </summary>
        /// <param name="length">length</param>
        /// <param name="nonAlphaNumericChars">nonAlphaNumericChars</param>
        /// <returns>password</returns>
        public static string GeneratePassword(int length, int nonAlphaNumericChars)
        {
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            string allowedNonAlphaNum = "!@#$%^&*()_-+=[{]};:<>|./?";

            if (nonAlphaNumericChars > length || length <= 0 || nonAlphaNumericChars < 0)
                throw new ArgumentOutOfRangeException();

            char[] pass = new char[length];
            int[] pos = new int[length];
            int i = 0, j = 0, temp = 0;
            bool flag = false;

            //Random the position values of the pos array for the string Pass
            while (i < length - 1)
            {
                j = 0;
                flag = false;
                temp = random.Next(0, length);
                for (j = 0; j < length; j++)
                    if (temp == pos[j])
                    {
                        flag = true;
                        j = length;
                    }

                if (!flag)
                {
                    pos[i] = temp;
                    i++;
                }
            }

            //Random the AlphaNumericChars
            for (i = 0; i < length - nonAlphaNumericChars; i++)
                pass[i] = allowedChars[random.Next(0, allowedChars.Length)];

            //Random the NonAlphaNumericChars
            for (i = length - nonAlphaNumericChars; i < length; i++)
                pass[i] = allowedNonAlphaNum[random.Next(0, allowedNonAlphaNum.Length)];

            //Set the sorted array values by the pos array for the rigth posistion
            char[] sorted = new char[length];
            for (i = 0; i < length; i++)
                sorted[i] = pass[pos[i]];

            string Pass = new String(sorted);

            return Pass;
        }

        /// <summary>
        /// Creates a password hash
        /// </summary>
        /// <param name="password">Password</param>
        /// <param name="salt">Salt</param>
        /// <returns>Password hash</returns>
        public static string CreatePasswordHash(string password, string salt)
        {
            //MD5, SHA1
            string passwordFormat = "SHA1";
            return FormsAuthentication.HashPasswordForStoringInConfigFile(password + salt, passwordFormat);
        }

        /// <summary>
        /// Creates a salt
        /// </summary>
        /// <param name="size">A salt size</param>
        /// <returns>A salt</returns>
        public static string CreateSalt(int size)
        {
            var provider = new RNGCryptoServiceProvider();
            byte[] data = new byte[size];
            provider.GetBytes(data);
            return Convert.ToBase64String(data);
        }

        /// <summary>
        /// Create a numeric salt
        /// </summary>
        /// <param name="size">A salt size</param>
        /// <returns>A int Salt</returns>
        public static UInt32 CreateNumericSalt(int size)
        {
            try
            {
                var provider = new RNGCryptoServiceProvider();
                byte[] data = new byte[size];
                provider.GetBytes(data);
                return BitConverter.ToUInt32(data, 0);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// CreateHash
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string CreateHash(string s)
        {
            using (SHA1 cs = SHA1.Create())
            {
                StringBuilder sb = new StringBuilder();
                byte[] hash;

                hash = cs.ComputeHash(Encoding.UTF8.GetBytes(s));
                sb.Length = 0;
                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }
        /// <summary>
        /// EncryptSeed
        /// </summary>
        /// <param name="seed">seed</param>
        /// <returns>string</returns>
        public static string EncryptSeed(string seed)
        {
            Byte[] byteArray = System.Text.Encoding.Unicode.GetBytes(seed);
            var value = MachineKey.Protect(byteArray, "");
            return Convert.ToBase64String(value);

        }
        /// <summary>
        /// DecryptSeed
        /// </summary>
        /// <param name="seed">seed</param>
        /// <returns></returns>
        public static string DecryptSeed(string seed)
        {
            var value = MachineKey.Unprotect(Encoding.UTF8.GetBytes(seed), "");
            return Convert.ToBase64String(value);
        }
        
        
        public static string EncryptInfo(string toEncrypt)
        {
            try
            {
                if (toEncrypt != null && toEncrypt.Length > 0)
                {
                    byte[] keyArray = UTF8Encoding.UTF8.GetBytes("G120098HGFARTY866554HHFGFG09078J"); // 256-AES key   12345678901234567890123456789012            
                    byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
                    RijndaelManaged rDel = new RijndaelManaged();
                    rDel.Key = keyArray;
                    rDel.Mode = CipherMode.ECB; // http://msdn.microsoft.com/en-us/library/system.security.cryptography.ciphermode.aspx
                    rDel.Padding = PaddingMode.PKCS7; // better lang support
                    ICryptoTransform cTransform = rDel.CreateEncryptor();
                    byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                    return Convert.ToBase64String(resultArray, 0, resultArray.Length);
                }
                else
                    return toEncrypt;
            }
            catch (Exception ex)
            {
                return toEncrypt;
            }
        }

        public static string DecryptInfo(string toDecrypt)
        {
            try
            {
                if (toDecrypt != null && toDecrypt.Length > 0)
                {
                    byte[] keyArray = UTF8Encoding.UTF8.GetBytes("G120098HGFARTY866554HHFGFG09078J"); // AES-256 key            
                    byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);
                    RijndaelManaged rDel = new RijndaelManaged();
                    rDel.Key = keyArray;
                    rDel.Mode = CipherMode.ECB; // http://msdn.microsoft.com/en-us/library/system.security.cryptography.ciphermode.aspx
                    rDel.Padding = PaddingMode.PKCS7; // better lang support
                    ICryptoTransform cTransform = rDel.CreateDecryptor();
                    byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                    return UTF8Encoding.UTF8.GetString(resultArray);
                }
                else
                    return toDecrypt;
            }
            catch (Exception ex)
            {
                return toDecrypt;
            }
        }
        
    }

}

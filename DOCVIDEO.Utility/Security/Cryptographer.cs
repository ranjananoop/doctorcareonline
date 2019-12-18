
#region USING

using System;
using System.Security;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

#endregion

/*
	Secret key encryption algorithms: (DES, RC2, Rijndael, TripleDES)
	Private-key algorithms are relatively fast and can be used to encrypt and decrypt large streams of data. 
	Private-key algorithms are known as block ciphers because they encrypt data one block at a time. 
	A block cipher will encrypt the same input block into the same output block based on the algorithm and key. 
	If anything were known about the structure of the data, patterns could be detected and the key could possibly be reverse engineered. 
	To combat this, the classes in the .NET Framework use a process known as chaining where information from the previous block is used in encrypting the current block. 
	This helps prevent the key from being discovered. It requires an initialization vector (IV) be given to encrypt the first block of data.
	
	Public-key encryption algorithms: (DSA, RSA)
	Public-key encryption has a much larger keyspace, or range of possible values for the key, and is therefore less susceptible to exhaustive attacks that try every possible key. 
	A public key is easy to distribute because it does not have to be secured. Public-key algorithms can be used to create digital signatures to verify the identity of the sender of data. 
	However, public-key algorithms are extremely slow (compared to secret-key algorithms) and are not designed to encrypt large amounts of data. 
	Public-key algorithms are useful only for transferring very small amounts of data. Typically, public-key encryption is used to encrypt a key and IV to be used by a secret-key algorithm. 
	After the key and IV are transferred, then secret-key encryption is used for the remainder of the session. 
	
	Digital signature algorithms (Hash):
*/

namespace DOCVIDEO.Utility.Security
{
    #region ENUMS

    #region SymmetricAlgorithms
    /// <summary>
	/// Enumeration of Algorithms that can be used for Symmetric Encryption
	/// </summary>
	public enum SymmetricAlgorithms 
    {
	
		/// <summary>
		/// Data Encryption Standard (DES) algorithm encrypts and decrypts data in 64-bit blocks, 
		/// using a 64-bit key. Even though the key is 64-bit, the effective key strength is only 56-bits. 
		/// There are hardware devices advanced enough that they can search all possible DES keys in a reasonable amount of time. 
		/// This makes the DES algorithm breakable, and the algorithm is considered somewhat obsolete.
		/// </summary>
		DES,
		/// <summary>
		/// RC2 is a variable key-size block cipher. The key size can vary from 8-bit up to 64-bits for the key. 
		/// It was specifically designed as a more secure replacement to DES. The processing speed is two to three times faster than DES. 
		/// However, the RC2CryptoServiceProvider available in the .NET Framework is limited to 8 characters, or a 64-bit key. 
		/// The 8-character limitation makes it susceptible to the same brute force attack as DES.
		/// </summary>
		RC2,
		/// <summary>
		/// Rijndael algorithm, one of the Advanced Encryption Standard (AES) algorithms, was designed as a replacement for the DES algorithms. 
		/// The key strength is stronger than DES and was designed to out perform DES. The key can vary in length from 128, 192, to 256 bits in length.
		/// </summary>
		Rijndael,
		/// <summary>
		/// TripleDES algorithm uses three successive iterations of the DES algorithm. 
		/// The algorithm uses either two or three keys. Just as the DES algorithm, the key size is 64-bit per key with an effective key strength of 56-bit per key. 
		/// The TripleDES algorithm was designed to fix the shortcomings of the DES algorithm, but the three iterations result in a processing speed three times slower than DES alone.
		/// </summary>
		TripleDES,

	}
    #endregion

    #region HashAlgorithms
    /// <summary>
	/// Enumeration of Algorithms that can be used for Hashing
	/// </summary>
	public enum HashAlgorithms 
    {
		/// <summary>
		/// MD5 is intended for use with digital signature applications, which require that large files must be compressed 
		/// by a secure method before being encrypted with a secret key, under a public key cryptosystem.  
		/// The MD5 algorithm is an extension of MD4, which the critical review found to be fast, 
		/// but possibly not absolutely secure. In comparison, MD5 is not quite as fast as the MD4 algorithm, 
		/// but offers much more assurance of data security.
		/// </summary>
		MD5,

		/// <summary>
		/// The most commonly used function in the family, SHA-1, is employed in a large variety of popular security applications and protocols, including TLS, SSL, PGP, SSH, S/MIME, and IPSec. 
		/// SHA-1 is considered to be the successor to MD5, an earlier, widely-used hash function. Both are reportedly compromised. 
		/// In some circles, it is suggested that SHA-256 or greater be used for critical technology. 
		/// The SHA algorithms were designed by the National Security Agency (NSA) and published as a US government standard.
		/// </summary>
		SHA1,

		/// <summary>
		/// An additional SHA family hash with a digest that is computed with 32-bit words
		/// </summary>
		SHA256,

		/// <summary>
		/// An additional SHA family hash that is a truncated version of SHA-256 or SHA-512 
		/// with different initialization vectors.
		/// </summary>
		SHA384,

		/// <summary>
		/// An additional SHA family hash with a digest that is computed with 64-bit words
		/// </summary>
		SHA512,

    }
    #endregion

    #endregion

    /// <summary>
	/// Provides a Generic Cryptographer which can use any Algorithm for Encrypting, Decrypting, and Hashing
	/// </summary>
	[System.Diagnostics.DebuggerNonUserCode()]
	public class Cryptographer 
    {
		
		#region GENERATE KEYS

        #region GenerateIV
        /// <summary>
		/// Generates an IV for the specified Algorithm
		/// </summary>
		/// <param name="Algorithm"></param>
		/// <returns></returns>
		[System.Diagnostics.DebuggerStepThrough()]
		public static byte[] GenerateIV(SymmetricAlgorithms Algorithm) 
        {
			switch (Algorithm) 
            {
				case SymmetricAlgorithms.Rijndael :
					// Rijndael needs a 16, 24, or 32 byte Key
					return new byte[] {0xF, 0x6F, 0x13, 0x2E, 0x35, 0xC2, 0xCD, 0xF9, 0x5, 0x46, 0x9C, 0xEA, 0xA8, 0x4B, 0x73, 0xCC};

				default:
					return new byte[] {0xF, 0x6F, 0x13, 0x2E, 0x35, 0xC2, 0xCD, 0xF9};
			}
        }
        #endregion

        #region GenerateKey
        /// <summary>
		/// Generates a Key to be used in a SymmetricAlgorithm
		/// </summary>
		/// <param name="Key">The Key or Password to use</param>
		/// <param name="Algorithm">The Algorithm being used for encryption</param>
		/// <returns></returns>
		[System.Diagnostics.DebuggerStepThrough()]
		public static byte[] GenerateKey(string Key, SymmetricAlgorithm Algorithm) 
        {
			// Adjust the Key if neccessary
			if (Algorithm.LegalKeySizes.Length > 0) 
            {
				int keySize = Key.Length * 8;
				int minSize = Algorithm.LegalKeySizes[0].MinSize;
				int maxSize = Algorithm.LegalKeySizes[0].MaxSize;
				int skipSize = Algorithm.LegalKeySizes[0].SkipSize;
                if (keySize > maxSize)
                {
                    Key = Key.Substring(0, maxSize / 8);	// Extract maximum size allowed
                }
                else if (keySize < maxSize)
                {
                    int validSize = (keySize <= minSize) ? minSize : (keySize - keySize % skipSize) + skipSize;	// Set valid size
                    if (keySize < validSize) Key = Key.PadRight(validSize / 8, '*');	// Pad the key with asterisk to make up the size
                }
			}

			byte[] salt = System.Text.Encoding.UTF8.GetBytes(Key);
			PasswordDeriveBytes key = new PasswordDeriveBytes(Key, salt);
			return key.GetBytes(Key.Length);
        }
        #endregion

        #region GenerateMachineKey
        /// <summary>
		/// Generates a Key that can be used for the machineKey setting in a web.config file
		/// </summary>
		/// <param name="KeyLength">
		/// The length of the key to generate (40 - 128). 
		/// A Length of at least 48 is recommended as it uses Triple DES. 
		/// A [decryptionKey] can be 16 or 48.  If 16 is defined Standard DES encryption is used. 
		/// A [validationKey] can be of any length but 128 is recommended.
		/// </param>
		/// <example>
		/// string decryptionKey = Cryptographer.GenerateMachineKey(48);
		/// string validationKey = Cryptographer.GenerateMachineKey(128);
		/// </example>
		/// <returns></returns>
		public static string GenerateMachineKey(byte KeyLength) 
        {
			if (KeyLength < 40 || KeyLength > 128) 
				throw new ArgumentOutOfRangeException("KeyLength", "The KeyLength must be between 40 and 128");

			byte[] randomData = new byte[KeyLength / 2];
			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
			rng.GetBytes(randomData);
			rng = null;

			// Return the Key formatted as Hexadecimal
			System.Text.StringBuilder key = new System.Text.StringBuilder(KeyLength);
			for (int i = 0; i < randomData.Length; i++) 
            {
				key.AppendFormat("{0:X2}", randomData[i]);
			}
			randomData = null;
			return key.ToString();
        }
        #endregion

        #endregion

        #region ENCRYPTION - SYMMETRIC

        #region Encrypt(string Data, string Key, SymmetricAlgorithms Algorithm, bool Base64Encode)
        /// <summary>
		/// Encrypts a string of data and returns the data
		/// </summary>
		/// <param name="Data">The Data to encrypt</param>
		/// <param name="Key">The Key or Password used to encrypt the Data</param>
		/// <param name="Algorithm">A symmetric algorithm used to encrypt the Data</param>
		/// <param name="Base64Encode">True to return the encrypted data as a Base64 Encoded String (for use in Xml files)</param>
		/// <returns>An encrypted String of the Data</returns>
		public static string Encrypt(string Data, string Key, SymmetricAlgorithms Algorithm, bool Base64Encode) 
        { 
			// Determine the Algorithm
			SymmetricAlgorithm symAlgorithm = null;
			switch (Algorithm) 
            {
				case SymmetricAlgorithms.DES : symAlgorithm = new DESCryptoServiceProvider(); break;
				case SymmetricAlgorithms.RC2 : symAlgorithm = new RC2CryptoServiceProvider(); break;
				case SymmetricAlgorithms.Rijndael: symAlgorithm = new RijndaelManaged(); break;
				case SymmetricAlgorithms.TripleDES: symAlgorithm = new TripleDESCryptoServiceProvider(); break;
			}
			if (symAlgorithm == null) 
                return Data;

			try 
            {
				symAlgorithm.IV = Cryptographer.GenerateIV(Algorithm);
				symAlgorithm.Key = Cryptographer.GenerateKey(Key, symAlgorithm);
				symAlgorithm.Mode = CipherMode.CBC;

				byte[] cryptData = Cryptographer.Encrypt(Data, symAlgorithm);
				if (Base64Encode)
					return Convert.ToBase64String(cryptData);
				else
					return System.Text.Encoding.UTF8.GetString(cryptData);
			}
			catch 
            {
                return Data; 
            }
			finally 
            {
				if (symAlgorithm != null) 
                { 
                    symAlgorithm.Clear(); 
                    symAlgorithm = null; 
                }
			}
        }
        #endregion

        #region Encrypt(string Data, byte[] Key, byte[] Salt, SymmetricAlgorithms Algorithm, bool Base64Encode)
        /// <summary>
		/// Encrypts a string of data and returns the data
		/// </summary>
		/// <param name="Data">The Data to encrypt</param>
		/// <param name="Key">The Key or Password used to encrypt the Data</param>
		/// <param name="Salt">The Data used to hash the Encrypted Data</param>
		/// <param name="Algorithm">A symmetric algorithm used to encrypt the Data</param>
		/// <param name="Base64Encode">True to return the encrypted data as a Base64 Encoded String (for use in Xml files)</param>
		/// <returns>An encrypted String of the Data</returns>
		public static string Encrypt(string Data, byte[] Key, byte[] Salt, SymmetricAlgorithms Algorithm, bool Base64Encode) 
        {
			// Determine the Algorithm
			SymmetricAlgorithm symAlgorithm = null;
			switch (Algorithm) 
            {
				case SymmetricAlgorithms.DES: 
                    symAlgorithm = new DESCryptoServiceProvider(); 
                    break;
				case SymmetricAlgorithms.RC2: 
                    symAlgorithm = new RC2CryptoServiceProvider(); 
                    break;
				case SymmetricAlgorithms.Rijndael: 
                    symAlgorithm = new RijndaelManaged(); 
                    break;
				case SymmetricAlgorithms.TripleDES: 
                    symAlgorithm = new TripleDESCryptoServiceProvider(); 
                    break;
			}
			if (symAlgorithm == null) 
                return Data;

			try 
            {
				symAlgorithm.IV = Salt;
				symAlgorithm.Key = Key;
				symAlgorithm.Mode = CipherMode.CBC;

				byte[] cryptData = Cryptographer.Encrypt(Data, symAlgorithm);
				if (Base64Encode)
					return Convert.ToBase64String(cryptData);
				else
					return System.Text.Encoding.UTF8.GetString(cryptData);
			}
			catch 
            { 
                return Data; 
            }
			finally 
            {
				if (symAlgorithm != null) 
                { 
                    symAlgorithm.Clear(); 
                    symAlgorithm = null; 
                }
			}
        }
        #endregion

        #region Encrypt(string Data, SymmetricAlgorithm Algorithm)
        /// <summary>
		/// Encrypts a string of data and returns the data
		/// </summary>
		/// <param name="Data"></param>
		/// <param name="Algorithm"></param>
		/// <returns></returns>
		private static byte[] Encrypt(string Data, SymmetricAlgorithm Algorithm)
        {
			System.IO.MemoryStream ioStream = new System.IO.MemoryStream();
			CryptoStream cryptStream = new CryptoStream(ioStream, Algorithm.CreateEncryptor(), CryptoStreamMode.Write);
			try 
            {
				byte[] cryptData = System.Text.Encoding.UTF8.GetBytes(Data);
				cryptStream.Write(cryptData, 0, cryptData.Length);
				cryptStream.FlushFinalBlock();
				return ioStream.ToArray();
			}
			catch 
            {
                return null; 
            }
			finally 
            {
				if (cryptStream != null) 
                { 
                    cryptStream.Close(); 
                    cryptStream = null; 
                }
				if (ioStream != null) 
                { 
                    ioStream.Close(); 
                    ioStream = null; 
                }
			}
        }
        #endregion

        #endregion

        #region DECRYPTION - SYMMETRIC

        #region Decrypt(string Data, string Key, SymmetricAlgorithms Algorithm, bool Base64Encoded) 
        /// <summary>
		/// Decrypts the Data using the specified Algorithm
		/// </summary>
		/// <param name="Data">The Data to decrypt</param>
		/// <param name="Key">The Key or Password used to decrypt the Data</param>
		/// <param name="Algorithm">A Symmetric Algorithm used to decrypt the Data</param>
		/// <param name="Base64Encoded">True if the Data is Base64 Encoded</param>
		/// <returns>A decrypted String of the Data</returns>
		public static string Decrypt(string Data, string Key, SymmetricAlgorithms Algorithm, bool Base64Encoded) 
        { 
			// Determine the Algorithm
			SymmetricAlgorithm symAlgorithm = null;
			switch (Algorithm) 
            {
				case SymmetricAlgorithms.DES : 
                    symAlgorithm = new DESCryptoServiceProvider(); 
                    break;
				case SymmetricAlgorithms.RC2 : 
                    symAlgorithm = new RC2CryptoServiceProvider(); 
                    break;
				case SymmetricAlgorithms.Rijndael: 
                    symAlgorithm = new RijndaelManaged(); 
                    break;
				case SymmetricAlgorithms.TripleDES: 
                    symAlgorithm = new TripleDESCryptoServiceProvider(); 
                    break;
			}
			if (symAlgorithm == null) 
                return string.Empty;

			try 
            {
				symAlgorithm.IV = Cryptographer.GenerateIV(Algorithm);
				symAlgorithm.Key = Cryptographer.GenerateKey(Key, symAlgorithm);
				byte[] cryptData;
				if (Base64Encoded) 
					cryptData = Convert.FromBase64String(Data); 
				else 
					cryptData = System.Text.Encoding.UTF8.GetBytes(Data);
				return Cryptographer.Decrypt(cryptData, symAlgorithm);
			}
			catch 
            { 
                return string.Empty; 
            }
			finally 
            {
				if (symAlgorithm != null) 
                { 
                    symAlgorithm.Clear(); 
                    symAlgorithm = null; 
                }
			}
        }
        #endregion

        #region Decrypt(string Data, byte[] Key, byte[] Salt, SymmetricAlgorithms Algorithm, bool Base64Encoded) 
        /// <summary>
		/// 
		/// </summary>
		/// <param name="Data">The Data to decrypt</param>
		/// <param name="Key">The Key or Password used to decrypt the Data</param>
		/// /// <param name="Salt"></param>
		/// <param name="Algorithm">A Symmetric Algorithm used to decrypt the Data</param>
		/// <param name="Base64Encoded">True if the Data is Base64 Encoded</param>
		/// <returns>A decrypted String of the Data</returns>
		public static string Decrypt(string Data, byte[] Key, byte[] Salt, SymmetricAlgorithms Algorithm, bool Base64Encoded) 
        {
			// Determine the Algorithm
			SymmetricAlgorithm symAlgorithm = null;
			switch (Algorithm) 
            {
				case SymmetricAlgorithms.DES: 
                    symAlgorithm = new DESCryptoServiceProvider(); 
                    break;
				case SymmetricAlgorithms.RC2: 
                    symAlgorithm = new RC2CryptoServiceProvider(); 
                    break;
				case SymmetricAlgorithms.Rijndael: 
                    symAlgorithm = new RijndaelManaged(); 
                    break;
				case SymmetricAlgorithms.TripleDES: 
                    symAlgorithm = new TripleDESCryptoServiceProvider(); 
                    break;
			}
			if (symAlgorithm == null) 
                return string.Empty;

			try 
            {
				symAlgorithm.IV = Salt;
				symAlgorithm.Key = Key;
				byte[] cryptData;
				if (Base64Encoded)
					cryptData = Convert.FromBase64String(Data);
				else
					cryptData = System.Text.Encoding.UTF8.GetBytes(Data);
				return Cryptographer.Decrypt(cryptData, symAlgorithm);
			}
			catch 
            { 
                return string.Empty; 
            }
			finally 
            {
				if (symAlgorithm != null) 
                { 
                    symAlgorithm.Clear(); 
                    symAlgorithm = null; 
                }
			}
        }
        #endregion

        #region Decrypt(byte[] Data, SymmetricAlgorithm Algorithm)
        /// <summary>
		/// Decrypts the Data using the specified Algorithm
		/// </summary>
		/// <param name="Data"></param>
		/// <param name="Algorithm"></param>
		/// <returns></returns>
		private static string Decrypt(byte[] Data, SymmetricAlgorithm Algorithm) 
        { 
			System.IO.MemoryStream ioStream = new System.IO.MemoryStream(Data, 0, Data.Length);
			CryptoStream cryptStream = new CryptoStream(ioStream, Algorithm.CreateDecryptor(), CryptoStreamMode.Read);
			// System.IO.StreamReader ioReader = new System.IO.StreamReader(ioStream);
			System.IO.StreamReader ioReader = new System.IO.StreamReader(cryptStream);
			try 
            {
				string sData = ioReader.ReadToEnd();
				return sData;
			}
			catch 
            {
                return string.Empty; 
            }
			finally 
            {
				if (cryptStream != null) { cryptStream.Close(); cryptStream = null; }
				if (ioReader != null) { ioReader.Close(); ioReader = null; }
				if (ioStream != null) { ioStream.Close(); ioStream = null; }
			}
        }
        #endregion

        #endregion

        #region Hash
        /// <summary>
		/// Computes a Hash of the Data using the specified Algorithm
		/// </summary>
		/// <param name="Data">The Data to Hash</param>
		/// <param name="Algorithm">The Hash Algorithm to use</param>
		/// <param name="Base64Encode">True to return a Base64 Encoded string (for use in Xml files).</param>
		/// <returns></returns>
		public static string Hash(string Data, HashAlgorithms Algorithm, bool Base64Encode) 
        {
			// Determine the Hash Algorithm
			HashAlgorithm hashAlgorithm = null;
			switch (Algorithm) 
            {
				case HashAlgorithms.MD5 : 
                    hashAlgorithm = new MD5CryptoServiceProvider(); 
                    break;
				case HashAlgorithms.SHA1 : 
                    hashAlgorithm = new SHA1Managed(); 
                    break;
				case HashAlgorithms.SHA256 : 
                    hashAlgorithm = new SHA256Managed(); 
                    break;
				case HashAlgorithms.SHA384 : 
                    hashAlgorithm = new SHA384Managed(); 
                    break;
				case HashAlgorithms.SHA512 : 
                    hashAlgorithm = new SHA512Managed(); 
                    break;
			}

			try 
            {
				byte[] hashData = System.Text.Encoding.UTF8.GetBytes(Data);
				hashData = hashAlgorithm.ComputeHash(hashData);
				if (Base64Encode)
					return Convert.ToBase64String(hashData);
				else
					return System.Text.Encoding.UTF8.GetString(hashData);
			}
			catch 
            { 
                return string.Empty; 
            }
			finally 
            {
				if (hashAlgorithm != null) 
                { 
                    hashAlgorithm.Clear(); 
                    hashAlgorithm = null; 
                }
			}			
		}
		#endregion

        #region GetRandomizer
        /// <summary>
		/// Returns a <see cref="System.Random"/> class initialized with a randomly generated seed.
		/// </summary>
		/// <returns></returns>
		public static System.Random GetRandomizer() 
        {
			byte[] randomData = new byte[4];
			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
			rng.GetBytes(randomData);
			rng = null;
			int seed1 = (randomData[0] & 0x70 << 24);
			int seed2 = (randomData[1] << 16);
			int seed3 = (randomData[2] << 8);
			int seed4 = randomData[3];
			int seed = (seed1 | seed2 | seed3 | seed4);
			return new Random(seed);
		}
		#endregion
		
	}

}
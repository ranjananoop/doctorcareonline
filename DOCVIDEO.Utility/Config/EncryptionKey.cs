
#region USING

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace DOCVIDEO.Utility
{

	/// <summary>
	/// </summary>
	[System.Diagnostics.DebuggerNonUserCode()]
	public class EncryptionKey 
    {
		
		#region DECLARATIONS

		private string _key, _salt;

		#endregion
		
		#region PROPERTIES

		/// <summary>
		/// Gets or sets the Key used for Encryption / Decryption
		/// </summary>
		public string Key 
        {
			get 
            {
				if (string.IsNullOrEmpty(this._key))
                    return string.Empty;
				return this._key;
			}
			set { this._key = value; }
		}

		/// <summary>
		/// Gets or sets the Salt to use for added security when Encrypting / Decrypting
		/// </summary>
		public string Salt 
        {
			get 
            {
                if (string.IsNullOrEmpty(this._salt))
                    return string.Empty;
				return this._salt;
			}
			set { this._salt = value; }
		}

		#endregion
		
		#region METHODS

        #region Encrypt
        /// <summary>
		/// Encrypts the Data
		/// </summary>
		/// <param name="Data"></param>
		/// <returns></returns>
		public string Encrypt(string Data) 
        {
			byte[] keyData = System.Text.Encoding.ASCII.GetBytes(this.Key);
			byte[] saltData = Convert.FromBase64String(this.Salt);
			return Security.Cryptographer.Encrypt(Data, keyData, saltData, Security.SymmetricAlgorithms.TripleDES, true);
        }
        #endregion

        #region Decrypt
        /// <summary>
		/// Encrypts the Data
		/// </summary>
		/// <param name="Data"></param>
		/// <returns></returns>
		public string Decrypt(string Data) 
        {
			byte[] keyData = System.Text.Encoding.ASCII.GetBytes(this.Key);
			byte[] saltData = Convert.FromBase64String(this.Salt);
			return Security.Cryptographer.Decrypt(Data, keyData, saltData, Security.SymmetricAlgorithms.TripleDES, true);
        }
        #endregion

        #endregion

    }

}
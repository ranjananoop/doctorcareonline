
#region USING

using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Text;

#endregion

namespace DOCVIDEO.Utility
{
	/// <summary>
	/// Summary description for WebConfigAccess.
	/// </summary>
    public class WebConfigAccess
    {
        #region STATIC METHODS

        #region GetConnString
        public static string GetConnString(string connectionName)
        {
            return BuildConnString("connections/" + connectionName);
        }
        #endregion

        #region GetConnString
        /// <summary>
        /// Will return the default Connection
        /// </summary>
        /// <returns></returns>
        public static string GetConnString()
        {
            return BuildConnString("connections/" + ConfigurationManager.AppSettings["connection"]);
        }
        #endregion

        #endregion

        #region private methods

        #region BuildConnString
        /// <summary>
        /// Builds the Connection String
        /// </summary>
        /// <returns></returns>
        private static string BuildConnString(string sectionName)
        {
            NameValueCollection nvc = (NameValueCollection)ConfigurationManager.GetSection(sectionName);
            EncryptionKey encKey = new EncryptionKey();
            encKey.Key = nvc["encryptionKey"];
            encKey.Salt = nvc["encryptionSalt"];

            StringBuilder sb = new StringBuilder();
            sb.Append(nvc["connectionString"]);
            sb.Append("User ID=");
            //sb.Append(encKey.Decrypt(nvc["uid"]));
            //sb.Append(";Password=");
            //sb.Append(encKey.Decrypt(nvc["pwd"]));
            sb.Append(nvc["uid"]);
            sb.Append(";Password=");
            sb.Append(nvc["pwd"]);
            sb.Append(";");

            return sb.ToString();
        }
        #endregion
        
        #endregion
    }
}

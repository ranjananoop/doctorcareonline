
#region USING

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;

#endregion

namespace DOCVIDEO.Utility
{
    public class DataConnection
    {
        #region DECLARATIONS
		
		// Property Variables
		private string _applicationKey;
		private string _server, _database;
		private string _user, _password;
		private string _dataProvider, _oleProvider, _options;
		private string _protocol;
		private EncryptionKey _encryptKey;

		#endregion

        #region PROPERTIES

        #region ApplicationKey
        /// <summary>
		/// Gets or sets the Application Key
		/// </summary>
		public string ApplicationKey 
        {
			get 
            {
				if (string.IsNullOrEmpty(this._applicationKey)) 
                    return string.Empty;
				return this._applicationKey; 
			}
			set { this._applicationKey = value; }
        }
        #endregion

        #region Server
        /// <summary>
		/// Gets or sets the Server
		/// </summary>
		public string Server 
        {
			get 
            {
				if (string.IsNullOrEmpty(this._server)) 
                    return string.Empty;
				return this._server;
			}
			set { this._server = value; }
        }
        #endregion

        #region Database
        /// <summary>
		/// Gets or sets the Database
		/// </summary>
		public string Database 
        {
			get 
            {
				if (string.IsNullOrEmpty(this._database))
                    return string.Empty;
				return this._database;
			}
			set { this._database = value; }
        }
        #endregion

        #region Options
        /// <summary>
		/// Gets or sets the Connection Options
		/// </summary>
		public string Options 
        {
			get 
            {
				if (string.IsNullOrEmpty(this._options)) 
                    return string.Empty;
				return this._options;
			}
			set { this._options = value; }
        }
        #endregion

        #region Protocol
        /// <summary>
		/// Gets or sets the Connection Protocol (Trusted or Standard)
		/// </summary>
		public string Protocol 
        {
			get 
            {
				if (string.IsNullOrEmpty(this._protocol)) 
                    return string.Empty;
				return this._protocol;
			}
			set { this._protocol = value; }
        }
        #endregion

        #region UserID
        /// <summary>
		/// Gets or sets the User ID
		/// </summary>
		public string UserID 
        {
			get 
            {
				if (string.IsNullOrEmpty(this._user)) 
                    return string.Empty;
				return this._user;
			}
			set { this._user = value; }
        }
        #endregion

        #region Password
        /// <summary>
		/// Gets or sets the Password
		/// </summary>
		public string Password 
        {
			get 
            {
				if (string.IsNullOrEmpty(this._password))
                    return string.Empty;
				return this._password;
			}
			set { this._password = value; }
        }
        #endregion

        #region DataProvider
        /// <summary>
		/// Gets or sets the DataProvider (System.Data.SqlClient, System.Data.OracleClient, etc.)
		/// </summary>
		public string DataProvider 
        {
			get 
            {
				if (string.IsNullOrEmpty(this._dataProvider)) 
                    return string.Empty;
				return this._dataProvider;
			}
			set { this._dataProvider = value; }
        }
        #endregion

        #region OleDbProvider
        /// <summary>
		/// Gets or sets the OleDbProvider (Microsoft.Jet.OLEDB.4.0, etc.).  
		/// This is only used when the DataProvider = System.Data.OleDb
		/// </summary>
		public string OleDbProvider 
        {
			get 
            {
				if (string.IsNullOrEmpty(this._oleProvider)) 
                    return string.Empty;
				return this._oleProvider;
			}
			set { this._oleProvider = value; }
        }
        #endregion

        #region EncryptionKey
        /// <summary>
		/// Gets or sets the EncryptionKey data
		/// </summary>
		public EncryptionKey EncryptionKey 
        {
			get 
            {
				if (this._encryptKey == null) 
                    this._encryptKey = new EncryptionKey();
				return this._encryptKey; 
			}
			set { this._encryptKey = value; }
        }
        #endregion

        #region ConnectionString
        public string ConnectionString
        {
            get { return this.ToConnectionString(); }
        }
        #endregion

        #endregion

        #region Methods

        #region ToString()
        /// <summary>
		/// Returns a System.String representation of the DataPortalContext
		/// </summary>
		/// <returns></returns>
		public override string ToString() 
        {
			string value = string.Empty;
			if (!string.IsNullOrEmpty(this._server))
                value += string.Format("Server={0};", this._server);
			if (!string.IsNullOrEmpty(this._database)) 
                value += string.Format("Database={0};", this._database);
			return value;
        }
        #endregion

        #region ToConnectionString()
        /// <summary>
		/// Returns the Connection String
		/// </summary>
		/// <returns></returns>
		private string ToConnectionString() 
        {
			if (string.IsNullOrEmpty(this._dataProvider)) 
                return string.Empty;
			bool bTrustedConnection = (this._protocol.ToLower() == "trusted" ? true : false);

			DbProviderFactory factory = DbProviderFactories.GetFactory(this._dataProvider);
            if (factory == null) 
                return string.Empty;

			// Determine if the Credentials are Encrypted
			if (!string.IsNullOrEmpty(this._encryptKey.Key)) 
            {
				byte[] keyData = System.Text.Encoding.ASCII.GetBytes(this._encryptKey.Key);
				byte[] saltData = Convert.FromBase64String(this._encryptKey.Salt);
				this._user = Security.Cryptographer.Decrypt(this._user, keyData, saltData, Security.SymmetricAlgorithms.TripleDES, true);
				this._password = Security.Cryptographer.Decrypt(this._password, keyData, saltData, Security.SymmetricAlgorithms.TripleDES, true);
			}

			// Build the Connection String
			DbConnectionStringBuilder builder = factory.CreateConnectionStringBuilder();
			if (builder is System.Data.SqlClient.SqlConnectionStringBuilder) 
            {
				SqlConnectionStringBuilder sqlBuilder = (builder as SqlConnectionStringBuilder);
				sqlBuilder.DataSource = this._server;
				sqlBuilder.InitialCatalog = this._database;
				sqlBuilder.LoadBalanceTimeout = 600;
				sqlBuilder.PersistSecurityInfo = false;
				sqlBuilder.Pooling = true;
				sqlBuilder.Replication = false;
				sqlBuilder.IntegratedSecurity = bTrustedConnection;
				if (string.IsNullOrEmpty(this._user) & (this._server.ToLower().Contains("(local)") || this._server.ToLower() == Environment.MachineName.ToLower()))
                
                {
					sqlBuilder.IntegratedSecurity = true;
				}
				if (!sqlBuilder.IntegratedSecurity) {
					sqlBuilder.UserID = this._user;
					sqlBuilder.Password = this._password;
				}
			}
			else if (builder is System.Data.OleDb.OleDbConnectionStringBuilder) 
            {
				OleDbConnectionStringBuilder oleBuilder = (builder as OleDbConnectionStringBuilder);
				oleBuilder.DataSource = this._server;
				oleBuilder.Provider = this._oleProvider;
				if (this._oleProvider.ToLower().Contains("microsoft"))
					oleBuilder.FileName = this._database;
				else
					oleBuilder.Add("Database", this._database);
				oleBuilder.PersistSecurityInfo = false;
				if (!string.IsNullOrEmpty(this._user)) oleBuilder.Add("UID", this._user);
				if (!string.IsNullOrEmpty(this._password)) oleBuilder.Add("PWD", this._password);
			}
			else if (builder is System.Data.Odbc.OdbcConnectionStringBuilder) 
            {
				OdbcConnectionStringBuilder odbcBuilder = (builder as OdbcConnectionStringBuilder);
				odbcBuilder.Driver = this._oleProvider;
				odbcBuilder.Dsn = this._database;
				if (!string.IsNullOrEmpty(this._user)) odbcBuilder.Add("UID", this._user);
				if (!string.IsNullOrEmpty(this._password)) odbcBuilder.Add("PWD", this._password);
			}
			else
				return string.Empty;

			// Parse the Data Provider Options
			if (!string.IsNullOrEmpty(this._options))
            {
				string[] sOptions = this._options.Split(new char[] { char.Parse(";") });
				if (sOptions != null && sOptions.Length > 0) 
                {
					foreach (string sOption in sOptions) 
                    {
						string[] sParts = sOption.Split(new char[] { char.Parse("=") });
						if (sParts != null && sParts.Length == 2) builder.Add(sParts[0], sParts[1]);
					}
				}
			}

			// Encrypt the Credentials
			if (!string.IsNullOrEmpty(this._encryptKey.Key))
            {
				byte[] keyData = System.Text.Encoding.ASCII.GetBytes(this._encryptKey.Key);
				byte[] saltData = Convert.FromBase64String(this._encryptKey.Salt);
				this._user = Security.Cryptographer.Encrypt(this._user, keyData, saltData, Security.SymmetricAlgorithms.TripleDES, true);
				this._password = Security.Cryptographer.Encrypt(this._password, keyData, saltData, Security.SymmetricAlgorithms.TripleDES, true);
			}

			return builder.ConnectionString;
		}
        #endregion

		#endregion
    }
}

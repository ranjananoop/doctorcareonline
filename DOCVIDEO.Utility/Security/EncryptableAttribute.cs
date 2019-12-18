
#region USING

using System;
using System.Reflection;

#endregion

namespace DOCVIDEO.Utility.Security
{

	/// <summary>
	/// Attribute used for defining how an object Property or Field should be Encrypted
	/// </summary>
	// [System.Diagnostics.DebuggerNonUserCode()]
	[System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
	public class EncryptableAttribute : System.Attribute 
    {
		#region DECLARATIONS
		
		// Property Variables
		SymmetricAlgorithms _algorithm = SymmetricAlgorithms.TripleDES;
		byte[] _key, _salt;

		#endregion
		
		#region PROPERTIES

		/// <summary>
		/// Gets or sets the Encryption Algorithm
		/// </summary>
		public SymmetricAlgorithms Algorithm 
        {
			get { return this._algorithm; }
			set { this._algorithm = value; }
		}

		/// <summary>
		/// Gets or sets the Key Data
		/// </summary>
		public byte[] Key 
        {
			get { return this._key; }
			set { this._key = value; }
		}

		/// <summary>
		/// Gets or sets the Salt Data (provides better Encryption)
		/// </summary>
		public byte[] Salt
        {
			get { return this._salt; }
			set { this._salt = value; }
		}

		#endregion
		
		#region INTERNAL METHODS

        #region SetFieldValue
        /// <summary>
		/// Sets a Property Value on the Target object
		/// </summary>
		[System.Diagnostics.DebuggerNonUserCode()]
		private static void SetFieldValue(FieldInfo Field, object Target, object Value) 
        {
			Type pType = GetPropertyType(Field.FieldType);
			Type vType = GetPropertyType(Value.GetType());
			if (((Value == null || Convert.IsDBNull(Value)) & (Field.FieldType.IsGenericType)) && (Field.FieldType.GetGenericTypeDefinition() == typeof(Nullable<>)))
				Field.SetValue(Target, null);
			else if (pType.Equals(vType))
				Field.SetValue(Target, Value);	// Types match, just set value
			else if (Value != null && !Convert.IsDBNull(Value)) {
				// Coerce the Value
				if (pType.Equals(typeof(Guid)))
					Field.SetValue(Target, new Guid(Value.ToString()));
				else if (pType.IsEnum && vType.Equals(typeof(string)))
					Field.SetValue(Target, Enum.Parse(pType, Value.ToString()));
				else
					Field.SetValue(Target, Convert.ChangeType(Value, pType));
			}
        }
        #endregion

        #region SetPropertyValue
        /// <summary>
		/// Sets a Property Value on the Target object
		/// </summary>
		[System.Diagnostics.DebuggerNonUserCode()]
		private static void SetPropertyValue(PropertyInfo Prop, object Target, object Value, object DefaultValue) 
        {
			// Ensure that there is a value
			if (Value == null || Convert.IsDBNull(Value)) 
                Value = DefaultValue;

			Type pType = GetPropertyType(Prop.PropertyType);
			Type vType = GetPropertyType(Value.GetType());
			if (((Value == null || Convert.IsDBNull(Value)) & (Prop.PropertyType.IsGenericType)) && (Prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)))
				Prop.SetValue(Target, null, null);
			else if (pType.Equals(vType))
				Prop.SetValue(Target, Value, null);		// Types match, just set value
			else if (Value != null && !Convert.IsDBNull(Value)) {
				// Coerce the Value
				if (pType.Equals(typeof(Guid)))
					Prop.SetValue(Target, new Guid(Value.ToString()), null);
				else if (pType.IsEnum && Enum.IsDefined(pType, Value))
					Prop.SetValue(Target, Enum.Parse(pType, Value.ToString(), true), null);
				else
					Prop.SetValue(Target, Convert.ChangeType(Value, pType), null);
			}
        }
        #endregion

        #region GetPropertyType
        /// <summary>
		/// Returns a property's type, dealing with Nullable(Of T) if necessary.
		/// </summary>
		[System.Diagnostics.DebuggerNonUserCode()]
		private static Type GetPropertyType(Type propertyType) 
        {
			Type type = propertyType;
			if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>))) 
                return Nullable.GetUnderlyingType(type);
			return type;
        }
        #endregion

        #endregion

        #region ENCRYPTION METHODS (STATIC)

        #region Encrypt
        /// <summary>
		/// Encrypts any Properties or Fields marked at Encryptable
		/// </summary>
		/// <param name="Target"></param>
		/// <returns></returns>
		public static void Encrypt(object Target) 
        {
			try 
            {
				EncryptFields(Target);
				EncryptProperties(Target);
			}
			catch (Exception ex)
            {
                throw ex; 
            }
        }
        #endregion

        #region EncryptFields
        /// <summary>
		/// Encrypts the Fields of an Object
		/// </summary>
		/// <param name="Target"></param>
		private static void EncryptFields(object Target) 
        {
			if (Target == null) 
                return;
			if (Target is System.Data.DataTable || Target is System.Data.DataRow) 
                return;

			object oValue = null;
			try 
            {
				Type type = Target.GetType();
				FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
				foreach (FieldInfo field in fields) 
                {
					Type fieldType = EncryptableAttribute.GetPropertyType(field.FieldType);
					if (fieldType.IsClass
						&& fieldType.FullName != "System.String"
						&& fieldType.FullName != "System.String[]"
						&& fieldType.FullName != "System.Object"
						&& fieldType.FullName != "System.Object[]"
						&& fieldType.FullName.Contains("List") == false) 
					{
						oValue = field.GetValue(Target);
						EncryptFields(oValue);
						continue;
					}

					object[] attribs = field.GetCustomAttributes(typeof(EncryptableAttribute), true);
					if (attribs.Length == 0) 
                        continue;

					EncryptableAttribute attrib = (EncryptableAttribute)attribs[0];
					oValue = field.GetValue(Target);
					if (oValue != null && !string.IsNullOrEmpty(oValue.ToString())) 
                    {	
						oValue = Cryptographer.Encrypt(oValue.ToString(), attrib.Key, attrib.Salt, attrib.Algorithm, true);
						SetFieldValue(field, Target, oValue);	// Set the Value
					}
				}
			}
			catch (Exception ex)
            { 
                throw ex; 
            }
        }
        #endregion 

        #region EncryptProperties
        /// <summary>
		/// Encrypts the Properties of an Object
		/// </summary>
		/// <param name="Target"></param>
		private static void EncryptProperties(object Target) 
        {
			if (Target == null) 
                return;
			if (Target is System.Data.DataTable) 
                return;

			object oValue = null;
			try 
            {
				Type type = Target.GetType();
				PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
				foreach (PropertyInfo prop in props) 
                {
					Type propType = EncryptableAttribute.GetPropertyType(prop.PropertyType);
					if (prop.PropertyType.IsClass
						&& propType.FullName != "System.String"
						&& propType.FullName != "System.String[]"
						&& propType.FullName != "System.Object"
						&& propType.FullName != "System.Object[]"
						&& propType.FullName.Contains("List") == false)
					{
						oValue = prop.GetValue(Target, null);
						EncryptProperties(oValue);
						continue;
					}

					if (!prop.CanWrite) 
                        continue;
					object[] attribs = prop.GetCustomAttributes(typeof(EncryptableAttribute), true);
					if (attribs.Length == 0) 
                        continue;

					EncryptableAttribute attrib = (EncryptableAttribute)attribs[0];
					oValue = prop.GetValue(Target, null);
					if (oValue != null && !string.IsNullOrEmpty(oValue.ToString())) 
                    {
						oValue = Cryptographer.Encrypt(oValue.ToString(), attrib.Key, attrib.Salt, attrib.Algorithm, true);
						SetPropertyValue(prop, Target, oValue, "");	// Set the Value
					}
				}
			}
			catch (Exception ex)
            {
                throw ex; 
            }
        }
        #endregion

        #endregion

        #region DECRYPTION METHODS (STATIC)

        #region Decrypt
        /// <summary>
		/// Decrypts any Properties or Fields marked at Encryptable
		/// </summary>
		/// <param name="Target"></param>
		/// <returns></returns>
		public static void Decrypt(object Target) 
        {
			try 
            {
				DecryptFields(Target);
				DecryptProperties(Target);
			}
			catch (Exception ex)
            { 
                throw ex; 
            }
        }
        #endregion

        #region DecryptFields
        /// <summary>
		/// Decrypts the Fields of an Object
		/// </summary>
		/// <param name="Target"></param>
		private static void DecryptFields(object Target) 
        {
			if (Target == null) 
                return;
			if (Target is System.Data.DataTable || Target is System.Data.DataRow) 
                return;

			object oValue = null;
			try 
            {
				Type type = Target.GetType();
				FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
				foreach (FieldInfo field in fields) 
                {
					Type fieldType = EncryptableAttribute.GetPropertyType(field.FieldType);
					if (fieldType.IsClass
						&& fieldType.FullName != "System.String"
						&& fieldType.FullName != "System.String[]"
						&& fieldType.FullName != "System.Object"
						&& fieldType.FullName != "System.Object[]"
						&& fieldType.FullName.Contains("List") == false)
					{
						oValue = field.GetValue(Target);
						DecryptFields(oValue);
						continue;
					}

					object[] attribs = field.GetCustomAttributes(typeof(EncryptableAttribute), true);
					if (attribs.Length == 0) 
                        continue;

					EncryptableAttribute attrib = (EncryptableAttribute)attribs[0];
					oValue = field.GetValue(Target);
					if (oValue != null && !string.IsNullOrEmpty(oValue.ToString())) 
                    {
						oValue = Cryptographer.Decrypt(oValue.ToString(), attrib.Key, attrib.Salt, attrib.Algorithm, true);
						SetFieldValue(field, Target, oValue);	// Set the Value
					}
				}
			}
			catch(Exception ex) 
            {
                throw ex; 
            }
        }
        #endregion

        #region DecryptProperties
        /// <summary>
		/// Decrypts the Properties of an Object
		/// </summary>
		/// <param name="Target"></param>
		private static void DecryptProperties(object Target) 
        {
			if (Target == null) 
                return;
			if (Target is System.Data.DataTable) 
                return;

			object oValue = null;
			try 
            {
				Type type = Target.GetType();
				PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
				foreach (PropertyInfo prop in props) 
                {
					Type propType = EncryptableAttribute.GetPropertyType(prop.PropertyType);
					if (prop.PropertyType.IsClass
						&& propType.FullName != "System.String"
						&& propType.FullName != "System.String[]"
						&& propType.FullName != "System.Object"
						&& propType.FullName != "System.Object[]"
						&& propType.FullName.Contains("List") == false) 
					{
						oValue = prop.GetValue(Target, null);
						DecryptProperties(oValue);
						continue;
					}

					object[] attribs = prop.GetCustomAttributes(typeof(EncryptableAttribute), true);
					if (attribs.Length == 0) continue;

					EncryptableAttribute attrib = (EncryptableAttribute)attribs[0];
					oValue = prop.GetValue(Target, null);
					if (oValue != null && !string.IsNullOrEmpty(oValue.ToString())) 
                    {
						oValue = Cryptographer.Decrypt(oValue.ToString(), attrib.Key, attrib.Salt, attrib.Algorithm, true);
						SetPropertyValue(prop, Target, oValue, "");	// Set the Value
					}
				}
			}
			catch (Exception ex)
            { 
                throw ex; 
            }
        }
        #endregion

        #endregion

        #region CONSTRUCTORS

        #region EncryptableAttribute(SymmetricAlgorithms Algorithm)
        /// <summary>
		/// Initializes a new EncryptionAttribute
		/// </summary>
		public EncryptableAttribute(SymmetricAlgorithms Algorithm) 
        {
			this._algorithm = Algorithm;
        }
        #endregion

        #region EncryptableAttribute(SymmetricAlgorithms Algorithm, byte[] Key, byte[] Salt)
        /// <summary>
		/// Initializes a new EncryptionAttribute
		/// </summary>
		public EncryptableAttribute(SymmetricAlgorithms Algorithm, byte[] Key, byte[] Salt) 
        {
			this._algorithm = Algorithm;
			this._key = Key;
			this._salt = Salt;
        }
        #endregion

        #endregion

    }

}
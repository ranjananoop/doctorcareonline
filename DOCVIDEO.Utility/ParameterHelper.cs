using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOCVIDEO.Utility
{
    public static class ParameterHelper
    {
        #region AddToParameters
        public static void AddToParameters(List<System.Data.SqlClient.SqlParameter> parameters, string name,
            string value)
        {


            if (!string.IsNullOrEmpty(value))
            {
                SqlParameter param = new SqlParameter(name, System.Data.SqlDbType.VarChar);
                param.Value = value;

                parameters.Add(param);
            }
        }
        #endregion

        #region AddToParameters
        public static void AddToParameters(List<System.Data.SqlClient.SqlParameter> parameters, string name,
            int value)
        {
            if (value != int.MinValue)
                parameters.Add(new System.Data.SqlClient.SqlParameter(name, value));
        }
        #endregion

        #region AddToParameters
        public static void AddToParameters(List<System.Data.SqlClient.SqlParameter> parameters, string name,
            DateTime value)
        {
            if (value != DateTime.MinValue)
                parameters.Add(new System.Data.SqlClient.SqlParameter(name, value));
        }
        #endregion

        #region AddToParameters
        public static void AddToParameters(List<System.Data.SqlClient.SqlParameter> parameters, string name,
            bool value)
        {
            parameters.Add(new System.Data.SqlClient.SqlParameter(name, value));
        }
        #endregion

        #region AddToParameters
        public static void AddToParameters(List<System.Data.SqlClient.SqlParameter> parameters, string name,
            char value)
        {
            //if (value != char.MinValue)
            parameters.Add(new System.Data.SqlClient.SqlParameter(name, value));
        }
        #endregion

        #region AddToParameters
        public static void AddToParameters(List<System.Data.SqlClient.SqlParameter> parameters, string name,
            double value)
        {
            if (value != double.MinValue)
                parameters.Add(new System.Data.SqlClient.SqlParameter(name, value));
        }
        #endregion

        #region AddToParameters
        public static void AddToParameters(List<System.Data.SqlClient.SqlParameter> parameters, string name,
            decimal value)
        {
            if (value != decimal.MinValue)
                parameters.Add(new System.Data.SqlClient.SqlParameter(name, value));
        }
        #endregion
    }
}

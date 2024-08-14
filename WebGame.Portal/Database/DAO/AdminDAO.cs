using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using TraditionGame.Utilities;

namespace Game.Accounts.Database.DAO
{
    public class AdminDAO
    {
        private static readonly Lazy<AdminDAO> _instance = new Lazy<AdminDAO>(() => new AdminDAO());

        public static AdminDAO Instance
        {
            get { return _instance.Value; }
        }

        /// <summary>
        /// thêm mới người  tài khoản admin
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="PhoneContact"></param>
        /// <param name="Email"></param>
        /// <param name="Level"></param>
        /// <param name="Status"></param>
        /// <param name="RoleID"></param>
        /// <param name="AccountID"></param>
        /// <param name="Password"></param>
        /// <param name="Wallet"></param>
        /// <param name="Response"></param>
     
        public int AdminTransferToUser2(long adminId, long receiverId, long amount, string note, int ServiceID,out long transId, out long wallet)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[8];
                pars[0] = new SqlParameter("@_AdminId", adminId);
                pars[1] = new SqlParameter("@_ReceiverId", receiverId);
                pars[2] = new SqlParameter("@_Amount", amount);
                pars[3] = new SqlParameter("@_Note", note);
                pars[4] = new SqlParameter("@_TransID", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[5] = new SqlParameter("@_Wallet", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[6] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[7] = new SqlParameter("@_ServiceID", ServiceID);
                db.ExecuteNonQuerySP("SP_Admin_Transfer_To_User_Cskh", pars);
                transId = ConvertUtil.ToLong(pars[4].Value);
                wallet = ConvertUtil.ToLong(pars[5].Value);
                return ConvertUtil.ToInt(pars[6].Value);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                transId = 0;
                wallet = 0;
                return -99;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }
     
    }
}
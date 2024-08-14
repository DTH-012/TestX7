using MsWebGame.CSKH.Database.DTO;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class MOMODAO
    {
       
            private static readonly Lazy<MOMODAO> _instance = new Lazy<MOMODAO>(() => new MOMODAO());

            public static MOMODAO Instance
            {
                get { return _instance.Value; }
            }



        public List<UserMomoRequest> UserMomoRequesList(  long? RequestID, long? UserID, string NickName, string RequestCode, 
            string RefKey, string RefSendKey, DateTime? FromRequestDate, DateTime? ToRequestDate,
            int? Status, int? ServiceID,  int ? PartnerID,string MomoReceive,int CurrentPage, int RecordPerpage, out int TotalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[15];
                param[0] = new SqlParameter("@_RequestID", SqlDbType.BigInt);
                param[0].Value = RequestID??(object)DBNull.Value;
                param[1] = new SqlParameter("@_RequestCode", SqlDbType.VarChar);
                param[1].Size = 50;
                param[1].Value = RequestCode??(object)DBNull.Value;
                param[2] = new SqlParameter("@_RefKey", SqlDbType.VarChar);
                param[2].Size = 250;
                param[2].Value = RefKey??(object)DBNull.Value;
                param[3] = new SqlParameter("@_RefSendKey", SqlDbType.VarChar);
                param[3].Size = 250;
                param[3].Value = RefSendKey??(object)DBNull.Value;
                param[4] = new SqlParameter("@_NickName", SqlDbType.VarChar);
                param[4].Size = 20;
                param[4].Value = NickName??(object)DBNull.Value;
                param[5] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[5].Value = UserID??(object)DBNull.Value;
                param[6] = new SqlParameter("@_Status", SqlDbType.Int);
                param[6].Value = Status??(object)DBNull.Value;
                param[7] = new SqlParameter("@_FromRequestDate", SqlDbType.DateTime);
                param[7].Value = FromRequestDate??(object)DBNull.Value;
                param[8] = new SqlParameter("@_ToRequestDate", SqlDbType.DateTime);
                param[8].Value = ToRequestDate??(object)DBNull.Value;
                param[9] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[9].Value = ServiceID??(object)DBNull.Value;
                param[10] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                param[10].Value = CurrentPage;
                param[11] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                param[11].Value = RecordPerpage;
                param[12] = new SqlParameter("@_TotalRecord", SqlDbType.Int);
                param[12].Direction = ParameterDirection.Output;
                param[13] = new SqlParameter("@_PartnerID", SqlDbType.Int);
                param[13].Value = PartnerID ?? (object)DBNull.Value;
                param[14] = new SqlParameter("@_MomoReceive", SqlDbType.VarChar);
                param[14].Size = 200;
                param[14].Value = MomoReceive ?? (object)DBNull.Value;

                var _lstUserMomoReques = db.GetListSP<UserMomoRequest>("SP_UserMomoRequest_Admin_List", param.ToArray());
                TotalRecord = ConvertUtil.ToInt(param[12].Value);
                return _lstUserMomoReques;
             
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
            TotalRecord = 0;
            return null;
        }






    }
}
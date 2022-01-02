using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace dbControl
{
	class DBCon
	{
		private static string dbInfo = "";
		private static SqlConnection connectedDB;

		public static SqlConnection GetInstance()
		{
			connectedDB = new SqlConnection(dbInfo);

			return connectedDB;
		}
	}

	class dbAccess_Select
	{
		public string Query { get; set; }
		private DataSet ds_dataset;
		//private DataTable dt;

		public DataTable AccSelect()
		{
			try
			{
				DBCon.GetInstance().Open();
			}
			catch
			{
				Console.WriteLine("이미 열려있는 커넥션");
			}

			SqlDataAdapter getData = new SqlDataAdapter(Query, DBCon.GetInstance());
			ds_dataset = new DataSet();
			getData.Fill(ds_dataset);
			DBCon.GetInstance().Close();
			return ds_dataset.Tables[0];
		}
	}

	class dbAccess_InsertUpdate
	{
		public string Query { get; set; }

		public void InsertUpdate()
		{/*
            try
            {
				SqlConnection insertcon = DBCon.GetInstance();
				SqlCommand cmd_Insert = new SqlCommand(Query, insertcon);

				insertcon.Open();
				cmd_Insert.ExecuteNonQuery();
				insertcon.Close();
			}
            catch
            {
				Console.WriteLine("DB 문제");
            }
			*/
			SqlConnection insertcon = DBCon.GetInstance();

			try
			{
				using (insertcon)
				{
					insertcon.Open();
					SqlCommand cmd = new SqlCommand();
					cmd.Connection = insertcon;
					cmd.CommandText = Query;
					cmd.ExecuteNonQuery();
					insertcon.Close();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e + "\n DB에러");
				if (insertcon.State == ConnectionState.Open)
				{
					insertcon.Close();
				}
			}
		}

	}
}

using System;
using System.IO;
using SQLite;
using Xamarin.Forms;
using Packet_Sender_Mobile.iOS;


[assembly: Dependency(typeof(SQLiteDb))]

namespace Packet_Sender_Mobile.iOS
{
	public class SQLiteDb : ISQLiteDb
	{
		public SQLiteAsyncConnection GetConnection()
		{
			var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var path = Path.Combine(documentsPath, "PSSQLite.db3");

			return new SQLiteAsyncConnection(path);
		}
	}
}

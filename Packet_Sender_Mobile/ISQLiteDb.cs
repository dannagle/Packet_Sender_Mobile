using SQLite;

namespace Packet_Sender_Mobile
{
	public interface ISQLiteDb
	{
		SQLiteAsyncConnection GetConnection();
	}
}

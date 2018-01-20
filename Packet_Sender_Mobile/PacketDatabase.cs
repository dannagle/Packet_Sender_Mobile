using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
namespace Packet_Sender_Mobile
{
    public class PacketDatabase
    {

		readonly SQLiteAsyncConnection database;

		public PacketDatabase(string dbPath)
		{
			database = new SQLiteAsyncConnection(dbPath);
			database.CreateTableAsync<Packet>().Wait();
		}

		public Task<List<Packet>> GetPacketsAsync()
		{
			return database.Table<Packet>().ToListAsync();
		}

        /*
		public Task<List<Packet>> GetPacketsNotDoneAsync()
		{
			return database.QueryAsync<Packet>("SELECT * FROM [Packet] WHERE [Done] = 0");
		}
		*/
        
        public Task<Packet> GetPacketAsync(string name)
        {
            return database.Table<Packet>().Where(i => i.name == name).FirstOrDefaultAsync();
        }

        public Task<int> SavePacketAsync(Packet item)
		{
            if (!string.IsNullOrWhiteSpace(item.name))
            {
                return database.InsertOrReplaceAsync(item);

            }
            else
            {
                return Task.FromResult(0);
            }

        }

        public void TruncatePackets()
        {
            database.DropTableAsync<Packet>().Wait();
            database.CreateTableAsync<Packet>().Wait();
        }


		public Task<int> DeletePacketAsync(Packet item)
		{
			return database.DeleteAsync(item);
		}
	}
}

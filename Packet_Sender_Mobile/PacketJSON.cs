using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;


namespace Packet_Sender_Mobile
{

    [JsonObject]
    class PacketSetJSON
    {

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }

        [JsonProperty("path")]
        public string path { get; set; }
        
        [JsonProperty("count")]
        public int count { get; set; }

        [JsonProperty("lastupdate")]
        public string lastupdate { get; set; }

        [JsonProperty("packetjson")]
        public string packetjson { get; set; }

    }






    [JsonObject]
    class PacketJSON
    {

        [JsonProperty("errorstring")]
        public string errorstring { get; set; }

        [JsonProperty("fromip")]
        public string fromip { get; set; }

        [JsonProperty("fromport")]
        public string fromport { get; set; }

        [JsonProperty("hexstring")]
        public string hexstring { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("port")]
        public string port { get; set; }

        [JsonProperty("repeat")]
        public float repeat { get; set; }

        [JsonProperty("sendresponse")]
        public string sendresponse { get; set; }

        [JsonProperty("tcporudp")]
        public string tcporudp { get; set; }

        [JsonProperty("toip")]
        public string toip { get; set; }


        public static List<Packet> ToPacketList(List<PacketJSON> jList)
        {
            var returnList = new List<Packet>();

            for (int i = 0; i < jList.Count(); i++) {
                returnList.Add(jList[i].ToPacket());
            }

            return returnList;

		}
			
         
        public Packet ToPacket()
		{
            Packet returnPkt = new Packet();
            returnPkt.name = name;
            returnPkt.hex = hexstring;
            returnPkt.fromip = fromip;
            returnPkt.toip = toip;
            returnPkt.error = errorstring;
            returnPkt.toport = Convert.ToInt32(port);
            returnPkt.method = tcporudp;
            returnPkt.sendResponse = Convert.ToInt32(sendresponse);
            returnPkt.repeat = repeat;
			returnPkt.timestamp = Packet.getTimeStamp();
			returnPkt.receiveBeforeSend = false;
			returnPkt.delayAfterConnect = 0;
			returnPkt.persistent = false;

			return returnPkt;
		}


	}


}

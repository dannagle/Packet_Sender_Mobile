using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Diagnostics;
using SQLite;


namespace Packet_Sender_Mobile
{
    public class Packet
    {

		[PrimaryKey]
		public string name { get; set; }
        [Indexed]
        public string timestamp { get; set; }
        public string fromip { get; set; }
        public int fromport { get; set; }
        public string toip { get; set; }
        public int toport { get; set; }

        private string _method;
        public string method {

            get
            {
                return this._method;

            }
            set {

                if (value.ToUpper().Contains("T"))
                {
                    _method = "TCP";
                }
                else
                {
                    _method = "UDP";
                }


            }
        }
        public string error { get; set; }


		public string hex { get; set; }
		
        public string ascii { 
            get {
                return hexToASCII(hex);
            } 

            set {
                hex = ASCIITohex(value);
                
            } }


		public string nameshort
		{
			get
			{
                int maxChars = 30;
                string value = name;
				return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
			}

		}





		public float repeat { get; set; }
        public int sendResponse { get; set; }
        public bool receiveBeforeSend { get; set; }
        public int delayAfterConnect { get; set; }
        public bool persistent { get; set; }

        public Packet()
        {
            clear();
        }


        public void SavePacket()
        {
        }


        public static string getTimeStamp()
        {
            return DateTime.Now.ToString("hh:mm:ss.ffffff tt", CultureInfo.InvariantCulture);
        }

        public bool isTCP()
        {
            return (method.ToLower() == "tcp");
        }

        public bool isUDP()
        {
            return (method.ToLower() == "udp");
        }

        public bool isSSL()
        {
            return (method.ToLower() == "ssl");
        }


        public bool unitTestConvert(string hex, string ascii) {

            string toascii = hexToASCII(hex);
            string tohex = ASCIITohex(ascii);

            if (toascii == ascii)
            {
                if (tohex == hex)
                {
                    return true;

                }
                else
                {
                    Debug.WriteLine("hex: " + hex);
                    Debug.WriteLine("tohex: " + tohex);
                    Debug.WriteLine("ascii: " + ascii);
                }
            }
            else
            {
                Debug.WriteLine("ascii: " + ascii);
                Debug.WriteLine("toascii: " + toascii);
                Debug.WriteLine("hex: " + hex);

            }




            return false;

        }

        public bool unitTests()
        {

            string asciiText1 = @"\00\01\02\03\04\05\06\07\08\t\n\0b\0c\r\0e\0f";
            string hexText1 = @"00 01 02 03 04 05 06 07 08 09 0a 0b 0c 0d 0e 0f";

            string asciiText2 = @"GET /testme HTTP/1.1\r\nHost: localhost:50003\r\nUser-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:56.0) Gecko/20100101 Firefox/56.0\r\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8\r\nAccept-Language: en-US,en;q=0.5\r\nAccept-Encoding: gzip, deflate\r\nCookie: n2300session=709b22bc-5aaa-462c-9be0-14e602a8730112924%0A\r\nConnection: keep-alive\r\nUpgrade-Insecure-Requests: 1\r\n\r\n";
            string hexText2 = @"47 45 54 20 2f 74 65 73 74 6d 65 20 48 54 54 50 2f 31 2e 31 0d 0a 48 6f 73 74 3a 20 6c 6f 63 61 6c 68 6f 73 74 3a 35 30 30 30 33 0d 0a 55 73 65 72 2d 41 67 65 6e 74 3a 20 4d 6f 7a 69 6c 6c 61 2f 35 2e 30 20 28 57 69 6e 64 6f 77 73 20 4e 54 20 31 30 2e 30 3b 20 57 69 6e 36 34 3b 20 78 36 34 3b 20 72 76 3a 35 36 2e 30 29 20 47 65 63 6b 6f 2f 32 30 31 30 30 31 30 31 20 46 69 72 65 66 6f 78 2f 35 36 2e 30 0d 0a 41 63 63 65 70 74 3a 20 74 65 78 74 2f 68 74 6d 6c 2c 61 70 70 6c 69 63 61 74 69 6f 6e 2f 78 68 74 6d 6c 2b 78 6d 6c 2c 61 70 70 6c 69 63 61 74 69 6f 6e 2f 78 6d 6c 3b 71 3d 30 2e 39 2c 2a 2f 2a 3b 71 3d 30 2e 38 0d 0a 41 63 63 65 70 74 2d 4c 61 6e 67 75 61 67 65 3a 20 65 6e 2d 55 53 2c 65 6e 3b 71 3d 30 2e 35 0d 0a 41 63 63 65 70 74 2d 45 6e 63 6f 64 69 6e 67 3a 20 67 7a 69 70 2c 20 64 65 66 6c 61 74 65 0d 0a 43 6f 6f 6b 69 65 3a 20 6e 32 33 30 30 73 65 73 73 69 6f 6e 3d 37 30 39 62 32 32 62 63 2d 35 61 61 61 2d 34 36 32 63 2d 39 62 65 30 2d 31 34 65 36 30 32 61 38 37 33 30 31 31 32 39 32 34 25 30 41 0d 0a 43 6f 6e 6e 65 63 74 69 6f 6e 3a 20 6b 65 65 70 2d 61 6c 69 76 65 0d 0a 55 70 67 72 61 64 65 2d 49 6e 73 65 63 75 72 65 2d 52 65 71 75 65 73 74 73 3a 20 31 0d 0a 0d 0a";


            string asciiText3 = @"the quick brown fox jumped over the lazy dogs!";
            string hexText3 = @"74 68 65 20 71 75 69 63 6b 20 62 72 6f 77 6e 20 66 6f 78 20 6a 75 6d 70 65 64 20 6f 76 65 72 20 74 68 65 20 6c 61 7a 79 20 64 6f 67 73 21";

            if (!unitTestConvert(hexText1, asciiText1))
            {
                return false;
            }
            if (!unitTestConvert(hexText2, asciiText2))
            {
                return false;
            }
            if (!unitTestConvert(hexText3, asciiText3))
            {
                return false;
            }


            return true;
        }

        public void Clone(Packet packet)
        {
            name = packet.name;
            hex = packet.hex;
            fromip = packet.fromip;
            fromport = packet.fromport;
            toip = packet.toip;
            toport = packet.toport;
            error = packet.error;
            toport = packet.toport;
            method = packet.method;
            sendResponse = packet.sendResponse;
            repeat = packet.repeat;
            timestamp = packet.timestamp;
            receiveBeforeSend = packet.receiveBeforeSend;
            delayAfterConnect = packet.delayAfterConnect;
            persistent = packet.persistent;
        }

        public void clear() {

            name = "";
            hex = "";
            fromip = "";
            toip = "";
            error = "";
            toport = 55005;
            method = "tcp";
            sendResponse = 0;
            repeat = 0;
            timestamp = getTimeStamp();
            receiveBeforeSend = false;
            delayAfterConnect = 0;
            persistent = false;

        }

        //Remove double-whitespace and trims string
        public static string stringSimplified(string str) {

            string[] hexSplit = str.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            if (hexSplit.Length > 0)
            {
                string sep = " ";
                return String.Join(sep, hexSplit).Trim();
            }
            else {
                return "";
            }

        }

        public static int hexToInt(char hex)
        {

            hex = Char.ToLower(hex);

            if (hex == 'f')
            {
                return 15;
            }
            if (hex == 'e')
            {
                return 14;
            }
            if (hex == 'd')
            {
                return 13;
            }
            if (hex == 'c')
            {
                return 12;
            }
            if (hex == 'b')
            {
                return 11;
            }
            if (hex == 'a')
            {
                return 10;
            }

            return Int32.Parse("" + hex);

        }


        public static string ASCIITohex(string ascii) {


            if (ascii.Length == 0)
            {
                return "";
            }

            StringBuilder asciiText = new StringBuilder();
            asciiText.Append(ascii);
            string hexText = "";
            char charTest;
            bool msb = false;
            bool lsb = false;
            int lsbInt = 0;
            int msbInt = 0;

            // qDebug() << __FILE__ << "/" << __LINE__;

            //convert special sequences to raw numbers.
            asciiText = asciiText.Replace("\\\\", "\\5c");
            asciiText = asciiText.Replace("\\n", "\\0a");
            asciiText = asciiText.Replace("\\r", "\\0d");
            asciiText = asciiText.Replace("\\t", "\\09");

            // qDebug() << __FILE__ << "/" << __LINE__;
            if (asciiText.Length > 0)
            {
                if (asciiText[(asciiText.Length - 1)] == '\\') //last char is a slash
                {
                    asciiText.Append("00");
                }
            }

            // qDebug() << __FILE__ << "/" << __LINE__;
            if (asciiText.Length > 2)
            {
                if (asciiText[(asciiText.Length - 2)] == '\\') //second last char is a slash
                {
                    //slide 0 in between

                    // qDebug() << __FILE__ << "/" << __LINE__ <<"second last is slash";

                    charTest = asciiText[(asciiText.Length - 1)];                    
                    asciiText[asciiText.Length - 1] = '0';
                    asciiText.Append(charTest);
                }
            }
            // qDebug() << __FILE__ << "/" << __LINE__ <<"analyze" << asciiText;


            for (int i = 0; i < asciiText.Length; i++)
            {
                msb = false;
                lsb = false;
                lsbInt = 0;
                msbInt = 0;

                charTest = asciiText[(i)];

                // qDebug() << __FILE__ << "/" << __LINE__ <<"checking" << charTest;

                if (charTest == '\\')
                {
                    // qDebug() << __FILE__ << "/" << __LINE__ <<"found slash";
                    if (i + 1 < asciiText.Length)
                    {
                        msbInt = hexToInt(asciiText[(i + 1)]);
                        if (msbInt > -1)
                        {
                            msb = true;
                        }
                        // qDebug() << __FILE__ << "/" << __LINE__ <<"msb convert test is" << msb;

                    }
                    if (i + 2 < asciiText.Length)
                    {
                        lsbInt = hexToInt(asciiText[(i + 2)]);
                        if (lsbInt > -1)
                        {
                            lsb = true;
                        }
                        // qDebug() << __FILE__ << "/" << __LINE__ <<"lsb convert test is" << lsb;
                    }

                    if (msb)
                    {
                        hexText += String.Format("{0:X}", msbInt); // (string::number(msbInt, 16));
                        // qDebug() << __FILE__ << "/" << __LINE__ <<"hexText append result" << hexText;
                        i++;
                    }

                    if (lsb)
                    {
                        hexText += String.Format("{0:X}", lsbInt); //(string::number(, 16));
                        // qDebug() << __FILE__ << "/" << __LINE__ <<"hexText append" << hexText;
                        i++;
                    }

                }
                else
                {
                    // qDebug() << __FILE__ << "/" << __LINE__ <<"no slash";
                    lsbInt = ((int)charTest) & 0xff;
                    if (lsbInt > 0 && lsbInt < 16)
                    {
                        hexText += ("0");
                    }
                    hexText += String.Format("{0:X}", lsbInt); //(string::number(lsbInt, 16));
                    // qDebug() << __FILE__ << "/" << __LINE__ <<"appended lsbInt:" << string::number(lsbInt, 16);
                }

                hexText += (" ");
                // qDebug() << __FILE__ << "/" << __LINE__ <<"hex test now " << hexText;

            }

            return hexText.ToLower().Trim();
        }
        public static string hexToASCII(string hex)
        {


            List<string> hexSplit = new List<string>();
            hexSplit.Clear();

            //remove invalid characters of popular deliminators...
            hex = hex.Replace(",", " ");
            hex = hex.Replace(".", " ");
            hex = hex.Replace(":", " ");
            hex = hex.Replace(";", " ");
            hex = hex.Replace("0x", " ");
            hex = hex.Replace("x", " ");
            hex = hex.Replace("\n", " ");
            hex = hex.Replace("\r", " ");
            hex = hex.Replace("\t", " ");

            string hexText = stringSimplified(hex);
            if (hexText.Length == 0)
            {
                return "";
            }

            if ((hexText.Length % 2 != 0))
            {
                //Not divisible by 2. What should I do?
                if (!hexText.Contains(" ") && hexText.Length > 2)
                {
                    //Seems to be one big hex stream. Front-load it with a 0.
                    hexText = ("0") + hexText;
                }
            }


            if (!hexText.Contains(" ") && hexText.Length > 2 && hexText.Length % 2 == 0)
            {
                //does not contain any spaces.  Maybe one big hex stream?
                List<string> hexList = new List<string>();
                hexList.Clear();
                string append = "";
                for (int i = 0; i < hexText.Length; i += 2)
                {
                    append = "";
                    append += (hexText[i]);
                    append += (hexText[i + 1]);
                    hexList.Add(append);
                }
                string sep = " ";
                hexText = String.Join(sep, hexList).Trim();
                hex = hexText;
            }

            hexSplit = hexText.Split(new char[0], StringSplitOptions.RemoveEmptyEntries).ToList();
            string asciiText = "";
            UInt32 convertInt = 0;
            bool ok = false;
            int malformed = 0;
            bool malformedBool = false;
            char malformedChar = '\0';


            char checkSpace = hex[hex.Length - 1];
            if (checkSpace == ' ')
            {
                hexText += (" ");
            }

            hex = hexText;

            // qDebug() << __FILE__ << "/" << __LINE__  << __FUNCTION__ <<"analyze hex split" << hexSplit;

            for (int i = 0; i < hexSplit.Count; i++)
            {
                if (hexSplit[i].Length > 2)
                {
                    malformedBool = true;
                    malformed = i;
                    malformedChar = hexSplit[i][2];
                    // qDebug() << __FILE__ << "/" << __LINE__ << __FUNCTION__  << "malformed at"<< string::number(i) << "is" << malformedChar;
                    break;
                }

            }

            if (malformedBool)
            {
                string fixText = "";
                string testChar = "";

                for (int i = 0; i < malformed; i++)
                {
                    fixText += (hexSplit[i]);
                    fixText += (" ");
                }

                ok = false;


                testChar += (malformedChar);
                try
                {
                    UInt32 number = Convert.ToUInt32(malformedChar + "", 16);
                    ok = true;
                   
                }
                catch (Exception)
                {
                }


                // qDebug() << __FILE__ << "/" << __LINE__  << __FUNCTION__ << "malformed digitvalue" << malformedChar.digitValue();

                if (ok)
                {
                    fixText += (hexSplit[malformed][0]);
                    fixText += (hexSplit[malformed][1]);
                    fixText += (" ");
                    fixText += (malformedChar);
                }
                hexText = stringSimplified(fixText);
                hex = hexText;
                hexSplit = hexText.Split(null).ToList();
            }



            for (int i = 0; i < hexSplit.Count(); i++)
            {


                ok = false;

                try
                {
                    convertInt = Convert.ToUInt32(hexSplit[i] + "", 16);
                    ok = true;

                }
                catch (Exception)
                {
                }
                


                // qDebug() << __FILE__ << "/" << __LINE__ << __FUNCTION__  <<"hex at"<< string::number(i) << "is" << string::number(convertInt);
                if (ok)
                {
                    if (convertInt >= 0x20 && convertInt <= 0x7e && convertInt != '\\')
                    {
                        // qDebug() << __FILE__ << "/" << __LINE__  << __FUNCTION__ << "Converted to " << QChar(convertInt);
                        asciiText +=  ("" + ((char)(convertInt)));
                    }
                    else
                    {
                        asciiText += ("\\");
                        switch ((char)convertInt)
                        {
                            case '\n':
                                asciiText += ("n");
                                break;
                            case '\r':
                                asciiText += ("r");
                                break;
                            case '\t':
                                asciiText += ("t");
                                break;
                            case '\\':
                                asciiText += ("\\");
                                break;
                            default:
                                if (convertInt < 16)
                                {
                                    asciiText += ("0");
                                }
                                asciiText += String.Format("{0:x}", convertInt);
                                break;
                        }

                    }

                }
                else
                {
                    // qDebug() << __FILE__ << "/" << __LINE__  << __FUNCTION__ << "Convert failed";
                    hexSplit[i] = "";
                    string sep = " ";
                    hex = String.Join(sep, hexSplit);
                }

            }


            return asciiText;
        }
        public static string byteArrayToHex(byte[] data)
        {
            StringBuilder hex = new StringBuilder(data.Length * 2);
            foreach (byte b in data)
            {
                hex.Append(" ");
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString().Trim();
        }

        public static byte[] HEXtoByteArray(string hex)
        {
            //remove invalid characters of popular deliminators...
            hex = hex.Replace(",", "");
            hex = hex.Replace(".", "");
            hex = hex.Replace(":", "");
            hex = hex.Replace(";", "");
            hex = hex.Replace("0x", "");
            hex = hex.Replace("x", "");
            hex = hex.Replace("\n", "");
            hex = hex.Replace("\r", "");
            hex = hex.Replace("\t", "");
            hex = hex.Replace(" ", "");
            hex = hex.Trim().ToUpper();

            if (hex.Length % 2 == 1) {
                //prepend a 0
                hex = "0" + hex;
            }


            List<byte> arr = new List<byte>();

            for (int i = 0; i < hex.Length; i+=2)
            {
                int firstnibble = (GetHexVal(hex[i])) << 4;
                int lastnibble = GetHexVal(hex[i+1]);
                arr.Add((byte) (firstnibble | lastnibble));
            }

            return arr.ToArray();


        }

        public byte[] getByteArray()
        {            
            return HEXtoByteArray(hex);
        }


        private static int GetHexVal(char hex)
        {
            switch (hex) {
                case '0':
                    return 0;
                case '1':
                    return 1;
                case '2':
                    return 2;
                case '3':
                    return 3;
                case '4':
                    return 4;
                case '5':
                    return 5;
                case '6':
                    return 6;
                case '7':
                    return 7;
                case '8':
                    return 8;
                case '9':
                    return 9;
                case 'A':
                    return 10;
                case 'B':
                    return 11;
                case 'C':
                    return 12;
                case 'D':
                    return 13;
                case 'E':
                    return 14;
                case 'F':
                    return 15;

            }
            return 0;
        }


        public string packetimagesend
        {
            get {
                Packet testpacket = this;
                
                testpacket.fromip = "you";
                return testpacket.packetimagetraffic;

            }


        }

        public string trafficpath
        {
            get
            {
                if (fromip.ToLower() == "you")
                {
                    return "You -> " + toip + ":" + toport;
                }
                else
                {
                    return fromip + ":" + fromport + " -> You";
                }
            }
        }


        public string packetimagetraffic
        {
            get {

                string mtest = method.ToLower();
                if (fromip.ToLower() == "you") {
                    if (mtest == "tcp")
                    {
                        return "tx_tcp.png";
                    }

                    if (mtest == "udp")
                    {
                        return "tx_udp.png";
                    }

                    if (mtest == "ssl")
                    {
                        return "tx_ssl.png";
                    }

                }

                if (toip.ToLower() == "you")
                {
                    if (mtest == "tcp")
                    {
                        return "rx_tcp.png";
                    }

                    if (mtest == "udp")
                    {
                        return "rx_udp.png";
                    }

                    if (mtest == "ssl")
                    {
                        return "rx_ssl.png";
                    }

                }


                return "rx.png";
            }
        
        }


        public static List<Packet> GetDemoPackets()
        {
        			
            return new List<Packet>
            {
                new Packet { toport = 55969, method = "udp", fromip = "You", toip = "192.168.1.30",  ascii = "Hello Packet Sender" },
                new Packet { toport = 55969, method = "udp", fromip = "asdf",  toip = "192.168.1.30",  ascii = "This is another string of data" },
                new Packet { toport = 50003, method = "tcp", fromip = "You",  toip = "192.168.1.30",  ascii = "More Data-power!" },
                new Packet { toport = 50003, method = "tcp", fromip = "asdf",  toip = "192.168.1.30",  ascii = "TCP going to you" },
            };
        }

    }
}

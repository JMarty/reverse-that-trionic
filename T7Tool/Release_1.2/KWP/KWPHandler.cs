using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace T7Tool.KWP
{
    /// <summary>
    /// KWPResult represents the result returned by the request methods.
    /// </summary>
    enum KWPResult
    {
        OK,
        NOK,
        Timeout,
        DeviceNotConnected
    }

    /// <summary>
    /// KWPHandler implements messages for the KWP2000 (Key Word Protocol 2000) protocol (also called
    /// ISO 14230-4). Not all messages are implemented.
    /// </summary>
    class KWPHandler
    {
        /// <summary>
        /// IKWPDevice to be used by KWPHandler.
        /// </summary>
        private static IKWPDevice m_kwpDevice;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="a_kwpDevice">IKWPDevice to be used by KWPHandler.</param>
        public static void setKWPDevice(IKWPDevice a_kwpDevice)
        {
            m_kwpDevice = a_kwpDevice;
        }

        public static KWPHandler getInstance()
        {
            if (m_kwpDevice == null)
                throw new Exception("KWPDevice not set.");
            if (m_instance == null)
                m_instance = new KWPHandler();
            return m_instance;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        private KWPHandler()
        {
            timerDelegate = new TimerCallback(this.sendKeepAlive);
        }

        public void sendKeepAlive(Object stateInfo)
        {
            sendUnknownRequest();
        }


        /// <summary>
        /// This method starts a KWP session. It must be called before any request can be made.
        /// </summary>
        /// <returns>true on success, otherwise false.</returns>
        public bool startSession()
        {
            return m_kwpDevice.startSession();
        }

        /// <summary>
        /// This method opens the IKWPDevice used for communication.
        /// Device must be opened before any requests can be made.
        /// </summary>
        /// <returns>true on success, otherwise false.</returns>
        public bool openDevice()
        {
            return m_kwpDevice.open();
        }

        /// <summary>
        /// This method closes the IKWPDevice used for communication.
        /// </summary>
        /// <returns>true on success, otherwise false.</returns>
        public bool closeDevice()
        {
            if(m_kwpDevice != null)
                return m_kwpDevice.close();
            return false;
        }

        /// <summary>
        /// Send a request for sequrity access.
        /// This is needed to access protected functions (flash reading and writing).
        /// </summary>
        /// <returns>True on success, otherwise false.</returns>
        public bool requestSequrityAccess()
        {
            //Try method 1
            if (requestSequrityAccessLevel(1))
                return true;
            if (requestSequrityAccessLevel(2))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Send a request for a sequrity access with one out of two methods to 
        /// calculate the key.
        /// </summary>
        /// <param name="a_method">Key calculation method [1,2]</param>
        /// <returns>true if sequrity access was granted, otherwise false</returns>
        private bool requestSequrityAccessLevel(uint a_method)
        {
            KWPReply reply = new KWPReply();
            KWPResult result;
            byte[] seed = new byte[2];
            byte[] key = new byte[2];
            // Send a seed request.
            result = sendRequest(new KWPRequest(0x27, 0x05), out reply);
            if (result != KWPResult.OK)
                return false;
            if (reply.getData().Length < 2)
                return false;
            seed[0] = reply.getData()[0];
            seed[1] = reply.getData()[1];
            if (a_method == 1)
                key = calculateKey(seed, 0);
            else
                key = calculateKey(seed, 1);
            // Send key reply.
            result = sendRequest(new KWPRequest(0x27, 0x06, key), out reply);
            if (result != KWPResult.OK)
                return false;
            //Check if sequrity was granted.
            if ((reply.getMode() == 0x67) && (reply.getData()[0] == 0x34))
                return true;

            return false;
        }

        /// <summary>
        /// This method sends a request for the VIN (Vehicle ID Number).
        /// </summary>
        /// <param name="r_vin">The requested VIN.</param>
        /// <returns>KWPResult</returns>
        public KWPResult getVIN(out string r_vin)
        {
            KWPReply reply = new KWPReply();
            KWPResult result;
            result = sendRequest(new KWPRequest(0x1A,0x90), out reply);
            if (result == KWPResult.OK)
            {
                r_vin = getString(reply);
                return KWPResult.OK;
            }
            else
            {
                r_vin = "";
                return KWPResult.Timeout;
            }
        }

        /// <summary>
        /// Get E85 adaption status.
        /// </summary>
        /// <param name="r_vin">The adaptino status for E85 [.</param>
        /// <returns>KWPResult</returns>
        public KWPResult getE85AdaptionStatus(out string r_status)
        {
            KWPReply reply = new KWPReply();
            KWPResult result;
            r_status = "Error";
            result = sendRequest(new KWPRequest(0x21, 0xA5), out reply);
            if (result == KWPResult.OK)
            {
                byte[] res = reply.getData();
                if(reply.getData()[0] == 1)
                    r_status = "Forced";
                if(reply.getData()[0] == 2)
                    r_status = "Ongoing";
                if(reply.getData()[0] == 3)
                    r_status = "Completed";
                if(reply.getData()[0] == 4)
                    r_status = "Unknown";
                if(reply.getData()[0] == 5)
                    r_status = "Not started";
                return KWPResult.OK;
            }
            else
            {
                r_status = "";
                return KWPResult.Timeout;
            }
        }

        /// <summary>
        /// Force adaption for E85.
        /// </summary>
        /// <returns>KWPResult</returns>
        public KWPResult forceE85Adaption()
        {
            KWPReply reply = new KWPReply();
            KWPResult result;
            byte[] data = new byte[2]; data[0] = 0; data[1] = 0;
            result = sendRequest(new KWPRequest(0x3B, 0xA6, data), out reply);
            if (result != KWPResult.OK)
                return result;
            byte[] data2 = new byte[1]; data2[0] = 1; 
            result = sendRequest(new KWPRequest(0x3B, 0xA5, data2), out reply);
            return result;
        }

        /// <summary>
        /// This method requests the E85 level.
        /// </summary>
        /// <param name="r_level">The requested E85 level.</param>
        /// <returns>KWPResult</returns>
        public KWPResult getE85Level(out int r_level)
        {
            KWPReply reply = new KWPReply();
            KWPResult result;
            int level;
            result = sendRequest(new KWPRequest(0x21, 0xA7), out reply);
            if (result == KWPResult.OK)
            {
                level = (reply.getData()[0] << 8) | reply.getData()[1];
                r_level = level / 10;
                return KWPResult.OK;
            }
            else
            {
                r_level = 0;
                return KWPResult.Timeout;
            }
        }

        /// <summary>
        /// This method sets the E85 level.
        /// </summary>
        /// <param name="a_level">The E85 level.</param>
        /// <returns>KWPResult</returns>
        public KWPResult setE85Level(int a_level)
        {
            KWPReply reply = new KWPReply();
            KWPResult result;
            int sendlevel = a_level * 10;
            byte[] level = new byte[2];
            level[0] = (byte)(sendlevel >> 8);
            level[1] = (byte)sendlevel;
            result = sendRequest(new KWPRequest(0x3B, 0xA7, level), out reply);
            if (result == KWPResult.OK)
            {
                return KWPResult.OK;
            }
            else
            {
                return KWPResult.Timeout;
            }
        }

        /// <summary>
        /// This method sends a request for the immobilizer ID.
        /// </summary>
        /// <param name="r_immo">The requested immo ID.</param>
        /// <returns>KWPResult</returns>
        public KWPResult getImmo(out string r_immo)
        {
            KWPReply reply = new KWPReply();
            KWPResult result;
            r_immo = "";
            result = sendRequest(new KWPRequest(0x1A, 0x92), out reply);
            if (result != KWPResult.OK)
                return result;
            r_immo = getString(reply);
            return result;
        }

        /// <summary>
        /// This method sends a request to get the offset of the symbol table.
        /// </summary>
        /// <param name="r_immo">Start address of the symbol table.</param>
        /// <returns>KWPResult</returns>
        public KWPResult getSymbolTableOffset(out UInt16 r_offset)
        {
            KWPReply reply = new KWPReply();
            KWPResult result;
            r_offset = 0;
            result = sendRequest(new KWPRequest(0x1A, 0x9B), out reply);
            if (result != KWPResult.OK)
                return result;
            r_offset = getUint16(reply);
            return result;
        }

        /// <summary>
        /// This method sends a request for the software part number.
        /// </summary>
        /// <param name="r_swPartNo">The requested sofware part number.</param>
        /// <returns>KWPResult</returns>
        public KWPResult getSwPartNumber(out string r_swPartNo)
        {
            KWPReply reply = new KWPReply();
            KWPResult result;
            result = sendRequest(new KWPRequest(0x1A, 0x94), out reply);
            r_swPartNo = getString(reply);
            return result;
        }

        /// <summary>
        /// This method sends a request for the software version.
        /// </summary>
        /// <param name="r_swVersion">The requested software version.</param>
        /// <returns>KWPResult</returns>
        public KWPResult getSwVersion(out string r_swVersion)
        {
            KWPReply reply = new KWPReply();
            KWPResult result;
            r_swVersion = "";
            result = sendRequest(new KWPRequest(0x1A, 0x95), out reply);
            if (result != KWPResult.OK)
                return result;
            r_swVersion = getString(reply);
            return result;
        }

        /// <summary>
        /// This method sends a request for the software version using diagnostic routine 0x51.
        /// </summary>
        /// <param name="r_swVersion">The requested software version.</param>
        /// <returns>KWPResult</returns>
        public KWPResult getSwVersionFromDR51(out string r_swVersion)
        {
            KWPReply reply = new KWPReply();
            KWPResult result;
            r_swVersion = "";
            result = sendRequest(new KWPRequest(0x31, 0x51), out reply);
            if (result != KWPResult.OK)
                return result;
            r_swVersion = getString(reply);
            return result;
        }

        /// <summary>
        /// This method sends a request for the engine type description.
        /// </summary>
        /// <param name="r_swVersion">The requested engine type description.</param>
        /// <returns>KWPResult</returns>
        public KWPResult getEngineType(out string r_swVersion)
        {
            KWPReply reply = new KWPReply();
            KWPResult result;
            r_swVersion = "";
            result = sendRequest(new KWPRequest(0x1A, 0x97), out reply);
            if (result != KWPResult.OK)
                return result;
            r_swVersion = getString(reply);
            return result;
        }

        /// <summary>
        /// sendEraseRequest sends an erase request to the ECU.
        /// This method must be called before the ECU can be flashed.
        /// </summary>
        /// <returns>KWPResult</returns>
        public KWPResult sendEraseRequest()
        {
            KWPReply reply = new KWPReply();
            KWPReply reply2 = new KWPReply();
            KWPResult result = KWPResult.Timeout;
            int i = 0;

            //First erase message. Up to 5 retries.
            //Mode = 0x31
            //PID = 0x52
            //Expected result is 0x71
            result = sendRequest(new KWPRequest(0x31, 0x52), out reply);
            if (result != KWPResult.OK)
                return result;
            while (reply.getMode() != 0x71) 
            {
                System.Threading.Thread.Sleep(1000);
                result = sendRequest(new KWPRequest(0x31, 0x52), out reply);
                if (i++ > 15) return KWPResult.Timeout;
            }
            if (result != KWPResult.OK) 
                return result;

            //Second erase message. Up to 10 retries.
            //Mode = 0x31
            //PID = 0x53
            //Expected result is 0x71
            i = 0;
            result = sendRequest(new KWPRequest(0x31, 0x53), out reply2);
            if (result != KWPResult.OK)
                return result;
            while (reply2.getMode() != 0x71)
            {
                System.Threading.Thread.Sleep(1000);
                result = sendRequest(new KWPRequest(0x31, 0x53), out reply2);
                if (i++ > 20) return KWPResult.Timeout;
            }

            //Erase confirm message
            //Mode = 0x3E
            //Expected result is 0x7E
            result = sendRequest(new KWPRequest(0x3E, 0x53), out reply2);

            return result;
        }

        /// <summary>
        /// This method sets up the address and length for writing to flash. It must be called before
        /// the sendWriteDataRequest method is called.
        /// </summary>
        /// <param name="a_address">The addres to start writing to.</param>
        /// <param name="a_length">The length to write</param>
        /// <returns>KWPResult</returns>
        public KWPResult sendWriteRequest(uint a_address, uint a_length)
        {
            KWPReply reply = new KWPReply();
            KWPResult result;
            byte[] addressAndLength = new byte[7];
            //set address (byte 0 to 2)
            addressAndLength[0] = (byte)(a_address >> 16);
            addressAndLength[1] = (byte)(a_address >> 8);
            addressAndLength[2] = (byte)(a_address);
            //set length (byte 3 to 6);
            addressAndLength[3] = (byte)(a_length >> 24);
            addressAndLength[4] = (byte)(a_length >> 16);
            addressAndLength[5] = (byte)(a_length >> 8);
            addressAndLength[6] = (byte)(a_length);

            //Send request
            //Mode = 0x34
            //PID = no PID used by this request
            //Data = aaallll (aaa = address, llll = length)
            //Expected result = 0x74
            result = sendRequest(new KWPRequest(0x34, addressAndLength), out reply);
            if (result != KWPResult.OK)
                return result;
            if (reply.getMode() != 0x74)
                return KWPResult.NOK;
            else
                return result;
        }

        /// <summary>
        /// This method send data to be written to flash. sendWriteRequest must be called before this method.
        /// </summary>
        /// <param name="a_data">The data to be written.</param>
        /// <returns>KWPResult</returns>
        public KWPResult sendWriteDataRequest(byte[] a_data)
        {
            KWPReply reply = new KWPReply();
            KWPResult result;

            //Send request
            //Mode = 0x36
            //PID = no PID used by this request
            //Data = data to be flashed
            //Expected result = 0x76
            result = sendRequest(new KWPRequest(0x36, a_data), out reply);
            if (reply.getMode() != 0x76)
                return KWPResult.NOK;
            else
                return result;
        }

        /// <summary>
        /// This method requests data to be transmitted.
        /// </summary>
        /// <param name="a_data">The data to be transmitted.</param>
        /// <returns>KWPResult</returns>
        public KWPResult sendDataTransferRequest(out byte[] a_data)
        {
            KWPReply reply = new KWPReply();
            KWPResult result;
          

            //Send request
            //Mode = 0x36
            //PID = no PID used by this request
            //Data = no data
            //Expected result = 0x76
            result = sendRequest(new KWPRequest(0x36), out reply);
            a_data = reply.getData();
            if (reply.getMode() != 0x76)
                return KWPResult.NOK;
            else
                return result;
        }

        /// <summary>
        /// Send unknown request
        /// </summary>
        /// <returns>KWPResult</returns>
        public KWPResult sendUnknownRequest()
        {
            KWPReply reply = new KWPReply();
            KWPResult result;

            //Send request
            //Mode = 0x3E
            //PID = no PID used by this request
            //Expected result = 0x7E
            result = sendRequest(new KWPRequest(0x3E), out reply);
            if (result == KWPResult.Timeout)
                return result;
            if (reply.getMode() != 0x7E)
                return KWPResult.NOK;
            else
                return result;
        }

        /// <summary>
        /// This method send a request to download the symbol map.
        /// After this request has been made data can be fetched with a data transfer request
        /// </summary>
        /// <param name="a_data">The data to be written.</param>
        /// <returns>KWPResult</returns>
        public KWPResult sendReadSymbolMapRequest()
        {
            KWPReply reply = new KWPReply();
            KWPResult result;

            //Send request
            //Mode = 0x31
            //PID = 0x50
            //Data = data to be flashed
            //Expected result = 0x71
            result = sendRequest(new KWPRequest(0x31, 0x50), out reply);
            if (result == KWPResult.Timeout)
                return result;
            if (reply.getMode() != 0x71)
                return KWPResult.NOK;
            else
                return result;
        }

        /// <summary>
        /// This method send a request for reading from ECU memory (both RAM and flash). 
        /// It sets up start address and the length to read.
        /// </summary>
        /// <param name="a_address">The address to start reading from.</param>
        /// <param name="a_length">The total length to read.</param>
        /// <returns>true on success, otherwise false</returns>
        public bool sendReadRequest(uint a_address, uint a_length)
        {
            KWPReply reply = new KWPReply();
            KWPResult result;
            byte[] lengthAndAddress = new byte[5];
            //set length (byte 0 and 1)
            lengthAndAddress[0] = (byte)(a_length >> 8);
            lengthAndAddress[1] = (byte)(a_length);
            //set address (byte 2 to 4);
            lengthAndAddress[2] = (byte)(a_address >> 16);
            lengthAndAddress[3] = (byte)(a_address >> 8);
            lengthAndAddress[4] = (byte)(a_address);
            result = sendRequest(new KWPRequest(0x2C, 0xF0, 0x03, lengthAndAddress), out reply);
            if (result == KWPResult.Timeout)
                return false;
            if (result == KWPResult.OK)
                return true;
            else
                return false;
        }

        /// <summary>
        /// This method send a request for reading a symbol. 
        /// </summary>
        /// <param name="a_address">The symbol number to read [0..0xFFFF-1].</param>
        /// <returns></returns>
        public bool setSymbolRequest(uint a_symbolNumber)
        {
            KWPReply reply = new KWPReply();
            KWPResult result;
            byte[] symbolNumber = new byte[5];
            //First two bytes should be zero
            symbolNumber[0] = 0;
            symbolNumber[1] = 0;
            symbolNumber[2] = 0x80;
            //set symbol number (byte 2 to 3);
            symbolNumber[3] = (byte)(a_symbolNumber >> 8);
            symbolNumber[4] = (byte)(a_symbolNumber);
            result = sendRequest(new KWPRequest(0x2C, 0xF0, 0x03, symbolNumber), out reply);
            if (result == KWPResult.OK)
                return true;
            else
                return false;
        }

        /// <summary>
        /// This method writes to a symbol in RAM.
        /// The ECU must not be write protected for this to work.
        /// </summary>
        /// <param name="a_symbolNumber">Symbol number to write to.</param>
        /// <param name="a_data">Data to write.</param> 
        /// <returns></returns>
        public bool writeSymbolRequest(uint a_symbolNumber, byte[] a_data)
        {
            KWPReply reply = new KWPReply();
            KWPResult result;
            byte[] symbolNumberAndData = new byte[3 + a_data.Length];
            //First two bytes should be the symbol number
            symbolNumberAndData[0] = (byte)(a_symbolNumber >> 8);
            symbolNumberAndData[1] = (byte)(a_symbolNumber);
            symbolNumberAndData[2] = (byte)(0);
            for (int i = 0; i < a_data.Length; i++)
                symbolNumberAndData[i + 3] = a_data[i];
            result = sendRequest(new KWPRequest(0x3D, 0x80, symbolNumberAndData), out reply);
            if (result != KWPResult.OK)
                return false;
            if (reply.getData()[0] == 0x7D)
                return true;
            else
                return false;
        }

        /// <summary>
        /// This method sends a request to exit data transfer exit. It should be called when a 
        /// read session has been finished.
        /// </summary>
        /// <returns>true on success, otherwise false.</returns>
        public bool sendDataTransferExitRequest()
        {
            KWPReply reply = new KWPReply();
            KWPResult result;
            result = sendRequest(new KWPRequest(0x82, 0x00), out reply);
            if (result == KWPResult.OK)
                return true;
            else 
                return false;
        }

        /// <summary>
        /// This method send a request to receive data from flash. The sendReadRequest
        /// method must be called before this.
        /// </summary>
        /// <param name="r_data">The requested data.</param>
        /// <returns></returns>
        public bool sendRequestDataByOffset(out byte[] r_data)
        {
            KWPReply reply = new KWPReply();
            KWPResult result;
            result = sendRequest(new KWPRequest(0x21, 0xF0), out reply);
            if (result == KWPResult.Timeout)
            {
                r_data = reply.getData();
                return false;
            }
            r_data = reply.getData();
            if (result == KWPResult.OK)
                return true;
            else 
                return false;
        }

        public static void startLogging()
        {
            DateTime dateTime = DateTime.Now;
            String fileName = "kwplog.txt";
            if (!File.Exists(fileName))
                File.Create(fileName);
            m_logFileStream = new StreamWriter(fileName);
            m_logFileStream.WriteLine("New logging started: " + dateTime);
            m_logFileStream.WriteLine();
            m_logginEnabled = true;
        }

        public static void stopLogging()
        {
            m_logginEnabled = false;
            if(m_logFileStream != null)
                m_logFileStream.Close();
        }

        /// <summary>
        /// This method sends a KWPRequest and returns a KWPReply.
        /// </summary>
        /// <param name="a_request">The request.</param>
        /// <param name="a_reply">The reply.</param>
        /// <returns>KWPResult</returns>
        private KWPResult sendRequest(KWPRequest a_request, out KWPReply a_reply)
        {
            KWPReply reply = new KWPReply();
            RequestResult result;
            a_reply = new KWPReply();
            if(stateTimer == null)
                stateTimer = new System.Threading.Timer(sendKeepAlive, new Object(), 1000, 1000);
            stateTimer.Change(10000, 1000);
            m_requestMutex.WaitOne();
            if (!m_kwpDevice.isOpen())
                return KWPResult.DeviceNotConnected;
            if (m_logginEnabled)
                m_logFileStream.WriteLine(a_request.ToString());
            result = m_kwpDevice.sendRequest(a_request, out reply);
            a_reply = reply;
            if (result == RequestResult.NoError)
            {
                if (m_logginEnabled)
                {
                    m_logFileStream.WriteLine(reply.ToString());
                    m_logFileStream.WriteLine();
                    m_logFileStream.Flush();
                }
                m_requestMutex.ReleaseMutex();
                stateTimer.Change(1000, 1000);
                return KWPResult.OK;
            }
            else
            {
                if (m_logginEnabled)
                    m_logFileStream.WriteLine("Timeout");
                m_requestMutex.ReleaseMutex();
                stateTimer.Change(1000, 1000);
                return KWPResult.Timeout;
            }
        }

        /// <summary>
        /// Helper method for transforming the information in a KWPReply to a string.
        /// </summary>
        /// <param name="a_reply">The KWPReply.</param>
        /// <returns>A string representing the information in the a_reply.</returns>
        private string getString(KWPReply a_reply)
        {
            if (a_reply.getData().Length == 0)
                return "";
            Encoding ascii = Encoding.ASCII;
            ascii.GetChars(a_reply.getData(), 0, a_reply.getData().Length);
            return ascii.GetString(a_reply.getData(), 0, a_reply.getData().Length);
        }

        private UInt16 getUint16(KWPReply a_reply)
        {
            UInt16 uinteger;
            uinteger = (UInt16)((a_reply.getData()[1] << 8) | (a_reply.getData()[0]));
            return uinteger;
        }

        /// <summary>
        /// Calculate key for a seed.
        /// </summary>
        /// <param name="a_seed">Byte array with two bytes representing the seed.</param>
        /// <param name="a_method">Type of method to use for calculation [0,1].</param>
        /// <returns>Byte array with two bytes representing the key.</returns>
        private byte[] calculateKey(byte[] a_seed, uint a_method)
        {
            int key;
            byte[] returnKey = new byte[2];
            int seed = a_seed[0] << 8 | a_seed[1];
    
            key = seed << 2;
            key &= 0xFFFF;
            key ^= (a_method == 1 ? 0x4081 : 0x8142);
            key -= (a_method == 1 ? 0x1F6F : 0x2356);
            key &= 0xFFFF;

            returnKey[0] = (byte)((key >> 8) & 0xFF);
            returnKey[1] = (byte)(key & 0xFF);

            return returnKey;
        }

        private static bool m_logginEnabled = false;
        private static StreamWriter m_logFileStream;
        private static KWPHandler m_instance;
        private Mutex m_requestMutex = new Mutex();
        private TimerCallback timerDelegate;
        private System.Threading.Timer stateTimer;

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using T5CANLib.CAN;
using System.Threading;

namespace T5CANLib
{
    public class T5CAN
    {
        private ICANDevice m_canDevice;
        private CANListener m_canListener;
        private string CR = "\r";
        private string NL = "\n";
        private string TIMEOUT = "timeout";

        public void setCANDevice(ICANDevice a_device)
        {
            m_canDevice = a_device;
            if(m_canListener == null)
                m_canListener = new CANListener();
            m_canDevice.addListener(m_canListener);
        }

        public bool openDevice(out string r_swVersion)
        {
            r_swVersion = "";
            if(m_canDevice.open() != OpenResult.OK)
                return false;
            return true;
        }

        public string getSymbolTable()
        {
            string symbolTable = "";
            symbolTable = sendCommand("S", 1);
            symbolTable = sendCommand(CR, "END" + CR + NL);
            return symbolTable;
        }

        public string getSWVersion()
        {
            string r_swVersion = "";
            r_swVersion = sendCommand("s", 1);
            r_swVersion = sendCommand(CR, CR + NL);
            return trimString(r_swVersion);
        }


        public byte[] readRAM(UInt16 address, uint length)
        {
            length += 5;                        //The gods wants us to read 5 extra bytes.
            byte[] data = new byte[length];
            byte[] tmpData = new byte[6];
            byte[] retData = new byte[length - 5];
            uint nrOfReads = length / 6;
            if ((length % 6) > 1)
                nrOfReads++;
            for (int i = 0; i < nrOfReads; i++)
            {
                tmpData = sendReadCommand(address);
                address += 6;
                for (int j = 0; j < 6; j++)
                {
                    if ((i * 6 + j) == length)
                    {
                        //Time to sacrifice 5 bytes to the gods.
                        for (int k = 0; k < length - 5; k++)
                            retData[k] = data[k + 5];
                        return retData;
                    }
                    data[i * 6 + j] = tmpData[j];
                }
            }

            //Time to sacrifice 5 bytes to the gods.
            for (int i = 0; i < length - 5; i++)
                retData[i] = data[i + 5];
            return retData;
        }

        /// <summary>
        /// Write a byte array to an address.
        /// </summary>
        /// <param name="address">Address. Must be greater than 0x1000</param>
        /// <param name="data">Data to be written</param>
        /// <returns></returns>
        public bool writeRam(UInt16 address, byte[] data)
        {
           /* if (address < 0x1000)
                throw new Exception("Invalid address");*/
            UInt16 sendAddress = address;
            for (int i = 0; i < data.Length; i++)
            {
                sendWriteCommand(sendAddress++, data[i]);
            }
            return true;
        }

        private bool sendWriteCommand(UInt16 address, byte data)
        {
            string addr = Convert.ToString(address, 16);
            addr = addr.ToUpper();
            string sendData = Convert.ToString(data, 16);
            while(addr.Length < 4)
                addr = "0" + addr;
            if (sendData.Length < 2)
                sendData = "0" + sendData;
            sendCommandByteForRead("W"[0]);
            waitNoAck();
            sendCommandByteForRead(addr.Substring(0, 1)[0]);
            waitNoAck();
            sendCommandByteForRead(addr.Substring(1, 1)[0]);
            waitNoAck();
            sendCommandByteForRead(addr.Substring(2, 1)[0]);
            waitNoAck();
            sendCommandByteForRead(addr.Substring(3, 1)[0]);
            waitNoAck();
            sendCommandByteForRead(sendData.Substring(0, 1)[0]);
            waitNoAck();
            sendCommandByteForRead(sendData.Substring(1, 1)[0]);
            waitNoAck();
            sendCommandByteForRead(CR[0]);
            waitNoAck(); 

            return true;
        }

        private byte[] sendReadCommand(UInt16 address)
        {
            byte[] retData = new byte[6];
            CANMessage msg = new CANMessage(0x005, 0, 8);
            ulong cmd = 0x77C4FC00000000C7;
            cmd |= (ulong)((byte)(address)) << 4 * 8;
            cmd |= (ulong)((byte)(address >> 8)) << 3*8;
            msg.setData(cmd);
            if (!m_canDevice.sendMessage(msg))
                throw new Exception("Couldn't send message");
            CANMessage response = new CANMessage();
            response = m_canListener.waitForMessage(0x00C, 1000);
            ulong data = response.getData();
            for (int i = 2; i < 8; i++)
                retData[7 - i] = (byte)(data >> i * 8);

            return retData;
        }

        private string sendCommand(string a_command)
        {
            string retString = "";
            sendCommandByte(a_command[0]);
            retString = waitForResponse();
            return retString;
        }

        private void sendNoAck(string a_command)
        {
            sendCommandByte(a_command[0]);
        }

        private string sendCommand(string a_command, uint a_nrOfBytes)
        {
            string retString = "";
            string str = "";
            sendCommandByte(a_command[0]);
            for (int i = 0; i < a_nrOfBytes; i++ )
            {
                str = waitForResponse();
                if (str.Equals(TIMEOUT))
                {
                    return retString;
                }
                retString += str;
            }
            return retString;
        }


        private string sendCommand(string a_command, string a_endChar)        
        {            
            string retString = "";            
            sendCommandByte(a_command[0]);            
            string recChar = "";            
            do            
            {                
                recChar = waitForResponse();                
                if (recChar.Equals(TIMEOUT))                
                {                                     
                    return retString;                
                }                
                retString += recChar;                
                if(!a_endChar.EndsWith(CR+NL))      //Not variable length.                
                {                    
                    if (retString.Length > a_endChar.Length)                    
                    {                                               
                        return retString;                    
                    }                
                }            
            }            
            while (!retString.EndsWith(a_endChar));                        
            return retString;        
        } 

        private void sendCommandByte(char a_commandByte)
        {
            CANMessage msg = new CANMessage(0x005, 0, 8);
            ulong cmd = 0x0000000000000000;
            cmd |= (ulong)a_commandByte;
            cmd <<= 8;
            cmd |= (ulong)0xC4;
            cmd |= 0xFFFFFFFFFFFF0000;
            msg.setData(cmd);
            uint nrOfResends = 0;
            while (!m_canDevice.sendMessage(msg))
            {
                if(nrOfResends++ > 50000)
                    throw new Exception("Couldn't send message"); ;
            }
        }

        private void sendCommandByteForRead(char a_commandByte)
        {
            CANMessage msg = new CANMessage(0x006, 0, 2);
            ulong cmd = 0x0000000000000000;
            cmd |= (ulong)a_commandByte;
            cmd <<= 8;
            cmd |= (ulong)0xC4;
            msg.setData(cmd);
            if (!m_canDevice.sendMessage(msg))
                throw new Exception("Couldn't send message");

        }

        private string waitForResponse()
        {
            string returnString = "";
            bool timeout = false;
            CANMessage response = new CANMessage();
            response = m_canListener.waitForMessage(0x00C, 1000);
            if (timeout)
            {
                return TIMEOUT;
            }
            if ((byte)response.getData() != 0xC6)
                throw new Exception("Error receiveing data");
            if (response.getLength() < 8)
            {
                returnString = "";
                return returnString;
            }
            returnString += (char)((response.getData() >> 16) & 0xFF);
            sendAck();
            return returnString;
        }

        private string waitNoAck()
        {
            string returnString = "";
            bool timeout = false;
            CANMessage response = new CANMessage();
            response = m_canListener.waitForMessage(0x00C, 1000);
            if (timeout)
            {
                return TIMEOUT;
            }
            if ((byte)response.getData() != 0xC6)
                throw new Exception("Error receiveing data");
            if (response.getLength() < 8)
            {
                returnString = "";
                return returnString;
            }
            returnString += (char)((response.getData() >> 16) & 0xFF);
            return returnString;
        }
        

        private void sendAck()
        {
            if (m_canDevice == null)
                throw new Exception("CAN device not set");
            CANMessage ack = new CANMessage(0x006, 0, 2);
            ack.setData(0x00000000000000C6);
            if (!m_canDevice.sendMessage(ack))
                throw new Exception("Couldn't send message");
        }

        private void findSynch()
        {
            //System.console.WriteLine("###### Looking for synch ######");
            string str = "";
            char ch;
            bool timeout = false;
            CANMessage ack = new CANMessage(0x006, 0, 2);
            CANMessage response = new CANMessage();
            ack.setData(0x00000000000000C6);
            do
            {
                if (!m_canDevice.sendMessage(ack))
                    throw new Exception("Couldn't send message");
                response = m_canListener.waitForMessage(0x00C, 1000);
                if (timeout)
                {
                    return;
                }
                if ((byte)response.getData() != 0xC6)
                    throw new Exception("Error receiveing data");
                ch = (char)((response.getData() >> 16) & 0xFF);
                str += ch;
                if (str.EndsWith(CR + NL + NL + NL))
                {
                    return;
                }
                if (str.EndsWith(NL + NL + NL + NL))
                    str = sendCommand(CR, 1);
            }
            while (!str.EndsWith("END" + CR + NL));
        }

        string trimString(string a_string)
        {
            a_string = a_string.TrimStart('>');
            a_string = a_string.TrimEnd('\n');
            a_string = a_string.TrimEnd('\r');
            return a_string;
        }
    }
}

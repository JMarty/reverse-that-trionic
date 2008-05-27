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
            m_canListener.setCANDevice(m_canDevice);
        }

        public bool openDevice(out string r_swVersion)
        {
            r_swVersion = "";
            m_canListener.setWaitMessageID(0x00C);
            if(m_canDevice.open() != OpenResult.OK)
                return false;

            r_swVersion = getSWVersion();

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
            byte[] data = new byte[length];
            byte[] tmpData = new byte[6];
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
                        return data;
                    data[i * 6 + j] = tmpData[j];
                }
            }

            return data;
        }

        private byte[] sendReadCommand(UInt16 address)
        {
            bool timeout = false;
            byte[] retData = new byte[6];
            CANMessage msg = new CANMessage(0x005, 0, 8);
            ulong cmd = 0x77C4FC00000000C7;
            cmd |= (ulong)((byte)(address)) << 4 * 8;
            cmd |= (ulong)((byte)(address >> 8)) << 3*8;
            msg.setData(cmd);
            if (!m_canDevice.sendMessage(msg))
                throw new Exception("Couldn't send message");
            CANMessage response = new CANMessage();
            response = m_canListener.waitForMessage(0x00C, 1000, out timeout);
            ulong data = response.getData();
            for (int i = 2; i < 8; i++)
                retData[i - 2] = (byte)(data >> i * 8);

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
                    ////System.console.Write(" TIMEOUT ");
                    return retString;
                }
                retString += str;
            }
            //Thread.Sleep(100);
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
                    //System.console.Write(" TIMEOUT ");                    
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
            //Thread.Sleep(100);
            string str = "";
            if(a_commandByte == 0x0D)
                str = "CR";
            if(a_commandByte == 0x0A)
                str = "NL";
            //System.console.Write("\nCommand: " + str + a_commandByte);
            //System.console.Write("\nResponse:");
            CANMessage msg = new CANMessage(0x005, 0, 8);
            ulong cmd = 0x0000000000000000;
            cmd |= (ulong)a_commandByte;
            cmd <<= 8;
            cmd |= (ulong)0xC4;
            cmd |= 0xFFFFFFFFFFFF0000;
            msg.setData(cmd);
            if (!m_canDevice.sendMessage(msg))
                throw new Exception("Couldn't send message");
            
        }

        private string waitForResponse()
        {
            string returnString = "";
            bool timeout = false;
            CANMessage response = new CANMessage();
            response = m_canListener.waitForMessage(0x00C, 1000, out timeout);
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
           /* if(returnString.Equals(CR))
                System.console.Write(" CR");
            else if(returnString.Equals(NL))
                System.console.Write(" NL\n");
            else
                System.console.Write(returnString);*/
            sendAck();
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
                response = m_canListener.waitForMessage(0x00C, 1000, out timeout);
                if (timeout)
                {
                    return;
                }
                if ((byte)response.getData() != 0xC6)
                    throw new Exception("Error receiveing data");
                ch = (char)((response.getData() >> 16) & 0xFF);
                //System.console.Write(ch);
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

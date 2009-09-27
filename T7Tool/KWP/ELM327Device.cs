using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace T7Tool.KWP
{
    class ELM327Device : IKWPDevice
    {

        bool m_deviceIsOpen = false;
        SerialPort m_serialPort = new SerialPort();
        string m_portName = "";
        int m_portSpeed = 9600;

        /// <summary>
        /// Constructor for ELM327Device.
        /// </summary>
        public ELM327Device()
        {
          
        }


        public void setPort(string a_portName)
        {
            m_portName = a_portName;
        }

        public void setPortSpeed(int a_portSpeed)
        {
            m_portSpeed = a_portSpeed;
        }

        /// <summary>
        /// This method starts a new KWP session. It must be called before the sendRequest
        /// method can be called.
        /// </summary>
        /// <returns>true on success, otherwise false.</returns>
        public bool startSession()
        {
            string str = "";
            try
            {
                m_serialPort.Write("ATSP5\r");
                str = m_serialPort.ReadTo(">");

                m_serialPort.Write("ATAL\r");
                str = m_serialPort.ReadTo(">");

                m_serialPort.Write("ATSH8011F1\r");    //Set header
                str = m_serialPort.ReadTo(">");
                m_serialPort.Write("1A90\r");             //Read VIN. This is only done to initiate the bus.
                str = m_serialPort.ReadTo(">");
                if (str.StartsWith("BUS INIT: ERROR"))
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// This method sends a KWP request and returns a KWPReply. The method returns
        /// when a reply has been received, after a failure or after a timeout.
        /// The open and startSession methods must be called and returned possitive result
        /// before this method is used.
        /// </summary>
        /// <param name="a_request">The KWPRequest.</param>
        /// <param name="r_reply">The reply to the KWPRequest.</param>
        /// <returns>RequestResult.</returns>
        public RequestResult sendRequest(KWPRequest a_request, out KWPReply r_reply)
        {
            string sendString = "";
            string receiveString = "";
            byte[] reply = new byte[0xFF];
            try
            {
                for (int i = 1; i < a_request.getData().Length; i++)
                {
                    string tmpStr = a_request.getData()[i].ToString("X");
                    if (tmpStr.Length == 1)
                        sendString += "0" + tmpStr + " ";
                    else
                        sendString += tmpStr + " ";
                }
                sendString += "\r";
                
                m_serialPort.Write(sendString);
                //receiveString = "5A 90 59 53 33 45 46 35 39 45 32 33 33 30 32 30 38 32 37 \r\n\r\n";// m_serialPort.ReadTo(">");
                receiveString = m_serialPort.ReadTo(">");
                string tmpString = receiveString;
                       
                int insertPos = 1;
                string subString = "";

                while (receiveString.Length > 4)
                {
                    int index = receiveString.IndexOf(" ");
                    subString = receiveString.Substring(0, index);
                    reply[insertPos] = (byte)Convert.ToInt16("0x" + subString, 16);
                    insertPos++;
                    receiveString = receiveString.Remove(0, index + 1);
                }
                insertPos--;

                reply[0] = (byte)insertPos; //Length

                r_reply = new KWPReply(reply, a_request.getNrOfPID());
                return RequestResult.NoError;
            }
            catch (Exception)
            {
                r_reply = new KWPReply(reply, a_request.getNrOfPID());
                return RequestResult.ErrorSending;
            }
        }

        /// <summary>
        /// This method opens a KWP device for usage.
        /// </summary>
        /// <returns>true on success, otherwise false.</returns>
        public bool open()
        {
            m_serialPort.BaudRate = m_portSpeed;
            m_serialPort.Handshake = Handshake.None;
            m_serialPort.ReadTimeout = 3000;
            m_serialPort.Parity = Parity.None;
            m_serialPort.StopBits = StopBits.One;
            m_serialPort.DtrEnable = true;
            m_serialPort.RtsEnable = true;
            bool readException = false;
            string str;

            string port = m_portName;
            readException = false;
            if (m_serialPort.IsOpen)
                m_serialPort.Close();
            m_serialPort.PortName = port;

            try
            {
                m_serialPort.Open();
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }

            if (m_portSpeed == 38400)
            {
                m_serialPort.BaudRate = 57600;
                m_serialPort.Write("ATZ\r");    //Reset all
                Thread.Sleep(3000);
                m_serialPort.BaudRate = 38400;
            }
            m_serialPort.Write("ATZ\r");    //Reset all
            Thread.Sleep(3000);

            //Try to set up ELM327
            try
            {
                str = m_serialPort.ReadTo(">");
            }
            catch(Exception)
            {
                readException = true;
            }

            



            string answer;
            try
            {
                m_serialPort.Write("ATL1\r");   //Linefeeds On
                str = m_serialPort.ReadTo(">");
                m_serialPort.Write("ATE0\r");   //Echo off
                str = m_serialPort.ReadTo(">");
                m_serialPort.Write("ATAT2\r");   //Automatic timing
                str = m_serialPort.ReadTo(">");
                string localStr = "";
                if (m_portSpeed == 38400)  //Try setting the speed to 57600
                {
                    m_serialPort.Write("AT BRD 45\r");   //Automaic timing
                    str = m_serialPort.ReadLine();
                    if (str.StartsWith("OK"))
                    {
                        try
                        {
                            m_serialPort.BaudRate = 57600;
                            Thread.Sleep(100);
                            localStr += m_serialPort.ReadLine() +"#";
                            m_serialPort.Write("\r");
                            localStr += m_serialPort.ReadTo(">");
                        }
                        catch (Exception e)
                        {
                            //Could not connect at 57600 baud
                            m_serialPort.BaudRate = 38400;
                        }
                    }
                }
                m_serialPort.Write("ATI\r");    //Print version
                answer = m_serialPort.ReadTo(">");

            }
            catch (Exception)
            {
                return false;
            }
            if (answer.StartsWith("ELM327 v1.2") || answer.StartsWith("ELM327 v1.3"))
            {
                m_deviceIsOpen = true;
                return true;
            }

            return false;
        }

        /// <summary>
        /// This method closes a KWP device for usage.
        /// </summary>
        /// <returns>true on success, otherwise false.</returns>
        public bool close()
        {
            m_serialPort.Close();
            return true;
        }

        /// <summary>
        /// This method checks if the IKWPDevice is opened or not.
        /// </summary>
        /// <returns>true if device is open, otherwise false.</returns>
        public bool isOpen()
        {
            return m_deviceIsOpen;
        }
    }
}

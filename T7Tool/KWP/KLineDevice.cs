using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace T7Tool.KWP
{
    /// <summary>
    /// KLineDevice implements a sub set of ISO 14230-2. It works with interface adapters
    /// that do electrical transformation between RS232 and K-Line (like the VAGCOM interface) including
    /// devices that has built-in USB-RS232 adapter.
    /// </summary>
    class KLineDevice: IKWPDevice
    {

        bool m_deviceIsOpen = false;
        SerialPort m_serialPort = new SerialPort();
        string m_portName = "";

        /// <summary>
        /// Constructor for ELM327Device.
        /// </summary>
        public KLineDevice()
        {
          
        }

        public void setPort(string a_portName)
        {
            m_portName = a_portName;
        }


        /// <summary>
        /// This method starts a new KWP session. It must be called before the sendRequest
        /// method can be called.
        /// </summary>
        /// <returns>true on success, otherwise false.</returns>
        public bool startSession()
        {
            Thread.Sleep(300);                  //Bus idle 300 ms before initialization
            m_serialPort.BaudRate = 360;
            m_serialPort.DataBits = 8;
            byte[] sendByte = new byte[1];
            sendByte[0] = 0;
            try
            {
                m_serialPort.Write(sendByte, 0, 1);
            }
            catch (Exception)
            {
                return false;
            }
            //Sending 0x0 at 360 baud will take about 25 ms
            //Now wait 25 ms
            Thread.Sleep(25);
            m_serialPort.BaudRate = 10400;
            //Time to send Start Communication Request
            //Start request is always 3 byte header
            byte[] startRequest = new byte[5];
            startRequest[0] = 0x81;     //Format byte, physical addressing, one byte of data
            startRequest[1] = 0x11;     //Target address, T7 ECU
            startRequest[2] = 0xF1;     //Source address, tester
            startRequest[3] = 0x81;     //Start request service ID
            startRequest[4] = calculateChecksum(startRequest, 0, startRequest.Length - 1);

            byte[] response = new byte[8];
            try
            {
                m_serialPort.Write(startRequest, 0, startRequest.Length);
                m_serialPort.Read(response, 0, 8);
            }
            catch (Exception)
            {
                return false;
            }
            if (response[4] != 0xC1)        //C1 is positive response ID
                return false;
            m_deviceIsOpen = true;
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

            //The length of the request is Fmt + Src + Tgt + Len + Data + CRC
            //Len is included in KWPRequest.getData() (first byte)
            byte[] request = new byte[1 + 1 + 1 + a_request.getData().Length + 1];
            byte[] responseHeader = new byte[4];
            byte[] responseData;
            request[0] = 0x80;             //Format byte
            request[1] = 0x11;             //Adress of T7 ECU
            request[2] = 0xF1;             //Tester address
            for (int i = 0; i < a_request.getData().Length; i++)
                request[i + 3] = a_request.getData()[i];
            request[request.Length - 1] = calculateChecksum(request, 0, request.Length - 1);
            int length = 0;
            byte checksum = 0;
            try
            {
                m_serialPort.Write(request, 0, request.Length);

                //Start by reading header to find out how many bytes to read
                m_serialPort.Read(responseHeader, 0, 4);
                length = responseHeader[3];
                responseData = new byte[length + 1];        // Add 1 for checksum
                m_serialPort.Read(responseData, 0, responseData.Length);
            }
            catch (Exception)
            {
                r_reply = new KWPReply();
                return RequestResult.ErrorSending;
            }
           
            byte[] reply = new byte[length + 1];
            reply[0] = responseHeader[3];               //Length
            for (int i = 0; i < responseData.Length - 1; i++)
                reply[i + 1] = responseData[i];

            if ((byte)checksum != responseData[responseData.Length - 1])
            {
                r_reply = new KWPReply();
                return RequestResult.ErrorReceiving;
            }
            checksum = calculateChecksum(reply, 0, reply.Length - 1);
            r_reply = new KWPReply(reply, a_request.getNrOfPID());
            return RequestResult.NoError;
        }

        /// <summary>
        /// This method opens a KWP device for usage.
        /// </summary>
        /// <returns>true on success, otherwise false.</returns>
        public bool open()
        {
            m_serialPort.BaudRate = 10400;
            m_serialPort.Handshake = Handshake.None;
            m_serialPort.ReadTimeout = 3000;
            m_serialPort.Parity = Parity.None;
            m_serialPort.StopBits = StopBits.One;
            m_serialPort.DtrEnable = true;
            m_serialPort.RtsEnable = true;
            m_serialPort.PortName = m_portName;
            
            if (m_serialPort.IsOpen)
                m_serialPort.Close();
            
            try
            {
                m_serialPort.Open();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
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

        private byte calculateChecksum(byte[] a_data, int a_index, int a_length)
        {
            byte cs = 0;
            for (int i = a_index; i < a_length; i++)
                cs = (byte) ((a_data[i] + cs) % 256);
            return cs;
        }
    }
}

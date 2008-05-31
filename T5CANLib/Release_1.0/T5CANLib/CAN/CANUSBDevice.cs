using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace T5CANLib.CAN
{
    /// <summary>
    /// CANUSBDevice is an implementation of ICANDevice for the Lawicel CANUSB device
    /// (www.canusb.com). 
    /// 
    /// All incomming messages are published to registered ICANListeners.
    /// </summary>
    public class CANUSBDevice : ICANDevice
    {

        static uint m_deviceHandle = 0;
        Thread m_readThread;
        Object m_synchObject = new Object();
        bool m_endThread = false;

        /// <summary>
        /// Constructor for CANUSBDevice.
        /// </summary>
        public CANUSBDevice()
        {
            m_readThread = new Thread(readMessages);
        }

        /// <summary>
        /// Destructor for CANUSBDevice.
        /// </summary>
        ~CANUSBDevice()
        {
            lock (m_synchObject)
            {
                m_endThread = true;
            }
            close();
        }

        /// <summary>
        /// readMessages is the "run" method of this class. It reads all incomming messages
        /// and publishes them to registered ICANListeners.
        /// </summary>
        public void readMessages()
        {
            int readResult = 0;
            LAWICEL.CANMsg r_canMsg = new LAWICEL.CANMsg();
            CANMessage canMessage = new CANMessage();
            while (true)
            {
                lock (m_synchObject)
                {
                    if (m_endThread)
                        return;
                }
                readResult = LAWICEL.canusb_Read(m_deviceHandle, out r_canMsg);
                if (readResult > 0)
                {
                    canMessage.setID(r_canMsg.id);
                    canMessage.setLength(r_canMsg.len);
                    canMessage.setTimeStamp(r_canMsg.timestamp);
                    canMessage.setFlags(r_canMsg.flags);
                    canMessage.setData(r_canMsg.data);
                    lock (m_listeners)
                    {
                        foreach (ICANListener listener in m_listeners)
                        {
                            listener.handleMessage(canMessage);
                        }
                    }
                }
                else if (readResult == LAWICEL.ERROR_CANUSB_NO_MESSAGE)
                {
                    Thread.Sleep(1);
                }
            }
        }

        /// <summary>
        /// The open method tries to connect to both busses to see if one of them is connected and
        /// active. The first strategy is to listen for any CAN message. If this fails there is a
        /// check to see if the application is started after an interrupted flash session. This is
        /// done by sending a message to set address and length (only for P-bus).
        /// </summary>
        /// <returns>OpenResult.OK is returned on success. Otherwise OpenResult.OpenError is
        /// returned.</returns>
        override public OpenResult open()
        {
            close();
            m_deviceHandle = LAWICEL.canusb_Open(IntPtr.Zero,
                "0x40:0x37",
                LAWICEL.CANUSB_ACCEPTANCE_CODE_ALL,
                LAWICEL.CANUSB_ACCEPTANCE_MASK_ALL,
                LAWICEL.CANUSB_FLAG_TIMESTAMP);

            if (m_deviceHandle > 0)
            {
                if (m_readThread.ThreadState == ThreadState.Unstarted)
                    m_readThread.Start();
                return OpenResult.OK;
            }
            else
            {
                close();
                return OpenResult.OpenError;
            }
        }

        /// <summary>
        /// The close method closes the CANUSB device.
        /// </summary>
        /// <returns>CloseResult.OK on success, otherwise CloseResult.CloseError.</returns>
        override public CloseResult close()
        {
            int res = LAWICEL.canusb_Close(m_deviceHandle);
            m_deviceHandle = 0;
            if (LAWICEL.ERROR_CANUSB_OK == res)
            {
                return CloseResult.OK;
            }
            else
            {
                return CloseResult.CloseError;
            }
        }

        /// <summary>
        /// isOpen checks if the device is open.
        /// </summary>
        /// <returns>true if the device is open, otherwise false.</returns>
        override public bool isOpen()
        {
            if (m_deviceHandle > 0)
                return true;
            else
                return false;
        }


        /// <summary>
        /// sendMessage send a CANMessage.
        /// </summary>
        /// <param name="a_message">A CANMessage.</param>
        /// <returns>true on success, othewise false.</returns>
        override public bool sendMessage(CANMessage a_message)
        {
            LAWICEL.CANMsg msg = new LAWICEL.CANMsg();
            msg.id = a_message.getID();
            msg.len = a_message.getLength();
            msg.flags = a_message.getFlags();
            msg.data = a_message.getData();
            int writeResult;
            writeResult = LAWICEL.canusb_Write(m_deviceHandle, ref msg);
            if (writeResult == LAWICEL.ERROR_CANUSB_OK)
                return true;
            else
                return false;
        }

       

        /// <summary>
        /// waitForMessage waits for a specific CAN message give by a CAN id.
        /// </summary>
        /// <param name="a_canID">The CAN id to listen for</param>
        /// <param name="timeout">Listen timeout</param>
        /// <param name="r_canMsg">The CAN message with a_canID that we where listening for.</param>
        /// <returns>The CAN id for the message we where listening for, otherwise 0.</returns>
        private uint waitForMessage(uint a_canID, uint timeout, out LAWICEL.CANMsg r_canMsg)
        {
            int readResult = 0;
            int nrOfWait = 0;
            while (nrOfWait < timeout)
            {
                readResult = LAWICEL.canusb_Read(m_deviceHandle, out r_canMsg);
                if (readResult == LAWICEL.ERROR_CANUSB_OK)
                {
                    if (r_canMsg.id != a_canID)
                        continue;
                    return (uint)r_canMsg.id; 
                }
                else if (readResult == LAWICEL.ERROR_CANUSB_NO_MESSAGE)
                {
                    Thread.Sleep(1);
                    nrOfWait++;
                }
            }
            r_canMsg = new LAWICEL.CANMsg(); 
            return 0;
        }

        /// <summary>
        /// waitAnyMessage waits for any message to be received.
        /// </summary>
        /// <param name="timeout">Listen timeout</param>
        /// <param name="r_canMsg">The CAN message that was first received</param>
        /// <returns>The CAN id for the message received, otherwise 0.</returns>
        private uint waitAnyMessage(uint timeout, out LAWICEL.CANMsg r_canMsg)
        {
            int readResult = 0;
            int nrOfWait = 0;
            while (nrOfWait < timeout)
            {
                readResult = LAWICEL.canusb_Read(m_deviceHandle, out r_canMsg);
                if (readResult == LAWICEL.ERROR_CANUSB_OK)
                {
                    return (uint)r_canMsg.id;
                }
                else if (readResult == LAWICEL.ERROR_CANUSB_NO_MESSAGE)
                {
                    Thread.Sleep(1);
                    nrOfWait++;
                }
            }
            r_canMsg = new LAWICEL.CANMsg();
            return 0;
        }

      
    }


           
}

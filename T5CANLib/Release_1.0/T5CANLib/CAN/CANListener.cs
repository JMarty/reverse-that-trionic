using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace T5CANLib.CAN
{
    /// <summary>
    /// CANListener is used by the CANDevice for listening for CAN messages.
    /// </summary>
    class CANListener : ICANListener
    {
        private CANMessage m_canMessage = new CANMessage();
        private uint m_waitMsgID = 0;
        private AutoResetEvent m_resetEvent = new AutoResetEvent(false);

        public CANMessage waitForMessage(uint a_canID, int a_timeout)
        {
            CANMessage retMsg;
            lock (m_canMessage)
            {
                m_waitMsgID = a_canID;
            }
            m_resetEvent.WaitOne(a_timeout, false);
            lock (m_canMessage)
            {
                retMsg = m_canMessage;
            }

            return retMsg;
        }

        override public void handleMessage(CANMessage a_message)
        {
            bool messageReceived = false;
            lock (m_canMessage)
            {
                if (a_message.getID() == m_waitMsgID)
                {
                    m_canMessage = a_message;
                    messageReceived = true;
                }
            }
            if(messageReceived)
                m_resetEvent.Set();
        }
    }
}

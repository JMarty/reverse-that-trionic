using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Be.Windows.Forms;
using T7Tool.Parser;
using T7Tool.Flasher;
using T7Tool.KWP;
using System.IO;
using System.Threading;

namespace T7Tool.Forms
{
    public partial class RealTimeSymbolForm : Form
    {
        //private Be.Windows.Forms.HexBox hexBox;
        private Be.Windows.Forms.DynamicByteProvider m_dynamicByteProvider;
        private SymbolMapParser m_parser = new SymbolMapParser();
        private T7Flasher m_t7Flasher = T7Flasher.getInstance();
        private System.Threading.Timer m_stateTimer;
        private TimerCallback timerDelegate;
        private System.Threading.Timer m_symbolTimer;
        private TimerCallback symbolDelegate;
        private InfoFormStatic m_infoForm = new InfoFormStatic();
        private bool m_readingSymbolsStarted = false;
        string m_filename = "";
        private bool m_doingChange = false;

        public RealTimeSymbolForm()
        {
            InitializeComponent();
            this.hexTabPage.Controls.Add(hexBox);
            m_dynamicByteProvider = new DynamicByteProvider(new byte[0]);
            hexBox.ByteProvider = m_dynamicByteProvider;
            KWPHandler.getInstance().getSwVersion(out m_filename);
            m_filename += "SymbolTable.bin";
            KWPHandler.getInstance().requestSequrityAccess();
            if (!File.Exists(m_filename))
            {
                m_t7Flasher.readSymbolMap(m_filename);
                m_infoForm.setText("Reading symbol table");
                m_infoForm.Show();
                m_readingSymbolsStarted = true;
            }
            else
                readSymbolTable(m_filename);
            timerDelegate = new TimerCallback(this.flasherInfo);
            symbolDelegate = new TimerCallback(this.symbolInfo);
            m_stateTimer = new System.Threading.Timer(timerDelegate, new Object(), 1000, 250);
        }

        private void RealTimeSymbolForm_Load(object sender, EventArgs e)
        {
            if(m_stateTimer == null)
                m_stateTimer = new System.Threading.Timer(timerDelegate, new Object(), 1000, 250);
        }

        public void flasherInfo(Object stateInfo)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            switch (m_t7Flasher.getStatus())
            {
                case T7Flasher.FlashStatus.Completed:
                    {
                        if (m_readingSymbolsStarted == true)
                        {
                            m_readingSymbolsStarted = false;
                            m_infoForm.setText("Completed");
                            m_infoForm.Hide();
                            readSymbolTable(m_filename);
                        }
                        break;
                    }
                case T7Flasher.FlashStatus.Reading:
                    {
                        m_infoForm.Show();
                        m_infoForm.BringToFront();
                        m_infoForm.setText("Reading symbol table. kBytes: "+m_t7Flasher.getNrOfBytesRead()/1024);
                        break;
                    }
            }
        }

        private void RealTimeSymbolForm_Disposed(object sender, EventArgs e)
        {
            this.Hide();
            if (m_symbolTimer != null)
                m_symbolTimer.Dispose();
            if (m_stateTimer != null)
                m_stateTimer.Dispose();
        }

        public void readSymbolTable(string a_fileName)
        {
            m_parser.parse(a_fileName);
            List<Symbol> symbolList = m_parser.getSymbolList();
            foreach (Symbol symbol in symbolList)
            {
                symbolListBox.Items.Add(symbol.getSymbolName());
            }
        }

        private void symbolListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Symbol symbol = m_parser.getSymbol((UInt32)symbolListBox.SelectedIndex);
            SymbolNameLabel.Text = "Name: " + symbol.getSymbolName();
            SymbolLengthLabel.Text = "Length: 0x" + symbol.getDataLength().ToString("X");
            if(symbol.getRAMAddress() != 0)
                AddressLabel.Text = "Address: 0x" + symbol.getRAMAddress().ToString("X");
            else
                AddressLabel.Text = "Address: 0x" + symbol.getROMAddress().ToString("X");
            DataTypeLabel.Text = "Data type: 0x" + symbol.getSymbolType().ToString("X");

            updateSymbol();
            if(m_symbolTimer == null)
                m_symbolTimer = new System.Threading.Timer(symbolDelegate, new Object(), 250, 250);
        }

        public void symbolInfo(Object stateInfo)
        {
            if (m_doingChange)
                return;
            updateSymbol();
        }

        private void updateSymbol()
        {
            KWPHandler.getInstance().setSymbolRequest((uint)symbolListBox.SelectedIndex);
            byte[] data;
            KWPHandler.getInstance().sendRequestDataByOffset(out data);
            m_dynamicByteProvider.DeleteBytes(0, m_dynamicByteProvider.Length);
            m_dynamicByteProvider.Bytes.InsertRange(0, data);
            m_dynamicByteProvider.ApplyChanges();
            hexBox.Refresh();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void hexBox_ChangeComplete(object sender, KeyEventArgs e)
        {
            if (!e.KeyData.Equals(Keys.Enter))
                return;
            byte[] bytes = new byte[m_dynamicByteProvider.Length];
            for (int i = 0; i < m_dynamicByteProvider.Length; i++)
                bytes[i] = m_dynamicByteProvider.ReadByte(i);
            KWPHandler.getInstance().writeSymbolRequest((uint)symbolListBox.SelectedIndex, bytes);
            m_doingChange = false;
        }


        private void hexBox_Change(object sender, EventArgs e)
        {
            m_doingChange = true;
        }

    }
}
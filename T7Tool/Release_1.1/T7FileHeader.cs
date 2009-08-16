using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace T7Tool
{
    /// <summary>
    /// T7FileHeader represents the header (or, rather, tailer) of a T7 firmware file.
    /// The header contains meta data that describes some important parts of the firmware.
    /// 
    /// The header consists of several fields here represented by the FileHeaderField class.
    /// </summary>
    class T7FileHeader
    {
        string m_chassisID = "";
        string m_immobilizerID = "";
        string m_softwareVersion = "";
        string m_carDescription = "";
        long fileLength;
        int m_checksumF2;
        int m_checksumFB;
        int m_fwLength;

        /// <summary>
        /// FileHeaderField represents a field in the file header.
        /// Each field consists of a field ID, a field length and data.
        /// </summary>
        class FileHeaderField
        {
            public byte m_fieldID;
            public byte m_fieldLength;
            public byte[] m_data = new byte[255];
        }

        /// <summary>
        /// This method saves a T7 file. This method should be called after one or more fields have
        /// been changed and you want to save the result.
        /// </summary>
        /// <param name="a_filename">File name of the file where to save the T7 file.</param>
        /// <returns>true on success, otherwise false.</returns>
        public bool save(string a_filename)
        {
            if (!File.Exists(a_filename))
                File.Create(a_filename);
            FileStream fs = new FileStream(a_filename, FileMode.Open, FileAccess.ReadWrite);
            FileHeaderField fhf;
            fs.Seek(0, SeekOrigin.End);
            long writePos;
            fileLength = fs.Position;
            do
            {
                writePos = fs.Position;
                fhf = readField(fs);
                switch (fhf.m_fieldID)
                {
                    case 0x90:  setHeaderString(fhf, m_chassisID);
                                fs.Position = writePos;
                                writeField(fs, fhf);
                                break;
                    case 0x97:  setHeaderString(fhf, m_carDescription);
                                fs.Position = writePos;
                                writeField(fs, fhf);
                                break;
                    case 0x95:  setHeaderString(fhf, m_softwareVersion);
                                fs.Position = writePos;
                                writeField(fs, fhf);
                                break;
                    case 0x92:  setHeaderString(fhf, m_immobilizerID);
                                fs.Position = writePos;
                                writeField(fs, fhf);
                                break;
                    case 0xFB:  setHeaderIntValue(fhf, m_checksumFB);
                                fs.Position = writePos;
                                writeField(fs, fhf);
                                break;
                    case 0xF2:  setHeaderIntValue(fhf, m_checksumF2);
                                fs.Position = writePos;
                                writeField(fs, fhf);
                                break;
                    case 0xFE:  setHeaderIntValue(fhf, m_fwLength);
                                fs.Position = writePos;
                                writeField(fs, fhf);
                                break;
                    default:
                                break;
                }   
            }
            while (fhf.m_fieldID != 0xFF && fhf.m_fieldID != 0xF9 );    // Don't write past 0xF9 "End of header"
            fs.Close();
            return true;
        }

        /// <summary>
        /// This method initiates this class with a new T7 file.
        /// </summary>
        /// <param name="a_filename">Name of the file to read.</param>
        /// <returns>True on success, otherwise false.</returns>
        public bool init(string a_filename)
        {
            m_checksumF2 = 0;
            m_checksumFB = 0;
            m_chassisID = "";
            m_immobilizerID = "";
            m_softwareVersion = "";
            m_carDescription = "";

            if (!File.Exists(a_filename)) 
                return false;
            FileStream fs = new FileStream(a_filename, FileMode.Open, FileAccess.Read);
            FileHeaderField fhf;
            fs.Seek(0, SeekOrigin.End);
            fileLength = fs.Position;
            do
            {
                fhf = readField(fs);
                switch (fhf.m_fieldID)
                {
                    case 0x90:  m_chassisID = getHeaderString(fhf);
                                break;
                    case 0x97: m_carDescription = getHeaderString(fhf);
                                break;
                    case 0x95: m_softwareVersion = getHeaderString(fhf);
                                break;
                    case 0x92: m_immobilizerID = getHeaderString(fhf);
                                break;
                    case 0xFB: m_checksumFB = getHeaderIntValue(fhf);
                                break;
                    case 0xF2: m_checksumF2 = getHeaderIntValue(fhf);                                
                                break;
                    case 0xFE: m_fwLength = getHeaderIntValue(fhf);
                                break;
                    default:
                        break;
                }
            }
            while (fhf.m_fieldID != 0xFF && fhf.m_fieldID != 0xF9 );    // Don't read past 0xF9 "End of header"
            fs.Close();
            return true;
        }


        /// <summary>
        /// This method tranforms the data of a FileheaderField to a string.
        /// </summary>
        /// <param name="a_fileHeaderField">The FileHeaderField.</param>
        /// <returns>A string representing the information in the FileHeaderField.</returns>
        private string getHeaderString(FileHeaderField a_fileHeaderField)
        {
            Encoding ascii = Encoding.ASCII;
            ascii.GetChars(a_fileHeaderField.m_data, 0, a_fileHeaderField.m_fieldLength);
            return ascii.GetString(a_fileHeaderField.m_data, 0, a_fileHeaderField.m_fieldLength);
        }

        /// <summary>
        /// This method sets the data in a FileHeaderField to the values given by a string.
        /// </summary>
        /// <param name="a_fileHeaderField">The FileHeaderField.</param>
        /// <param name="a_string">The string to set.</param>
        private void setHeaderString(FileHeaderField a_fileHeaderField, string a_string)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(a_string);
            a_fileHeaderField.m_data = bytes;
        }

        /// <summary>
        /// This method transforms the information from a four byte field to a integer.
        /// </summary>
        /// <param name="a_fileHeaderField">The FileHeaderField.</param>
        /// <returns>The integer contained in the FileHeaderField.</returns>
        private int getHeaderIntValue(FileHeaderField a_fileHeaderField)
        {
            int intValue = 0;
            intValue |= a_fileHeaderField.m_data[0];
            intValue <<= 8;
            intValue |= a_fileHeaderField.m_data[1];
            intValue <<= 8;
            intValue |= a_fileHeaderField.m_data[2];
            intValue <<= 8;
            intValue |= a_fileHeaderField.m_data[3];
            return intValue;
        }

        /// <summary>
        /// This method sets the information in a four byte field to represent a integer value.
        /// </summary>
        /// <param name="a_fileHeaderField">The FileHeaderField.</param>
        /// <param name="a_value">The value that the field should contain.</param>
        private void setHeaderIntValue(FileHeaderField a_fileHeaderField, int a_value)
        {
            a_fileHeaderField.m_data[3] = (byte) a_value;
            a_value >>= 8;
            a_fileHeaderField.m_data[2] = (byte) a_value;
            a_value >>= 8;
            a_fileHeaderField.m_data[1] = (byte) a_value;
            a_value >>= 8;
            a_fileHeaderField.m_data[0] = (byte) a_value;
        }

        /// <summary>
        /// This method consumes the file header and returns a new FileHeaderField each
        /// time it is called until all fields has been consumed. If the last field has been
        /// read a FileHeaderField with ID=0xFF is returned.
        /// </summary>
        /// <param name="a_fileStream">The FileStream to read from.</param>
        /// <returns>A FileHeaderField.</returns>
        private FileHeaderField readField(FileStream a_fileStream)
        {
            FileHeaderField fhf = new FileHeaderField();
            a_fileStream.Position -= 1;
            fhf.m_fieldLength = (byte) a_fileStream.ReadByte();
            a_fileStream.Position -= 2;
            fhf.m_fieldID = (byte) a_fileStream.ReadByte();
            if (fhf.m_fieldID == 0xFF)
                return fhf;
            for (int i = 0; i < fhf.m_fieldLength; i++)
            {
                a_fileStream.Position -= 2;
                fhf.m_data[i] = (byte)a_fileStream.ReadByte();

            }
            a_fileStream.Position -= 1;
            return fhf;
        }

        /// <summary>
        /// This method writes a FileHeaderField to the file header.
        /// </summary>
        /// <param name="a_fileStream">The FileStream to write to.</param>
        /// <param name="a_fhf">The FileHeaderField.</param>
        private void writeField(FileStream a_fileStream, FileHeaderField a_fhf)
        {
            a_fileStream.Position -= 3;         // Skip ID and length
            for (int i = 0; i < a_fhf.m_fieldLength; i++)
            {
                a_fileStream.WriteByte(a_fhf.m_data[i]);
                a_fileStream.Position -= 2;

            }
            a_fileStream.Position += 1;
        }

        /// <summary>
        /// Get chassis ID (VIN).
        /// </summary>
        /// <returns>Chassis ID.</returns>
        public string getChassisID()
        {
            return m_chassisID;
        }

        /// <summary>
        /// Set chassis ID (VIN).
        /// </summary>
        /// <param name="a_string">The chassis ID to write.</param>
        public void setChassisID(string a_string)
        {
             m_chassisID = a_string;
        }

        /// <summary>
        /// Get immobilizer ID.
        /// </summary>
        /// <returns>Immobilizer ID.</returns>
        public string getImmobilizerID()
        {
            return m_immobilizerID;
        }

        /// <summary>
        /// Set immobilizer ID.
        /// </summary>
        /// <param name="a_string">Immobilizer ID.</param>
        public void setImmobilizerID(string a_string)
        {
            m_immobilizerID = a_string; 
        }

        /// <summary>
        /// Get software version.
        /// </summary>
        /// <returns>Software version.</returns>
        public string getSoftwareVersion()
        {
            return m_softwareVersion;
        }

        /// <summary>
        /// Set software version.
        /// </summary>
        /// <param name="a_string">Software version.</param>
        public void setSoftwareVersion(string a_string)
        {
            m_softwareVersion = a_string;
        }

        /// <summary>
        /// Get car description.
        /// </summary>
        /// <returns>Car descrption.</returns>
        public string getCarDescription()
        {
            return m_carDescription;
        }

        /// <summary>
        /// Set car description.
        /// </summary>
        /// <param name="a_string">Car description.</param>
        public void setCarDescription(string a_string)
        {
            m_carDescription = a_string;
        }

        /// <summary>
        /// Get checksum F2.
        /// </summary>
        /// <returns>Checksum F2.</returns>
        public int getChecksumF2()
        {
            return m_checksumF2;
        }

        /// <summary>
        /// Set checksum F2.
        /// </summary>
        /// <param name="a_checksum">Checksum F2.</param>
        public void setChecksumF2(int a_checksum)
        {
            m_checksumF2 = a_checksum;
        }

        /// <summary>
        /// Set checksum FB.
        /// </summary>
        /// <param name="a_checksum">Checksum FB.</param>
        public void setChecksumFB(int a_checksum)
        {
            m_checksumFB = a_checksum;
        }

        /// <summary>
        /// Get checksum FB.
        /// </summary>
        /// <returns>Checksum FB.</returns>
        public int getChecksumFB()
        {
            return m_checksumFB;
        }

        /// <summary>
        /// Get firmware length (useful size of FW).
        /// </summary>
        /// <returns>Firmware length.</returns>
        public int getFWLength()
        {
            return m_fwLength;
        }
    }



}

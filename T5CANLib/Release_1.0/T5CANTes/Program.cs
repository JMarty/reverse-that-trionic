using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using T5CANLib.CAN;
using T5CANLib;

namespace T5CANTest
{
    class Program
    {
        static void Main(string[] args)
        {
            T5CAN t5can = new T5CAN();
            CANUSBDevice canusb = new CANUSBDevice();
            string swVersion = "";
            string symbolTable = "";
            t5can.setCANDevice(canusb);
            if (!t5can.openDevice(out swVersion))
            {
                System.Console.WriteLine("Could not open CAN device");
                return;
            }

            swVersion = t5can.getSWVersion();
            System.Console.WriteLine("SW version: " + swVersion);
            System.Console.WriteLine();
           /* System.Console.WriteLine("Fetching symbol table...");
            symbolTable =t5can.getSymbolTable();
            System.Console.WriteLine("Symbol table: " + symbolTable);
            System.Console.WriteLine();*/

            UInt16 length = 32;
            /* string fileName = "dumpram.bin";
             System.Console.WriteLine("Dumping 32 kB of RAM to dumpram.bin: ");
             byte[] ram = t5can.readRAM(0x0000, length);
            
            


             if (File.Exists(fileName))
                 File.Delete(fileName);
             FileStream fileStream = File.Create(fileName, length);
             fileStream.Write(ram, 0, length);
             fileStream.Close();*/
            UInt16 address = 0x1000;
            byte[] ram = t5can.readRAM(address, 128);
            Program.printByteArray(ram);
            byte[] writeRam = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };

            t5can.writeRam(address, writeRam);
            System.Console.WriteLine();

            ram = t5can.readRAM(address, 128);
            Program.printByteArray(ram);

            Environment.Exit(0);
        }

        static private void printByteArray(byte[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (i % 16 == 0)
                    System.Console.WriteLine();
                string str = Convert.ToString(array[i], 16);
                if (str.Length == 1)
                    System.Console.Write("0");
                System.Console.Write(str + " ");

            }
        }
    }
}

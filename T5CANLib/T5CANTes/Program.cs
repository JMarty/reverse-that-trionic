using System;
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
            //symbolTable =t5can.getSymbolTable();
            //System.Console.WriteLine("Symbol table: " + symbolTable);
            //System.Console.WriteLine();

            byte[] ram = t5can.readRAM(0x0, 32);
            System.Console.WriteLine("RAM: ");
            for (int i = 0; i < ram.Length; i++)
            {
                if (i % 16 == 0)
                    System.Console.WriteLine();
                string str = Convert.ToString(ram[i], 16);
                if (str.Length == 1)
                    System.Console.Write("0");
                System.Console.Write(str + " ");
                
            }
            Environment.Exit(0);
        }
    }
}

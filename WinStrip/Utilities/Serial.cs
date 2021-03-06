﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using WinStrip.Enums;

namespace WinStrip.Utilities
{

    public class Serial
    {
        public int BaudRate = 115200;
        private SerialPort port = null;
        public int MaxBufferLength { get; private set; }
        private int MaxChunkSize { get; set; }
        private char Separator { get; set; } //chars used when writeline needs to split up a line 

        public bool isConnected { get { return port == null ? false : port.IsOpen; } }

        public Serial()
        {
            MaxChunkSize = 255;
            Separator = '@';
        }

        public string RemoveChars(string str, IEnumerable<char> toExclude)
        {
            StringBuilder sb = new StringBuilder(str.Length);
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (!toExclude.Contains(c))
                    sb.Append(c);
            }
            return sb.ToString();
        }

        public void WriteLine(string textToSend)
        {
            textToSend = RemoveChars(textToSend, new[] {'\r', '\n' });
            textToSend = textToSend.Trim();

            int len = textToSend.Length;
            if (len > MaxBufferLength)
                return; //invalid

            if (len > MaxChunkSize)
            {
                string chunk;
                int chunkLen = MaxChunkSize;
                while(len > 0)
                {
                    chunk      = textToSend.Substring(0, chunkLen);
                    textToSend = textToSend.Remove   (0, chunkLen);
                    
                    len = textToSend.Length;
                    if (len > 0)
                        chunk += Separator;
                    port.WriteLine(chunk);
                    
                    chunkLen = len < MaxChunkSize ? len : MaxChunkSize;
                }
            } else { 
                port.WriteLine(textToSend);
            }
        }

        public void WriteJson(string json)
        {
            WriteLine(json);
        }
        public string ReadLine()
        {
            var str = port.ReadLine().Trim();
            int len = str.Length;
            if (len < MaxChunkSize)
            {
                return str;
            }

            string line = str;
            str = "";
                       // MaxChunkSize + '@'
            while (len == MaxChunkSize + 1  && line.EndsWith("@"))
            {
                str += line.Substring(0, len - 1);
                line = port.ReadLine().Trim();
                len = line.Length;
            }
            str += line;
            return str;
        }

        public string ReadLine(int waitTimeInMilliseconds)
        {
            var oldTimeout = port.ReadTimeout;
            port.WriteTimeout = waitTimeInMilliseconds;
            try
            {
                return port.ReadLine().Trim();
            } catch (Exception ex){
                throw ex;
            } finally
            {
                port.ReadTimeout = oldTimeout;
            }
            
        }
        public void Close()
        {
            port.Close();
        }

        private void delay(int milliSeconds)
        {
            var now = DateTime.Now;
            var end = now.AddMilliseconds(milliSeconds);
            while (now < end)
            {
                now = DateTime.Now;
            }
        }

        private bool GetEssentialsFromDevice()
        {
            var cmd = SerialCommand.STATUS.ToString();
            port.WriteLine(cmd);
            if (ValidateSerialCommandResponse.Validate(SerialCommand.STATUS, ReadLine()))
            {
                cmd = SerialCommand.BUFFERSIZE.ToString();
                port.WriteLine(cmd);
                var strBuffer = ReadLine();
                if (!ValidateSerialCommandResponse.Validate(SerialCommand.BUFFERSIZE, strBuffer))
                    return false;
                MaxBufferLength = Convert.ToInt32(strBuffer);

                cmd = SerialCommand.SEPARATOR.ToString();
                port.WriteLine(cmd);
                strBuffer = ReadLine();
                if (!ValidateSerialCommandResponse.Validate(SerialCommand.SEPARATOR, strBuffer))
                    return false;
                Separator = strBuffer[0];
                return true;
            }
            return false;
        }
        public bool OpenSerialPort(string portName, int baudRate)
        {
            if (port != null && port.PortName == portName && isConnected)
            {
                return true; //already connected to this port
            }
            if (isConnected)
                port.Close();

            port = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);

            try
            {
                port.ReadTimeout = 1200;
                port.WriteTimeout = 1200;
                port.Open();
                var cmd = SerialCommand.STATUS.ToString();
                if (!port.IsOpen)
                    return false;

                return GetEssentialsFromDevice();
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException($"Error: Port {portName} is in use!");
            }
            catch (Exception)
            {

               //{"The read timed out."} or {"The write timed out."}

                //Try ones more because for the first time the device starts, this needs to be done twise.
                try { 
                if (GetEssentialsFromDevice())
                    return true;
                } 
                catch(Exception)
                {
                    port.Close();
                    return false;
                }

                port.Close();
                return false;
            }
        }

        public string[] getPortNames() {
            return SerialPort.GetPortNames();
        }
    }
}

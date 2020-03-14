using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace WinStrip
{

    class Serial
    {
        public int BaudRate = 115200;
        private SerialPort port = null;
        public int MaxBufferLength { get; private set; }
        private int MaxChunkSize { get; set; }
        private char Separator { get; set; } //chars used when writeline needs to split up a line 

        public bool isConnected { get { return port == null ? false : port.IsOpen; } }

        public Serial()
        {
            MaxChunkSize = 10;
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
            return port.ReadLine().Trim();
        }
        public bool OpenSerialPort(string portName, int baudRate)
        {
            if (isConnected)
                port.Close();

            port = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);

            try
            {
                port.ReadTimeout = 1200;
                port.WriteTimeout = 1200;
                port.Open();
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
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"Error: Port {portName} is in use!");
            }
            catch (Exception ex)
            {

                /*if (ex.HResult == -2146233083 || ex.HResult == -2146233083)
                {   //{"The read timed out."} or {"The write timed out."}
                    Console.WriteLine($"Port operation on {portName} took too long, {ex.Message}");
                    port.Close();
                    return false;
                }*/
                Console.WriteLine("Error opening port: " + ex);
                port.Close();
                
                return false;
            }

            return false;

        }

        public string[] getPortNames() {
            return SerialPort.GetPortNames();
        }

    }
}

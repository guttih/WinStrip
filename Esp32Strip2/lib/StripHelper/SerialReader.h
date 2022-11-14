#ifndef _SERIAL_READER_H
#define _SERIAL_READER_H

#include <Arduino.h>


class SerialReader
{

private:
    int maxBufferLength = 64;
    char separator = '@';


public:
    String inputString = "";         // a String to hold incoming data
    bool stringComplete = false;  // whether the string is complete

    SerialReader( int maxBufferLength = 3300 );
    void serialEvent();
    int getMaxLength()
    {
        return maxBufferLength;
    };
    char getSeparator()
    {
        return separator;
    };
    void resetBuffer();
};

#endif //_SERIAL_READER_H
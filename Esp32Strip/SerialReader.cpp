#include "SerialReader.h"

SerialReader::SerialReader(int maxBufferLength) {
    
    this->maxBufferLength = maxBufferLength;
    inputString.reserve(this->maxBufferLength);
}

void SerialReader::resetBuffer() {
    inputString = "";
    stringComplete = false;
}
void SerialReader::serialEvent() {

    while (Serial.available()) {
        
        char inChar = (char)Serial.read();
        
        if (inChar == '\n') {
            int len = inputString.length();
            bool end = true;

            if (len > 0) {
                char lastChar = inputString.charAt(len - 1);
                if (lastChar == separator) {
                    inputString.remove(len - 1);
                    end = false;
                }
            }
            stringComplete = end;


        }
        else {
            inputString += inChar;
        }
    } //while
}
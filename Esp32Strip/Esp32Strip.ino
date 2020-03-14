#include "SerialReader.h"

/*
 Name:		Esp32Strip.ino
 Created:	3/14/2020 11:24:27 AM
 Author:	gutti

 If you need to send longer strings than 255 split them up by adding ";" at then end of the string
     
     For example, if you want to send the string "This is a long string.\n" in three shipments,
     you could send it like this:
        1) "This is;\n"
        2) " a long;\n"
        3) " string.\n"
*/

SerialReader reader;

struct COMMAND_TEXT
{
    const char Name[20];
};

enum COMMAND
{
    COMMAND_INVALID,
    COMMAND_STATUS,
    COMMAND_BUFFERSIZE,
    COMMAND_SEPARATOR,
    COMMAND_COUNT
};

COMMAND_TEXT commands[COMMAND::COMMAND_COUNT] = {
    { "INVALID"},
    { "STATUS"},
    { "BUFFERSIZE"},
    {"SEPARATOR"}
};

void runCommand(int cmd) {
    switch (cmd) {
        case COMMAND_STATUS :       Serial.println("OK");
                                    break;

        case COMMAND_BUFFERSIZE:    Serial.println(reader.getMaxLength());
                                    break;
    
        case COMMAND_SEPARATOR:     Serial.println(reader.getSeparator());
                                    break;
    }
}

// return -1 if no command is found
int getCommand(const char *str) {
    for (int i = COMMAND::COMMAND_STATUS; i < COMMAND::COMMAND_COUNT; i++) {
        if (strcmp(str, commands[i].Name) == 0)
            return i;
    }
    return COMMAND::COMMAND_INVALID;
}





void setup() {
    // initialize serial:
    Serial.begin(500000);
    
    // reserve 200 bytes for the inputString:
    Serial.println("Max string length is " + reader.getMaxLength());
    if (reader.getMaxLength() > 255) {
        Serial.println("If you need send longer strings than 255 you will need to split the sending up and end split strings with the simbol \";\".  Do not add ; to the last part of the split string.");
    }
}

void processJson(String str) {
    Serial.println(str);
}

void processNewString() {

    int len = reader.inputString.length();
    
    if (len < 1) {
        reader.resetBuffer();
        return; //nothing to do
    }
        
    char firstChar = reader.inputString.charAt(0);

    if (firstChar == '{') {
        processJson(reader.inputString);
    } else if (len < 20) {
        int i = getCommand(reader.inputString.c_str());
        if (i > -1)
            runCommand(i);
    }

    reader.resetBuffer();
}


void loop() {
    reader.serialEvent();
    
    if (reader.stringComplete) {
        processNewString();
    }

}


#include <FastLED.h>
#include "SerialReader.h"
#include "Json.h"
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

//STRIP_TYPE, DATA_PIN, CLOCK_PIN, COLOR_SCHEME > (leds, NUM_LEDS
#include "StripHelper.h"



#define NUM_LEDS 20 
#define CLOCK_PIN 13  /*green wire*/
#define DATA_PIN  14  /*blue wire*/
/*
#define STRIP_TYPE WS2801
#define COLOR_SCHEME RBG
*/

#define STRIP_TYPE APA102
#define COLOR_SCHEME BGR 


#define BRIGHTNESS_DEVICE_PIN 15

CRGB leds[NUM_LEDS];
StripHelper stripper;


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
    COMMAND_PROGRAMCOUNT,
    COMMAND_PROGRAMINFO,
    COMMAND_ALLSTATUS,
    COMMAND_COLORS,
    COMMAND_VALUES,
    COMMAND_PIXELCOUNT,
    COMMAND_COUNT
};

COMMAND_TEXT commands[COMMAND::COMMAND_COUNT] = {
    { "INVALID"},
    { "STATUS"},
    { "BUFFERSIZE"},
    {"SEPARATOR"},
    {"PROGRAMCOUNT"},
    {"PROGRAMINFO"},
    {"ALLSTATUS"},
    {"COLORS"},
    {"VALUES"},
    {"PIXELCOUNT"}
};


void SerialPrintLine(String str) {
    
    int len = str.length();
    if (len < 255)
    {
        Serial.println(str);
        return;
    }

    //we need to send chuncks
    int MaxChunkSize = 255;
    int chunkLen = MaxChunkSize;
    String chunk;
    
    while (str.length() > 0) {
        chunk = str.substring(0, chunkLen);
        str.remove(0, chunkLen);

        len = str.length();
        if (len > 0)
            chunk += reader.getSeparator();

        Serial.println(chunk);

        chunkLen = len < MaxChunkSize ? len : MaxChunkSize;
    }
}
void runCommand(int cmd) {
    switch (cmd) {
        case COMMAND_STATUS :       Serial.println("OK");
                                    break;

        case COMMAND_BUFFERSIZE:    Serial.println(reader.getMaxLength());
                                    break;
    
        case COMMAND_SEPARATOR:     Serial.println(reader.getSeparator());
                                    break;
        case COMMAND_PROGRAMCOUNT:   Serial.println(stripper.getProgramCount());
                                    break;
        case COMMAND_PROGRAMINFO:                            
                                    SerialPrintLine(stripper.getAllProgramInfosAsJsonArray());
                                    break;
        case COMMAND_ALLSTATUS:     SerialPrintLine(stripper.toJson());
                                    break;
        case COMMAND_COLORS:        Serial.println(stripper.getColorsAsJson());
                                    break;
        case COMMAND_VALUES:        Serial.println(stripper.getValuesAsJson());
            break;
        case COMMAND_PIXELCOUNT:    Serial.println(stripper.getCount());
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


void processJson(String str) {
    //Serial.println(str);
    Json parser(str.c_str());
    if (!parser.isValid()) {
        //Serial.println("Invalid json string");
        return;
    }
        
    
    //Serial.println(parser.toString());
    JsonData *root = parser.getRootObject();
    if (root->getType() != JSONTYPE_OBJECT)
        return;
    


    JsonData* current;
    unsigned long ulCom, ulDelay, ulBrightness, ulColor;
    unsigned long stepDelay;
    const int VARIABLE_COUNT = 3;
    unsigned long values[VARIABLE_COUNT] = { 0,0,0 };
    int i;

    current = root->getChild("com");
    if (current == NULL) { return; }
    ulCom = current->getValueAsULong();

    current = root->getChild("delay");
    if (current == NULL) { return; }
    ulDelay = current->getValueAsULong();

    current = root->getChild("brightness");
    if (current == NULL) { return; }
    ulBrightness = current->getValueAsULong();


    current = root->getChild("values");
    if (current == NULL)
        current = root->getChild("values");
    if (current == NULL || current->getValueType() != JSONTYPE::JSONTYPE_ARRAY) { return; }
    current = current->getChildAt(0);//we got the key, let's get the array
    current = current->getChildAt(0);//now let's get the first item in the array

    i = 0;
    while (current && i < VARIABLE_COUNT) {
        values[i] = current->getValueAsULong();
        current = current->getNext();
        i++;
    }

    current = root->getChild("colors");
    if (current == NULL)
        current = root->getChild("colors");
    if (current == NULL || current->getValueType() != JSONTYPE::JSONTYPE_ARRAY) { return; }
    //we got the key, let's get the array
    current = current->getChildAt(0);
    //now let's get the first item in the array
    current = current->getChildAt(0);
    i = 0;
    while (current && i < COLOR_COUNT) {
        ulColor = current->getValueAsULong();
        stripper.setColorBank(i, stripper.decodeColor(ulColor));
        current = current->getNext();
        i++;
    }

    stripper.setNewValues((STRIP_PROGRAMS)ulCom, ulDelay, values[0], values[1], values[2]);
    stripper.setBrightness(ulBrightness);
    stripper.initProgram(stripper.getProgram());

    /*if ((STRIP_PROGRAMS)ulCom == STRIP_PROGRAMS::RESET) {
        devicePins.get(BRIGHTNESS_DEVICE_PIN)->setValue(stripper.getBrightness() * 4);
    }*/
    
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

void stripInit() {
    pinMode(DATA_PIN, OUTPUT);
    pinMode(CLOCK_PIN, OUTPUT);
    FastLED.addLeds<STRIP_TYPE, DATA_PIN, CLOCK_PIN, COLOR_SCHEME>(leds, NUM_LEDS);
    stripper.initialize(&FastLED);
    Serial.println("- - - - - - - -     Available strip commands     - - - - - - - -");
    Serial.println(stripper.getAllProgramNames());
    Serial.println("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
    stripper.initProgram(stripper.getProgram());
}

void setup() {
    // initialize serial:
    Serial.begin(500000);
    while (!Serial) {
        ; // wait for serial port to connect. Needed for native USB port only
    }
    // reserve 200 bytes for the inputString:
    Serial.println("Max string length is " + reader.getMaxLength());
    if (reader.getMaxLength() > 255) {
        Serial.println("If you need send longer strings than 255 you will need to split the sending up and end split strings with the simbol \";\".  Do not add ; to the last part of the split string.");
    }

    stripInit();

}

void loop() {
    reader.serialEvent();
    
    if (reader.stringComplete) {
        processNewString();
    }

    stripper.run();

}


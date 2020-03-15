#include "StripHelper.h"

StripHelper::StripHelper() {
    init();
}

void StripHelper::initialize(CFastLED* controller) {
    this->fastLED = controller;
    leds = fastLED->leds();
    ledCount = fastLED->size();
}

CFastLED* StripHelper::getController() {
    return this->fastLED;
}

void StripHelper::fillByIndex(int startIndex, int endIndex, const struct CRGB& color) {
    for (int i = startIndex; i < endIndex + 1; i++) {
        leds[i] = color;
    }
}

void StripHelper::init() {
    ledCount = 0;
    tempArray = NULL;
    reset();

}

void StripHelper::reset() {
    ulWorker1 = 0;
    iWorker1 = 0;
    lightLastTime = 0;
    step = 0;
    turns = 0;
    direction = 1;
    stepDelay = 0;
    byteWorker1 = 0;
    value1 = 0;
    value2 = 0;
    value3 = 0;
    stripMustBeOff = false;
    brightness = 1;
    program = STRIP_PROGRAMS::SINGLE_COLOR;
    stripColors[0] = CRGB::Magenta;
    stripColors[1] = CRGB::Red;
    stripColors[2] = CRGB::Green;
    stripColors[3] = CRGB::Blue;
    stripColors[4] = CRGB::White;
    stripColors[5] = CRGB::Brown;

    if (tempArray != NULL) {
        delete[] tempArray;
        tempArray = NULL;
    }
}

void StripHelper::setNewValues(STRIP_PROGRAMS program, unsigned long stepDelay, unsigned long value1, unsigned long value2, unsigned long value3) {
    this->program = program;
    this->stepDelay = stepDelay;
    this->value1 = value1;
    this->value2 = value2;
    this->value3 = value3;
}

//will reverse current direction set turns to 0
//if new direction is forward  then step will be set to first pixel index
//if new direction is backward then step will be set to the last pixel index
void StripHelper::toggleDirection() {
    step = 0;
    turns = 0;
    bool directionForward = direction == 1 ? false : true;
    if (!directionForward) {
        direction = -1;
        step = getLast();
    }
    else
        direction = 1;
}

// returns true if step had to be fixed
//return false if step was ok;

bool StripHelper::fixStep() {
    if (step >= ledCount) {
        step = 0;
        turns++;
        return false;
    }
    else if (step < 0) {
        turns--;
        step = ledCount - 1;
        return false;
    }
    return true;
}

// Increments the step and returns the incremented step
int StripHelper::stepUp() {
    step += direction;
    fixStep();
    return step;
}

// Decrements the step and returns the decremented step
int StripHelper::stepDown() {
    step += direction * -1;
    fixStep();
    return step;
}

int StripHelper::getCount() {
    return ledCount;
}


//if index out of bounds Black is returned
CRGB StripHelper::getColorBank(uint8_t index) {
    if (index >= COLOR_COUNT)
        return CRGB::Black;
    return stripColors[index];
}

bool StripHelper::setColorBank(uint8_t index, CRGB newColor) {
    if (index >= COLOR_COUNT)
        return false;
    stripColors[index] = newColor;
    return true;

}

int StripHelper::getLast() {
    return ledCount - 1;
}

int StripHelper::getStep() {
    return step;
}

int StripHelper::getTurns() {
    return turns;
}

unsigned long StripHelper::getStepDelay() {
    return stepDelay;
}
STRIP_PROGRAMS StripHelper::getProgram() {
    return program;
}

//if step was set the return is true
//if step was out of bounds and had to be fixed then return is false
bool StripHelper::setStep(int newStep) {
    step = newStep;
    return !fixStep();
}

void StripHelper::setBrightness(uint8_t brightness) {
    this->brightness = brightness;
}

uint8_t StripHelper::getBrightness() {
    return brightness;
}


int StripHelper::getDirection() {
    return direction;
}

// if forward is true then direction is forward
// if forward is true then direction is backward
int StripHelper::setDirection(bool forward) {
    direction = forward ? 1 : -1;
}

String StripHelper::getProgramName(STRIP_PROGRAMS stripProgram) {

    switch (stripProgram) {
        case OFF         : return "Off";
        case RESET       : return "Reset";
        case SINGLE_COLOR: return "Single color";
        case MULTI_COLOR : return "Multicolor";
        case UP          : return "Up";
        case DOWN        : return "Down";
        case UP_DOWN     : return "Up and down";
        case RAINBOW     : return "Rainbow";
        case CYLON       : return "Cylon";
        case STARS       : return "Stars";
        case SECTIONS    : return "Sections";

    }
    return "Invalid program";
}

String StripHelper::getProgramDescription(STRIP_PROGRAMS stripProgram) {

    switch (stripProgram) {
    case OFF: return "Turns the strip off";
    case RESET: return "resets all strip values.  Like turning the strip off, and then on again without ever loosing Wifi connection.";
    case SINGLE_COLOR: return "First color will be set to all of the strip pixels";
    case MULTI_COLOR: return "Display each of your selected colors in series through out the strip.";
    case UP: return "Color 0 is the color of a a pixel running up the strip. Color 1 is the background.";
    case DOWN: return "Color 0 is the color of a a pixel running down the strip. Color 1 is the background.";
    case UP_DOWN: return "Color 0 is the color of a a pixel running up and down the strip. Color 1 is the background.";
    case STARS: return "Color 0 is the color of a start pixel.  Color 1 is the background color.";
    case RAINBOW: return "There is no change if both values 0.  Try changing them to figure out what you like.  The program is very versatile. Test and see :)";
    case CYLON: return "Multiple color will flow down the strip.";
    case SECTIONS: return "Dived the strip to 4 color sections.  Possbile colors are from 0-4. Color 0 is for section 1, color 1 is for section 2 and so on.";
    }
    return "This program is one of the available program.  The possible programs are only from 0 to " + String(((int)STRIP_PROGRAMS::STRIP_PROGRAMS_COUNT) - 1) + ".";
}

String StripHelper::getProgramInfoAsJsonArray(STRIP_PROGRAMS stripProgram) {
    int valuesUsed = 0;
    int colors = 0;
    String values = "[";
    switch (stripProgram) {
    case OFF:
    case RESET:   break;
    case RAINBOW:   values += "\"Increment by\",\"Delta hue\"";
        break;
    case SINGLE_COLOR:   colors = 1;
        break;
    case MULTI_COLOR:   colors = COLOR_COUNT;
        values += "\"Number of colors\"";
        break;
    case UP:
    case DOWN:
    case UP_DOWN:   break;
    case STARS:   colors = 2; values += "\"Number of stars\",\"Delay between new stars\"";
        break;
    case CYLON:   colors = 0;
        //  this would be for two values values+="\"Delay between fades\",\"Something else\""; 
        break;
    case SECTIONS:   colors = 4;
        values += "\"Starting number of section 2\",\"Starting number of section 3\",\"Starting number of section 4\"";
        break;

    default:   return "{}";
    }
    values += "]";

    String ret = "{";
    ret += MakeJsonKeyVal("name", quotes(getProgramName(stripProgram)));
    ret += "," + MakeJsonKeyVal("description", quotes(getProgramDescription(stripProgram)));

    ret += "," + MakeJsonKeyVal("values", values);
    ret += "," + MakeJsonKeyVal("colors", String(colors));
    ret += "}";
    return ret;
}

String StripHelper::getAllProgramNames() {

    int len = STRIP_PROGRAMS_COUNT;
    String ret = "";
    for (int i = 0; i < len; i++) {

        if (i > 0) ret += "\n";
        if (i < 10)
            ret += " ";
        ret += String(i);
        ret += " : ";
        ret += getProgramName((STRIP_PROGRAMS)i);
    }
    return ret;
}


String StripHelper::getAllProgramNamesAsJsonArray() {

    int len = STRIP_PROGRAMS_COUNT;
    String ret = "[";
    for (int i = 0; i < len; i++) {
        if (i > 0)
            ret += ",";
        ret += "\"" + getProgramName((STRIP_PROGRAMS)i) + "\"";
    }
    ret += "]";
    return ret;
}

String StripHelper::getAllProgramInfosAsJsonArray() {

    int len = STRIP_PROGRAMS_COUNT;
    String ret = "[";
    for (int i = 0; i < len; i++) {
        if (i > 0)
            ret += ",";
        ret += getProgramInfoAsJsonArray((STRIP_PROGRAMS)i);
    }
    ret += "]";
    return ret;
}

// adds quotation mark at the beginning and the end of string
String StripHelper::quotes(String value) {

    return "\"" + value + "\"";
}

String StripHelper::MakeJsonKeyVal(String key, String Value) {

    return "\"" + key + "\":" + Value;
}

uint32_t StripHelper::encodeColor(CRGB color) {
    uint32_t uiColor = (uint32_t)color.r << 16 |
        (uint32_t)color.g << 8 |
        (uint32_t)color.b;
    return uiColor;
}

String StripHelper::ulToString(uint32_t number) {
    char buf[16];
    ltoa(number, buf, 10);
    return String(buf);
}

CRGB StripHelper::decodeColor(uint32_t uiColor) {
    uint8_t r, g, b;
    r = (uint8_t)(uiColor >> 16);
    g = (uint8_t)(uiColor >> 8);
    b = (uint8_t)uiColor;
    return CRGB(r, g, b);
}

String StripHelper::toJson() {

    int len = STRIP_PROGRAMS_COUNT;
    String ret = "{";

    ret += MakeJsonKeyVal("programs", getAllProgramInfosAsJsonArray());
    ret += "," + MakeJsonKeyVal("delay", String(stepDelay));
    ret += "," + MakeJsonKeyVal("com", String(program));
    ret += "," + MakeJsonKeyVal("values", "[" + String(value1) + "," + String(value2) + "," + String(value3) + "]");
    ret += ",\"colors\":[";
    for (int i = 0; i < COLOR_COUNT; i++) {
        if (i > 0)
            ret += ",";
        ret += ulToString(encodeColor(stripColors[i]));
    }
    ret += "]";


    ret += "}";
    return ret;
}

void StripHelper::programCylonFadeall() {
    for (int i = 0; i < this->getCount(); i++) { leds[i].nscale8(250); }
}

void StripHelper::programCylon() {
    stepUp();
    leds[getStep()] = CHSV(byteWorker1++, 255, 255);
    fastLED->show();
    programCylonFadeall();
}

void StripHelper::programMultiColor() {
    int colorCount = value1 <= COLOR_COUNT && value1 > 0 ? value1 : COLOR_COUNT;

    stepUp();
    for (int i = 0; i < this->getCount(); i++) {
        leds[i] = getColorBank(i % colorCount);
    }
    fastLED->show();
}
void StripHelper::programSections() {
    int startOfSection2 = value1 < ledCount ? value1 : ledCount - 1;
    int startOfSection3 = value2 < ledCount ? value2 : ledCount - 1;
    int startOfSection4 = value3 < ledCount ? value3 : ledCount - 1;
    stepUp();
    fillByIndex(0, startOfSection2 - 1, getColorBank(0));
    fillByIndex(startOfSection2, startOfSection3 - 1, getColorBank(1));
    fillByIndex(startOfSection3, startOfSection4 - 1, getColorBank(2));
    fillByIndex(startOfSection4, ledCount - 1, getColorBank(3));
    fastLED->show();
}

void StripHelper::programStepOne(CRGB onColor, CRGB trailColor) {

    int index = getStep();

    if (getDirection() == 1 && index == 0)
        leds[getLast()] = trailColor;
    else if (getDirection() == -1 && index == getLast())
        leds[0] = trailColor;
    else
        leds[index - getDirection()] = trailColor;
    leds[index] = onColor;
    fastLED->show();
}

void StripHelper::programUpDown() {
    programStepOne(getColorBank(0), getColorBank(1));

    stepUp();
    if (getTurns() == 1) {
        toggleDirection();
    }
    else if (getTurns() == -1) {
        leds[getLast()] = getColorBank(1);
        toggleDirection();
    }
}
void StripHelper::programOff() {
    fill_solid(leds, getCount(), CRGB::Black);
    fastLED->show();
}

int StripHelper::getProgramCount()
{
    return STRIP_PROGRAMS_COUNT;
}

void StripHelper::programSingleColor() {
    fill_solid(leds, getCount(), getColorBank(0));
    fastLED->show();
}


// Helper function that blends one uint8_t toward another by a given amount
void StripHelper::nblendU8TowardU8(uint8_t& cur, const uint8_t target, uint8_t amount)
{
    if (cur == target) return;

    if (cur < target) {
        uint8_t delta = target - cur;
        delta = scale8_video(delta, amount);
        cur += delta;
    }
    else {
        uint8_t delta = cur - target;
        delta = scale8_video(delta, amount);
        cur -= delta;
    }
}

// Blend one CRGB color toward another CRGB color by a given amount.
// Blending is linear, and done in the RGB color space.
// This function modifies 'cur' in place.
CRGB StripHelper::fadeTowardColor(CRGB& cur, const CRGB& target, uint8_t amount)
{
    nblendU8TowardU8(cur.red, target.red, amount);
    nblendU8TowardU8(cur.green, target.green, amount);
    nblendU8TowardU8(cur.blue, target.blue, amount);
    return cur;
}

void StripHelper::programStarsSelectNewStars() {
    unsigned int starCount = value1, index;

    unsigned int sections = getCount() / starCount;
    for (unsigned int i = 0; i < starCount; i++) {
        index = random16(i * sections, i * sections + (sections - 1));
        tempArray[i] = index;
    }
}

void StripHelper::programStarsInit() {

    ulWorker1 = 0;
    iWorker1 = 1;
    unsigned int starCount = value1;

    setDirection(true);  toggleDirection();
    if (starCount >= getCount())
        return; //nuts

    if (tempArray != NULL) {
        delete[] tempArray;
        tempArray = NULL;
    }

    if (starCount < 1)
        return;

    tempArray = new unsigned int[starCount]();

    programStarsSelectNewStars();

}

void StripHelper::programRainbow() {
    stepUp();
    fill_rainbow(leds, getCount(), byteWorker1 += value1, value2);
    fastLED->show();
}

void StripHelper::programStars() {
    if (getProgram() != STRIP_PROGRAMS::STARS || tempArray == NULL || value1 < 1) {

        return; //to be sure that the temArray slots exit
    }

    fill_solid(leds, getCount(), getColorBank(1));
    if (ulWorker1 > 30) {
        iWorker1 = -1;
    }

    ulWorker1 += iWorker1;
    unsigned int starCount = value1,
        fadeDelay = value2;

    CRGB starColor = getColorBank(0);
    CRGB backColor = getColorBank(1);
    starColor = fadeTowardColor(backColor, starColor, ulWorker1 * 8);
    for (unsigned int i = 0; i < starCount; i++) {
        leds[tempArray[i]] = starColor;
    }
    fastLED->show();

    if (ulWorker1 == 0 && iWorker1 == -1) {

        programStarsSelectNewStars();
        iWorker1 = 1;
        delay(fadeDelay);
    }
}

void StripHelper::run() {
    if (stripMustBeOff) {
        programOff();
        return;
    }
    unsigned long time = millis();

    if (lightLastTime + getStepDelay() > time)
        return;

    if (fastLED->getBrightness() != getBrightness()) {
        fastLED->setBrightness(getBrightness());
    }


    lightLastTime = time;
    switch (getProgram()) {
    case OFF: programOff(); break;
    case SINGLE_COLOR: programSingleColor(); break;
    case MULTI_COLOR: programMultiColor(); break;
    case UP:
    case DOWN: programStepOne(getColorBank(0), getColorBank(1)); stepUp(); break;
    case UP_DOWN: programUpDown(); break;
    case RAINBOW: programRainbow(); break;
    case STARS: programStars(); break;
    case CYLON: programCylon(); break;
    case SECTIONS: programSections(); break;

    }
}

//steps one pixel at a time and changes the the pixels aftier
void StripHelper::initProgram(STRIP_PROGRAMS programToSet) {
    switch (programToSet) {

    case OFF:
    case SINGLE_COLOR: break;
    case MULTI_COLOR: setDirection(true); break;
    case RESET: reset(); initProgram(getProgram());     break;
    case UP: setDirection(true);  break;
    case DOWN: setDirection(false); break;
    case UP_DOWN: setDirection(false); toggleDirection(); break;
    case RAINBOW: setDirection(true); break;
    case CYLON: setDirection(true);  toggleDirection(); break;
    case STARS: programStarsInit(); break;

    }
}
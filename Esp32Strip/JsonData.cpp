#include "JsonData.h"
#include <tuple>



/// <summary>
/// Parses the json_string into a new JsonData object
/// </summary>
/// <example>
/// Example one:
/// @code{.xml}
///     // Create an empty json object
///     JsonData jsEmptyObject("{}");
/// @endcode
/// Example two:
/// @code{.xml}
///     // Create an empty json array
///     JsonData jsEmptyArray("[]");
/// @endcode
/// </example>
/// <param name="jsonString">Must be a valid JSON string</param>
JsonData::JsonData(const char* jsonString)   // NOLINT(cppcoreguidelines-pro-type-member-init, hicpp-member-init)
{
    //parse calls init which will initialize members
    parse(jsonString, NULL);
}

/// <summary>
/// Parses a json string into a new JsonData object
/// </summary>
/// <param name="jsonString">A valid json string for the current object</param>
/// <param name="parent">Parent of this json object if no parent pass NULL</param>
JsonData::JsonData(const String jsonString, JsonData* parent)  // NOLINT(cppcoreguidelines-pro-type-member-init, hicpp-member-init)
{
    //parse calls init which will initialize members
    parse(jsonString, parent);
}

/// <summary>
/// Creates a new Json object with a invalid valueType
/// </summary>
/// <param name="type">The type of object this is</param>
/// <param name="parent">Parent of this json object if no parent pass NULL</param>
JsonData::JsonData(JSONTYPE type, JsonData* parent) {  // NOLINT(cppcoreguidelines-pro-type-member-init, hicpp-member-init)
    //init will initialize member values
    init(type, JSONTYPE_INVALID, parent);
}


/// <summary>
/// Initializes all member variables
/// </summary>
/// <param name="type">The type of this object</param>
/// <param name="valueType">The value type of this object</param>
/// <param name="parent">Parent of this json object if no parent pass NULL</param>
void JsonData::init(JSONTYPE type, JSONTYPE valueType, JsonData* parent) {
    this->mParent = parent;
    mType = type;
    mValueType = valueType;
    mFirstChild = NULL;
    mNext = NULL;
    if (parent) {
        if (type != parent->mValueType) {
            parent->mValueType = type;
        }
        JsonData* lastChild = getLastChild(parent);
        if (lastChild)
            lastChild->mNext = this;
        else
            parent->mFirstChild = this;
    }
}

/// <summary>
/// Initializes all member variables and parses the jsonString
/// </summary>
/// <param name="jsonString">Json string object</param>
/// <param name="parent">Parent, pass NULL if this is the root object (no parent)</param>
void JsonData::parse(const String jsonString, JsonData* parent) {
    // 
    String strJson(trim(jsonString));
    init(JSONTYPE_INVALID, JSONTYPE_INVALID, parent);
    const int len = strJson.length();
    if (len < 2)
        return;
    const char firstChar = strJson.charAt(0);
    if (strJson.charAt(len - 1) != getClosingToken(firstChar))
        return;//invalid json

    if (firstChar == '[')
        mType = JSONTYPE_ARRAY;
    else if (firstChar == '{')
        mType = JSONTYPE_OBJECT;
    else
        return;  //invalid json
    if (len == 2)
        return; //an empty json

    if (mType == JSONTYPE_ARRAY)
        array(&strJson, this, false);
    else  if (mType == JSONTYPE_OBJECT)
        object(&strJson, this);

}

/// <summary>
/// Searches for children of a given parent.
/// </summary>
/// <param name="parent">The parent to search for last child in.  Must not be NULL.</param>
/// <returns>A pointer to the last child of the parent object.  Return NULL if parent has not children</returns>
JsonData* JsonData::getLastChild(JsonData* parent) {
    if (parent->mFirstChild == NULL)
        return NULL;

    JsonData* child = parent->mFirstChild;
    while (child->mNext) {
        child = child->mNext;
    }

    return child;
}

/// <summary>
/// Searches for the last sibling of a node;
/// </summary>
/// <param name="previous">Will search for the last sibling after this node. </param>
/// <returns>Pointer to the last sibling after previous node.  Returns NULL if previous is the last node.</returns>
JsonData* JsonData::getLastNode(JsonData* previous) {

    if (!previous)
        return NULL;

    JsonData* pNode = previous->mNext;
    if (!pNode)
        return NULL;

    while (pNode->mNext) {
        pNode = pNode->mNext;
    }

    return pNode;
}
//key and val must have a parent

/// <summary>
/// Will create a new Object and set it's values according to the given parameters.
/// No parsing will take place.  Makes the last child of parent point to this one.
/// </summary>
/// <param name="value"></param>
/// <param name="type"></param>
/// <param name="valueType"></param>
/// <param name="parent">Parent of this json object if no parent pass NULL</param>
JsonData::JsonData(String value, JSONTYPE type, JSONTYPE valueType, JsonData* parent) {  // NOLINT
    init(type, valueType, parent);
    mValue = value;
}

/// <summary>
/// Checks if a character is any of the json closing tokens 
/// note quotes " are tricky, it can also be a starting token but we return true for that
/// </summary>
/// <param name="c">The Character to check</param>
/// <returns></returns>
bool JsonData::isClosingToken(const char c) {

    switch (c) {
    case ']':
    case '}':
    case '"':
    case ',': return true;

    default: return false;
    }
}

/// <summary>
/// Searches for an closing token of the given opening token
/// </summary>
/// <param name="openingToken">The opening token to find a closing token for</param>
/// <returns>The closing token for an opening token.   if '+' is returned then any of the closing tokens can be a closing token. If no closing token was found then ' ' is returned.</returns>
char JsonData::getClosingToken(const char openingToken) {
    switch (openingToken) {
    case '[':   return ']';
    case '{':   return '}';
    case '"':   return '"';
    case ':':   return '+';

    case '-':/*Fyrir integer or float*/
    case '1':
    case '2':
    case '3':
    case '4':
    case '5':
    case '6':
    case '7':
    case '8':
    case '9':
        //todo hvað með
    case '0': /*Fyrir floating point*/
    case 'f': /*Fyrir bool*/
    case 't':
    case 'n': /*Fyrir null*/
        return '+';

    default:   return ' ';
    }
}


/// <summary>
/// Searches for end of string.
/// </summary>
/// <param name="searchMe">a pointer to the string to search</param>
/// <param name="startAt">index of the first carater in the string.  That is first character afthe the "</param>
/// <param name="length">length of the searchMe.</param>
/// <returns>Index of where the string ends (index of the ")</returns>
int getIndexOfClosingString(String* searchMe, int startAt, int length) {
    int index = startAt;
    if (length < 2 || index < 1 || index >= length)
        return -1;
    char before,
        check = searchMe->charAt(index - 1);


    while (index < length) {
        before = check;
        check = searchMe->charAt(index);

        if (check == '\"' && before != '\\') {
            return index;
        }
        index++;
    }
    return -1;
}

/// <summary>
/// If one of these three start tokens [ { " are found at the beginning of
/// the string  the function returns the position of it's closing token.
/// </summary>
/// <param name="string">First char of this string will be used to search the same string for it's closing token</param>
/// <param name="ignoreStrings">If you are searching for a closing string token (first char is ") then set this parameter to false.</param>
/// <returns>
///     Success: Index of the closing token.  Fail: Returns -1 if startToken is not found.  
///     Returns -2 if startToken is found but no end token.
/// </returns>
int JsonData::getIndexOfClosingToken(String* string, bool ignoreStrings = true) {
    int length = string->length();
    if (length < 1)
        return -1;  //maybe this should be 2
    const char cStart = string->charAt(0);
    const char cEnd = getClosingToken(cStart);
    if (cEnd == ' ') return -1;

    char before, check;

    int index = 1,
        level = 1;
    char startToken = cStart,
        endToken = cEnd;

    if (!ignoreStrings && string->charAt(0) == '\"') {
        //read untill end of string
        return getIndexOfClosingString(string, 1, length);
    };

    while (index < length) {
        before = string->charAt(index - 1);
        check = string->charAt(index);
        if (check == startToken && startToken != endToken && endToken != '+') {
            if (!ignoreStrings && startToken == '\"' && before == '\\') // if \" is within a string it should be ignored
            {/*Ignore this case*/
            }
            else
                level++;
        }
        else if (check == endToken || (endToken == '+' && isClosingToken(check))) {
            if (!ignoreStrings && endToken == '\"' && before == '\\') // if \" is within a string it should be ignored
            {/*ignore this case*/
            }
            else
                level--;
        }

        if (level == 0)
            return index;
        index++;
    }
    return -1;
}

/// <summary>
/// Checks if the character is a digit.
/// </summary>
/// <param name="c">The character to check.</param>
/// <returns>True if it is a digit.  False it is not.</returns>
bool JsonData::isDigit(const char c) {
    return (c >= '0' && c <= '9');
}

/// <summary>
/// Searches for a value in the string, creates an object and removes that value from the string
/// Lexer: 
///        value     =  [ string | number | object | array | "true" | "false" | "null" ]
///        number    = [ @ignore("null") [int frac? exp?] ]
///        object    = [ '{' members? '}' ]
///        array     = [ '[' elements? ']' ]
///        string      = [ '"' text '"' ]
/// PS.
///    We are skipping "true" | "false" | "null" | frac | exp for now
/// </summary>
/// <param name="valuesString">A json string to search for the first value</param>
/// <param name="parent">There must be a parent of a value.</param>
/// <returns>A pointer to the object created.  If value was found, the function returns NULL</returns>
JsonData* JsonData::value(String* valuesString, JsonData* parent)
{
    if (!parent)
        return NULL;
    const int len = valuesString->length();
    if (len < 1)
        return NULL;

    String strKey, strValue;
    const char firstChr = valuesString->charAt(0);

    JSONTYPE valType = getValueTypeFromChar(firstChr);
    int endOfVal = getIndexOfClosingToken(valuesString);
    JsonData* pNewObject;
    switch (valType)
    {
    case JSONTYPE_OBJECT:
        return object(valuesString, parent);
    case JSONTYPE_ARRAY:
        return array(valuesString, parent, true);
    case JSONTYPE_STRING:
        pNewObject = pair(valuesString, parent);
        if (!pNewObject)
        {
            //this is a string
            endOfVal = getIndexOfClosingToken(valuesString, false);
            if (endOfVal < 1) return NULL;
            strValue = valuesString->substring(1, endOfVal);
            int delLen = endOfVal + 1;
            if (delLen + 1 < valuesString->length()) {
                delLen += valuesString->charAt(delLen) == ',';
            }
            *valuesString = valuesString->substring(delLen);

            if (!validateValue(valType, strValue))
                return setRootInvalid();
            pNewObject = new JsonData(strValue, valType, valType, parent);

        }
        return pNewObject;
    case JSONTYPE_ULONG:
    case JSONTYPE_LONG:
        if (endOfVal == -1)
        {
            JSONTYPE tempType = getType(*valuesString);
            if (tempType != valType && tempType != JSONTYPE_FLOAT) {
                return setRootInvalid();
            }
            else
                endOfVal = len;
        }
        strValue = valuesString->substring(0, endOfVal);
        if (endOfVal < valuesString->length()) {
            endOfVal += valuesString->charAt(endOfVal) == ',';
        }
        *valuesString = valuesString->substring(endOfVal);
        if (getType(strValue) == JSONTYPE_FLOAT) {
            valType = JSONTYPE_FLOAT;
        }

        if (!validateValue(valType, strValue))
            return setRootInvalid();
        pNewObject = new JsonData(strValue, valType, valType, parent);
        return pNewObject;
    case JSONTYPE_BOOL:
    case JSONTYPE_NULL:
        if (endOfVal == -1)
        {
            JSONTYPE tempType = getType(*valuesString);
            if (tempType != valType) {
                return setRootInvalid();
            }
            else
                endOfVal = len;
        }
        strValue = valuesString->substring(0, endOfVal);
        if (endOfVal < valuesString->length()) {
            endOfVal += valuesString->charAt(endOfVal) == ',';
        }
        *valuesString = valuesString->substring(endOfVal);

        if (!validateValue(valType, strValue))
            return setRootInvalid();
        pNewObject = new JsonData(strValue, valType, valType, parent);
        return pNewObject;
    default:
        return NULL;
    }
}


/// <summary>
/// Extracts the first value found in the string
/// Lexer: elements  = [ value [',' value]* ]
/// </summary>
/// <param name="strValues">Json string with one or more values</param>
/// <param name="parent">parent must not be NULL</param>
/// <returns></returns>
JsonData* JsonData::elements(String* strValues, JsonData* parent)
{
    // all values must be children of parent 

    const int len = strValues->length();
    if (parent == NULL)
        return setRootInvalid();
    if (len < 1)
        return NULL;
    JsonData* pLast, * p = NULL;
    do {
        pLast = p;
        p = value(strValues, parent);
    } while (p);
    return pLast;
}

/// <summary>
/// Processes one ore more pairs and adds them to parent.
/// Lexer: members   = [ pair [',' pair]* ]
/// </summary>
/// <param name="pairs">One ore move Pairs</param>
/// <param name="parent">Parent of all pairs.  Must not be NULL</param>
/// <returns>The last Pair object added to parent</returns>
JsonData* JsonData::members(String* pairs, JsonData* parent)
{
    const int len = pairs->length();
    if (parent == NULL ||
        len < 5 || // "x":1
        pairs->charAt(0) != '\"'
        )
        return NULL;

    JsonData* pLast;


    //hér þarf að finna út hvort object fari í child eða next

    int keyIndexOfFirstChar, keyLength, valueIndexOfFirstChar,
        valueLength, pairLength;
    bool thereIsAnotherPair;


    String firstPairString = "";

    JsonData* p = NULL;
    do {
        pLast = p;
        if (!getPairIndexes(pairs, thereIsAnotherPair, keyIndexOfFirstChar, keyLength, valueIndexOfFirstChar, valueLength, pairLength)
            || valueLength == 0)
        {
            break;
        }

        firstPairString = pairs->substring(0, pairLength);
        *pairs = pairs->substring(pairLength + thereIsAnotherPair);
        p = pair(&firstPairString, parent);

    } while (p);
    return pLast;
}

/// <summary>
/// Parses members of an object from a string.
/// Lexer: members   = [ pair [',' pair]* ]
///        pair      = [ string ':' value ]
/// </summary>
/// <param name="members">Json string which will be parsed.</param>
/// <param name="parent">Parent of all members.  Must not be NULL.</param>
/// <returns></returns>
JsonData* JsonData::object(String* members, JsonData* parent)
{
    const int len = members->length();
    if (parent == NULL || len < 2 || members->charAt(0) != '{')
        return NULL;

    //remove brackets around first object
    int endOf = getIndexOfClosingToken(members);
    if (endOf < 1)
        return NULL;
    String strArr = members->substring(1, endOf);
    endOf++;
    const bool moreItems = (endOf < len) && (members->charAt(endOf) == ',');
    if (moreItems)
        endOf++;
    *members = members->substring(endOf);
    JsonData* current = parent;
    if (parent != NULL)
    {
        if (strArr.length() == 0) {
            return new JsonData(JSONTYPE_OBJECT, parent);
        }
        if (parent->mType == JSONTYPE_ARRAY)
            current = new JsonData(JSONTYPE_OBJECT, parent);
        else if (strArr.length() > 0)
        {
            JSONTYPE objType = getValueTypeFromChar(strArr.charAt(0));
            if (parent->mType == JSONTYPE_KEY_VALUE)
                current = new JsonData(JSONTYPE_OBJECT, parent);
        }
        else
            return current;
    }
    return this->members(&strArr, current);
}

/// <summary>
/// Gets information about a pair, if it is a valid pair
/// </summary>
/// <param name="pairs">A string with one or more pairs with NO surrounding {}</param>
/// <param name="thereIsAnotherPair">true if there are more pairs in this string</param>
/// <param name="keyIndexOfFirstChar">Position of where the first char in the key</param>
/// <param name="keyLength">length of the key</param>
/// <param name="valueIndexOfFirstChar">Position of where the first char in the value</param>
/// <param name="valueLength">length of the value</param>
/// <param name="pairLength">Length of the pair</param>
/// <returns>true: All information about a pair was found and stored in parameters
///          False:Pair invalid no parameter value is unchanged</returns>
bool JsonData::getPairIndexes(String* pairs, bool& thereIsAnotherPair,
    int& keyIndexOfFirstChar, int& keyLength,
    int& valueIndexOfFirstChar, int& valueLength,
    int& pairLength)
{
    ///////////// start /////////////////
    const int len = pairs->length();
    if (len < 5 || // "x":1
        pairs->charAt(0) != '\"')
        return false;

    //we need to find end of value to find endOfPair
    const int indexOfKeyEnd = getIndexOfClosingToken(pairs, false);
    if (len < indexOfKeyEnd + 2 || indexOfKeyEnd < 1)  return false;
    //got end of key
    int endOfPair = pairs->indexOf(':', indexOfKeyEnd + 1);
    int startOfValue = endOfPair + 1;
    if (startOfValue - 1 != indexOfKeyEnd + 1 && endOfPair + 2 < len) return false;
    //got start of value;
    String strValue = pairs->substring(startOfValue);
    const int isStringValue = getValueTypeFromChar(strValue.charAt(0)) == JSONTYPE_STRING;
    int endOfValue = getIndexOfClosingToken(&strValue, !isStringValue);
    int iComma = strValue.indexOf(",");
    if (endOfValue == -1)
    {
        if (iComma > -1)
            endOfValue = iComma - 1;
        else
            endOfValue = strValue.length() - 1; //-1 hér er munur á parse
        JSONTYPE type = getType(strValue.substring(0, endOfValue + 1));
        if (type == JSONTYPE_INVALID) {
            return false;
        }
    }
    else {
        if (endOfValue + 1 < strValue.length()) {
            if (strValue.charAt(endOfValue) == ',')
                endOfValue--;
        }
    }
    endOfPair = startOfValue + endOfValue;
    bool morePares = false;
    if (endOfPair + 1 < len) {
        morePares = pairs->charAt(endOfPair + 1) == ',';
    }
    ///////////// endir /////////////////
    keyIndexOfFirstChar = 1;
    keyLength = indexOfKeyEnd - keyIndexOfFirstChar;
    valueIndexOfFirstChar = startOfValue;
    valueLength = endOfValue + 1;
    pairLength = endOfPair + 1;
    thereIsAnotherPair = morePares;

    return true;
}

/// <summary>
/// Extracts the first key value pair found in the members string and removes it from the string members
/// </summary>
/// <param name="keyValues">One or more pair.  Pairs must start with key like this "key ":</param>
/// <param name="parent">Parent to one or more pairs in members.  parent must not be NULL</param>
/// <returns>Returns a newly added pair to parent.  If a pair is not found the return values is NULL;</returns>
JsonData* JsonData::pair(String* keyValues, JsonData* parent)
{
    if (!parent)
        return setRootInvalid();

    bool thereIsAnotherPair;
    int     keyIndexOfFirstChar, keyLength,
        valueIndexOfFirstChar, valueLength,
        pairLength;

    if (!getPairIndexes(keyValues, thereIsAnotherPair,
        keyIndexOfFirstChar, keyLength,
        valueIndexOfFirstChar, valueLength,
        pairLength))
    {
        return NULL;
    }
    String strKeyX = keyValues->substring(keyIndexOfFirstChar, keyIndexOfFirstChar + keyLength);
    String strValX = keyValues->substring(valueIndexOfFirstChar, valueIndexOfFirstChar + valueLength);
    JSONTYPE valType = getValueTypeFromChar(strValX.charAt(0));
    bool isStringValue = valType == JSONTYPE_STRING;

    if (valType == JSONTYPE_INVALID)
        return setRootInvalid();

    JsonData* p = new JsonData(strKeyX, JSONTYPE_KEY_VALUE, valType, parent);

    value(&strValX, p);
    *keyValues = keyValues->substring(pairLength);
    return p;
}

/// <summary>
/// Check if a value is a valid JSONTYPE
/// </summary>
/// <param name="jsonValueType">The type of the value to check</param>
/// <param name="stringValue">The value</param>
/// <returns></returns>
bool JsonData::validateValue(const JSONTYPE jsonValueType, String stringValue)
{
    if (jsonValueType == JSONTYPE_STRING)
        return true; //all strings are valid

    //todo:: use JSONTYPE instead
    switch (jsonValueType)
    {
    case JSONTYPE_ARRAY: return JSONTYPE_ARRAY == getType(stringValue);
    case JSONTYPE_OBJECT: return JSONTYPE_OBJECT == getType(stringValue);
    case JSONTYPE_ULONG: return JSONTYPE_ULONG == getType(stringValue);
    case JSONTYPE_LONG: return JSONTYPE_LONG == getType(stringValue);
    case JSONTYPE_FLOAT: return JSONTYPE_FLOAT == getType(stringValue);
    case JSONTYPE_BOOL: return JSONTYPE_BOOL == getType(stringValue);
    case JSONTYPE_NULL: return JSONTYPE_NULL == getType(stringValue);
    case JSONTYPE_KEY_VALUE: return true;
    }

    return false;
}

/// <summary>
/// Parses elements of an array from a string, and creates objects from them.
/// Lexer: elements  = [ value [',' value]* ]
/// </summary>
/// <param name="strElements">Json string of elements which are one or more values</param>
/// <param name="parent">Parent of all values.  Must not be NULL.</param>
/// <param name="canBeMoreThanOne">todo: do I need this variable.</param>
/// <returns>A pointer to the last value created.  If nothing was created NULL will be returned.</returns>
JsonData* JsonData::array(String* strElements, JsonData* parent, bool canBeMoreThanOne)
{
    // all elements must be children of parent 
    const int len = strElements->length();
    if (parent == NULL || len < 2 || strElements->charAt(0) != '[')
        return setRootInvalid();


    if (!canBeMoreThanOne) {

        if (strElements->charAt(len - 1) != ']')
            return setRootInvalid();

        *strElements = strElements->substring(1, len - 1);
        return elements(strElements, parent);
    }

    //there can be more than array

    int endOf = getIndexOfClosingToken(strElements);
    if (endOf < 1)
        return setRootInvalid();

    String strArr = strElements->substring(1, endOf);
    endOf++;
    if (endOf < len)
        endOf += strElements->charAt(endOf) == ',';

    *strElements = strElements->substring(endOf);
    JsonData* pNew = new JsonData(JSONTYPE_ARRAY, parent);
    return elements(&strArr, pNew);

}


/// <summary>
/// Will tell you if the current object is invalid
/// </summary>
bool JsonData::isValid() const
{
    return (mType == JSONTYPE_KEY_VALUE && mValueType != JSONTYPE_INVALID) ||
        (mType != JSONTYPE_INVALID);
}

/// <summary>
/// Will return the value of the current object.
/// </summary>
/// <returns>A string showing the value. If value type is a string then quotation at the beginning and end of the returned string.</returns>
String JsonData::valueToString() {
    String strRet;
    if (mType == JSONTYPE_STRING || mValueType == JSONTYPE_STRING)
    {
        strRet = "\"";
        strRet += mValue.c_str();
        strRet += "\"";
        return strRet;
    }
    if (mValueType == JSONTYPE_ARRAY)
    {
        if (mFirstChild)
            strRet = mFirstChild->toString().c_str();
        return strRet;
    }
    else if (mValueType == JSONTYPE_OBJECT)
    {
        if (mFirstChild)
            strRet = mFirstChild->toString().c_str();
        return strRet;
    }

    strRet = mValue.c_str();
    return strRet;
}

/// <summary>
/// Returns the object as a JSON string.  
/// This string should be a valid JSON string, ready to be sent over the wire.
/// </summary>
/// <returns>The current object returned as an JSON String.</returns>
String JsonData::toString() {

    if (!isValid())
        return "";

    String str;
    switch (mType)
    {
    case JSONTYPE_ARRAY:
        str = "[";
        if (mFirstChild)
            str += mFirstChild->toString().c_str();
        break;
    case JSONTYPE_OBJECT:
        str = "{";
        if (mFirstChild)
            str += mFirstChild->toString().c_str();
        break;
    case JSONTYPE_KEY_VALUE:
        str = "\""; str += mValue.c_str(); str += "\":";
        str += mFirstChild->toString().c_str();
        break;
    case JSONTYPE_STRING:
    case JSONTYPE_ULONG:
    case JSONTYPE_LONG:
    case JSONTYPE_FLOAT:
    case JSONTYPE_BOOL:
    case JSONTYPE_NULL:
        str += valueToString().c_str();
        break;

    default:                //this should never happen
        return "";
    }

    if (mType == JSONTYPE_ARRAY)
        str += "]";
    else if (mType == JSONTYPE_OBJECT)
        str += "}";
    if (mNext != NULL) {
        str += ",";
        str += mNext->toString().c_str();
    }

    return str;
}


String JsonData::toTree(JsonData* current, int level)
{
    if (current == NULL)
        return "";

    String str = "";
    String tabs = "";
    String strCurrent((long)(current), HEX);
    String strParent((long)(current->mParent), HEX);
    String strFirstChild((long)(current->mFirstChild), HEX);
    String strNext((long)(current->mNext), HEX);

    for (int i = 0; i < level; i++) { tabs += "\t"; }

    str += "\n"; str += tabs; str += "Current  :"; str += strCurrent;   str += " firstChild:"; str += strFirstChild;   str += " next:"; str += strNext; str += " parent:"; str += strParent;
    str += "\n"; str += tabs; str += "Type     :";  str += jsonTypeString(current->mType);
    str += "\n"; str += tabs; str += "ValueType:";  str += jsonTypeString(current->mValueType);
    str += "\n"; str += tabs; str += "Value    :";  str += current->mValue;
    str += toTree(current->mFirstChild, level + 1);
    str += toTree(current->mNext, level);
    return str;
}

String JsonData::jsonTypeString(const JSONTYPE type)
{
    switch (type)
    {
    case JSONTYPE_ARRAY: return "JSONTYPE_ARRAY";
    case JSONTYPE_OBJECT: return "JSONTYPE_OBJECT";
    case JSONTYPE_KEY_VALUE: return "JSONTYPE_KEY_VALUE";
    case JSONTYPE_STRING: return "JSONTYPE_STRING";
    case JSONTYPE_ULONG: return "JSONTYPE_ULONG";
    case JSONTYPE_LONG: return "JSONTYPE_LONG";
    case JSONTYPE_FLOAT: return "JSONTYPE_FLOAT";
    case JSONTYPE_BOOL: return "JSONTYPE_BOOL";
    case JSONTYPE_NULL: return "JSONTYPE_NULL";
    }

    return "JSONTYPE_INVALID";
}

/// <summary>
/// Prints the object as a tree
/// </summary>
/// <returns>A string ready to be sent to the console</returns>
String JsonData::toTree()
{
    String str;
    return toTree(this, 0);
}

/// <summary>
/// Guesses the value type from a first char in a value.  
/// This method is not perfect but sometimes enough.
/// </summary>
/// <param name="firstCharInValue">This first char of value string</param>
/// <returns>Success: The selected value.  Fail: JSONTYPE_INVALID </returns>
JSONTYPE JsonData::getValueTypeFromChar(char firstCharInValue)
{
    JSONTYPE valType = JSONTYPE::JSONTYPE_INVALID;
    switch (firstCharInValue)
    {
    case '\"':
        valType = JSONTYPE_STRING;
        break;

    case '[':
        valType = JSONTYPE_ARRAY;
        break;

    case '{':
        valType = JSONTYPE_OBJECT;
        break;
    case '-':  //todo: do a better test
        valType = JSONTYPE_LONG;
        break;
    case 'f':  //todo: do a better test
    case 't':  //todo: do a better test
        valType = JSONTYPE_BOOL;
        break;
    case 'n': valType = JSONTYPE_NULL;
        break;

    default:
        if (isDigit(firstCharInValue))
            valType = JSONTYPE_ULONG; //to do a better test
    }
    return valType;
}

/// <summary>
/// Gets type type of a string (for array,ojbect and string only first char is checked to determine the type)
/// </summary>
/// <param name="strValue">The string to check if it is a number.  example1: "-2.12"  example2:"0.1" </param>
/// <returns>True if string is a floating point number otherwise false.</returns>
JSONTYPE JsonData::getType(String strValue) {

    const int len = strValue.length();
    if (len < 1)
        return JSONTYPE_INVALID;

    char c = strValue.charAt(0);

    switch (c)
    {
    case '\"':  return JSONTYPE_STRING;

    case '[':   return JSONTYPE_ARRAY;

    case '{':   return JSONTYPE_OBJECT;

    case 't':
    case 'f':
        if (strValue == "true" || strValue == "false")
            return JSONTYPE_BOOL;
        else
            return JSONTYPE_INVALID;
    case 'n':
        if (strValue == "null")
            return JSONTYPE_NULL;
        else
            return JSONTYPE_INVALID;
    }

    //check for numbers
    bool startsWithMinus = c == '-';

    if (startsWithMinus && len < 2)
        return JSONTYPE_INVALID;

    int i = strValue.charAt(0) == '-' ? 1 : 0;
    int foundDot = false;

    while (i < len)
    {
        c = strValue.charAt(i);
        if (!isdigit(c))
        {
            if (c == '.') {
                if (foundDot)
                    return JSONTYPE_INVALID;//only one dot is allowed and no char else
                foundDot = true;
            }
            else
                return JSONTYPE_INVALID; //not a digit
        }
        i++;
    }

    //is invalid int
    if (startsWithMinus && len > 1 && strValue.charAt(1) == '0' && !foundDot)
        return JSONTYPE_INVALID; //integer cannot start with "-0"

    //is invalid float
    if (len > 1 + startsWithMinus && strValue.charAt(startsWithMinus) == '0' && strValue.charAt(1 + startsWithMinus) != '.')
        return JSONTYPE_INVALID;  // valid is "0." and "-0.": invalid  are "01." and "-01."  

    if (foundDot)
        return JSONTYPE_FLOAT;

    if (startsWithMinus)
        return JSONTYPE_LONG;

    return JSONTYPE_ULONG;

}

/// <summary>
///  Sets the root object to an invalid object type.
/// </summary>
/// <returns>Always returns NULL.</returns>
JsonData* JsonData::setRootInvalid() {
    JsonData* current = this, * parent = this->mParent;

    while (parent) {
        current = parent;
        parent = current->mParent;
    }

    current->mType = JSONTYPE_INVALID;
    return NULL;
}

/// <summary>
/// Will remove the last sibling of a node.
/// </summary>
/// <param name="pNode">The previous node</param>
/// <returns>True if last sibling was removed.  False if pNode has no siblings.</returns>
bool JsonData::removeLast(JsonData* pNode) {

    JsonData* p = getLastNode(pNode);
    if (!p) return false; //there are no nodes left

    pNode->mNext = NULL;
    //cout << "Deleting " << p->mKey.c_str() << ":" << p->mValue.c_str() << endl;
    delete p;
    return true;
}


/// <summary>
/// Searches for the top node of the specified node.
/// </summary>
/// <param name="current">The node to search for the greatest ancestor of.</param>
/// <returns>Pointer to the root node.  If no parent or ancestor is found the return value is NULL.</returns>
JsonData* JsonData::getRootNode(JsonData* current) {

    if (!current)
        return NULL;
    JsonData* p = current->mParent;
    while (p) {
        if (p->mParent == NULL)
            return p;
        p = p->mParent;
    }

    return NULL;
}

JsonData* JsonData::getPointingNode(JsonData* findMe) {
    if (!findMe || !findMe->mParent)
        return NULL;

    return findPointingNode(getRootNode(findMe), findMe);
}

/// <summary>
/// Searches for the node pointing to this node.  A pointing node can be 
/// pointing to this node from the variables mFirstChild or mNext.
/// </summary>
/// <param name="startFrom">Where to start the search from</param>
/// <param name="findMe">The node to search for</param>
/// <returns>Pointer to the node pointing to the findMe node.</returns>
JsonData* JsonData::findPointingNode(JsonData* startFrom, JsonData* findMe) {

    if (!startFrom)
        return NULL;

    if (startFrom->mNext == findMe || startFrom->mFirstChild == findMe)
        return startFrom;

    JsonData* p = findPointingNode(startFrom->mNext, findMe);
    if (p)
        return p;
    return findPointingNode(startFrom->mFirstChild, findMe);

}

/// <summary>
/// Removes a node from memory and adjusts nodes which 
/// point to it so they will not point to it any more
/// </summary>
/// <param name="pNode">A node to remove from object chain/list and memory</param>
/// <returns>True if a node and it's children was deleted from memory.  False if the node was not deleted.</returns>
bool JsonData::destroyIncludingChildren(JsonData* pNode) {

    if (!pNode)
        return false;

    JsonData* pPrevious = getPointingNode(pNode);

    destroyIncludingChildren(pNode->mNext);
    destroyIncludingChildren(pNode->mFirstChild);


    if (pPrevious) {
        if (pNode == pPrevious->mNext)
            pPrevious->mNext = NULL;
        if (pNode == pPrevious->mFirstChild)
            pPrevious->mFirstChild = NULL;
    }

    delete pNode;
    return true;
}

/// <summary>
/// Deconstruct-or for the JsonData object
/// Will remove the object and it's children from memory
/// </summary>
JsonData::~JsonData()
{
    destroyIncludingChildren(mFirstChild);
    destroyIncludingChildren(mNext);
}

/// <summary>
/// Checks if a character is a white space character.
/// </summary>
/// <param name="c">The character to check</param>
/// <returns>True if it is a white space character otherwise false.</returns>
bool JsonData::isWhitespace(const char c) {
    switch (c) {
    case ' ':
    case '\t':
    case '\r':
    case '\n':
    case '\f':
    case '\b':
        return true;

    default:    return false;
    }
}

/// <summary>
/// Will remove all white space characters from a string.
/// The function will not touch sections of a string within quotation marks.
/// Comments will also be removed from the json object
/// </summary>
/// <param name="jsonStringToTrim">The string to remove whitespaces from</param>
/// <returns>A string without white spaces</returns>
String JsonData::trim(String jsonStringToTrim) {
    const int len = jsonStringToTrim.length();
    char* buffer = new char[len + 1];
    memset(buffer, 0, len + 1);

    int bufPos = 0;
    bool inString = false;
    for (int i = 0; i < len; i++) {
        char c = jsonStringToTrim.charAt(i);
        if (!inString) {
            //we are not inside a string, so here could be a comment
            if (c == '/' && i + 2 <= len)
            {   //here could be a comment
                const char nextChar = jsonStringToTrim.charAt(i + 1);
                int ignoreToIndex = -1;
                if (nextChar == '/') {
                    //We are inside a double slash comment.  We need to ignore all until end line
                    if (i + 2 == len)
                        break; //we are at end of string and the last two chars are //
                    ignoreToIndex = jsonStringToTrim.indexOf('\n', i + 2) + 1;
                    if (ignoreToIndex - i < 3) {
                        ignoreToIndex = -1;//invalid comment so let's do nothing
                    }
                }
                else if (nextChar == '*') {
                    //we are inside a slash star comment  /*some text*/
                    ignoreToIndex = jsonStringToTrim.indexOf("*/", i + 2) + 2; // /**/
                    if (ignoreToIndex - i < 5)
                        ignoreToIndex = -1;//invalid comment so let's do nothing
                }

                if (ignoreToIndex > -1 && ignoreToIndex < len) {

                    i = ignoreToIndex;
                    c = jsonStringToTrim.charAt(i);
                }
                else if (ignoreToIndex == len) {
                    break;//nothing more to copy
                }
            }


        } //if (!inString) {
        if (c == '\"')
        {
            if (i > 0 && jsonStringToTrim.charAt(i - 1) == '\\')
            {/*unchanged because \" should be ignored in strings*/
            }
            else
                inString = !inString;  //in or out of a string
        }


        if (inString || !isWhitespace(c))
        {
            buffer[bufPos++] = c;
        }
    }
    String strRet(buffer);
    delete[] buffer;
    return strRet;
}


/// <summary>
/// Gets a a pointer to a child at a specific index.  
/// If no child is found at given location NULL is returned
/// </summary>
/// <param name="index">Zero based index of the child to get</param>
/// <returns>Pointer to the object.  If no object is found NULL is returned</returns>
JsonData* JsonData::getChildAt(unsigned int index)
{
    unsigned int i = 0;
    JsonData* current = mFirstChild;
    while (current != NULL) {
        if (i == index)
            return current;
        current = current->mNext;
        i++;
    }
    return NULL;
}

/// <summary>
/// Searches for a child with a specific value.  Good for searching 
/// for a spesific JSONTYPE_KEY_VALUE f.example
/// </summary>
/// <param name="value">Value to search for.  the search is case sensitive</param>
/// <returns>Pointer to the object.  If no object is found NULL is returned</returns>
JsonData* JsonData::getChild(String value)
{
    JsonData* current = mFirstChild;
    while (current != NULL) {
        if (value == current->mValue)
            return current;
        current = current->mNext;
    }
    return NULL;
}

/// <summary>
/// Gets a a pointer to the next sibling of this object.  
/// This function does NEVER return a child object.
/// If no sibling is found NULL is returned
/// </summary>
/// <returns>Pointer to the next sibling. If no sibling exists, NULL is returned.</returns>
JsonData* JsonData::getNext()
{
    return mNext;
}

/// <summary>
/// Gets the value of an object.
/// If the object is a JSONTYPE_KEY_VALUE then the child object value is NOT returned.
/// </summary>
/// <returns>The value as a string</returns>
const String JsonData::getValue()
{
    return mValue;
}

/// <summary>
/// Returns the value of an object as a String.
/// If the object is a JSONTYPE_KEY_VALUE then the child object value is returned as a String
/// </summary>
/// <returns>The values as a String</returns>
String JsonData::getValueAsString()
{
    if (mType == JSONTYPE_KEY_VALUE && mFirstChild) {
        return mFirstChild->getValueAsString();
    }

    return getValue();
}

/// <summary>
/// Converts the value of an object from string to a float.
/// If the object is a JSONTYPE_KEY_VALUE then the child object value is returned as a number
/// </summary>
/// <returns>Success: If no valid conversion could be performed because the String doesn’t start with a digit, 
/// a zero is returned. Data type: float.  Fail: the number 999999999</returns>
float JsonData::getValueAsFloat()
{
    if (mType == JSONTYPE_KEY_VALUE && mFirstChild &&
        (mValueType == JSONTYPE_FLOAT || mValueType == JSONTYPE_ULONG || mValueType == JSONTYPE_LONG)) {
        return mFirstChild->getValueAsFloat();
    }
    if (mType == JSONTYPE_FLOAT || mType == JSONTYPE_ULONG || mType == JSONTYPE_LONG)
        return mValue.toFloat();

    return JSONDATA_ERRORNUMBER; //999999999
}

/// <summary>
/// Converts the value of an object from string to a unsigned long.
/// Note the type of the value must be a positive number (JSONTYPE_ULONG). 
/// If the object is a JSONTYPE_KEY_VALUE then the child object value is returned as a number 
/// </summary>
/// <returns>Succsess: A unsinged long number.  Fail:  999999999</returns>
unsigned long JsonData::getValueAsULong()
{
    bool     useChild = mType == JSONTYPE_KEY_VALUE && mFirstChild;
    JSONTYPE typeOfValue = (useChild) ? mValueType : mType;
    bool     valueIsValid = (typeOfValue == JSONTYPE_ULONG);

    if (!valueIsValid)
        return JSONDATA_ERRORNUMBER; //999999999

    if (useChild)
        return atol(mFirstChild->mValue.c_str());

    return atol(mValue.c_str());
}

/// <summary>
/// Converts the value of an object from string to a long number.
/// If the object is a JSONTYPE_KEY_VALUE then the child object value is returned as a number 
/// </summary>
/// <returns>Succsess: A long number.  Fail:  999999999</returns>
long JsonData::getValueAsLong()
{
    bool     useChild = mType == JSONTYPE_KEY_VALUE && mFirstChild;
    JSONTYPE typeOfValue = (useChild) ? mValueType : mType;
    bool     valueIsValid = (typeOfValue == JSONTYPE_LONG || typeOfValue == JSONTYPE_ULONG);

    if (!valueIsValid)
        return JSONDATA_ERRORNUMBER; //999999999

    if (useChild)
        return atol(mFirstChild->mValue.c_str());

    return atol(mValue.c_str());
}

/// <summary>
/// Converts the value of an object from string to a integer number.
/// If the object is a JSONTYPE_KEY_VALUE then the child object value is returned as a number 
/// </summary>
/// <returns>Succsess: A integer number.  Fail:  999999999</returns>
int JsonData::getValueAsInt()
{
    bool     useChild = mType == JSONTYPE_KEY_VALUE && mFirstChild;
    JSONTYPE typeOfValue = (useChild) ? mValueType : mType;
    bool     valueIsValid = (typeOfValue == JSONTYPE_LONG || typeOfValue == JSONTYPE_ULONG);

    if (!valueIsValid)
        return JSONDATA_ERRORNUMBER; //999999999

    if (useChild)
        return mFirstChild->mValue.toInt();

    return mValue.toInt();
}
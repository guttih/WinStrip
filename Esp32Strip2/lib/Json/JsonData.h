#ifndef _JSONDATA_H
#define _JSONDATA_H

#ifndef NULL
#define NULL nullptr
#endif // !NULL

#include <Arduino.h>

/// <summary>
/// The errornumber to return when a numberfunction fails;
/// Like: getValueAsInt(), toFloat(), getValueAsLong(), getValueAsULong()
/// </summary>
#define JSONDATA_ERRORNUMBER 999999999;

//See informanation on JSON on https://www.json.org (nice pictures)
//See more info here : https://www.crockford.com/mckeeman.html
//todo: Hexadecimal digits can be represented as u and then 4 hexadecimal digits. (\u hex hex hex hex)
//      hex can be ('0'..'9' or 'A'..'F' or 'a'..'f')
//todo: Exponent numbers see second answser: https://stackoverflow.com/questions/19554972/json-standard-floating-point-numbers
//      example1: 1e-005 example2: 2.99792458e8   Exponent can be ('E' sign digits) or ('e' sign digits)
/// <summary>
/// Enumeration for which type of json object the json data is.
/// </summary>
enum JSONTYPE {
    JSONTYPE_INVALID,
    JSONTYPE_ARRAY,
    JSONTYPE_OBJECT,
    JSONTYPE_KEY_VALUE,
    JSONTYPE_STRING,
    JSONTYPE_ULONG,
    JSONTYPE_LONG,
    JSONTYPE_FLOAT,
    JSONTYPE_BOOL,
    JSONTYPE_NULL,
};

/// <summary>
/// Class for representing elements and objects in a json object.
/// You can use it to browse and get values and add and remove objects from a json.
/// </summary>
/// <example>
/// Example hello World:
/// @code{.xml}
///     // Create an json object with one key named "hello"
///     // and one value which is the string "world"
///     JsonData js("{\"hello\":\"world\"}");
/// @endcode
/// </example>
class JsonData
{

private:

    String mValue;
    JSONTYPE mType;
    JSONTYPE mValueType;
    JsonData* mFirstChild{},
            * mNext{},
            * mParent{};

    JsonData( JSONTYPE type, JsonData* parent );
    JsonData( String jsonString, JsonData* parent );
    JsonData( String value, JSONTYPE type, JSONTYPE valueType, JsonData* parent );


    static bool      isClosingToken( char c );
    static char      getClosingToken( char openingToken );
    static int       getIndexOfClosingToken( String* string, bool ignoreStrings );
    static bool      isDigit( char c );
    static bool      removeLast( JsonData* pNode );
    static bool      destroyIncludingChildren( JsonData* pNode );
    static JSONTYPE  getValueTypeFromChar( char firstCharInValue );
    static JSONTYPE  getType( String strValue );
    static JsonData* getLastChild( JsonData* parent );
    static JsonData* getLastNode( JsonData* previous );
    static JsonData* getPointingNode( JsonData* findMe );
    static JsonData* findPointingNode( JsonData* startFrom, JsonData* findMe );
    static JsonData* getRootNode( JsonData* current );
    void             init( JSONTYPE type, JSONTYPE valueType, JsonData* parent );
    void             parse( const String jsonString, JsonData* parent );
    JsonData* setRootInvalid();
    String           valueToString();
    JsonData* array( String* elements, JsonData* parent, bool canBeMoreThanOne );
    JsonData* object( String* members, JsonData* parent );
    JsonData* elements( String* values, JsonData* parent );
    bool             getPairIndexes( String* pairs, bool& thereIsAnotherPair,
                                     int& keyIndexOfFirstChar, int& keyLength,
                                     int& valueIndexOfFirstChar, int& valueLength,
                                     int& pairLength );
    JsonData* members( String* pairs, JsonData* parent );
    JsonData* pair( String* keyValues, JsonData* parent );
    static bool       validateValue( const JSONTYPE jsonvaluetype, String string );
    JsonData* value( String* valuesString, JsonData* parent );
    static String     jsonTypeString( JSONTYPE type );
    String            toTree( JsonData* current, int level );
    static bool       isWhitespace( const char c );
public:
    JsonData( const char* jsonString );
    ~JsonData();
    String toString();
    String toTree();
    static String  trim( String jsonStringToTrim );
    bool           isValid() const;
    /// <summary>
    /// Checks if the current object has any child objects
    /// </summary>
    /// <returns>
    /// true if this object has one ore more child object(s).  Otherwize false
    /// </returns>
    bool          hasChildren()
    {
        return this->mFirstChild != NULL;
    };
    JsonData* getChildAt( unsigned int index );
    JsonData* getChild( String value );
    JsonData* getNext();
    const String  getValue();
    String        getValueAsString();
    float         getValueAsFloat();
    unsigned long getValueAsULong();
    long          getValueAsLong();
    int           getValueAsInt();
    JSONTYPE      getType()      const
    {
        return mType;
    }
    JSONTYPE      getValueType() const
    {
        return mValueType;
    }
};

#endif // _JSONDATA_H
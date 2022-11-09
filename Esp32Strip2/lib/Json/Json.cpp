#include "Json.h"


/// <summary>
/// Parses the json_string into a new JsonData object
/// </summary>
/// <example>
/// // Create an empty object
/// @code{.xml}
/// // Create an empty json object
///   JsonData jsEmptyObject("{}");
///
/// // Create an empty json array
///   JsonData jsEmptyArray("[]");
/// @endcode
/// </example>
/// <param name="jsonString">Must be a valid JSON string</param>
Json::Json( const char* jsonString )
{
    mData = new JsonData( jsonString );
}


/// <summary>
/// A deconstructor for the Json object.  It will free the object from memory.
/// This function fires automatically when this object is no longer needed.
/// </summary>
Json::~Json()
{
    if( mData )
    {
        //cout << "Deleting " << mData->getKey().c_str() << ":" << mData->getValue().c_str() << endl;
        delete mData;
        mData = NULL;
    }
}

/// <summary>
/// Returns the object as a JSON string.
/// This string should be a valid JSON string, ready to be sent over the wire.
/// </summary>
/// <returns>The current object returned as an JSON String.</returns>
String Json::toString() const
{
    return mData->toString();
}

/// <summary>
/// Showing objects and sub-objects In a treeview.
/// That is child objects have additional tabs relative to parent objects.
/// </summary>
/// <returns>The json object as a tree view string.</returns>
String Json::toTree() const
{
    return mData->toTree();
}

/// <summary>
/// Removes all unnecessary white spaces like tab end line and carriage return
/// </summary>
/// <param name="jsonStringToTrim">The input string that will be unchanged</param>
/// <returns>Copy of the input string without all white spaces</returns>
String Json::trim( String jsonStringToTrim )
{

    return JsonData::trim( jsonStringToTrim );
}

/// <summary>
/// Will tell you if the current object is invalid
/// </summary>
bool Json::isValid() const
{
    return mData->isValid();
}



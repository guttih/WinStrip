#ifndef _JSON_H
#define _JSON_H

#include "JsonData.h"


/// <summary>
/// A class for parsing json strings
/// </summary>
/// <example>
/// Example on how to create a json object from a string:
/// @code{.xml}
/// Json json("{\"hello\":\"world\",\"array\":[1,2,-4,-5.22,\"string in a array\"]}");
/// @endcode
/// </example>
class Json
{
    JsonData* mData;
public:
    Json(const char* jsonString);
    ~Json();
    String toString() const;
    String toTree() const;
    static String trim(String jsonStringToTrim);
    bool isValid() const;
    /// <summary>
    /// Use this function access the root JsonData object
    /// </summary>
    /// <returns>
    /// The root JsonData object.
    /// If the object is invalid NULL is returned.
    /// </returns>
    JsonData* getRootObject() { return mData == NULL || !mData->isValid() ? NULL : mData; };
};

#endif 
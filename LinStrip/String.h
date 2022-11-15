#ifndef _XSTRING_H
#define _XSTRING_H

#include <string>
#include <iostream>
#include <vector>
#include <sstream>
#include <iomanip>
#include <bitset>

#pragma GCC diagnostic ignored "-Wsign-conversion"

enum NUMBER_TYPE
{
    DEC,
    BIN,
    HEX
};

class String
{
private:
    std::string mStr;

    std::string substringHelper( int start, int count )
    {
        std::string str = mStr;
        return str.substr( start, count );
    }
    std::string substringHelper( int start )
    {
        std::string str = mStr;
        return str.substr( start );
    }
    void init( String str )
    {
        mStr = str.c_str();
    }

public:
    String()
    {
        init( "" );
    }
    String( const std::string str )
    {

        init( str.c_str() );
    }
    String( const char *cStr )
    {
        mStr = cStr;
    }

    String( long number, NUMBER_TYPE type )
    {

        std::stringstream stream;
        switch( type )
        {
            case NUMBER_TYPE::HEX:
                stream << "0x"
                << std::setfill( '0' ) << std::setw( sizeof( long ) * 2 )
                << std::hex << number;
                break;

            case NUMBER_TYPE::BIN:
                stream << "0b";
                stream << std::bitset< 32 >( number );
                break;

            case NUMBER_TYPE::DEC:
                stream << std::to_string( number );
                break;
            default:
                break;
        }
        mStr = stream.str();
    }
    String substring( int start, int to )
    {
        return String( substringHelper( start, to - start ) );
    }
    String substring( int start )
    {
        return String( substringHelper( start ) );
    }

    const char *c_str()
    {
        return mStr.c_str();
    }
    int length()
    {
        return ( int ) ( size_t ) ( mStr.length() );
    }
    char charAt( size_t pos )
    {
        return mStr.at( pos );
    }

    int indexOf( const char *cStr, int startFrom = 0 )
    {
        return ( int ) ( size_t ) mStr.find( cStr, startFrom );
    }
    int indexOf( String strFindMe, int startFrom = 0 )
    {
        return ( int ) ( size_t ) mStr.find( strFindMe.c_str(), startFrom );
    }
    int indexOf( const char ch, int startFrom = 0 )
    {
        return ( int ) ( size_t ) mStr.find( ch, startFrom );
    }

    String &operator+=( const String &rhs )
    {
        mStr = mStr + rhs.mStr;
        return *this;
    }

    String &operator+=( const char *rhs )
    {
        mStr += rhs;
        return *this;
    }

    String &operator+( const char *rhs )
    {
        mStr += rhs;
        return *this;
    }

    String operator+( const char *cStr ) const
    {
        return mStr + cStr;
    }
    String operator+( const String right ) const
    {
        return mStr + right.mStr;
    }

    bool operator==( String rhs ) const
    {
        return this->mStr == rhs.mStr;
    }
    bool operator!=( String rhs ) const
    {
        return !( *this == rhs );
    }
    float toFloat()
    {
        return ( float ) std::stod( mStr );
    }

    int toInt()
    {
        return std::stoi( mStr );
    }
};
/*
    String operator+(String& lhs, String& rhs)
    {
        string tmp = lhs.c_str();
        tmp += rhs.c_str();
        return tmp;
    }*/
inline bool operator==( String &lhs, const char *rhs )
{
    const std::string str1 = lhs.c_str();
    return str1 == rhs;
}
inline bool operator==( String &lhs, String &rhs )
{
    return lhs == rhs.c_str();
}


#endif // !_XSTRING_H

#include "SerialStatus.h"

SerialStatus::SerialStatus()
{

}

SerialStatus::~SerialStatus()
{

}

bool SerialStatus::setIsValid( bool value )
{
    m_isValid = value;
    return m_isValid;
}

bool SerialStatus::isValid()
{
    bool success =
        m_com        >= 0 && m_com        < SERIAL_COMMAND::COMMAND_COUNT &&
        m_brightness >= 0 && m_brightness < 256
        && m_programs.count() == ( SERIAL_COMMAND::COMMAND_COUNT ) -1;


    if( !success )
        m_isValid = false;

    return m_isValid;
}
bool SerialStatus::parse( QString jsonAllStatusResponse )
{

    JsonG::Json json( jsonAllStatusResponse.toStdString().c_str() );
    setIsValid( true );
    if( !json.isValid() )
        return setIsValid( false );

    JsonData *root=json.getRootObject();
    JsonData *programs = root->getChild( "programs" ),
             *delay = root->getChild( "delay" ),
             *com = root->getChild( "com" ),
             *brightness = root->getChild( "brightness" ),
             *values = root->getChild( "values" ),
             *colors = root->getChild( "colors" );
    if( !programs || !delay || !com || !brightness || !values || !colors || programs->getValueType() != JSONTYPE_ARRAY )
    {
        return setIsValid( false );
    }

    JsonData *current = programs->getChildAt( 0 );
    if( current )
        current = current->getChildAt( 0 );
    if( m_programs.count() > 0 )
        m_programs.clear();
    SerialProgramInformation spi;
    while( current )
    {

        spi.parseJsonProgramInformation( current );
        if( spi.isValid() )
        {

            m_programs.append( new SerialProgramInformation( spi ) );
        }
        current = current->getNext();
    }

    current = root->getChild( "values" );
    if( current == NULL || current->getValueType() != JSONTYPE_ARRAY )
    {
        return false;
    }
    current = current->getChildAt( 0 );
    int value, i=-1;
    if( current )
        current = current->getChildAt( 0 );
    while( current  && i < 2 )
    {
        i++;
        value = current->getValueAsInt();
        m_values[ i ]=value;
        current = current->getNext();
    }


    current = root->getChild( "colors" );
    if( current == NULL || current->getValueType() != JSONTYPE_ARRAY )
    {
        return false;
    }
    current = current->getChildAt( 0 );
    if( current )
        current = current->getChildAt( 0 );

    unsigned long ulValue;
    i=-1;
    while( current && i < 5 )
    {
        i++;
        ulValue = current->getValueAsULong();
        m_colors[ i ]=ulValue;
        current = current->getNext();
    }

    m_com = com->getValueAsInt();
    m_brightness = brightness->getValueAsInt();
    m_delay = delay->getValueAsInt();


    return isValid();
}

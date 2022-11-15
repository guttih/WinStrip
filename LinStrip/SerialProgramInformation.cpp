#include "SerialProgramInformation.h"
#include "Json.h"

void SerialProgramInformation::reset()
{
    this->m_isValid = false;
    this->m_values.clear();
    this->m_description="";
    this->m_colors=0;
}
bool SerialProgramInformation::parseJsonProgramInformation( JsonData* jsonData )
{
    reset();
    JsonData* current = jsonData->getChild( "name" );
    if( current == NULL )
    {
        return false;
    }

    m_name = QString::fromStdString( current->getValueAsString().c_str() );
    if( m_name.length() < 1 )
        return false;

    current = jsonData->getChild( "description" );
    if( current == NULL )
    {
        return false;
    }
    m_description = QString::fromStdString( current->getValueAsString().c_str() );

    current = jsonData->getChild( "colors" );
    if( current == NULL )
    {
        return false;
    }
    m_colors = current->getValueAsInt();

    current = jsonData->getChild( "values" );
    if( current == NULL || current->getValueType() != JSONTYPE_ARRAY )
    {
        return false;
    }
    current = current->getChildAt( 0 );
    if( current )
        current = current->getChildAt( 0 );
    while( current )
    {
        m_values.append( QString::fromStdString( current->getValueAsString().c_str() ) );
        current = current->getNext();
    }
    m_isValid = true;




    return m_isValid;
}
SerialProgramInformation::SerialProgramInformation()
{
    reset();
}
SerialProgramInformation::SerialProgramInformation( JsonData* jsonData )
{
    reset();
    parseJsonProgramInformation( jsonData );

}

SerialProgramInformation::~SerialProgramInformation()
{

}

bool SerialProgramInformation::isValid()
{
    return m_isValid;
}



SerialProgramInformation &SerialProgramInformation::operator=( const SerialProgramInformation &rhs )
{
    this->m_colors= rhs.m_colors;
    this->m_name=rhs.m_name;
    this->m_description=rhs.m_description;
    this->m_values = rhs.m_values;
    this->m_isValid = rhs.m_isValid;
    return *this;
}

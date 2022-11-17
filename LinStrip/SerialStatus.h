#ifndef SERIALSTATUS_H
#define SERIALSTATUS_H

#include <QList>
#include "serialcommands.h"
#include "SerialProgramInformation.h"

class SerialStatus
{
public:
    QList< SerialProgramInformation  * > m_programs;
    int m_delay;
    int m_com;
    int m_brightness;
    int m_values[ 4 ];
    unsigned int m_colors[ 6 ];

    SerialStatus();
    ~SerialStatus();
    bool parse( QString jsonAllStatusResponse );
    bool isValid();


private:
    bool setIsValid( bool value );
    bool m_isValid = false;
};

#endif

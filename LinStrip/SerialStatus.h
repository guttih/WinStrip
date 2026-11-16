#ifndef SERIALSTATUS_H
#define SERIALSTATUS_H

#include <QList>
#include "serialcommands.h"
#include "SerialProgramInformation.h"

class SerialStatus
{
public:
    QList< SerialProgramInformation  * > m_programs;
    int m_delay = 0;
    int m_com = -1;
    int m_brightness = 0;
    int m_values[ 4 ] = {};
    unsigned int m_colors[ 6 ] = {};

    SerialStatus();
    ~SerialStatus();
    bool parse( QString jsonAllStatusResponse );
    bool isValid();


private:
    Q_DISABLE_COPY(SerialStatus)
    bool m_isValid = false;
};

#endif

#ifndef SERIALPROGRAMINFORMATION_H
#define SERIALPROGRAMINFORMATION_H

#include <QString>
#include <QStringList>
#include "Json.h"

class SerialProgramInformation
{
public:
    SerialProgramInformation &operator=( const SerialProgramInformation &rhs );
    SerialProgramInformation();
    SerialProgramInformation( JsonData *jsonData );
    ~SerialProgramInformation();
    bool isValid();
    bool parseJsonProgramInformation( JsonData* jsonData );
    void reset();

private:
    bool m_isValid = false;
    QString m_name;
    QString m_description;
    QStringList m_values;
    int m_colors;
};

#endif

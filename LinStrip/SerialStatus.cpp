#include "SerialStatus.h"
#include <QJsonArray>
#include <QJsonDocument>
#include <QJsonObject>
#include <cmath>
#include <limits>

namespace {
bool integer(const QJsonValue &value, int minimum, int maximum)
{
    const double number = value.toDouble(-1);
    return value.isDouble() && std::isfinite(number) && number == std::floor(number)
        && number >= minimum && number <= maximum;
}
}

SerialStatus::SerialStatus() = default;

SerialStatus::~SerialStatus()
{
    qDeleteAll(m_programs);
}

bool SerialStatus::isValid()
{
    return m_isValid && !m_programs.isEmpty() && m_com >= 0 && m_com < m_programs.size()
        && m_brightness >= 0 && m_brightness <= 255 && m_delay >= 0;
}

bool SerialStatus::parse(QString response)
{
    m_isValid = false;
    const auto document = QJsonDocument::fromJson(response.toUtf8());
    if (!document.isObject())
        return false;
    const auto object = document.object();
    if (!object["programs"].isArray() || !object["values"].isArray() || !object["colors"].isArray())
        return false;
    const auto programs = object["programs"].toArray();
    const auto values = object["values"].toArray();
    const auto colors = object["colors"].toArray();
    if (programs.isEmpty() || !integer(object["com"], 0, programs.size() - 1) ||
        !integer(object["brightness"], 0, 255) ||
        !integer(object["delay"], 0, std::numeric_limits<int>::max()) ||
        values.size() != 3 || colors.size() != 6)
        return false;
    for (const auto &value : values)
        if (!integer(value, 0, std::numeric_limits<int>::max()))
            return false;
    for (const auto &color : colors)
        if (!integer(color, 0, 0xffffff))
            return false;

    QList<SerialProgramInformation *> parsed;
    for (const auto &program : programs) {
        // Preserve every metadata index; skipping an invalid entry would remap
        // subsequent effect IDs and could select the wrong physical effect.
        auto info = new SerialProgramInformation;
        if (!program.isObject() || !info->parse(program.toObject())) {
            delete info;
            qDeleteAll(parsed);
            return false;
        }
        parsed.append(info);
    }
    qDeleteAll(m_programs);
    m_programs = parsed;
    m_com = object["com"].toInt();
    m_delay = object["delay"].toInt();
    m_brightness = object["brightness"].toInt();
    for (int i = 0; i < 3; ++i)
        m_values[i] = values[i].toInt();
    for (int i = 0; i < 6; ++i)
        m_colors[i] = colors[i].toInt();
    m_isValid = true;
    return isValid();
}

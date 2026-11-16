#ifndef MESSAGEFRAMER_H
#define MESSAGEFRAMER_H

#include <QByteArray>
#include <functional>

// Bounded incremental LF/CRLF framing, independent of read sizes and timing.
class MessageFramer
{
public:
    static constexpr qsizetype MaxPayload = 3300;
    void reset();
    void feed(const QByteArray &bytes, const std::function<bool()> &expectSeparator,
              const std::function<void(const QByteArray &)> &message,
              const std::function<void()> &invalid);
private:
    void append(char byte);
    QByteArray payload;
    bool separator = false;
    bool carriageReturn = false;
    bool discarding = false;
};
#endif

#include "MessageFramer.h"

void MessageFramer::reset()
{
    payload.clear();
    separator = carriageReturn = discarding = false;
}

void MessageFramer::append(char byte)
{
    if (discarding)
        return;
    if (byte == '\0' || payload.size() == MaxPayload) {
        payload.clear();
        discarding = true;
        return;
    }
    payload.append(byte);
}

void MessageFramer::feed(const QByteArray &bytes, const std::function<bool()> &expectSeparator,
                         const std::function<void(const QByteArray &)> &message,
                         const std::function<void()> &invalid)
{
    for (char byte : bytes) {
        if (byte == '\n') {
            carriageReturn = false;
            // Legacy SEPARATOR reply is a literal @ line, only in this context.
            if (separator && payload.isEmpty() && !discarding && expectSeparator()) {
                reset();
                message("@");
            } else if (separator) {
                separator = false;
            } else {
                const bool bad = discarding;
                const QByteArray complete = payload;
                reset();
                if (bad)
                    invalid();
                else
                    message(complete);
            }
            continue;
        }
        if (carriageReturn) {
            if (separator) {
                append('@');
                separator = false;
            }
            append('\r');
            carriageReturn = false;
        }
        if (byte == '\r') {
            carriageReturn = true;
            continue;
        }
        if (separator) {
            append('@');
            separator = false;
        }
        if (byte == '@')
            separator = true;
        else
            append(byte);
    }
}

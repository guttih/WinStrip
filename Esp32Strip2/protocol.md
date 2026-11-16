# LED strip communication protocol

This is the contract shared by Esp32Strip2, [LinStrip](../LinStrip), and the
existing Windows controller. The default firmware enables USB at 115200 baud
and Bluetooth Classic SPP, advertised as `WinStrip`, on the confirmed
ESP32-WROOM-32. Both connections use the same commands and framing and control
the same strip. If both send updates, the last processed update takes effect.

## Framing and connection ownership

- Requests end with LF (`\n`). An immediately preceding CR is also accepted.
  Replies end with CRLF (`\r\n`). Commands are case-sensitive.
- A trailing `@` immediately before LF or CRLF continues the same logical
  message. The receiver removes this marker and line ending and concatenates
  the next chunk without inserting spaces. An `@` elsewhere is payload.
- Multiple complete requests in one read are processed separately, in order.
  Incomplete input is retained until the message is completed or the connection
  is reset; there is no time-based message boundary.
- Each connection has its own fixed buffer for at most 3300 payload bytes,
  excluding continuation markers, line endings, and the terminating NUL.
  Oversized input or an embedded NUL causes the entire logical message to be
  discarded, including all remaining continuation chunks. Processing resumes
  after its final LF. The firmware sends no overflow/error acknowledgement.
- One call reads at most 256 wire bytes and returns after the first complete
  logical message. Other input remains in the stream for later loop iterations.
- All replies, including numeric metadata, go only to the requesting stream.
  Bluetooth resets partial input and queued replies on disconnect and binds
  reads/replies to a session generation, including when disconnect and reconnect
  occur between loop iterations. Bytes from a new session are kept separate.
- Bluetooth supports one client at a time, with bounded 4096-byte wire queues
  for input and output in addition to its logical-message buffer. A queue
  overflow, write error, or five seconds without write/congestion progress
  invalidates the session and requests disconnection; it does not execute
  truncated input. Clients should await query replies before sending more
  queries. Long metadata replies use the same continuation format as USB.

For example, these wire chunks form a single `STATUS` request:

```text
STA@\n
TUS\n
```

Long replies use chunks of up to 255 payload bytes, with `@\r\n` between
chunks and `\r\n` after the final chunk. Chunking does not define the logical
message limit.

## Queries

| Request | Reply payload |
| --- | --- |
| `STATUS` | `OK` |
| `BUFFERSIZE` | `3300` |
| `SEPARATOR` | `@` |
| `PROGRAMCOUNT` | Number of lighting effects, currently `11` |
| `PROGRAMINFO` | JSON array of effect metadata |
| `ALLSTATUS` | JSON object with effect metadata and current values/colors |
| `COLORS` | JSON object containing `colors` |
| `VALUES` | JSON object containing `delay`, `com`, `brightness`, `values` |
| `PIXELCOUNT` | LED count, currently `62` |
| `HARDWARE` | JSON object with strip type, color scheme, pins, pixel count |
| `VALUES_COLORS` | JSON object with values and colors |

`SEPARATOR` retains the legacy literal reply `@\r\n`. This particular reply
must be interpreted in the context of its query; treating it as an ordinary
continuation chunk would wait for a nonexistent next chunk. No wire-format
change is made here.

Effect IDs are zero-based indexes in the effect metadata array. The `com`
field is an effect ID; it is unrelated to the communication-command enum and
its `COMMAND_COUNT`. Effects and communication commands can evolve separately.

## JSON updates

An effect update supplies `com`, `delay` (milliseconds), `brightness`, and up
to three parameter values. Existing controllers normally send all three:

```json
{"com":8,"delay":30,"brightness":90,"values":[1,15,0]}
```

The six color-bank entries are packed RGB integers (`0xRRGGBB`), sent as decimal
numbers. A colors-only update is supported, for example:

```json
{"colors":[16711680,65280,255,16777215,0,16711935]}
```

`colors` may also accompany an effect update. JSON updates do not send an
acknowledgement. Use a subsequent query to read the resulting state. Unknown
text commands and rejected JSON do not produce debug text on the connection.

## Diagnostics and controller follow-up

Normal builds disable application/driver logs and serve protocol requests over
both USB and Bluetooth. USB remains available for manual protocol queries when
the desktop controller releases it. A controller should still tolerate boot/reset
output from the ESP32 itself when opening USB resets the board.

The `esp32dev-usb-debug` build explicitly reserves USB for startup, connection,
heap, and invalid-JSON diagnostics. USB protocol handling is disabled in that
build; commands and replies use Bluetooth. USB logging starts during setup and
does not need a connected Bluetooth client. The `esp32dev-usb-only` build serves
USB alone without the Bluetooth stack. See [development.md](development.md).

Bluetooth initialization/pairing uses the installed Arduino `BluetoothSerial`
defaults. Replies use asynchronous, handle-specific SPP writes; the loop does
not wait for Bluetooth congestion or write acknowledgements. Stars frame and
pause timing also return to the loop without blocking waits.

LinStrip currently assembles replies on a one-second timer and clears serial
data after emitting a reply. Its Bluetooth integration should instead frame
bytes as they arrive, preserve every complete message, and track requests and
disconnects independently of the visible tab. USB `QSerialPort` and Bluetooth
`QBluetoothSocket` can share this handling through `QIODevice`. Its status
validation must also use the effect metadata rather than `COMMAND_COUNT`.

Windows needs no new command format for this refactor. Windows application work
is deferred; later framing and Bluetooth changes should be checked against this
same contract without changing its USB behavior.

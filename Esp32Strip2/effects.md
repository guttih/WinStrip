# Trail, Aurora and Frost

The effect list now has 11 programs. Up, Down, and Up and down have been
replaced by one Trail program, as requested. Both USB and Bluetooth expose the
same list and parameter metadata.

| ID | Program |
| --- | --- |
| 0 | Off |
| 1 | Reset |
| 2 | Single color |
| 3 | Multicolor |
| 4 | Trail |
| 5 | Stars |
| 6 | Rainbow |
| 7 | Cylon |
| 8 | Sections |
| 9 | Aurora |
| 10 | Frost |

Reconnect the desktop after flashing to reload the list. Previously copied
numeric JSON commands for IDs 5 and above must use the new IDs shown here.

## Trail

Trail combines the three previous movement programs. It preserves the color
bank: color 0 is the moving light, color 1 is the background. A fading tail
follows the light; its pixels are redrawn against the background each frame.

| Parameter | Meaning |
| --- | --- |
| Value 1 | Direction: 0 = up and down, 1 = up, 2 = down |
| Value 2 | Trail length, 0 to LED count minus one |

Delay controls speed. Value 3 is unused. Direction values above 2 select 0;
oversized trails are capped at LED count minus one. A zero-length trail is a
single moving pixel. Up and Down wrap around; Up and down reverses at each end
without duplicating the endpoint frame or wrapping the tail across the ends.

Example: bounce with an eight-pixel tail:

```json
{"com":4,"delay":30,"brightness":90,"values":[0,8,0]}
```

## Frost

Frost uses the first two entries of the existing color bank without replacing
them: color 0 is the steady background and color 1 supplies the sparkle light. For the
original icy look, choose faint white `0c0c0c` and full white `ffffff`
in the controller before selecting Frost.
Sparkles have staggered, independent flash/pause cycles rather than one shared
on/off gate. The count limits the number of simultaneous sparkles; the actual
number varies as individual flashes begin and end. Each slot chooses a pixel
in its own part of the strip, so active sparkles never collide. Even count 62
keeps their timing staggered rather than flashing the whole strip together.

Every non-sparkling pixel stays exactly at color 0 on every frame. An active
sparkle adds color 1 to that pixel's background, clamping RGB channels to 255,
then returns to the exact background after its flash. This never dims a pixel
below its background. Choose a dim background to leave room for highlights;
full white already has no brightness headroom.

| Parameter | Meaning | Zero/default |
| --- | --- | --- |
| Value 1 | Maximum simultaneous sparkles, capped at LED count | 5 |
| Value 2 | Flash duration, 10-60000 milliseconds | 40 ms |
| Value 3 | Pause for each sparkle, 10-60000 milliseconds | 80 ms |

Brightness scales the complete effect. Frost refreshes every 10 ms independently
of the generic Delay control, so a large inherited Delay cannot slow the blink.
Nonzero times below 10 ms are clamped to 10 ms. Timing is scheduled from elapsed
milliseconds, with no blocking waits or old Stars fade cycle.

Fast sparkle:

```json
{"com":10,"delay":10,"brightness":90,"values":[5,40,80]}
```

Slow sparkle with up to ten independently flashing pixels:

```json
{"com":10,"delay":10,"brightness":90,"values":[10,500,1000]}
```

## Aurora

Aurora uses the current color bank without changing it: color 0 is the
background and colors 1-3 are ribbon colors. Ribbons move back and forth at
different rates with soft edges, changing intensity and optional shimmer.
For a northern-lights palette, select dark blue `000008`, green `00ff50`,
teal `00a0b4`, and purple `7800c8` yourself in colors 0-3. These suggestions
also appear in the firmware program descriptions shown by the desktop app.

| Parameter | Meaning | Zero/default |
| --- | --- | --- |
| Value 1 | Ribbon width in pixels, capped at LED count | LED count / 3 + 1 |
| Value 2 | Shimmer intensity, capped at 255 | 0 disables shimmer |
| Value 3 | Number of ribbons, capped at 8 | 3 |

Delay controls animation speed; 30 ms is a useful starting point:

```json
{"com":9,"delay":30,"brightness":90,"values":[22,50,3]}
```

## Palettes, controllers and verification

Aurora and Frost both preserve the existing color bank when selected or when
parameters change. Colors change only through an explicit `colors` update or
the existing Reset program. Colors-only updates still work.
Query `VALUES_COLORS` or `ALLSTATUS` to read
normalized parameters and the current palette. The six-color bank is shared
with existing effects, as before.

The current LinStrip reads names, parameter labels and effect counts from
firmware metadata and supports the current effect list. Reconnect to reload the
list after flashing. Neither effect silently changes colors behind the UI.
Zero/default parameter normalization can still leave the displayed parameters
out of date until status is refreshed; a desktop follow-up should refresh
`ALLSTATUS` after an effect change to show the returned state. Desktop source
is unchanged in this firmware feature.

Windows application changes remain deferred. Its effect-count validation must
be checked against the current metadata array and new IDs on a Windows machine.
The removal of the old movement entries intentionally changes IDs 5 and above;
validate effect selections against effect count, not command count.

Run `bash test/run_host_tests.sh` for deterministic render checks: stable
background pixels, staggered sparkle limits, fast/slow on/off boundaries,
background restoration, saturating color addition, short strips and elapsed-time
rollover, plus Aurora motion/parameter extremes and Trail direction, reversal,
wrap, tail fading, and zero-length behavior.
The production rendering functions are shared with these tests; this does not
verify hardware timing, visual quality, or desktop interaction. After upload,
check all three Trail directions and length 0/8/61, the Aurora/Frost examples,
custom palettes, count 1/62, switching effects, and queries
over USB/Bluetooth while the animations run.

Verified on 2026-11-17: all 32 host checks and all three firmware builds passed.

| Build | Flash bytes | Static RAM bytes |
| --- | --- | --- |
| `esp32dev` | 1,170,309 / 1,310,720 | 56,628 / 327,680 |
| `esp32dev-usb-debug` | 1,171,609 / 1,310,720 | 53,316 / 327,680 |
| `esp32dev-usb-only` | 345,025 / 1,310,720 | 26,460 / 327,680 |

A source-derived conservative bound for `ALLSTATUS` with the 11 effect entries
and maximum numeric state fields is 2425 payload bytes (2454 with framing),
within the 3300-byte message limit and 4096-byte Bluetooth wire queues. This is
a serializer-size estimate, not a device capture. No firmware was uploaded and
no physical LED, USB/SPP, or desktop interaction checks were performed here.

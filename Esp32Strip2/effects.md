# Aurora and Frost

New programs are appended to the existing effect list: Aurora is `com:11`,
Frost is `com:12`, and `PROGRAMCOUNT` is now 13. IDs 0-10, including Stars at 7,
are unchanged. Both work over the normal USB and Bluetooth protocol.

## Frost

Frost uses the first two entries of the existing color bank without replacing
them: color 0 is the steady background and color 1 supplies the sparkle light. For the
original icy look, choose faint white `(12,12,12)` and full white `(255,255,255)`
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
{"com":12,"delay":10,"brightness":90,"values":[5,40,80]}
```

Slow sparkle with up to ten independently flashing pixels:

```json
{"com":12,"delay":10,"brightness":90,"values":[10,500,1000]}
```

## Aurora

Aurora blends drifting ribbons over a dark blue background. Its entry palette
is dark blue `(0,0,8)`, green `(0,255,80)`, teal `(0,160,180)`, and purple
`(120,0,200)` in colors 0-3. Ribbons move back and forth at different rates with
soft edges, changing intensity and optional shimmer.

| Parameter | Meaning | Zero/default |
| --- | --- | --- |
| Value 1 | Ribbon width in pixels, capped at LED count | LED count / 3 + 1 |
| Value 2 | Shimmer intensity, capped at 255 | 0 disables shimmer |
| Value 3 | Number of ribbons, capped at 8 | 3 |

Delay controls animation speed; 30 ms is a useful starting point:

```json
{"com":11,"delay":30,"brightness":90,"values":[22,50,3]}
```

## Palettes, controllers and verification

Aurora applies its entry palette when selected. Frost always preserves the
existing color bank when selected. Editing parameters while staying on the
same effect preserves custom colors. An explicit `colors` array in the same
effect update overrides Aurora's entry palette.
Colors-only updates still work. Query `VALUES_COLORS` or `ALLSTATUS` to read
normalized parameters and the current palette. The six-color bank is shared
with existing effects, as before.

The current LinStrip reads names, parameter labels and effect counts from
firmware metadata and supports these appended effects. Reconnect to reload the
list after flashing. Its UI may retain the previous colors/zero parameter values
until status is refreshed: firmware entry palettes and zero/default normalization
are not an acknowledgement. A desktop follow-up should refresh `ALLSTATUS`
after an effect change to show that returned state. Desktop source is unchanged
in this firmware feature.

Windows application changes remain deferred. Its effect-count validation must
be checked against the expanded metadata array on a Windows machine; preserve
existing effect IDs and validate against effect count, not command count.

Run `bash test/run_host_tests.sh` for deterministic render checks: stable
background pixels, staggered sparkle limits, fast/slow on/off boundaries,
background restoration, saturating color addition, short strips and elapsed-time
rollover, plus Aurora motion and parameter extremes.
The production rendering functions are shared with these tests; this does not
verify hardware timing, visual quality, or desktop interaction. After upload,
check both examples, custom palettes, count 1/62, switching effects, and queries
over USB/Bluetooth while the animations run.

Verified on 2026-11-17: all 28 host checks and all three firmware builds passed.

| Build | Flash bytes | Static RAM bytes |
| --- | --- | --- |
| `esp32dev` | 1,170,745 / 1,310,720 | 56,620 / 327,680 |
| `esp32dev-usb-debug` | 1,172,033 / 1,310,720 | 53,308 / 327,680 |
| `esp32dev-usb-only` | 345,509 / 1,310,720 | 26,452 / 327,680 |

A source-derived conservative bound for `ALLSTATUS` with the 13 effect entries
and maximum numeric state fields is 2461 payload bytes (2490 with framing),
within the 3300-byte message limit and 4096-byte Bluetooth wire queues. This is
a serializer-size estimate, not a device capture. No firmware was uploaded and
no physical LED, USB/SPP, or desktop interaction checks were performed here.

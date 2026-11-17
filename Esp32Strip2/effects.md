# Aurora and Frost

New programs are appended to the existing effect list: Aurora is `com:11`,
Frost is `com:12`, and `PROGRAMCOUNT` is now 13. IDs 0-10, including Stars at 7,
are unchanged. Both work over the normal USB and Bluetooth protocol.

## Frost

Frost uses the first two entries of the existing color bank without replacing
them: color 0 is the background and color 1 is the sparkle highlight. For the
original icy look, choose faint white `(12,12,12)` and full white `(255,255,255)`
in the controller before selecting Frost.
Each flash lights exactly the selected number of distinct pixels with color 1;
all remaining pixels keep color 0. During the pause every pixel returns to the
background. Each cycle chooses a new arrangement; individual pixels may repeat
between cycles. Count 62 makes the whole current strip flash.

| Parameter | Meaning | Zero/default |
| --- | --- | --- |
| Value 1 | Simultaneous sparkle count, capped at LED count | 5 |
| Value 2 | Flash duration, 10-60000 milliseconds | 40 ms |
| Value 3 | Background-only pause, 10-60000 milliseconds | 80 ms |

Brightness scales the complete effect. Frost refreshes every 10 ms independently
of the generic Delay control, so a large inherited Delay cannot slow the blink.
Nonzero times below 10 ms are clamped to 10 ms. Timing is scheduled from elapsed
milliseconds, with no blocking waits or old Stars fade cycle.

Fast sparkle:

```json
{"com":12,"delay":10,"brightness":90,"values":[5,40,80]}
```

Slow blink with ten pixels bright together:

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

Run `bash test/run_host_tests.sh` for deterministic render checks: exact sparkle
counts, fast/slow on/off boundaries, background restoration, reselection, short
strips and elapsed-time rollover, plus Aurora motion and parameter extremes.
The production rendering functions are shared with these tests; this does not
verify hardware timing, visual quality, or desktop interaction. After upload,
check both examples, custom palettes, count 1/62, switching effects, and queries
over USB/Bluetooth while the animations run.

Verified on 2026-11-17: all 28 host checks and all three firmware builds passed.

| Build | Flash bytes | Static RAM bytes |
| --- | --- | --- |
| `esp32dev` | 1,170,553 / 1,310,720 | 56,620 / 327,680 |
| `esp32dev-usb-debug` | 1,171,849 / 1,310,720 | 53,308 / 327,680 |
| `esp32dev-usb-only` | 345,325 / 1,310,720 | 26,452 / 327,680 |

A source-derived conservative bound for `ALLSTATUS` with the 13 effect entries
and maximum numeric state fields is 2476 payload bytes (2505 with framing),
within the 3300-byte message limit and 4096-byte Bluetooth wire queues. This is
a serializer-size estimate, not a device capture. No firmware was uploaded and
no physical LED, USB/SPP, or desktop interaction checks were performed here.

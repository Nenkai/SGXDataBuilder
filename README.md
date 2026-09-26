# SGXDBuilder

This tool allows building SGX audio file banks used in various PSP and PS3 games (sgd/sgh/sgb) from standard audio formats.

This is a format and PSP/PS3 Sound Library privately created by Sony/SCE and used in games published by them (listed below) for PSP and PS3 Games. This tool focuses on the `SGX` variant found in PS3 and PSP games.

It has been superseded by:
* `SXD` (aka `sndx`, `sceSndx` with file extensions `.sxd1`/`.sxd2`/`.sxd3`) - PS Vita/PS4, used in:
  * Gran Turismo Sport (PS4)
  * Gravity Rush (PS4)
  * Freedom Wars (PSV)
  * Soul Sacrifice (PSV)
  * Everybody's Golf VR (PS4)
  * The Last Guardian (PS4)
  * Fate/Estella (PS4)
  * Chaos Rings 2 (PSV)
  * Chaos Rings 3 (PSV)
  * Network Media Player [PCSF00635] (PSV)
  * Welcome Park [NPXS10007] (PSV)
* And then `SZD` (aka `sndz`, `sceSndz`) - PS4/PS5, used in:
  * Gran Turismo 7 (PS4/PS5)
  * Astro's Playroom (PS5)

## Unsupported features
* Sequenced Chunks/Files (SEQD)
* Notes (RGND, MIDI/Note playback)
* Literally anything else, it is a complex format designed to fine tune audio playback

## Supported Input formats:
* PS-ADPCM [PS3/PSP] - `.vag`
* AC3 [PS3] - `.ac3`
* AT3 [PSP] - `.at3`
* PCM 16 LE [PS3] - `.wav`

## List of games using SGX:
* Gran Turismo 5
* Gran Turismo 6
* Gran Turismo PSP
  * SE: PS-ADPCM
  * Has RGND/SEQD
* LocoRoco Cocoreccho
* Ape Escape Move
* Genji
* Kurohyo 1/2 [PSP]
  * SE/Voice: PS-ADPCM
  * BGM: Atrac3PLUS with RIFF header
  * Has WMRK
* Brave Story - New Traveler [PSP] 
  * SE/Voice: PS-ADPCM
  * BGM: Atrac3PLUS with RIFF header
  * Has RGND/SEQD
* Afrika
* Bleach: Soul Resurrección
* Ni No Kuni
* Boku no Natsuyasumi 3
* White Knight Chronicles I & II
* Tokyo Jungle
* Rain
* Kung Fu Rider

For SGXD playback, refer to [vgmstream](https://github.com/vgmstream/vgmstream).
The format has been [mostly documented](https://github.com/Nenkai/SGXDataBuilder/blob/master/SGXDBuilder/SGXD.bt) with debug symbols from Folklore, PS3

## General understanding of the formats (including SNDX/SZD)

A general SGX is composed of chunks, which will be read in this hierarchy in order to play music:

* Names - Defines a track by name, will point to one of the following (for example)
  * Wave - Waveform file (direct sound sample)
  * Region/Sequence - Midi, will point back to waveforms for instrument samples
  * Trans
 
In `sndx`, Names point to a "Request" list instead, which will then point to Waves/Sequences/Trans

# SGXDBuilder

This tool allows building SGX audio file banks used in various PSP and PS3 games (sgd/sgh/sgb) from standard audio formats.

This is a format and PSP/PS3 Sound Library privately created by Sony/SCE and used in games published by them (listed below) for PSP and PS3 Games.
It has been superseded by:
* `SXD` - PSV-PS4 (Gran Turismo Sport, Gravity Rush, Freedom Wars, Soul Sacrifice)
* `SZD` - PS4-PS5 (Gran Turismo 7, Astro's Playroom)

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

For SGXD playback, refer to [vgmstream](https://github.com/vgmstream/vgmstream).
The format has been [mostly documented](https://github.com/Nenkai/SGXDataBuilder/blob/master/SGXDBuilder/SGXD.bt) with debug symbols from Folklore, PS3

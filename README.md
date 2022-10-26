# SGXDBuilder

This tool allows building SGX audio file banks used in various PSP and PS3 games (sgd/sgh/sgb) from standard audio formats (wav/ac3).

This is a format and PS3 library privately created by Sony/SCE used by a few games published by them (listed below) for PSP and PS3 Games.
It has been superseded by:
* `SXD` - PSV-PS4 (Gran Turismo Sport, Gravity Rush, Freedom Wars, Soul Sacrifice)
* `SZD` - PS4-PS5 (Gran Turismo 7, Astro's Playroom)

## Unsupported features
* Sequenced Chunks/Files (SEQD)
* Notes (RGND, MIDI/Note playback)
* Literally anything else, it is a complex format designed to fine tune audio playback

## List of games using SGX:
* Gran Turismo 5
* Gran Turismo 6
* Gran Turismo PSP
* LocoRoco Cocoreccho
* Ape Escape Move
* Genji
* Kurohyo 1/2 
* Brave Story - New Traveler 
* Afrika
* Bleach: Soul Resurrección
* Ni No Kuni
* Boku no Natsuyasumi 3
* White Knight Chronicles I & II
* Tokyo Jungle
* Rain

For SGXD playback, refer to [vgmstream](https://github.com/vgmstream/vgmstream).
The format has been [mostly documented](https://github.com/Nenkai/SGXDataBuilder/blob/master/SGXDBuilder/SGXD.bt) with debug symbols from Folklore, PS3

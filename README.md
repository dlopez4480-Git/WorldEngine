# WorldEngine

This is the dev log of WorldEngine.

Notes on ARGS


[0] SEED
The number which seeds the randomization of the world. Using the same seed gives you the same world.

[1] SCALE
This string changes the sizing of the final world.

VERYSMALL:
SMALL:
MEDIUM:
LARGE:
VERYLARGE:

[2] NULL
[3] NULL
[4] NULL
[5] MAPTYPE
This string will change what type of world is generated

CONTINENTS&ISLANDS: Generates a map similar to Earth, with a scattering of large and medium sized continents with some smaller islands in a large world ocean
DEBUG: Uses a scale of the Earth, as a map seed. I've been using this to avoid the tedious process of actual land generation.

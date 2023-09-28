- Add a menu that asks if you're converting images to ascii or displaying ascii from a folder
- - request a file path that can be used by the app to grab folders of images from anywhere and put them anywhere

- When the app writes ascii text files, have it create a new directory with the same name as the folder holding the images to read

- Create more diverse ascii conversions that take into account a wider spectrum of brightness

- make presets for level of brightness.

- Create a system that takes any size image, and rather than compressing it compares its size to the window size and samples a grid of pixels from the image to inform what a single character on the console should be.
  - an image has 4 times as many pixels as there are chars on the grid, so each char should take into account a grid of 4x4 to determine brightness and what char should be placed there.
  - could even account for which section of the grid is brightest or darkest and use different characters to represent blending between brightness levels in a single "pixel"
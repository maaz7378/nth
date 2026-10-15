# shader-effects-inc/shaders inventory

The Shaders section (ED-NEW-shader-effects-1). Source: shader-effects-inc/shaders (MIT, THIRD_PARTY_NOTICES "shader-effects-inc shaders"), packages/core/src/shaders, one folder per effect, each effect's role, category and props read from its index.ts; the port follows each effect's emitted WGSL (the package's shader snapshots).

Total effects: 200. Roles: filter 65, generator 51, shapeEffect 23, simulation 19, warp 18, shape 15, media 5, structural 2, overlay 2.
Status: ported 50, wallpaper-capable generators still to port 0, not standalone 149 (each with its reason), not a live wallpaper 1 (SolidColor: a flat colour never moves).

| Effect | Role | Category | Props | Status | Why not (yet) a wallpaper |
|---|---|---|---|---|---|
| AngularBlur | filter | Blurs | 8 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Arc | shape | ? | 0 | NOT STANDALONE | a single shape (circle, star, ...): a building block for composition, not a wallpaper on its own |
| Ascii | filter | Stylize | 7 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Aurora | generator | Textures | 14 | PORTED (sei-aurora) | Centre Y defaults to 0.7 (the package's 0 puts the curtains' base on the edge, out of sight; its cover shows the base high up) |
| BarShift | warp | Distortions | 5 | NOT STANDALONE | distorts an input picture: composition over the current wallpaper, not standalone |
| BarnDoors | filter | Transitions | 4 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Beam | generator | Textures | 9 | PORTED (sei-beam) |  |
| Bend | warp | Distortions | 3 | NOT STANDALONE | distorts an input picture: composition over the current wallpaper, not standalone |
| Blob | generator | Textures | 15 | PORTED (sei-blob) | Origin (the corner the centre is measured from) is left out: Centre X and Y place it directly |
| BlockDissolve | filter | Transitions | 4 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| BlockNoise | generator | Textures | 8 | PORTED (sei-block-noise) | the package's optional multi-stop colour list is not offered (its two colours are) |
| BlueNoise | generator | Textures | 7 | PORTED (sei-blue-noise) | the package's optional multi-stop colour list is not offered (its two colours are) |
| Blur | filter | Blurs | 1 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Boids | simulation | Interactive | 15 | NOT STANDALONE | keeps state between frames (ping-pong buffers): needs a multi-pass state pipeline the one-pass wallpaper renderer does not have; reads the pointer |
| BokehBlur | filter | Blurs | 8 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| BrickPattern | generator | Textures | 13 | PORTED (sei-brick-pattern) |  |
| BrightnessContrast | filter | Adjustments | 2 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| BrushedMetal | shapeEffect | Shape Effects | 17 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| Bulge | warp | Distortions | 3 | NOT STANDALONE | distorts an input picture: composition over the current wallpaper, not standalone |
| CRTScreen | filter | Stylize | 8 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| CarbonFiber | shapeEffect | Shape Effects | 18 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| Chalkboard | filter | Stylize | 7 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| ChannelBlur | filter | Blurs | 3 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| CheckerWipe | filter | Transitions | 4 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Checkerboard | generator | Textures | 5 | PORTED (sei-checkerboard) |  |
| Chevron | generator | Textures | 9 | PORTED (sei-chevron) |  |
| ChromaFlow | simulation | Interactive | 8 | NOT STANDALONE | keeps state between frames (ping-pong buffers): needs a multi-pass state pipeline the one-pass wallpaper renderer does not have; reads the pointer |
| ChromaticAberration | filter | Stylize | 5 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Chrome | shapeEffect | Shape Effects | 18 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| Circle | shape | ? | 0 | NOT STANDALONE | a single shape (circle, star, ...): a building block for composition, not a wallpaper on its own |
| ColorWheel | generator | Textures | 8 | PORTED (sei-color-wheel) | colour spaces: Linear or OKLab (the package's OKLCH default maps to OKLab) |
| CompressionArtifacts | filter | Stylize | 1 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| ConcentricSpin | warp | Distortions | 6 | NOT STANDALONE | distorts an input picture: composition over the current wallpaper, not standalone |
| ConicGradient | generator | Textures | 6 | PORTED (sei-conic-gradient) | the package's optional multi-stop colour list is not offered (its two colours are) |
| ContourLines | filter | Stylize | 9 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| CornerPin | warp | Distortions | 5 | NOT STANDALONE | distorts an input picture: composition over the current wallpaper, not standalone |
| Crescent | shape | ? | 0 | NOT STANDALONE | a single shape (circle, star, ...): a building block for composition, not a wallpaper on its own |
| Cross | shape | ? | 0 | NOT STANDALONE | a single shape (circle, star, ...): a building block for composition, not a wallpaper on its own |
| Crystal | shapeEffect | Shape Effects | 16 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| CurlNoise | generator | Textures | 8 | PORTED (sei-curl-noise) | the package's optional multi-stop colour list is not offered (its two colours are) |
| CursorRipples | filter | Interactive | 5 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper; reads the pointer |
| CursorTrail | simulation | Interactive | 7 | NOT STANDALONE | keeps state between frames (ping-pong buffers): needs a multi-pass state pipeline the one-pass wallpaper renderer does not have; reads the pointer |
| DataMosh | simulation | Stylize | 7 | NOT STANDALONE | keeps state between frames (ping-pong buffers): needs a multi-pass state pipeline the one-pass wallpaper renderer does not have |
| DiamondGradient | generator | Textures | 8 | PORTED (sei-diamond-gradient) | the package's optional multi-stop colour list is not offered (its two colours are) |
| DiamondWipe | filter | Transitions | 4 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| DiffuseBlur | filter | Blurs | 3 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| DisplacementMap | filter | Distortions | 5 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Dither | filter | Stylize | 7 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| DotGrid | generator | Textures | 7 | PORTED (sei-dot-grid) | Twinkle defaults to 0.5 (the package's 0 leaves a still, flat texture, and every wallpaper moves) |
| DropShadow | filter | Stylize | 6 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Duotone | filter | Adjustments | 4 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Ellipse | shape | ? | 0 | NOT STANDALONE | a single shape (circle, star, ...): a building block for composition, not a wallpaper on its own |
| Emboss | shapeEffect | Shape Effects | 11 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| Engraving | filter | Stylize | 9 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| ErosionNoise | generator | Textures | 7 | PORTED (sei-erosion-noise) | the package's optional multi-stop colour list is not offered (its two colours are) |
| Exposure | filter | Adjustments | 1 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| FallingLines | generator | Textures | 11 | PORTED (sei-falling-lines) | the trail colour is opaque here (black by default, which reads as the package's transparent trail over the dark ground) |
| FilmGrain | filter | Stylize | 3 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| FilmStock | filter | Adjustments | 5 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Flip | warp | Distortions | 2 | NOT STANDALONE | distorts an input picture: composition over the current wallpaper, not standalone |
| FloatingParticles | simulation | Textures | 12 | NOT STANDALONE | keeps state between frames (ping-pong buffers): needs a multi-pass state pipeline the one-pass wallpaper renderer does not have |
| FlowField | warp | Distortions | 6 | NOT STANDALONE | distorts an input picture: composition over the current wallpaper, not standalone |
| Flower | shape | ? | 0 | NOT STANDALONE | a single shape (circle, star, ...): a building block for composition, not a wallpaper on its own |
| FlowingGradient | generator | Textures | 8 | PORTED (sei-flowing-gradient) |  |
| FlutedGlass | filter | Distortions | 14 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Fog | simulation | Interactive | 10 | NOT STANDALONE | keeps state between frames (ping-pong buffers): needs a multi-pass state pipeline the one-pass wallpaper renderer does not have; reads the pointer |
| Form3D | shapeEffect | Distortions | 8 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| FractalNoise | generator | Textures | 9 | PORTED (sei-fractal-noise) |  |
| Frost | shapeEffect | Shape Effects | 22 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| GaborNoise | generator | Textures | 9 | PORTED (sei-gabor-noise) | the package's optional multi-stop colour list is not offered (its two colours are) |
| Glass | shapeEffect | Shape Effects | 14 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| GlassTiles | filter | Distortions | 4 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Glitch | filter | Stylize | 7 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Glow | filter | Stylize | 3 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Godrays | generator | Textures | 8 | PORTED (sei-godrays) | its transparent default colour is black here (the wallpaper has no page behind it) |
| Goo | shapeEffect | Shape Effects | 26 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| GradientMap | filter | Stylize | 10 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Grayscale | filter | Adjustments | 0 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Grid | generator | Textures | 8 | PORTED (sei-grid) | its transparent default colour is black here (the wallpaper has no page behind it) |
| GridDistortion | warp | Interactive | 4 | NOT STANDALONE | distorts an input picture: composition over the current wallpaper, not standalone; reads the pointer |
| Group | structural | Utilities | 0 | NOT STANDALONE | a container (Group, KeyFrames): no picture of its own |
| HTMLInCanvas | media | Textures | 0 | NOT STANDALONE | needs an image, a video, a webcam or text as input: not a wallpaper |
| Halftone | filter | Stylize | 14 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Heart | shape | ? | 0 | NOT STANDALONE | a single shape (circle, star, ...): a building block for composition, not a wallpaper on its own |
| Heatmap | shapeEffect | Shape Effects | 15 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| HexGrid | generator | Textures | 8 | PORTED (sei-hex-grid) |  |
| Hologram | shapeEffect | Shape Effects | 22 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| Holographic | shapeEffect | Shape Effects | 16 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| HueShift | filter | Adjustments | 1 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| ImageTexture | media | Textures | 1 | NOT STANDALONE | needs an image, a video, a webcam or text as input: not a wallpaper |
| InkFlow | simulation | Interactive | 12 | NOT STANDALONE | keeps state between frames (ping-pong buffers): needs a multi-pass state pipeline the one-pass wallpaper renderer does not have; reads the pointer |
| Invert | filter | Adjustments | 0 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| IrisWipe | filter | Transitions | 4 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Irradiance | shapeEffect | Shape Effects | 17 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| IsometricCubes | generator | Textures | 9 | PORTED (sei-isometric-cubes) |  |
| Kaleidoscope | warp | Distortions | 2 | NOT STANDALONE | distorts an input picture: composition over the current wallpaper, not standalone |
| KeyFrames | overlay | Stylize | 12 | NOT STANDALONE | draws over another layer: composition |
| LensDistortion | filter | Stylize | 19 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| LensFlare | generator | Stylize | 18 | PORTED (sei-lens-flare) |  |
| LightEdge | shapeEffect | Shape Effects | 21 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| LightLeak | filter | Stylize | 10 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Line | generator | Shapes | 9 | PORTED (sei-line) |  |
| LinearBlur | filter | Blurs | 8 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| LinearGradient | generator | Textures | 7 | PORTED (sei-linear-gradient) | the package's optional multi-stop colour list is not offered (its two colours are) |
| LinearWipe | filter | Transitions | 4 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| LiquidMetal | shapeEffect | Shape Effects | 16 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| Liquify | warp | Interactive | 4 | NOT STANDALONE | distorts an input picture: composition over the current wallpaper, not standalone; reads the pointer |
| MagneticFilings | simulation | Interactive | 11 | NOT STANDALONE | keeps state between frames (ping-pong buffers): needs a multi-pass state pipeline the one-pass wallpaper renderer does not have; reads the pointer |
| Marble | generator | Textures | 8 | PORTED (sei-marble) |  |
| MeshGradient | generator | Textures | 11 | PORTED (sei-mesh-gradient) | the package's default five-stop palette is offered as five colours (its Colour A/B fallback is colours 1 and 5); the anchors it computes on the CPU are computed in the shader from the same formula; no output dither |
| Mirror | warp | Distortions | 1 | NOT STANDALONE | distorts an input picture: composition over the current wallpaper, not standalone |
| MultiPointGradient | generator | Textures | 12 | PORTED (sei-multi-point-gradient) |  |
| Nebula | shapeEffect | Shape Effects | 22 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| Neon | shapeEffect | Shape Effects | 24 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| NoiseDissolve | filter | Transitions | 5 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| ObjectTracker | overlay | Stylize | 20 | NOT STANDALONE | draws over another layer: composition |
| Obsidian | shapeEffect | Shape Effects | 20 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| PagePeel | filter | Transitions | 7 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Paper | filter | Stylize | 4 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Parallelogram | shape | ? | 0 | NOT STANDALONE | a single shape (circle, star, ...): a building block for composition, not a wallpaper on its own |
| ParticleField | simulation | Stylize | 16 | NOT STANDALONE | keeps state between frames (ping-pong buffers): needs a multi-pass state pipeline the one-pass wallpaper renderer does not have |
| ParticleFlow | simulation | Interactive | 12 | NOT STANDALONE | keeps state between frames (ping-pong buffers): needs a multi-pass state pipeline the one-pass wallpaper renderer does not have; reads the pointer |
| Particles | simulation | Shape Effects | 22 | NOT STANDALONE | keeps state between frames (ping-pong buffers): needs a multi-pass state pipeline the one-pass wallpaper renderer does not have |
| PerlinNoise | generator | Textures | 8 | PORTED (sei-perlin-noise) | the package's optional multi-stop colour list is not offered (its two colours are) |
| Perspective | warp | Distortions | 5 | NOT STANDALONE | distorts an input picture: composition over the current wallpaper, not standalone |
| PixelSort | simulation | Interactive | 6 | NOT STANDALONE | keeps state between frames (ping-pong buffers): needs a multi-pass state pipeline the one-pass wallpaper renderer does not have; reads the pointer |
| PixelThrow | simulation | Interactive | 7 | NOT STANDALONE | keeps state between frames (ping-pong buffers): needs a multi-pass state pipeline the one-pass wallpaper renderer does not have; reads the pointer |
| Pixelate | filter | Stylize | 3 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Plasma | generator | Textures | 9 | PORTED (plasma (live tier, ED-NEW-ocean-wallpaper-1)) |  |
| Plastic | shapeEffect | Shape Effects | 24 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| PolarCoordinates | warp | Distortions | 3 | NOT STANDALONE | distorts an input picture: composition over the current wallpaper, not standalone |
| Polygon | shape | ? | 0 | NOT STANDALONE | a single shape (circle, star, ...): a building block for composition, not a wallpaper on its own |
| Posterize | filter | Adjustments | 1 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Prism | generator | Textures | 13 | PORTED (sei-prism) |  |
| ProgressiveBlur | filter | Blurs | 6 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| RadialGradient | generator | Textures | 8 | PORTED (sei-radial-gradient) | the package's optional multi-stop colour list is not offered (its two colours are) |
| RadialWipe | filter | Transitions | 6 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| RandomBars | filter | Transitions | 5 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| ReactionDiffusion | simulation | Interactive | 20 | NOT STANDALONE | keeps state between frames (ping-pong buffers): needs a multi-pass state pipeline the one-pass wallpaper renderer does not have; reads the pointer |
| RectangularCoordinates | warp | Distortions | 2 | NOT STANDALONE | distorts an input picture: composition over the current wallpaper, not standalone |
| ReflectivePlane | filter | Stylize | 6 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Repeater | structural | Distortions | 29 | NOT STANDALONE | a container (Group, KeyFrames): no picture of its own |
| Ring | shape | ? | 0 | NOT STANDALONE | a single shape (circle, star, ...): a building block for composition, not a wallpaper on its own |
| RippleWipe | filter | Transitions | 5 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Ripples | generator | Textures | 9 | PORTED (sei-ripples) |  |
| RoundedRect | shape | ? | 0 | NOT STANDALONE | a single shape (circle, star, ...): a building block for composition, not a wallpaper on its own |
| Satin | generator | Textures | 18 | PORTED (sei-satin) | Folds is live here (a loop to 12), compile-time in the package; thread grain uses the library's gradient noise |
| Saturation | filter | Adjustments | 1 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Scratches | generator | Textures | 6 | PORTED (sei-scratches) |  |
| Sharpness | filter | Adjustments | 1 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Shatter | simulation | Interactive | 9 | NOT STANDALONE | keeps state between frames (ping-pong buffers): needs a multi-pass state pipeline the one-pass wallpaper renderer does not have; reads the pointer |
| SimplexNoise | generator | Textures | 8 | PORTED (sei-simplex-noise) | the package's optional multi-stop colour list is not offered (its two colours are) |
| SineWave | generator | Textures | 9 | PORTED (sei-sine-wave) |  |
| SliceWipe | warp | Transitions | 3 | NOT STANDALONE | distorts an input picture: composition over the current wallpaper, not standalone |
| Smoke | simulation | Interactive | 15 | NOT STANDALONE | keeps state between frames (ping-pong buffers): needs a multi-pass state pipeline the one-pass wallpaper renderer does not have; reads the pointer |
| SmokeFill | simulation | Shape Effects | 22 | NOT STANDALONE | keeps state between frames (ping-pong buffers): needs a multi-pass state pipeline the one-pass wallpaper renderer does not have |
| SmokeFlow | simulation | Interactive | 10 | NOT STANDALONE | keeps state between frames (ping-pong buffers): needs a multi-pass state pipeline the one-pass wallpaper renderer does not have; reads the pointer |
| Solarize | filter | Adjustments | 2 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| SolidColor | generator | Textures | 1 | NOT A LIVE WALLPAPER | one flat colour never moves, and every live wallpaper must (R-TEST-1); a flat colour is not an animated wallpaper (ED-NEW-shader-effects-2) |
| Sparkle | filter | Stylize | 8 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Spherize | filter | Distortions | 7 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Spiral | generator | Textures | 10 | PORTED (sei-spiral) |  |
| Star | shape | ? | 0 | NOT STANDALONE | a single shape (circle, star, ...): a building block for composition, not a wallpaper on its own |
| Stone | filter | Stylize | 0 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Strands | generator | Textures | 14 | PORTED (sei-strands) | the package's default four-stop gradient is offered as four colours |
| Stretch | warp | Distortions | 3 | NOT STANDALONE | distorts an input picture: composition over the current wallpaper, not standalone |
| Stripes | generator | Textures | 9 | PORTED (sei-stripes) |  |
| StudioBackground | generator | Textures | 19 | PORTED (sei-studio-background) | the four ambient glows the package orbits on the CPU orbit in the shader (same seeded constants, closed-form time) |
| SunBurst | generator | Textures | 8 | PORTED (sei-sunburst) |  |
| Surface3D | shapeEffect | Distortions | 0 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| Swirl | generator | Textures | 6 | PORTED (sei-swirl) |  |
| Teardrop | shape | ? | 0 | NOT STANDALONE | a single shape (circle, star, ...): a building block for composition, not a wallpaper on its own |
| Text | media | Textures | 14 | NOT STANDALONE | needs an image, a video, a webcam or text as input: not a wallpaper |
| ThinFilm | shapeEffect | Shape Effects | 21 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| TiltShift | filter | Blurs | 7 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| TimeTrail | simulation | Stylize | 12 | NOT STANDALONE | keeps state between frames (ping-pong buffers): needs a multi-pass state pipeline the one-pass wallpaper renderer does not have |
| Tint | filter | Adjustments | 3 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Trapezoid | shape | ? | 0 | NOT STANDALONE | a single shape (circle, star, ...): a building block for composition, not a wallpaper on its own |
| TriangularGrid | generator | Textures | 10 | PORTED (sei-triangular-grid) |  |
| Tritone | filter | Adjustments | 5 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Truchet | generator | Textures | 8 | PORTED (sei-truchet) |  |
| Twirl | warp | Distortions | 1 | NOT STANDALONE | distorts an input picture: composition over the current wallpaper, not standalone |
| VHS | filter | Stylize | 4 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| VenetianBlinds | filter | Transitions | 5 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Vesica | shape | ? | 0 | NOT STANDALONE | a single shape (circle, star, ...): a building block for composition, not a wallpaper on its own |
| Vibrance | filter | Adjustments | 1 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| VideoTexture | media | Textures | 2 | NOT STANDALONE | needs an image, a video, a webcam or text as input: not a wallpaper |
| Vignette | filter | Stylize | 5 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Voronoi | generator | Textures | 9 | PORTED (sei-voronoi) |  |
| Voxels | shapeEffect | Shape Effects | 31 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| Water | shapeEffect | Shape Effects | 23 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| Watercolor | filter | Stylize | 5 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| WaveDistortion | warp | Distortions | 5 | NOT STANDALONE | distorts an input picture: composition over the current wallpaper, not standalone |
| Waveform | generator | Textures | 18 | PORTED (sei-waveform) | the package's optional multi-stop colour list is not offered (its two colours are) |
| WaveletNoise | generator | Textures | 9 | PORTED (sei-wavelet-noise) | the package's optional multi-stop colour list is not offered (its two colours are) |
| Weave | generator | Textures | 5 | PORTED (sei-weave) |  |
| WebcamTexture | media | Textures | 1 | NOT STANDALONE | needs an image, a video, a webcam or text as input: not a wallpaper |
| Wool | filter | Stylize | 0 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| WorleyNoise | generator | Textures | 14 | PORTED (sei-worley-noise) |  |
| ZoomBlur | filter | Blurs | 9 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |

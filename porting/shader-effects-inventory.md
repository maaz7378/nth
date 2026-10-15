# shader-effects-inc/shaders inventory

The Shaders section (ED-NEW-shader-effects-1). Source: shader-effects-inc/shaders (MIT, THIRD_PARTY_NOTICES "shader-effects-inc shaders"), packages/core/src/shaders, one folder per effect, each effect's role, category and props read from its index.ts; the port follows each effect's emitted WGSL (the package's shader snapshots).

Total effects: 200. Roles: filter 65, generator 51, shapeEffect 23, simulation 19, warp 18, shape 15, media 5, structural 2, overlay 2.
Status: ported 6, wallpaper-capable generators still to port 45, not standalone 149 (each with its reason).

| Effect | Role | Category | Props | Status | Why not (yet) a wallpaper |
|---|---|---|---|---|---|
| AngularBlur | filter | Blurs | 8 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Arc | shape | ? | 0 | NOT STANDALONE | a single shape (circle, star, ...): a building block for composition, not a wallpaper on its own |
| Ascii | filter | Stylize | 7 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Aurora | generator | Textures | 13 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| BarShift | warp | Distortions | 5 | NOT STANDALONE | distorts an input picture: composition over the current wallpaper, not standalone |
| BarnDoors | filter | Transitions | 4 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Beam | generator | Textures | 9 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| Bend | warp | Distortions | 3 | NOT STANDALONE | distorts an input picture: composition over the current wallpaper, not standalone |
| Blob | generator | Textures | 15 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| BlockDissolve | filter | Transitions | 4 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| BlockNoise | generator | Textures | 1 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| BlueNoise | generator | Textures | 1 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| Blur | filter | Blurs | 1 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Boids | simulation | Interactive | 15 | NOT STANDALONE | keeps state between frames (ping-pong buffers): needs a multi-pass state pipeline the one-pass wallpaper renderer does not have; reads the pointer |
| BokehBlur | filter | Blurs | 8 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| BrickPattern | generator | Textures | 13 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| BrightnessContrast | filter | Adjustments | 2 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| BrushedMetal | shapeEffect | Shape Effects | 17 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| Bulge | warp | Distortions | 3 | NOT STANDALONE | distorts an input picture: composition over the current wallpaper, not standalone |
| CRTScreen | filter | Stylize | 8 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| CarbonFiber | shapeEffect | Shape Effects | 18 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| Chalkboard | filter | Stylize | 7 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| ChannelBlur | filter | Blurs | 3 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| CheckerWipe | filter | Transitions | 4 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Checkerboard | generator | Textures | 5 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| Chevron | generator | Textures | 9 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| ChromaFlow | simulation | Interactive | 8 | NOT STANDALONE | keeps state between frames (ping-pong buffers): needs a multi-pass state pipeline the one-pass wallpaper renderer does not have; reads the pointer |
| ChromaticAberration | filter | Stylize | 5 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Chrome | shapeEffect | Shape Effects | 18 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| Circle | shape | ? | 0 | NOT STANDALONE | a single shape (circle, star, ...): a building block for composition, not a wallpaper on its own |
| ColorWheel | generator | Textures | 8 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| CompressionArtifacts | filter | Stylize | 1 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| ConcentricSpin | warp | Distortions | 6 | NOT STANDALONE | distorts an input picture: composition over the current wallpaper, not standalone |
| ConicGradient | generator | Textures | 6 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| ContourLines | filter | Stylize | 9 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| CornerPin | warp | Distortions | 5 | NOT STANDALONE | distorts an input picture: composition over the current wallpaper, not standalone |
| Crescent | shape | ? | 0 | NOT STANDALONE | a single shape (circle, star, ...): a building block for composition, not a wallpaper on its own |
| Cross | shape | ? | 0 | NOT STANDALONE | a single shape (circle, star, ...): a building block for composition, not a wallpaper on its own |
| Crystal | shapeEffect | Shape Effects | 16 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| CurlNoise | generator | Textures | 1 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| CursorRipples | filter | Interactive | 5 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper; reads the pointer |
| CursorTrail | simulation | Interactive | 7 | NOT STANDALONE | keeps state between frames (ping-pong buffers): needs a multi-pass state pipeline the one-pass wallpaper renderer does not have; reads the pointer |
| DataMosh | simulation | Stylize | 7 | NOT STANDALONE | keeps state between frames (ping-pong buffers): needs a multi-pass state pipeline the one-pass wallpaper renderer does not have |
| DiamondGradient | generator | Textures | 8 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| DiamondWipe | filter | Transitions | 4 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| DiffuseBlur | filter | Blurs | 3 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| DisplacementMap | filter | Distortions | 5 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Dither | filter | Stylize | 7 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| DotGrid | generator | Textures | 7 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| DropShadow | filter | Stylize | 6 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Duotone | filter | Adjustments | 4 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Ellipse | shape | ? | 0 | NOT STANDALONE | a single shape (circle, star, ...): a building block for composition, not a wallpaper on its own |
| Emboss | shapeEffect | Shape Effects | 11 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| Engraving | filter | Stylize | 9 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| ErosionNoise | generator | Textures | 1 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| Exposure | filter | Adjustments | 1 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| FallingLines | generator | Textures | 11 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
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
| FractalNoise | generator | Textures | 0 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| Frost | shapeEffect | Shape Effects | 22 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| GaborNoise | generator | Textures | 2 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| Glass | shapeEffect | Shape Effects | 14 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| GlassTiles | filter | Distortions | 4 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Glitch | filter | Stylize | 7 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Glow | filter | Stylize | 3 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Godrays | generator | Textures | 7 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| Goo | shapeEffect | Shape Effects | 26 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| GradientMap | filter | Stylize | 10 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Grayscale | filter | Adjustments | 0 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Grid | generator | Textures | 8 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| GridDistortion | warp | Interactive | 4 | NOT STANDALONE | distorts an input picture: composition over the current wallpaper, not standalone; reads the pointer |
| Group | structural | Utilities | 0 | NOT STANDALONE | a container (Group, KeyFrames): no picture of its own |
| HTMLInCanvas | media | Textures | 0 | NOT STANDALONE | needs an image, a video, a webcam or text as input: not a wallpaper |
| Halftone | filter | Stylize | 14 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Heart | shape | ? | 0 | NOT STANDALONE | a single shape (circle, star, ...): a building block for composition, not a wallpaper on its own |
| Heatmap | shapeEffect | Shape Effects | 15 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| HexGrid | generator | Textures | 8 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| Hologram | shapeEffect | Shape Effects | 22 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| Holographic | shapeEffect | Shape Effects | 16 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| HueShift | filter | Adjustments | 1 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| ImageTexture | media | Textures | 1 | NOT STANDALONE | needs an image, a video, a webcam or text as input: not a wallpaper |
| InkFlow | simulation | Interactive | 12 | NOT STANDALONE | keeps state between frames (ping-pong buffers): needs a multi-pass state pipeline the one-pass wallpaper renderer does not have; reads the pointer |
| Invert | filter | Adjustments | 0 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| IrisWipe | filter | Transitions | 4 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Irradiance | shapeEffect | Shape Effects | 17 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| IsometricCubes | generator | Textures | 9 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| Kaleidoscope | warp | Distortions | 2 | NOT STANDALONE | distorts an input picture: composition over the current wallpaper, not standalone |
| KeyFrames | overlay | Stylize | 12 | NOT STANDALONE | draws over another layer: composition |
| LensDistortion | filter | Stylize | 19 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| LensFlare | generator | Stylize | 17 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| LightEdge | shapeEffect | Shape Effects | 21 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| LightLeak | filter | Stylize | 10 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Line | generator | Shapes | 9 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| LinearBlur | filter | Blurs | 8 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| LinearGradient | generator | Textures | 7 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| LinearWipe | filter | Transitions | 4 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| LiquidMetal | shapeEffect | Shape Effects | 16 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| Liquify | warp | Interactive | 4 | NOT STANDALONE | distorts an input picture: composition over the current wallpaper, not standalone; reads the pointer |
| MagneticFilings | simulation | Interactive | 11 | NOT STANDALONE | keeps state between frames (ping-pong buffers): needs a multi-pass state pipeline the one-pass wallpaper renderer does not have; reads the pointer |
| Marble | generator | Textures | 8 | PORTED (sei-marble) |  |
| MeshGradient | generator | Textures | 11 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| Mirror | warp | Distortions | 1 | NOT STANDALONE | distorts an input picture: composition over the current wallpaper, not standalone |
| MultiPointGradient | generator | Textures | 12 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
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
| PerlinNoise | generator | Textures | 1 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| Perspective | warp | Distortions | 5 | NOT STANDALONE | distorts an input picture: composition over the current wallpaper, not standalone |
| PixelSort | simulation | Interactive | 6 | NOT STANDALONE | keeps state between frames (ping-pong buffers): needs a multi-pass state pipeline the one-pass wallpaper renderer does not have; reads the pointer |
| PixelThrow | simulation | Interactive | 7 | NOT STANDALONE | keeps state between frames (ping-pong buffers): needs a multi-pass state pipeline the one-pass wallpaper renderer does not have; reads the pointer |
| Pixelate | filter | Stylize | 3 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Plasma | generator | Textures | 9 | PORTED (plasma (live tier, ED-NEW-ocean-wallpaper-1)) |  |
| Plastic | shapeEffect | Shape Effects | 24 | NOT STANDALONE | acts on a shape layer: composition, not standalone |
| PolarCoordinates | warp | Distortions | 3 | NOT STANDALONE | distorts an input picture: composition over the current wallpaper, not standalone |
| Polygon | shape | ? | 0 | NOT STANDALONE | a single shape (circle, star, ...): a building block for composition, not a wallpaper on its own |
| Posterize | filter | Adjustments | 1 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Prism | generator | Textures | 11 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| ProgressiveBlur | filter | Blurs | 6 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| RadialGradient | generator | Textures | 8 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| RadialWipe | filter | Transitions | 6 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| RandomBars | filter | Transitions | 5 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| ReactionDiffusion | simulation | Interactive | 20 | NOT STANDALONE | keeps state between frames (ping-pong buffers): needs a multi-pass state pipeline the one-pass wallpaper renderer does not have; reads the pointer |
| RectangularCoordinates | warp | Distortions | 2 | NOT STANDALONE | distorts an input picture: composition over the current wallpaper, not standalone |
| ReflectivePlane | filter | Stylize | 6 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Repeater | structural | Distortions | 29 | NOT STANDALONE | a container (Group, KeyFrames): no picture of its own |
| Ring | shape | ? | 0 | NOT STANDALONE | a single shape (circle, star, ...): a building block for composition, not a wallpaper on its own |
| RippleWipe | filter | Transitions | 5 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Ripples | generator | Textures | 8 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| RoundedRect | shape | ? | 0 | NOT STANDALONE | a single shape (circle, star, ...): a building block for composition, not a wallpaper on its own |
| Satin | generator | Textures | 18 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| Saturation | filter | Adjustments | 1 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Scratches | generator | Textures | 3 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| Sharpness | filter | Adjustments | 1 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Shatter | simulation | Interactive | 9 | NOT STANDALONE | keeps state between frames (ping-pong buffers): needs a multi-pass state pipeline the one-pass wallpaper renderer does not have; reads the pointer |
| SimplexNoise | generator | Textures | 5 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| SineWave | generator | Textures | 8 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| SliceWipe | warp | Transitions | 3 | NOT STANDALONE | distorts an input picture: composition over the current wallpaper, not standalone |
| Smoke | simulation | Interactive | 15 | NOT STANDALONE | keeps state between frames (ping-pong buffers): needs a multi-pass state pipeline the one-pass wallpaper renderer does not have; reads the pointer |
| SmokeFill | simulation | Shape Effects | 22 | NOT STANDALONE | keeps state between frames (ping-pong buffers): needs a multi-pass state pipeline the one-pass wallpaper renderer does not have |
| SmokeFlow | simulation | Interactive | 10 | NOT STANDALONE | keeps state between frames (ping-pong buffers): needs a multi-pass state pipeline the one-pass wallpaper renderer does not have; reads the pointer |
| Solarize | filter | Adjustments | 2 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| SolidColor | generator | Textures | 1 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| Sparkle | filter | Stylize | 8 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Spherize | filter | Distortions | 7 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Spiral | generator | Textures | 9 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| Star | shape | ? | 0 | NOT STANDALONE | a single shape (circle, star, ...): a building block for composition, not a wallpaper on its own |
| Stone | filter | Stylize | 0 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Strands | generator | Textures | 14 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| Stretch | warp | Distortions | 3 | NOT STANDALONE | distorts an input picture: composition over the current wallpaper, not standalone |
| Stripes | generator | Textures | 9 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| StudioBackground | generator | Textures | 19 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
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
| TriangularGrid | generator | Textures | 10 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| Tritone | filter | Adjustments | 5 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| Truchet | generator | Textures | 8 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
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
| Waveform | generator | Textures | 18 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| WaveletNoise | generator | Textures | 2 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| Weave | generator | Textures | 5 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| WebcamTexture | media | Textures | 1 | NOT STANDALONE | needs an image, a video, a webcam or text as input: not a wallpaper |
| Wool | filter | Stylize | 0 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |
| WorleyNoise | generator | Textures | 14 | PENDING (wallpaper-capable) | standalone generator: wallpaper-capable, port pending |
| ZoomBlur | filter | Blurs | 9 | NOT STANDALONE | needs an input picture: offered later as an effect over the current wallpaper (composition), not a standalone wallpaper |

# Implementation verification

Requirement tracker for the overnight brief of 2026-10-15 (cousin/glass-alive). Statuses: NOT STARTED, IN PROGRESS, IMPLEMENTED, TESTED, VISUALLY VERIFIED, BLOCKED. "Windows" is filled only from Claude's Windows runs.

| ID | Requirement | Source files | Status | Tests | Visual | Windows | Limits | Commit |
|----|-------------|--------------|--------|-------|--------|---------|--------|--------|
| 0.1 | ComposerCentreTests CS0118 | tests/Marrow.App.Tests/Views/Chat/ComposerCentreTests.cs | IMPLEMENTED | App.Tests do not build on Linux here (csc out of memory) | n/a | pending | | e830667c |
| 0.2 | CRLF restored (ED, Marrow.UI.csproj) | porting/engineering_decisions.md, src/Marrow.UI/Marrow.UI.csproj | TESTED | byte prefix check against 50607fca / 466ea0a3~1 | n/a | pending | | 083194f5, fb59e82c |
| 0.3 | Live wallpapers pass the loop proof | src/Marrow.UI/Theme/Wallpapers/live-wallpapers.json | TESTED | WallpaperLoopProofTests (all ids) | n/a | pending | | 29db9f6b |
| 0.4 | Catalogue 72 exact + live set exact | tests/Marrow.UI.Tests/Theme/Wallpapers/WallpaperLibraryTests.cs | TESTED | Catalogue_has_162..., Live_wallpapers_are_exactly_nths_own | n/a | pending | | 29db9f6b |
| 2.1 | Engines visibly different | src/Marrow.UI/Port/GlassMaterial.cs | TESTED | GlassLayersTests.The_three_engines_draw_different_glass_for_optical_reasons | headless frames only | pending | | (this commit) |
| 2.2 | Layers 1/2/3 compound | SwiftGlass.LayerOf, GlassCanvas.Depth/NestedGain/NestedMagnify | TESTED | GlassLayersTests.Each_layer_magnifies... | headless frames only | pending | | (this commit) |
| 2.4 | Desktop refraction | n/a | BLOCKED | n/a | n/a | n/a | No desktop pixels without screen capture (ED-NEW-glass-layers-1) | |
| 2.5a | Glass intensity slider | GlassSettings.Intensity, GlassSections.Intensity | TESTED | Glass_intensity_scales..., Intensity_is_stored_held_and_glides | headless frames only | pending | | (this commit) |
| 2.5b | See-through slider | GlassSections.SeeThrough, GlassSeeThrough | IMPLEMENTED | no App test run here | not seen | pending | same setting as Transparency | (this commit) |
| 4.3 | Settings not dark | Chrome.WindowBackdrop.SheetScrim | IMPLEMENTED | no rendered Settings test yet | not seen | pending | | (this commit) |
| 3.1 | Composer under the heading, no overlap; bigger rounded rectangle | ComposerCentre.cs, NewThreadPrompt.cs, ComposerView.cs (ComposerSurface.Idle) | IMPLEMENTED | ComposerCentreTests (App.Tests; not compiled here) | not seen | pending | | (this commit) |
| 3.3 | Morph centre to bottom pill on one spring | ComposerCentre.cs | IMPLEMENTED | ComposerCentreTests.The_centred_rectangle_morphs... | not seen | pending | glass thickness not separately animated | (this commit) |
| 3.4 | Bottom bar too high | ComposerArea.BottomGap 10 | IMPLEMENTED | none | not seen | pending | value chosen without a measured screenshot | (this commit) |
| 4.1-4.2 | Settings nav jelly pill: drag, stretch, land, open; click; keys; Reduce Motion | src/Marrow.UI/Port/Settings/CzControls/JellyNav.cs, SettingsView.cs, Chrome.SidebarRow | TESTED | JellyNavTests (5) | headless only | pending | | (this commit) |
| 5.1 | Shared liquid glass slider thumb | GlassThumb.cs, CzSliderRail.cs | TESTED (CzSlider) | GlassThumbTests | headless only | pending | BackdropOpacitySlider, EffortSlider, NthUi sliders not migrated | (this commit) |
| 5.2 | Switch glass thumb, Nocta kept | SettingsView.cs SmallSwitch.Thumb | IMPLEMENTED | none run here (App.Tests) | not seen | pending | | (this commit) |
| 6 | Sidebar in the main screen's shade | GlassLooks.cs (Veil, KindFrost), GlassMaterial.cs | TESTED | GlassSidebarShadeTests, The_sidebar_reads_over_a_busy_wallpaper | headless frames | pending | dark 15 luma under main (readability bound) | (this commit) |
| 7 | Gradient section, every shadergradient option live | ShaderGradientSettings.cs, ShaderGradientParams.cs, live-wallpapers.json (shader-gradient), GradientSections.cs, AppSettings.ShaderGradient | TESTED | ShaderGradientWallpaperTests (24), loop proof | headless PNGs (plane, sphere, water, env) | pending | ideas/params only (no licence); water plane edge aliasing at grazing angles | (this commit) |

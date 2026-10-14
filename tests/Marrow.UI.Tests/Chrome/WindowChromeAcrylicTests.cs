using Avalonia.Controls;
using Avalonia.Media;
using Marrow.Core.Port.Settings;
using Marrow.UI.Chrome;
using Marrow.UI.Port;
using Marrow.UI.Tests.Fixtures;

namespace Marrow.UI.Tests.Chrome;

// ED-236 (OD-0005, OD-0014; closes WindowManager STOP #5). WindowChrome.swift:116-117 makes the window clear
// (`isOpaque = false`, `backgroundColor = .clear`) and the content draws glass over it. On Windows the window itself
// composites Acrylic where it shows the desktop, with the app-drawn scrim over it (probe P3, porting/probes/p3/RESULT.md:
// "keep TransparencyLevelHint = AcrylicBlur as the window edge material, and draw the tint in the app"). The hint list
// is Avalonia's own fallback chain: the first level the platform supports is taken, so Transparent behind Acrylic gives
// the plain clear window where Acrylic cannot be had (a list of Acrylic alone ends as an opaque black window, P3's
// `ctl_none_opaque`). The native evidence is GlassWindowNativeTests.
[Collection("Headless")]
public sealed class WindowChromeAcrylicTests(HeadlessSession headless)
{
    [Fact]
    [Trait("Decision", "ED-236")]
    [Trait("Req", "PR-WIN-024")] // DroppyCode/UI/Chrome/WindowChrome.swift:116-117 through WindowManager.swift:101
    [Trait("Decision", "ED-NEW-glass-alive-2")] // the Acrylic chain is now the Glass setting's Frosted desktop, asked for exactly as before
    public void Configure_asks_for_the_Acrylic_backdrop_and_keeps_the_clear_window_as_the_fallback() => headless.OnUiThread(() =>
    {
        var before = GlassLooks.Tuning;
        GlassLooks.Tuning = before with { Desktop = DesktopBackdrop.Frosted };
        try
        {
            var window = new Window();
            WindowChrome.Configure(window);
            Assert.Equal([WindowTransparencyLevel.AcrylicBlur, WindowTransparencyLevel.Transparent], window.TransparencyLevelHint);
            Assert.Same(Brushes.Transparent, window.Background); // the clear window is still the base the scrim lies on
        }
        finally { GlassLooks.Tuning = before; }
    });

    [Fact]
    [Trait("Decision", "ED-NEW-glass-alive-2")] // rows #87, #32: the default desktop is clear (DWM's Transparent level alone, never an opaque window), and the setting moves a configured window both ways
    public void The_default_desktop_is_clear_and_the_setting_switches_a_configured_window() => headless.OnUiThread(() =>
    {
        var before = GlassLooks.Tuning;
        try
        {
            GlassLooks.Tuning = before with { Desktop = DesktopBackdrop.Clear };
            Assert.Equal([WindowTransparencyLevel.Transparent], WindowChrome.ClearBackdrop);
            Assert.Same(WindowChrome.ClearBackdrop, WindowChrome.BackdropFor(DesktopBackdrop.Clear));
            Assert.Same(WindowChrome.GlassBackdrop, WindowChrome.BackdropFor(DesktopBackdrop.Frosted));
            Assert.Equal(DesktopBackdrop.Clear, GlassSettings.Default.Desktop);
            var window = new Window();
            WindowChrome.Configure(window);
            Assert.Equal([WindowTransparencyLevel.Transparent], window.TransparencyLevelHint);
            Assert.Same(Brushes.Transparent, window.Background);
            GlassLooks.Tuning = before with { Desktop = DesktopBackdrop.Frosted };
            Assert.Equal([WindowTransparencyLevel.AcrylicBlur, WindowTransparencyLevel.Transparent], window.TransparencyLevelHint);
            GlassLooks.Tuning = before with { Desktop = DesktopBackdrop.Clear };
            Assert.Equal([WindowTransparencyLevel.Transparent], window.TransparencyLevelHint);
            var unconfigured = new Window();
            var untouched = unconfigured.TransparencyLevelHint;
            GlassLooks.Tuning = before with { Desktop = DesktopBackdrop.Frosted };
            Assert.Same(untouched, unconfigured.TransparencyLevelHint); // only windows the chrome configured follow the setting
        }
        finally { GlassLooks.Tuning = before; }
    });

    [Fact]
    [Trait("Decision", "ED-236")]
    public void The_glass_backdrop_hint_is_Acrylic_first_then_the_clear_window()
    {
        Assert.Equal(WindowTransparencyLevel.AcrylicBlur, WindowChrome.GlassBackdrop[0]);
        Assert.Equal(WindowTransparencyLevel.Transparent, WindowChrome.GlassBackdrop[^1]);
        Assert.Equal(2, WindowChrome.GlassBackdrop.Count);
    }
}

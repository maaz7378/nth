// ED-NEW-sidebar-shade-1 (Maaz, 2026-10-15: the sidebar is too light and pale beside the main screen): the sidebar's glass sits in the main
// screen's shade over the same wallpaper, in dark and light, while its words still read (its body stays calm over a busy wallpaper).
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Marrow.Core.Port.Settings;
using Marrow.UI.Port;
using Marrow.UI.Tests.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace Marrow.UI.Tests.Port;

[Collection("Headless")]
public sealed class GlassSidebarShadeTests(HeadlessSession headless, ITestOutputHelper output)
{
    private static readonly PixelRect P = GlassLiquidRender.Piece;
    private static readonly PixelRect Body = new(P.X + 60, P.Y + 50, P.Width - 120, P.Height - 100);

    private static double Mean(LiquidFrame f, PixelRect area)
    {
        double sum = 0;
        for (var y = area.Y; y < area.Bottom; y++) for (var x = area.X; x < area.Right; x++) sum += LiquidFrame.Luma(f.At(x, y));
        return sum / (area.Width * area.Height);
    }

    // the main screen over the wallpaper: the window's glass layer (WallpaperClear) and the window's tint, as WindowBackdrop draws them
    private static void MainScreen(Panel root, bool light)
    {
        var glass = new Border { Background = Brushes.Transparent };
        SwiftGlass.SetEffect(glass, GlassEffect.Regular(GlassShape.RoundedRectangle(0, RoundedCornerStyle.Continuous)).WithLook(GlassLook.WallpaperClear));
        var tint = new Border { Background = new SolidColorBrush(light ? Color.FromArgb(31, 246, 246, 248) : Color.FromArgb(31, 28, 28, 30)) };
        root.Children.Insert(1, glass);
        root.Children.Insert(2, tint);
    }

    [Theory]
    [Trait("Decision", "ED-NEW-sidebar-shade-1")]
    [InlineData(false)]
    [InlineData(true)]
    public void The_sidebar_sits_in_the_main_screens_shade(bool light)
    {
        var main = GlassLiquidRender.Render(headless, GlassSettings.Default, GlassSurface.Sidebar, light: light, decorate: root => MainScreen(root, light));
        var side = GlassLiquidRender.Render(headless, GlassSettings.Default, GlassSurface.Sidebar, light: light);
        // outside the piece: the main screen; inside: the sidebar, over the same stripes
        var mainShade = Mean(main, new PixelRect(10, 10, 70, 280));
        var sideShade = Mean(side, Body);
        output.WriteLine($"{(light ? "light" : "dark")}: main {mainShade:F1}, sidebar {sideShade:F1}");
        Assert.True(Math.Abs(sideShade - mainShade) < 18, $"sidebar {sideShade:F1} against the main screen {mainShade:F1}");
    }
}

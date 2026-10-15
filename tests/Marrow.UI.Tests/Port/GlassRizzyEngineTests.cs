// ED-NEW-glass-alive-5 (phase 1a): the glass drawn by rizzytoday/liquid-glass's engine. The piece sits over a horizontal grey ramp, so how far and
// which way the glass reads shows as how much darker or lighter it is than what lies directly under it: liquid-glass reads from past the left edge
// at the left rim and from past the right edge at the right rim (its red ramp and its negative scale), and hardly bends its neutral centre.
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Marrow.Core.Port.Settings;
using Marrow.UI.Port;
using Marrow.UI.Tests.Fixtures;
using Xunit;

namespace Marrow.UI.Tests.Port;

[Collection("Headless")]
public sealed class GlassRizzyEngineTests(HeadlessSession headless)
{
    private const int Width = 400, Height = 200, PieceLeft = 100, PieceTop = 40, PieceWidth = 200, PieceHeight = 120;

    // The ramp's level at x (0 at the window's left, 255 at its right).
    private static double Ramp(double x) => 255 * (x + 0.5) / Width;

    private Func<int, int, Color> Render(GlassSettings settings, bool light = false, Color? backdrop = null)
    {
        Func<int, int, Color>? result = null;
        headless.OnUiThread(() => result = RenderOnUi(settings, light, backdrop));
        return result!;
    }

    private static Func<int, int, Color> RenderOnUi(GlassSettings settings, bool light, Color? backdrop)
    {
        var (tuning, system, finish) = (GlassLooks.Tuning, GlassLooks.System, GlassCanvas.Finish);
        GlassLooks.System = new FixedGlassSystem();
        GlassCanvas.Finish = false; // ED-NEW-glass-alive-8: the repository's model alone, without nth's readability veil and rim light
        GlassLooks.Tuning = settings;
        var piece = new Border
        {
            Background = Brushes.Transparent, Width = PieceWidth, Height = PieceHeight, CornerRadius = new CornerRadius(24),
            HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness(PieceLeft, PieceTop, 0, 0),
        };
        SwiftGlass.SetEffect(piece, GlassEffect.Regular(GlassShape.RoundedRectangle(24, RoundedCornerStyle.Continuous)).OnSurface(GlassSurface.Panel));
        IBrush ramp = backdrop is { } solid ? new SolidColorBrush(solid) : new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative), EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
            GradientStops = { new GradientStop(Color.FromRgb(0, 0, 0), 0), new GradientStop(Color.FromRgb(255, 255, 255), 1) },
        };
        var window = new Window
        {
            Width = Width, Height = Height, Background = Brushes.Black, RequestedThemeVariant = light ? ThemeVariant.Light : ThemeVariant.Dark,
            Content = new Panel { Children = { new Panel { Background = ramp }, piece } },
        };
        window.Show();
        try
        {
            for (var i = 0; i < 4; i++) GlassScene.Drain();
            using var bitmap = window.CaptureRenderedFrame()!;
            var size = bitmap.PixelSize;
            var bytes = new byte[size.Width * size.Height * 4];
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try { bitmap.CopyPixels(new PixelRect(size), handle.AddrOfPinnedObject(), bytes.Length, size.Width * 4); }
            finally { handle.Free(); }
            var rgba = bitmap.Format == Avalonia.Platform.PixelFormat.Rgba8888;
            return (x, y) =>
            {
                var p = (y * size.Width + x) * 4;
                return Color.FromArgb(bytes[p + 3], bytes[p + (rgba ? 0 : 2)], bytes[p + 1], bytes[p + (rgba ? 2 : 0)]);
            };
        }
        finally
        {
            window.Close();
            (GlassLooks.Tuning, GlassLooks.System) = (tuning, system);
            GlassCanvas.Finish = finish;
        }
    }

    private static double Luma(Color c) => 0.2126 * c.R + 0.7152 * c.G + 0.0722 * c.B;

    private const int Middle = PieceTop + PieceHeight / 2;

    [Fact]
    [Trait("Decision", "ED-NEW-glass-alive-5")]
    public void The_rims_bend_what_lies_past_the_edges_into_the_glass_and_the_centre_stays_put()
    {
        var clear = GlassSettings.Default with { Engine = GlassEngine.Rizzy, Rizzy = RizzyGlass.Repo };
        var pixels = Render(clear);
        // the left rim shows what lies to its left (darker on the ramp), the right rim what lies to its right (lighter)
        Assert.True(Luma(pixels(PieceLeft + 4, Middle)) < Ramp(PieceLeft + 4) - 25, $"left rim {Luma(pixels(PieceLeft + 4, Middle))} vs under {Ramp(PieceLeft + 4)}");
        Assert.True(Luma(pixels(PieceLeft + PieceWidth - 5, Middle)) > Ramp(PieceLeft + PieceWidth - 5) + 25,
            $"right rim {Luma(pixels(PieceLeft + PieceWidth - 5, Middle))} vs under {Ramp(PieceLeft + PieceWidth - 5)}");
        // the neutral centre (alpha 0.93: 7 % of the ramp's bend) is what lies under it, within a few levels
        Assert.InRange(Luma(pixels(PieceLeft + PieceWidth / 2, Middle)), Ramp(PieceLeft + PieceWidth / 2) - 4, Ramp(PieceLeft + PieceWidth / 2) + 4);
        // outside the piece nothing changes
        Assert.InRange(Luma(pixels(PieceLeft - 6, Middle)), Ramp(PieceLeft - 6) - 2, Ramp(PieceLeft - 6) + 2);
    }

    [Fact]
    [Trait("Decision", "ED-NEW-glass-alive-5")]
    public void Each_channel_bends_by_its_own_scale()
    {
        // blue bends 140 on 180 of red: at the left rim on the rising ramp it reads from nearer, so it is lighter than red
        var pixels = Render(GlassSettings.Default with { Engine = GlassEngine.Rizzy, Rizzy = RizzyGlass.Repo.With("aberrationB", 40) });
        var rim = pixels(PieceLeft + 4, Middle);
        Assert.True(rim.B >= rim.R + 4, $"rim {rim}");
        var centre = pixels(PieceLeft + PieceWidth / 2, Middle);
        Assert.InRange(centre.B - centre.R, -3, 3);
    }

    [Fact]
    [Trait("Decision", "ED-NEW-glass-alive-5")]
    public void Scale_drives_the_bend_and_the_canvas_engine_does_not_draw_it()
    {
        double Rim(GlassSettings s) => Ramp(PieceLeft + 4) - Luma(Render(s)(PieceLeft + 4, Middle));
        var soft = Rim(GlassSettings.Default with { Engine = GlassEngine.Rizzy, Rizzy = RizzyGlass.Repo.With("scale", -60) });
        var hard = Rim(GlassSettings.Default with { Engine = GlassEngine.Rizzy, Rizzy = RizzyGlass.Repo.With("scale", -240) });
        Assert.True(hard > soft + 15, $"scale -240 {hard} vs -60 {soft}");
        var canvas = Rim(ClassicGlass.Default);
        Assert.True(hard > canvas + 15, $"liquid-glass {hard} vs canvas {canvas}");
    }

    [Fact]
    [Trait("Decision", "ED-NEW-glass-alive-5")]
    public void Frost_darkens_in_the_dark_and_lightens_in_the_light_so_text_stays_readable()
    {
        var grey = Color.FromRgb(128, 128, 128);
        var frosted = GlassSettings.Default with { Engine = GlassEngine.Rizzy, Rizzy = RizzyGlass.Repo.With("frost", 0.4) };
        var dark = Luma(Render(frosted, light: false, grey)(PieceLeft + PieceWidth / 2, Middle));
        var light = Luma(Render(frosted, light: true, grey)(PieceLeft + PieceWidth / 2, Middle));
        Assert.InRange(dark, 128 * 0.6 - 3, 128 * 0.6 + 3);
        Assert.InRange(light, 128 * 0.6 + 255 * 0.4 - 3, 128 * 0.6 + 255 * 0.4 + 3);
    }

    [Fact]
    [Trait("Decision", "ED-NEW-glass-alive-5")]
    public void Saturation_is_the_filter_effects_saturate_matrix()
    {
        var red = Color.FromRgb(200, 60, 60);
        var c = Render(GlassSettings.Default with { Engine = GlassEngine.Rizzy, Rizzy = RizzyGlass.Repo.With("saturation", 0) }, backdrop: red)(PieceLeft + PieceWidth / 2, Middle);
        var expected = 0.213 * 200 + 0.715 * 60 + 0.072 * 60;
        Assert.InRange(c.R, expected - 3, expected + 3);
        Assert.InRange(c.G, expected - 3, expected + 3);
        Assert.InRange(c.B, expected - 3, expected + 3);
    }

    [Fact]
    [Trait("Decision", "ED-NEW-glass-alive-5")]
    public void Reduce_transparency_draws_the_solid_canvas_surface_instead()
    {
        var reduced = GlassSettings.Default with { Engine = GlassEngine.Rizzy, Rizzy = RizzyGlass.Repo, ReduceTransparency = true };
        var canvas = reduced with { Engine = GlassEngine.Canvas };
        Assert.Equal(Render(canvas)(PieceLeft + 4, Middle), Render(reduced)(PieceLeft + 4, Middle));
    }
}

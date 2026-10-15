// ED-NEW-glass-alive-8: renders liquid glass pieces over a striped, coloured wallpaper, as a fresh profile draws them, for the liquid engines' pixel
// tests: a panel (and, optionally, a second piece on it: the nested lens) on a backdrop of vertical stripes, captured as one frame.
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Marrow.Core.Port.Settings;
using Marrow.UI.Port;
using Marrow.UI.Tests.Fixtures;

namespace Marrow.UI.Tests.Port;

internal sealed record LiquidFrame(int Width, int Height, byte[] Bytes, bool Rgba)
{
    public Color At(int x, int y)
    {
        var p = (y * Width + x) * 4;
        return Color.FromArgb(Bytes[p + 3], Bytes[p + (Rgba ? 0 : 2)], Bytes[p + 1], Bytes[p + (Rgba ? 2 : 0)]);
    }

    public static double Luma(Color c) => 0.2126 * c.R + 0.7152 * c.G + 0.0722 * c.B;

    /// <summary>The mean absolute difference of two frames over a rectangle, per channel (0 to 255).</summary>
    public double Difference(LiquidFrame other, PixelRect area)
    {
        double sum = 0;
        var n = 0;
        for (var y = area.Y; y < area.Bottom; y++)
            for (var x = area.X; x < area.Right; x++)
            {
                var (a, b) = (At(x, y), other.At(x, y));
                sum += Math.Abs(a.R - b.R) + Math.Abs(a.G - b.G) + Math.Abs(a.B - b.B);
                n += 3;
            }
        return n == 0 ? 0 : sum / n;
    }
}

internal static class GlassLiquidRender
{
    public const int Width = 480, Height = 300;
    public static readonly PixelRect Piece = new(90, 60, 300, 180);
    public static readonly PixelRect Inner = new(150, 110, 180, 60);
    public const int StripeWidth = 12;

    /// <summary>The wallpaper's stripes: alternating dark blue and warm white, 12 px wide, so a bend shows as stripes that move.</summary>
    public static Color Stripe(int x) => (x / StripeWidth) % 2 == 0 ? Color.FromRgb(20, 40, 120) : Color.FromRgb(240, 220, 190);

    public static LiquidFrame Render(HeadlessSession headless, GlassSettings settings, GlassSurface surface = GlassSurface.Panel, bool nested = false,
        bool light = false, bool stripes = true, bool finish = true, Action<Panel>? decorate = null, double corner = 28)
    {
        LiquidFrame? frame = null;
        headless.OnUiThread(() => frame = OnUi(settings, surface, nested, light, stripes, finish, decorate, corner));
        return frame!;
    }

    private static LiquidFrame OnUi(GlassSettings settings, GlassSurface surface, bool nested, bool light, bool stripes, bool finish, Action<Panel>? decorate, double corner)
    {
        var (tuning, system, shared) = (GlassLooks.Tuning, GlassLooks.System, GlassCanvas.Finish);
        GlassLooks.System = new FixedGlassSystem();
        GlassLooks.Tuning = settings;
        GlassCanvas.Finish = finish;
        var piece = new Border
        {
            Background = Brushes.Transparent, Width = Piece.Width, Height = Piece.Height, CornerRadius = new CornerRadius(corner),
            HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness(Piece.X, Piece.Y, 0, 0),
        };
        SwiftGlass.SetEffect(piece, GlassEffect.Regular(GlassShape.RoundedRectangle(corner, RoundedCornerStyle.Continuous)).OnSurface(surface));
        var root = new Panel();
        var wall = new Panel { Background = stripes ? StripeBrush() : new SolidColorBrush(Color.FromRgb(128, 128, 128)) };
        root.Children.Add(wall);
        root.Children.Add(piece);
        if (nested)
        {
            var inner = new Border
            {
                Background = Brushes.Transparent, Width = Inner.Width, Height = Inner.Height, CornerRadius = new CornerRadius(30),
                HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(Inner.X - Piece.X, Inner.Y - Piece.Y, 0, 0),
            };
            SwiftGlass.SetEffect(inner, GlassEffect.Regular(GlassShape.Capsule(RoundedCornerStyle.Continuous)).OnSurface(GlassSurface.Inner));
            piece.Child = new Panel { Children = { inner } };
        }
        decorate?.Invoke(root);
        var window = new Window
        {
            Width = Width, Height = Height, Background = Brushes.Black, RequestedThemeVariant = light ? ThemeVariant.Light : ThemeVariant.Dark,
            Content = root,
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
            var frame = new LiquidFrame(size.Width, size.Height, bytes, bitmap.Format == Avalonia.Platform.PixelFormat.Rgba8888);
            if (Environment.GetEnvironmentVariable("NTH_GLASS_DUMP") is { Length: > 0 } dir) Dump(frame, Path.Combine(dir, $"{settings.Drawn}-{surface}-{(nested ? "nested" : "flat")}-{(light ? "light" : "dark")}-{Guid.NewGuid():N}.png"));
            return frame;
        }
        finally
        {
            window.Close();
            (GlassLooks.Tuning, GlassLooks.System) = (tuning, system);
            GlassCanvas.Finish = shared;
        }
    }

    private static void Dump(LiquidFrame frame, string path)
    {
        using var image = new SkiaSharp.SKBitmap(frame.Width, frame.Height, SkiaSharp.SKColorType.Rgba8888, SkiaSharp.SKAlphaType.Unpremul);
        for (var y = 0; y < frame.Height; y++)
            for (var x = 0; x < frame.Width; x++)
            {
                var c = frame.At(x, y);
                image.SetPixel(x, y, new SkiaSharp.SKColor(c.R, c.G, c.B, 255));
            }
        using var data = image.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100);
        File.WriteAllBytes(path, data.ToArray());
    }

    private static IBrush StripeBrush()
    {
        var bitmap = new WriteableBitmap(new PixelSize(Width, 1), new Vector(96, 96), Avalonia.Platform.PixelFormat.Bgra8888, Avalonia.Platform.AlphaFormat.Premul);
        using (var fb = bitmap.Lock())
        {
            var row = new byte[Width * 4];
            for (var x = 0; x < Width; x++)
            {
                var c = Stripe(x);
                (row[x * 4], row[x * 4 + 1], row[x * 4 + 2], row[x * 4 + 3]) = (c.B, c.G, c.R, 255);
            }
            Marshal.Copy(row, 0, fb.Address, row.Length);
        }
        return new ImageBrush(bitmap) { Stretch = Stretch.Fill };
    }
}

// ED-NEW-ocean-wallpaper-1: nth's own live wallpapers draw, move with time, and answer both of their options.
using Marrow.UI.Theme.Wallpapers;
using SkiaSharp;
using Xunit;

namespace Marrow.UI.Tests.Theme.Wallpapers;

public sealed class LiveWallpapersTests
{
    public static TheoryData<string> Live() => new(WallpaperCatalogue.All.Where(w => w.Tier == "live").Select(w => w.Id));

    private static SKBitmap Frame(string id, double time, IReadOnlyDictionary<string, double>? options = null, bool dark = true)
    {
        var bitmap = new SKBitmap(240, 150);
        using var canvas = new SKCanvas(bitmap);
        using var renderer = new WallpaperRenderer(new WallpaperSelection(WallpaperKind.Animated, id, Options: options), dark);
        renderer.DrawFrameAt(canvas, 240, 150, time);
        canvas.Flush();
        if (Environment.GetEnvironmentVariable("NTH_GLASS_DUMP") is { Length: > 0 } dir)
        {
            using var data = bitmap.Encode(SKEncodedImageFormat.Png, 100);
            File.WriteAllBytes(Path.Combine(dir, $"wall-{id}-{time}-{(options is null ? "def" : string.Join("_", options.Values))}.png"), data.ToArray());
        }
        return bitmap;
    }

    private static double Difference(SKBitmap a, SKBitmap b)
    {
        double sum = 0;
        for (var y = 0; y < a.Height; y += 2)
            for (var x = 0; x < a.Width; x += 2)
            {
                var (p, q) = (a.GetPixel(x, y), b.GetPixel(x, y));
                sum += Math.Abs(p.Red - q.Red) + Math.Abs(p.Green - q.Green) + Math.Abs(p.Blue - q.Blue);
            }
        return sum / (a.Width / 2 * (a.Height / 2) * 3);
    }

    [Theory]
    [MemberData(nameof(Live))]
    [Trait("Decision", "ED-NEW-ocean-wallpaper-1")]
    public void It_draws_moves_and_answers_both_options(string id)
    {
        var wall = WallpaperCatalogue.Get(WallpaperKind.Animated, id);
        using var still = Frame(id, 0);
        double spread = 0;
        var first = still.GetPixel(10, 10);
        for (var x = 0; x < 240; x += 8) spread = Math.Max(spread, Math.Abs(still.GetPixel(x, 75).Red - first.Red) + Math.Abs(still.GetPixel(x, 75).Blue - first.Blue));
        Assert.True(spread > 10, $"{id}: flat frame");
        using var later = Frame(id, wall.Period / 4);
        Assert.True(Difference(still, later) > 0.5, $"{id}: does not move");
        foreach (var option in wall.Options)
        {
            using var low = Frame(id, wall.Period / 3, new Dictionary<string, double> { [option.Id] = option.Min });
            using var high = Frame(id, wall.Period / 3, new Dictionary<string, double> { [option.Id] = option.Max });
            Assert.True(Difference(low, high) > 0.3, $"{id}: {option.Id} changes nothing");
        }
    }
}

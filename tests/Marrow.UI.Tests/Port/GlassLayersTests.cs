// ED-NEW-glass-layers-1 (Maaz, 2026-10-15): every glass layer is a lens. The window's glass is layer 1, a panel on it layer 2, a control on that
// layer 3; each layer bends the wallpaper behind it more than the one under it (1.3 times the bend, 4.5 % more magnification of its centre). The
// three engines draw visibly different glass for reasons of their own optics, and the Glass intensity slider scales the bend. Each test measures
// the stripes of GlassLiquidRender's wallpaper through the glass: how far their edges moved (the bend, in px) and how much wider they look (the
// magnification), both read from where the luma crosses each row's own midpoint, so a tint, a veil or a frost cannot pass for a bend.
using Avalonia;
using Marrow.Core.Port.Settings;
using Marrow.UI.Port;
using Marrow.UI.Tests.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace Marrow.UI.Tests.Port;

[Collection("Headless")]
public sealed class GlassLayersTests(HeadlessSession headless, ITestOutputHelper output)
{
    private static readonly PixelRect P = GlassLiquidRender.Piece;
    // the band inside the left and right edges where every engine bends (its rim), and the piece's middle
    private static readonly PixelRect LeftBand = new(P.X + 3, P.Y + 50, 30, 80);
    private static readonly PixelRect RightBand = new(P.Right - 33, P.Y + 50, 30, 80);
    private static readonly PixelRect Core = new(P.X + 60, P.Y + 60, P.Width - 120, P.Height - 120);

    public static TheoryData<GlassEngine> Engines() => new() { GlassEngine.Rizzy, GlassEngine.Dasher, GlassEngine.GlassHq };

    private LiquidFrame Render(GlassSettings s, int layer) => GlassLiquidRender.Render(headless, s, GlassSurface.Panel, finish: false, layer: layer);

    private static double Bend(LiquidFrame f) => (GlassLiquidRender.EdgeShift(f, LeftBand) + GlassLiquidRender.EdgeShift(f, RightBand)) / 2;

    // Glass HQ's Regular material frosts and fills its body, so the stripes under it are too soft to find edges in: its lens is measured in
    // its Clear material (the same field and refraction, no frost).
    private static GlassSettings Of(GlassEngine engine, double intensity = 1) => GlassSettings.Default with
    {
        Engine = engine, Intensity = intensity,
        GlassHq = engine == GlassEngine.GlassHq ? GlassSettings.Default.GlassHq with { Material = GlassHqMaterial.Clear } : GlassSettings.Default.GlassHq,
    };

    private static readonly PixelRect Band = new(P.X + 3, P.Y + 60, 60, 60);
    private static readonly PixelRect Whole = new(P.X + 2, P.Y + 2, P.Width - 4, P.Height - 4);

    [Theory]
    [MemberData(nameof(Engines))]
    [Trait("Decision", "ED-NEW-glass-layers-1")]
    public void Each_layer_magnifies_the_wallpaper_more_than_the_one_under_it(GlassEngine engine)
    {
        var m = new[] { 1, 2, 3 }.Select(layer => GlassLiquidRender.Magnification(Render(Of(engine), layer), Core)).ToArray();
        output.WriteLine($"{engine}: magnification through layers 1, 2, 3: {m[0]:F4}, {m[1]:F4}, {m[2]:F4}");
        Assert.True(m[1] > m[0] + 0.03, $"{engine}: layer 2 {m[1]:F4} over layer 1 {m[0]:F4}");
        Assert.True(m[2] > m[1] + 0.03, $"{engine}: layer 3 {m[2]:F4} over layer 2 {m[1]:F4}");
    }

    [Fact]
    [Trait("Decision", "ED-NEW-glass-layers-1")]
    public void The_three_engines_draw_different_glass_for_optical_reasons()
    {
        var engines = new[] { GlassEngine.Rizzy, GlassEngine.Dasher, GlassEngine.GlassHq };
        var frames = engines.ToDictionary(e => e, e => Render(GlassSettings.Default with { Engine = e }, 2));
        // how the stripes look under the rim band (how far each engine's own profile squeezes or widens them): unchanged by any tint
        var rim = engines.ToDictionary(e => e, e => GlassLiquidRender.Magnification(Render(Of(e), 2), Band));
        foreach (var a in engines)
            foreach (var b in engines.Where(b => b > a))
            {
                var d = frames[a].Difference(frames[b], Whole);
                output.WriteLine($"{a} vs {b}: frames {d:F2}, rim magnification {rim[a]:F4} vs {rim[b]:F4}");
                Assert.True(d > 15, $"{a} vs {b}: the frames differ by only {d:F2}");
                Assert.True(Math.Abs(rim[a] - rim[b]) > 0.05, $"{a} vs {b}: the rims bend alike ({rim[a]:F4}, {rim[b]:F4})");
            }
    }

    [Theory]
    [MemberData(nameof(Engines))]
    [Trait("Decision", "ED-NEW-glass-layers-1")]
    public void Glass_intensity_scales_the_bend_from_flat_to_twice(GlassEngine engine)
    {
        var flat = Render(Of(engine, 0), 2);
        var one = Render(Of(engine, 1), 2);
        var two = Render(Of(engine, 2), 2);
        var bands = new[] { LeftBand, RightBand };
        var moved = bands.Average(b => one.Difference(flat, b));
        var (m0, m1, m2) = (GlassLiquidRender.Magnification(flat, Core), GlassLiquidRender.Magnification(one, Core), GlassLiquidRender.Magnification(two, Core));
        output.WriteLine($"{engine}: rim moved {moved:F2} from flat; magnification at 0, 1, 2: {m0:F4}, {m1:F4}, {m2:F4}");
        // at 0 the glass bends nothing: the stripes are where they lie
        Assert.InRange(m0, 0.995, 1.005);
        Assert.True(moved > 8, $"{engine}: intensity 1 moves the rim only {moved:F2} from flat");
        Assert.True(m2 > m1 + 0.02, $"{engine}: intensity 2 magnifies {m2:F4}, intensity 1 {m1:F4}");
    }

    [Fact]
    [Trait("Decision", "ED-NEW-glass-layers-1")]
    public void Intensity_is_stored_held_and_glides()
    {
        var s = GlassSettings.Default with { Intensity = 1.4 };
        Assert.Equal(1.4, GlassSettings.FromJson(s.ToJson()).Intensity, 6);
        Assert.Equal(2, (GlassSettings.Default with { Intensity = 9 }).Held().Intensity);
        Assert.Equal(1, GlassSettings.FromJson("{}"u8).Intensity);
        Assert.DoesNotContain("intensity", GlassSettings.Default.ToString(), StringComparison.Ordinal);
        Assert.Equal(1.2, GlassSettings.Default.Toward(s, 0.5).Intensity, 6);
    }
}

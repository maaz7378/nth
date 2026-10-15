// ED-NEW-glass-alive-8 (Maaz, 2026-10-15): the liquid glass must be visible on a fresh profile, on every glass surface, and every engine option must
// change what is drawn. Each pixel test renders a glass piece over a wallpaper of vertical stripes (GlassLiquidRender): a bend shows as stripes
// that move under the rim, a fringe as colours that are on neither stripe, a second lens as a piece on the panel that bends the bent stripes again.
using Avalonia;
using Avalonia.Media;
using Marrow.Core.Port.Settings;
using Marrow.UI.Port;
using Marrow.UI.Tests.Fixtures;
using Xunit;

namespace Marrow.UI.Tests.Port;

[Collection("Headless")]
public sealed class GlassLiquidEnginesTests(HeadlessSession headless)
{
    private static readonly PixelRect P = GlassLiquidRender.Piece;
    private static readonly PixelRect LeftRim = new(P.X + 2, P.Y + 50, 18, 80);
    private static readonly PixelRect Whole = new(P.X + 2, P.Y + 2, P.Width - 4, P.Height - 4);

    private LiquidFrame Render(GlassSettings s, GlassSurface surface = GlassSurface.Panel, bool nested = false, bool light = false, bool finish = true, double corner = 28) =>
        GlassLiquidRender.Render(headless, s, surface, nested, light, finish: finish, corner: corner);

    // How far the frame is from the bare stripes over an area (0 to 255 per channel).
    private static double FromStripes(LiquidFrame f, PixelRect area)
    {
        double sum = 0;
        var n = 0;
        for (var y = area.Y; y < area.Bottom; y++)
            for (var x = area.X; x < area.Right; x++)
            {
                var (a, b) = (f.At(x, y), GlassLiquidRender.Stripe(x));
                sum += Math.Abs(a.R - b.R) + Math.Abs(a.G - b.G) + Math.Abs(a.B - b.B);
                n += 3;
            }
        return sum / n;
    }

    // The most any pixel strays from the line between the two stripe colours (a colour on neither stripe nor between them: the fringe).
    private static double Fringe(LiquidFrame f, PixelRect area)
    {
        var (a, b) = (GlassLiquidRender.Stripe(0), GlassLiquidRender.Stripe(GlassLiquidRender.StripeWidth));
        double[] d = [b.R - a.R, b.G - a.G, b.B - a.B];
        var dd = d.Sum(v => v * v);
        double most = 0;
        for (var y = area.Y; y < area.Bottom; y++)
            for (var x = area.X; x < area.Right; x++)
            {
                var c = f.At(x, y);
                double[] v = [c.R - a.R, c.G - a.G, c.B - a.B];
                var t = Math.Clamp(v.Zip(d, (p, q) => p * q).Sum() / dd, 0, 1);
                most = Math.Max(most, Math.Sqrt(v.Select((p, i) => Math.Pow(p - t * d[i], 2)).Sum()));
            }
        return most;
    }

    private static double Spread(LiquidFrame f, PixelRect area)
    {
        var l = new List<double>();
        for (var y = area.Y; y < area.Bottom; y++)
            for (var x = area.X; x < area.Right; x++) l.Add(LiquidFrame.Luma(f.At(x, y)));
        var mean = l.Average();
        return Math.Sqrt(l.Average(v => (v - mean) * (v - mean)));
    }

    [Fact]
    [Trait("Decision", "ED-NEW-glass-alive-8")]
    public void A_fresh_profile_draws_every_glass_surface_with_liquid_glass() => headless.OnUiThread(() =>
    {
        var before = GlassLooks.Tuning;
        GlassLooks.Tuning = GlassSettings.Default;
        try
        {
            foreach (var surface in Enum.GetValues<GlassSurface>())
            {
                var canvas = GlassLooks.For(surface).Canvas;
                Assert.NotNull(canvas);
                Assert.True(canvas!.Liquid, $"{surface} is not liquid");
                Assert.Equal(GlassEngine.Rizzy, canvas.Engine);
            }
            GlassLooks.Tuning = GlassSettings.Default with { Liquid = false };
            foreach (var surface in Enum.GetValues<GlassSurface>()) Assert.False(GlassLooks.For(surface).Canvas!.Liquid, $"{surface} with Liquid glass off");
            foreach (var engine in new[] { GlassEngine.Dasher, GlassEngine.GlassHq })
            {
                GlassLooks.Tuning = GlassSettings.Default with { Engine = engine };
                foreach (var surface in Enum.GetValues<GlassSurface>()) Assert.Equal(engine, GlassLooks.For(surface).Canvas!.Engine);
            }
        }
        finally { GlassLooks.Tuning = before; }
    });

    [Theory]
    [Trait("Decision", "ED-NEW-glass-alive-8")]
    [InlineData(GlassEngine.Rizzy)]
    [InlineData(GlassEngine.Dasher)]
    [InlineData(GlassEngine.GlassHq)]
    public void The_rim_measurably_bends_the_wallpaper_and_outside_nothing_moves(GlassEngine engine)
    {
        var f = Render(GlassSettings.Default with { Engine = engine }, finish: false);
        var canvas = Render(ClassicGlass.Default, finish: false);
        // the stripes under the rim are not where they lie: they moved (the bend), not only dimmed
        Assert.True(FromStripes(f, LeftRim) > 35, $"{engine}: rim {FromStripes(f, LeftRim):F1} from the stripes");
        Assert.True(f.Difference(canvas, LeftRim) > 12, $"{engine}: rim {f.Difference(canvas, LeftRim):F1} from the canvas's");
        // past the piece (and its shadow) the wallpaper is untouched
        Assert.True(FromStripes(f, new PixelRect(10, 10, 60, 40)) < 1.5);
    }

    [Fact]
    [Trait("Decision", "ED-NEW-glass-alive-8")]
    public void The_default_glass_shows_a_colour_fringe_and_a_rim_light()
    {
        var bare = Render(GlassSettings.Default, finish: false);
        Assert.True(Fringe(bare, LeftRim) > 40, $"fringe {Fringe(bare, LeftRim):F1}");
        var lit = Render(GlassSettings.Default);
        // the lit (upper left) edge: the rim light's line
        var edge = new PixelRect(P.X + 30, P.Y, 120, 2);
        double Luma(LiquidFrame f) { double s = 0; for (var x = edge.X; x < edge.Right; x++) s += LiquidFrame.Luma(f.At(x, edge.Y)) + LiquidFrame.Luma(f.At(x, edge.Y + 1)); return s; }
        Assert.True(Luma(lit) > Luma(Render(GlassSettings.Default with { Rim = 0.1 })) + 10 * edge.Width * 2 / 10.0, "rim light");
    }

    [Theory]
    [Trait("Decision", "ED-NEW-glass-alive-8")]
    [InlineData(GlassEngine.Rizzy)]
    [InlineData(GlassEngine.Dasher)]
    [InlineData(GlassEngine.GlassHq)]
    public void A_piece_on_glass_is_a_second_lens_that_bends_the_bent_wallpaper_again(GlassEngine engine)
    {
        var s = GlassSettings.Default with { Engine = engine };
        var alone = Render(s);
        var nested = Render(s, nested: true);
        var inner = GlassLiquidRender.Inner;
        var core = new PixelRect(inner.X + 2, inner.Y + 4, inner.Width - 4, inner.Height - 8);
        // Glass HQ's Regular panel is frosted and filled, so what its nested lens bends is smooth (and it keeps the bend within its inset)
        var least = engine == GlassEngine.GlassHq ? 2.5 : 6;
        Assert.True(nested.Difference(alone, core) > least, $"{engine}: the second lens changes {nested.Difference(alone, core):F1}");
        // and the panel around it is the panel
        Assert.True(nested.Difference(alone, new PixelRect(P.X + 10, P.Y + 10, 30, 30)) < 1.5);
    }

    [Fact]
    [Trait("Decision", "ED-NEW-glass-alive-8")]
    public void The_sidebar_reads_over_a_busy_wallpaper_in_dark_and_light()
    {
        var body = new PixelRect(P.X + 60, P.Y + 50, P.Width - 120, P.Height - 100);
        var stripes = Spread(Render(ClassicGlass.Default with { Liquid = false }, GlassSurface.Sidebar, finish: false), new PixelRect(10, 10, 100, 40));
        foreach (var light in new[] { false, true })
        {
            var f = Render(GlassSettings.Default, GlassSurface.Sidebar, light: light);
            Assert.True(Spread(f, body) < 22, $"light {light}: body spread {Spread(f, body):F1} (stripes {stripes:F1})");
            var mean = 0.0;
            for (var y = body.Y; y < body.Bottom; y++) for (var x = body.X; x < body.Right; x++) mean += LiquidFrame.Luma(f.At(x, y));
            mean /= body.Width * body.Height;
            if (light) Assert.True(mean > 150, $"light body {mean:F0}"); else Assert.True(mean < 110, $"dark body {mean:F0}");
        }
    }

    [Fact]
    [Trait("Decision", "ED-NEW-glass-alive-8")]
    public void Liquid_glass_off_draws_the_classic_canvas_material()
    {
        var off = Render(GlassSettings.Default with { Liquid = false });
        var canvas = Render(ClassicGlass.Default);
        Assert.True(off.Difference(canvas, Whole) < 0.01);
    }

    // Every option of every engine changes the frame (no dead option): each option at its low end and at its high end over the stripes.
    public static TheoryData<GlassEngine, string> Options()
    {
        var data = new TheoryData<GlassEngine, string>();
        foreach (var k in RizzyGlass.Knobs) data.Add(GlassEngine.Rizzy, k.Id);
        foreach (var k in DasherGlass.Knobs) data.Add(GlassEngine.Dasher, k.Id);
        data.Add(GlassEngine.Dasher, "warp");
        foreach (var k in GlassHqGlass.Knobs) data.Add(GlassEngine.GlassHq, k.Id);
        foreach (var id in new[] { "material", "appearance", "tint", "concentric" }) data.Add(GlassEngine.GlassHq, id);
        return data;
    }

    [Theory]
    [Trait("Decision", "ED-NEW-glass-alive-8")]
    [MemberData(nameof(Options))]
    public void Every_engine_option_changes_the_rendered_glass(GlassEngine engine, string id)
    {
        var s = GlassSettings.Default with { Engine = engine };
        GlassSettings Low, High;
        var corner = id is "borderRadius" or "radius" ? 0 : 28;
        var nested = id is "inset" or "concentric";
        var surface = GlassSurface.Panel;
        switch (engine, id)
        {
            case (GlassEngine.Dasher, "warp"): (Low, High) = (s with { Dasher = s.Dasher with { Warp = false } }, s with { Dasher = s.Dasher with { Warp = true, BaseIntensity = 0.05 } }); break;
            case (GlassEngine.GlassHq, "material"): (Low, High) = (s with { GlassHq = s.GlassHq with { Material = GlassHqMaterial.Regular } }, s with { GlassHq = s.GlassHq with { Material = GlassHqMaterial.Clear } }); break;
            case (GlassEngine.GlassHq, "appearance"): (Low, High) = (s with { GlassHq = s.GlassHq with { Appearance = GlassHqAppearance.Light } }, s with { GlassHq = s.GlassHq with { Appearance = GlassHqAppearance.Dark } }); break;
            case (GlassEngine.GlassHq, "tint"): (Low, High) = (s, s with { GlassHq = s.GlassHq with { Tint = GlassTintColour.Pink } }); break;
            case (GlassEngine.GlassHq, "concentric"):
                (Low, High) = (s with { GlassHq = s.GlassHq with { Concentric = false, Material = GlassHqMaterial.Clear } }, s with { GlassHq = s.GlassHq with { Concentric = true, Inset = 0, Material = GlassHqMaterial.Clear } });
                break;
            case (GlassEngine.Rizzy, _):
            {
                var k = RizzyGlass.Knobs.First(x => x.Id == id);
                (Low, High) = (s with { Rizzy = s.Rizzy.With(id, k.Min) }, s with { Rizzy = s.Rizzy.With(id, k.Max) });
                break;
            }
            case (GlassEngine.Dasher, _):
            {
                var k = DasherGlass.Knobs.First(x => x.Id == id);
                var on = s with { Dasher = s.Dasher with { Warp = true } };
                (Low, High) = (on with { Dasher = on.Dasher.With(id, k.Min) }, on with { Dasher = on.Dasher.With(id, k.Max) });
                break;
            }
            default:
            {
                var k = GlassHqGlass.Knobs.First(x => x.Id == id);
                var on = s with { GlassHq = s.GlassHq with { Concentric = true, Material = GlassHqMaterial.Clear } };
                (Low, High) = (on with { GlassHq = on.GlassHq.With(id, k.Min) }, on with { GlassHq = on.GlassHq.With(id, k.Max) });
                break;
            }
        }
        // Glass HQ bends within its 20 px bezel: its bend is measured there
        var area = nested ? GlassLiquidRender.Inner : engine == GlassEngine.GlassHq && id == "refraction" ? LeftRim : Whole;
        var a = Render(Low, surface, nested, corner: corner);
        var b = Render(High, surface, nested, corner: corner);
        Assert.True(a.Difference(b, area) > 0.4, $"{engine} {id}: the frame moves {a.Difference(b, area):F2}");
    }
}

// ED-NEW-shader-gradient-1 (row #87; overnight brief item 7): every option of the Gradient wallpaper (shadergradient's whole option set) reaches its
// shader and changes the picture: each number at its two ends, each choice, each switch and each colour, rendered on the CPU raster.
using Marrow.Core.Port.Settings;
using Marrow.UI.Tests.Fixtures;
using Marrow.UI.Theme.Wallpapers;
using Xunit;

namespace Marrow.UI.Tests.Theme.Wallpapers;

[Collection("ShaderGradient")]
public sealed class ShaderGradientWallpaperTests
{
    public static IEnumerable<object[]> Knobs() => ShaderGradientSettings.Knobs.Select(k => new object[] { k.Id });

    private static byte[] Frame(ShaderGradientSettings s, double time = 0)
    {
        var before = ShaderGradientParams.Current;
        try
        {
            ShaderGradientParams.Current = s;
            var env = new WallpaperEnvironment(new SettableWallpaperClock(), new FixedWallpaperAccent("#f7630c"), new FixedWallpaperPosition());
            using var r = new WallpaperRenderer(new(WallpaperKind.Animated, ShaderGradientParams.WallpaperId), true, env);
            r.Settle();
            return WallpaperFrames.At(r, time, 64, 40);
        }
        finally { ShaderGradientParams.Current = before; }
    }

    [Theory]
    [MemberData(nameof(Knobs))]
    [Trait("Decision", "ED-NEW-shader-gradient-1")]
    public void Every_number_changes_the_picture_between_its_ends(string id)
    {
        var knob = ShaderGradientSettings.Knobs.Single(k => k.Id == id);
        // the range's ends act only with Range on, and the end only once the flow has swung toward it
        var range = id is "rangeStart" or "rangeEnd";
        var s = ShaderGradientSettings.Default with { Range = range };
        var time = id == "rangeEnd" ? 20 : 0;
        // an angle's two ends are the same turn (-180 and 180, 0 and 360), and the range's two ends the same point of the flow's loop, so each
        // number is checked at its low end against a quarter of the way up, and that against its high end
        var quarter = knob.Min + (knob.Max - knob.Min) / 4;
        var low = Frame(s.With(id, knob.Min), time);
        var mid = Frame(s.With(id, quarter), time);
        var high = Frame(s.With(id, knob.Max), time);
        Assert.True(WallpaperFrames.Diff(low, mid) >= 8, $"{id}: the picture moves only {WallpaperFrames.Diff(low, mid)}/255 between {knob.Min} and {quarter}");
        Assert.True(WallpaperFrames.Diff(mid, high) >= 8, $"{id}: the picture moves only {WallpaperFrames.Diff(mid, high)}/255 between {quarter} and {knob.Max}");
    }

    [Fact]
    [Trait("Decision", "ED-NEW-shader-gradient-1")]
    public void Each_shape_lighting_environment_switch_and_colour_changes_the_picture()
    {
        var d = ShaderGradientSettings.Default;
        var base_ = Frame(d);
        void Differs(string what, ShaderGradientSettings s) => Assert.True(WallpaperFrames.Diff(base_, Frame(s)) >= 8, what);
        Differs("sphere", d with { Shape = GradientShape.Sphere });
        Differs("water plane", d with { Shape = GradientShape.WaterPlane });
        Assert.True(WallpaperFrames.Diff(Frame(d with { Shape = GradientShape.Sphere }), Frame(d with { Shape = GradientShape.WaterPlane })) >= 8, "sphere and water differ");
        Differs("environment light", d with { Lighting = GradientLighting.Environment });
        var env = d with { Lighting = GradientLighting.Environment };
        Assert.True(WallpaperFrames.Diff(Frame(env), Frame(env with { Environment = GradientEnvironment.Dawn })) >= 8, "dawn");
        Assert.True(WallpaperFrames.Diff(Frame(env), Frame(env with { Environment = GradientEnvironment.Lobby })) >= 8, "lobby");
        Differs("grain off", d with { Grain = false });
        Differs("wireframe", d with { Wireframe = true });
        Differs("colour 1", d with { Color1 = "#00ff40" });
        Differs("colour 2", d with { Color2 = "#1030ff" });
        Differs("colour 3", d with { Color3 = "#ffff00" });
    }

    /// <summary>With NTH_GLASS_DUMP set, each shape and lighting at 480 x 300 as PNGs, to look at.</summary>
    [Fact]
    [Trait("Decision", "ED-NEW-shader-gradient-1")]
    public void Dumps_each_shape_when_asked()
    {
        if (System.Environment.GetEnvironmentVariable("NTH_GLASS_DUMP") is not { Length: > 0 } dir) return;
        var d = ShaderGradientSettings.Default;
        foreach (var (name, s) in new[] { ("plane", d), ("sphere", d with { Shape = GradientShape.Sphere }), ("water", d with { Shape = GradientShape.WaterPlane }),
            ("env-dawn", d with { Lighting = GradientLighting.Environment, Environment = GradientEnvironment.Dawn }), ("wire", d with { Wireframe = true }) })
        {
            var before = ShaderGradientParams.Current;
            try
            {
                ShaderGradientParams.Current = s;
                var env = new WallpaperEnvironment(new SettableWallpaperClock(), new FixedWallpaperAccent("#f7630c"), new FixedWallpaperPosition());
                using var r = new WallpaperRenderer(new(WallpaperKind.Animated, ShaderGradientParams.WallpaperId), true, env);
                r.Settle();
                using var surface = SkiaSharp.SKSurface.Create(new SkiaSharp.SKImageInfo(480, 300));
                r.DrawFrameAt(surface.Canvas, 480, 300, 5);
                using var image = surface.Snapshot(); using var data = image.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100);
                File.WriteAllBytes(Path.Combine(dir, $"gradient-{name}.png"), data.ToArray());
            }
            finally { ShaderGradientParams.Current = before; }
        }
    }

    [Fact]
    [Trait("Decision", "ED-NEW-shader-gradient-1")]
    public void Animate_off_stills_the_flow_and_on_it_moves()
    {
        var on = ShaderGradientSettings.Default;
        Assert.True(WallpaperFrames.Diff(Frame(on, 0), Frame(on, 10)) >= 8, "animated, the flow moves");
        var off = on with { Animate = false };
        Assert.True(WallpaperFrames.Diff(Frame(off, 0), Frame(off, 10)) <= 1, "not animated, it holds still");
    }

    [Fact]
    [Trait("Decision", "ED-NEW-shader-gradient-1")]
    public void The_settings_store_what_differs_and_read_back_leniently()
    {
        var s = ShaderGradientSettings.Default.With("uStrength", 7.5) with { Shape = GradientShape.Sphere, Color2 = "#123ABC", Grain = false, Environment = GradientEnvironment.Dawn };
        var back = ShaderGradientSettings.FromJson(s.ToJson());
        Assert.Equal(s.Held(), back);
        Assert.Equal("#123abc", back.Color2);
        Assert.Equal("{}", ShaderGradientSettings.Default.ToString());
        Assert.Equal(ShaderGradientSettings.Default, ShaderGradientSettings.FromJson("{\"uStrength\":\"x\",\"type\":7,\"color1\":\"red\"}"u8));
        Assert.Equal(10, ShaderGradientSettings.Default.With("uStrength", 99).Get("uStrength"));
        Assert.Equal(21, ShaderGradientSettings.Knobs.Count);
    }
}

[CollectionDefinition("ShaderGradient", DisableParallelization = true)]
public sealed class ShaderGradientCollection;

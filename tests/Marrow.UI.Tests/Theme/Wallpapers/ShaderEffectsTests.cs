// ED-NEW-shader-effects-1 (row #87; overnight brief item 8): the Shaders section's effects (shader-effects-inc/shaders, MIT) are live wallpapers
// of their own tier, each with all its own options; every option reaches its shader and changes the picture, and the settings store what changed.
using Marrow.Core.Port.Settings;
using Marrow.UI.Tests.Fixtures;
using Marrow.UI.Theme.Wallpapers;
using Xunit;

namespace Marrow.UI.Tests.Theme.Wallpapers;

[Collection("ShaderGradient")]
public sealed class ShaderEffectsTests
{
    public static IEnumerable<object[]> Props() => ShaderEffects.All.SelectMany(e => e.Props.Select(p => new object[] { e.Id, p.Id }));

    // ED-NEW-shader-effects-2: the defaults, with the other options an option's probe names set so its effect can show
    private static ShaderEffectSettings Probed(string effect, ShaderEffectProp p) =>
        p.Probe.Where(x => !x.Key.StartsWith('@')).Aggregate(ShaderEffectSettings.Default, (s, x) => s.With(effect, x.Key, x.Value));

    // a periodic effect (a colour wheel at speed 1 repeats every second) shows its speed at a value its probe names
    private static double High(ShaderEffectProp p, double fallback) =>
        p.Probe.TryGetValue("@hi", out var h) ? double.Parse(h, System.Globalization.CultureInfo.InvariantCulture) : fallback;

    private static byte[] Frame(string id, ShaderEffectSettings s, double time = 7, double? motion = null)
    {
        var before = ShaderEffects.Current;
        try
        {
            ShaderEffects.Current = s;
            var env = new WallpaperEnvironment(new SettableWallpaperClock(), new FixedWallpaperAccent("#f7630c"), new FixedWallpaperPosition());
            using var r = new WallpaperRenderer(new(WallpaperKind.Animated, id), true, env);
            if (motion is { } m) r.Set(new(WallpaperKind.Animated, id, Options: new Dictionary<string, double> { ["motion"] = m }), true);
            r.Settle();
            return WallpaperFrames.At(r, time, 64, 40);
        }
        finally { ShaderEffects.Current = before; }
    }

    [Fact]
    [Trait("Decision", "ED-NEW-shader-effects-1")]
    public void The_Shaders_tier_is_exactly_the_effects_ported_so_far_each_with_its_options()
    {
        Assert.Equal(new[]
        {
            "sei-swirl", "sei-sunburst", "sei-marble", "sei-flowing-gradient", "sei-voronoi",
            "sei-aurora", "sei-beam", "sei-blob", "sei-block-noise", "sei-blue-noise", "sei-brick-pattern", "sei-checkerboard", "sei-chevron",
            "sei-color-wheel", "sei-conic-gradient", "sei-diamond-gradient", "sei-dot-grid", "sei-falling-lines", "sei-curl-noise", "sei-erosion-noise",
            "sei-fractal-noise", "sei-gabor-noise", "sei-godrays", "sei-grid", "sei-hex-grid", "sei-isometric-cubes", "sei-lens-flare", "sei-line",
            "sei-linear-gradient", "sei-multi-point-gradient", "sei-perlin-noise", "sei-prism", "sei-radial-gradient", "sei-ripples", "sei-scratches",
            "sei-simplex-noise", "sei-sine-wave", "sei-spiral", "sei-stripes", "sei-triangular-grid", "sei-truchet", "sei-weave", "sei-wavelet-noise",
            "sei-worley-noise",
        }, ShaderEffects.All.Select(e => e.Id));
        Assert.Equal(ShaderEffects.All.Select(e => e.Id), WallpaperCatalogue.All.Where(w => w.Tier == ShaderEffects.Tier).Select(w => w.Id));
        foreach (var e in ShaderEffects.All)
        {
            Assert.True(e.Props.Sum(p => p.Width) <= ShaderEffects.Rows * 4, e.Id);
            foreach (var p in e.Props) Assert.Contains(p.Kind, new[] { "color", "number", "select" });
        }
        Assert.All(WallpaperCatalogue.All, w => Assert.Contains(w.Tier, new[] { "vibrant", "balanced", "minimal", "live", ShaderEffects.Tier }));
    }

    public static IEnumerable<object[]> Effects() => ShaderEffects.All.Select(e => new object[] { e.Id });

    [Theory]
    [MemberData(nameof(Effects))]
    [Trait("Decision", "ED-NEW-shader-effects-2")]
    public void An_effect_with_a_speed_moves_and_one_without_is_a_still_picture_that_only_drifts(string effect)
    {
        // every effect with a speed changes between two moments at its fastest; the package's still generators (Beam, Checkerboard, Blue
        // Noise...) have no speed: their picture is the same at any time, and only the wallpaper's slow drift (its size Motion's) moves it,
        // so with Motion at 0 their frames are identical, and at the default they move
        var e = ShaderEffects.Find(effect)!;
        var speed = e.Props.SingleOrDefault(p => p.Id == "speed");
        if (speed is not null)
        {
            var d = Probed(effect, speed).With(effect, "speed", High(speed, speed.Max));
            var diff = WallpaperFrames.Diff(Frame(effect, d, 3), Frame(effect, d, 17));
            Assert.True(diff >= 6, $"{effect}: {diff}/255 between 3 s and 17 s at speed {High(speed, speed.Max)}");
            return;
        }
        var still = ShaderEffectSettings.Default;
        Assert.Equal(0, WallpaperFrames.Diff(Frame(effect, still, 3, motion: 0), Frame(effect, still, 17, motion: 0)));
        Assert.True(WallpaperFrames.Diff(Frame(effect, still, 3, motion: 0.5), Frame(effect, still, 17, motion: 0.5)) >= 3, $"{effect}: no drift");
    }

    [Theory]
    [MemberData(nameof(Props))]
    [Trait("Decision", "ED-NEW-shader-effects-1")]
    public void Every_option_of_every_effect_changes_the_picture(string effect, string option)
    {
        var p = ShaderEffects.Find(effect)!.Props.Single(x => x.Id == option);
        var d = Probed(effect, p);
        if (p.Kind == "color")
        {
            var other = p.Colour == "#00e0a0" ? "#ff2040" : "#00e0a0";
            Assert.True(WallpaperFrames.Diff(Frame(effect, d), Frame(effect, d.With(effect, option, other))) >= 6, $"{effect}.{option}: the colour does not show");
            return;
        }
        if (option == "speed")
        {
            // speed is how fast the effect's time runs: a still effect and a moving one differ away from the loop's start
            Assert.True(WallpaperFrames.Diff(Frame(effect, d.With(effect, option, 0)), Frame(effect, d.With(effect, option, High(p, p.Max)))) >= 6, $"{effect}.speed");
            return;
        }
        var lo = p.Probe.TryGetValue("@lo", out var l) ? double.Parse(l, System.Globalization.CultureInfo.InvariantCulture) : p.Min;
        var hi = p.Probe.TryGetValue("@hi", out var h) ? double.Parse(h, System.Globalization.CultureInfo.InvariantCulture)
            : p.Kind == "select" ? p.Max : p.Min + (p.Max - p.Min) / 4;
        Assert.InRange(lo, p.Min, p.Max);
        Assert.InRange(hi, p.Min, p.Max);
        var low = Frame(effect, d.With(effect, option, lo));
        var mid = Frame(effect, d.With(effect, option, hi));
        Assert.True(WallpaperFrames.Diff(low, mid) >= 6, $"{effect}.{option}: {WallpaperFrames.Diff(low, mid)}/255 between {lo} and {hi}");
    }

    [Fact]
    [Trait("Decision", "ED-NEW-shader-effects-1")]
    public void The_settings_store_what_changed_and_read_back_leniently()
    {
        var s = ShaderEffectSettings.Default.With("sei-marble", "scale", 4.5).With("sei-voronoi", "colorA", "#112233") with { Shown = "sei-marble" };
        var back = ShaderEffectSettings.FromJson(s.ToJson());
        Assert.Equal(s, back);
        Assert.Equal(4.5, back.Number("sei-marble", "scale", 0));
        Assert.Equal("#112233", back.Colour("sei-voronoi", "colorA", "#000000"));
        Assert.Equal(ShaderEffectSettings.Default, ShaderEffectSettings.FromJson("[1]"u8));
        Assert.Equal("{}", ShaderEffectSettings.Default.ToString());
        Assert.Null(s.Without("sei-marble").Get("sei-marble", "scale"));
        // a number past its range is held to it in the shader's uniforms
        var marble = ShaderEffects.Find("sei-marble")!;
        Assert.Equal(10f, ShaderEffects.Uniforms(marble, ShaderEffectSettings.Default.With("sei-marble", "scale", 99))[12]);
    }
}

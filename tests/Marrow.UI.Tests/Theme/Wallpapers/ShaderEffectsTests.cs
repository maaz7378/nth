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

    private static byte[] Frame(string id, ShaderEffectSettings s, double time = 7)
    {
        var before = ShaderEffects.Current;
        try
        {
            ShaderEffects.Current = s;
            var env = new WallpaperEnvironment(new SettableWallpaperClock(), new FixedWallpaperAccent("#f7630c"), new FixedWallpaperPosition());
            using var r = new WallpaperRenderer(new(WallpaperKind.Animated, id), true, env);
            r.Settle();
            return WallpaperFrames.At(r, time, 64, 40);
        }
        finally { ShaderEffects.Current = before; }
    }

    [Fact]
    [Trait("Decision", "ED-NEW-shader-effects-1")]
    public void The_Shaders_tier_is_exactly_the_effects_ported_so_far_each_with_its_options()
    {
        Assert.Equal(new[] { "sei-swirl", "sei-sunburst", "sei-marble", "sei-flowing-gradient", "sei-voronoi" }, ShaderEffects.All.Select(e => e.Id));
        Assert.Equal(ShaderEffects.All.Select(e => e.Id), WallpaperCatalogue.All.Where(w => w.Tier == ShaderEffects.Tier).Select(w => w.Id));
        foreach (var e in ShaderEffects.All)
        {
            Assert.True(e.Props.Sum(p => p.Width) <= ShaderEffects.Rows * 4, e.Id);
            Assert.Contains(e.Props, p => p.Id == "speed");
            foreach (var p in e.Props) Assert.Contains(p.Kind, new[] { "color", "number", "select" });
        }
        Assert.All(WallpaperCatalogue.All, w => Assert.Contains(w.Tier, new[] { "vibrant", "balanced", "minimal", "live", ShaderEffects.Tier }));
    }

    [Theory]
    [MemberData(nameof(Props))]
    [Trait("Decision", "ED-NEW-shader-effects-1")]
    public void Every_option_of_every_effect_changes_the_picture(string effect, string option)
    {
        var p = ShaderEffects.Find(effect)!.Props.Single(x => x.Id == option);
        var d = ShaderEffectSettings.Default;
        if (p.Kind == "color")
        {
            var other = p.Colour == "#00e0a0" ? "#ff2040" : "#00e0a0";
            Assert.True(WallpaperFrames.Diff(Frame(effect, d), Frame(effect, d.With(effect, option, other))) >= 6, $"{effect}.{option}: the colour does not show");
            return;
        }
        if (option == "speed")
        {
            // speed is how fast the effect's time runs: a still effect and a moving one differ away from the loop's start
            Assert.True(WallpaperFrames.Diff(Frame(effect, d.With(effect, option, 0)), Frame(effect, d.With(effect, option, p.Max))) >= 6, $"{effect}.speed");
            return;
        }
        var quarter = p.Min + (p.Max - p.Min) / 4;
        var low = Frame(effect, d.With(effect, option, p.Min));
        var mid = Frame(effect, d.With(effect, option, p.Kind == "select" ? p.Max : quarter));
        Assert.True(WallpaperFrames.Diff(low, mid) >= 6, $"{effect}.{option}: {WallpaperFrames.Diff(low, mid)}/255 between {p.Min} and {(p.Kind == "select" ? p.Max : quarter)}");
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

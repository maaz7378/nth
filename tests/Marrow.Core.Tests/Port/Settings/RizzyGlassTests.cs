using System.Text;
using Marrow.Core.Port.Settings;

namespace Marrow.Core.Tests.Port.Settings;

// ED-NEW-glass-alive-5 (phase 1a): rizzytoday/liquid-glass as its own engine. Every expected number is the repository's (core/liquid-glass.ts
// resolveConfig and demo/index.html's sliders, commit 841af14) or a preset of those same options.
public sealed class RizzyGlassTests
{
    private static GlassSettings Read(string json) => GlassSettings.FromJson(Encoding.UTF8.GetBytes(json));

    private static string Json(GlassSettings settings) => Encoding.UTF8.GetString(settings.ToJson());

    [Fact]
    [Trait("Decision", "ED-NEW-glass-alive-5")]
    public void Repo_default_is_the_repository_resolve_config_untouched()
    {
        var r = RizzyGlass.Repo;
        Assert.Equal((50.0, -180.0, 0.0, 10.0, 20.0), (r.BorderRadius, r.Scale, r.AberrationR, r.AberrationG, r.AberrationB));
        Assert.Equal((11.0, 0.07, 50.0, 0.93), (r.Blur, r.Border, r.Lightness, r.Alpha));
        Assert.Equal((0.0, 1.0, 0.0), (r.Frost, r.Saturation, r.DisplaceBlur));
        Assert.Equal(GlassEnginePreset.Repo, r.Preset);
    }

    [Fact]
    [Trait("Decision", "ED-NEW-glass-alive-5")]
    public void Every_option_of_the_repository_is_a_setting_with_the_demo_slider_range()
    {
        Assert.Equal(["borderRadius", "scale", "aberrationR", "aberrationG", "aberrationB", "blur", "border", "lightness", "alpha", "frost", "saturation", "displaceBlur"],
            RizzyGlass.Knobs.Select(k => k.Id));
        var ranges = RizzyGlass.Knobs.ToDictionary(k => k.Id, k => (k.Min, k.Max));
        Assert.Equal((-300.0, -20.0), ranges["scale"]);
        Assert.Equal((-40.0, 40.0), ranges["aberrationB"]);
        Assert.Equal((0.0, 0.3), ranges["border"]);
        Assert.Equal((0.0, 3.0), ranges["saturation"]);
        foreach (var knob in RizzyGlass.Knobs)
        {
            Assert.InRange(RizzyGlass.Repo.Get(knob.Id), knob.Min, knob.Max);
            Assert.Equal(knob.Max, RizzyGlass.Repo.With(knob.Id, 1e9).Get(knob.Id));
            Assert.Equal(knob.Min, RizzyGlass.Repo.With(knob.Id, -1e9).Get(knob.Id));
        }
    }

    [Fact]
    [Trait("Decision", "ED-NEW-glass-alive-5")]
    public void The_presets_step_the_same_options_and_balanced_is_the_engines_first_options()
    {
        Assert.Equal(-90, RizzyGlass.Of(GlassEnginePreset.Subtle).Scale);
        var balanced = RizzyGlass.Of(GlassEnginePreset.Balanced);
        Assert.Equal((-200.0, 0.09, 0.9, 0.24, 1.15), (balanced.Scale, balanced.Border, balanced.Alpha, balanced.Frost, balanced.Saturation));
        Assert.Equal(-300, RizzyGlass.Of(GlassEnginePreset.Strong).Scale);
        Assert.Equal(GlassEngine.Canvas, GlassSettings.Default.Engine);
        Assert.Equal(balanced, GlassSettings.Default.Rizzy);
        Assert.Equal(GlassEnginePreset.Balanced, GlassSettings.Default.Rizzy.Preset);
        Assert.Null(balanced.With("scale", -210).Preset);
    }

    [Fact]
    [Trait("Decision", "ED-NEW-glass-alive-5")]
    public void Engine_and_options_round_trip_and_the_default_writes_nothing()
    {
        Assert.Equal("{}", Json(GlassSettings.Default));
        var liquid = GlassSettings.Default with { Engine = GlassEngine.Rizzy };
        Assert.Equal("{\"engine\":\"rizzy\"}", Json(liquid));
        Assert.Equal(liquid, Read(Json(liquid)));
        var custom = GlassSettings.Default with { Rizzy = GlassSettings.Default.Rizzy.With("scale", -260).With("displaceBlur", 1.5) };
        Assert.Equal("{\"rizzy\":{\"scale\":-260,\"displaceBlur\":1.5}}", Json(custom));
        Assert.Equal(custom, Read(Json(custom)));
        Assert.Equal(GlassSettings.Default.Engine, Read("{\"engine\":\"metal\"}").Engine);
        Assert.Equal(-300, Read("{\"rizzy\":{\"scale\":-9000,\"frost\":\"x\"}}").Rizzy.Scale);
        Assert.Equal(GlassSettings.Default.Rizzy.Frost, Read("{\"rizzy\":{\"frost\":\"x\"}}").Rizzy.Frost);
        Assert.Equal(GlassSettings.Default.Engine, (GlassSettings.Default with { Engine = (GlassEngine)9 }).Held().Engine);
    }

    [Fact]
    [Trait("Decision", "ED-NEW-glass-alive-5")]
    public void A_glide_moves_every_option_and_ends_exactly_on_the_target()
    {
        var from = GlassSettings.Default with { Rizzy = RizzyGlass.Of(GlassEnginePreset.Subtle) };
        var to = GlassSettings.Default with { Rizzy = RizzyGlass.Of(GlassEnginePreset.Strong) };
        Assert.Equal(-195, from.Toward(to, 0.5).Rizzy.Scale);
        Assert.Equal(to.Rizzy, from.Toward(to, 1).Rizzy);
        Assert.Equal(from.Rizzy, from.Toward(to, 0).Rizzy);
    }
}

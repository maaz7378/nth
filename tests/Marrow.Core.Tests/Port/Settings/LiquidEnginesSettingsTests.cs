using System.Text;
using Marrow.Core.Port.Settings;

namespace Marrow.Core.Tests.Port.Settings;

// ED-NEW-glass-alive-6..8: the Dasher and Glass HQ engines' options as their repositories have them, the engine picker and the Liquid glass switch.
public sealed class LiquidEnginesSettingsTests
{
    private static GlassSettings Read(string json) => GlassSettings.FromJson(Encoding.UTF8.GetBytes(json));
    private static string Json(GlassSettings s) => Encoding.UTF8.GetString(s.ToJson());

    [Fact]
    [Trait("Decision", "ED-NEW-glass-alive-8")]
    public void The_picker_offers_rizzy_dasher_glass_hq_and_canvas_and_a_fresh_profile_is_liquid()
    {
        Assert.Equal(["Rizzy", "Dasher", "Glass HQ", "Canvas"], GlassSettings.Engines.Select(e => e.Name));
        Assert.Equal(GlassEngine.Rizzy, GlassSettings.Default.Engine);
        Assert.True(GlassSettings.Default.Liquid);
        Assert.Equal(GlassEngine.Rizzy, GlassSettings.Default.Drawn);
        Assert.Equal(GlassEngine.Canvas, (GlassSettings.Default with { Liquid = false }).Drawn);
        Assert.Equal("{\"liquid\":false}", Json(GlassSettings.Default with { Liquid = false }));
        Assert.False(Read("{\"liquid\":false}").Liquid);
        Assert.Equal(GlassEngine.GlassHq, Read("{\"engine\":\"glasshq\"}").Engine);
    }

    [Fact]
    [Trait("Decision", "ED-NEW-glass-alive-6")]
    public void Dasher_has_every_slider_of_the_repository_with_its_default_and_range()
    {
        Assert.Equal(["edgeIntensity", "rimIntensity", "baseIntensity", "edgeDistance", "rimDistance", "baseDistance", "cornerBoost", "rippleEffect", "blurRadius", "tintOpacity", "borderRadius"],
            DasherGlass.Knobs.Select(k => k.Id));
        var r = DasherGlass.Repo;
        Assert.Equal((0.01, 0.05, 0.01, 0.15, 0.8, 0.1, 0.02, 0.1, 5.0, 0.2, 48.0, false),
            (r.EdgeIntensity, r.RimIntensity, r.BaseIntensity, r.EdgeDistance, r.RimDistance, r.BaseDistance, r.CornerBoost, r.RippleEffect, r.BlurRadius, r.TintOpacity, r.BorderRadius, r.Warp));
        var ranges = DasherGlass.Knobs.ToDictionary(k => k.Id, k => (k.Min, k.Max));
        Assert.Equal((0.0, 0.2), ranges["rimIntensity"]);
        Assert.Equal((1.0, 15.0), ranges["blurRadius"]);
        foreach (var k in DasherGlass.Knobs) Assert.InRange(r.Get(k.Id), k.Min, k.Max);
        var custom = GlassSettings.Default with { Dasher = DasherGlass.Repo with { Warp = true, RimIntensity = 0.12 } };
        Assert.Equal(custom, Read(Json(custom)));
        Assert.Equal(DasherGlass.Of(GlassEnginePreset.Balanced), GlassSettings.Default.Dasher);
    }

    [Fact]
    [Trait("Decision", "ED-NEW-glass-alive-7")]
    public void Glass_hq_has_every_material_option_of_the_repository()
    {
        var r = GlassHqGlass.Repo;
        Assert.Equal((GlassHqMaterial.Regular, GlassHqAppearance.Scene, 8.0, 60.0, true, GlassTintColour.None, true, GlassHqMotion.Full),
            (r.Material, r.Appearance, r.Radius, r.Refraction, r.Concentric, r.Tint, r.Interactive, r.Motion));
        var custom = GlassSettings.Default with
        {
            GlassHq = r with { Material = GlassHqMaterial.Clear, Appearance = GlassHqAppearance.Light, Tint = GlassTintColour.Blue, Motion = GlassHqMotion.None, Interactive = false, Concentric = false, Refraction = 90 },
        };
        Assert.Equal(custom, Read(Json(custom)));
        Assert.Equal("clear", (GlassHqGlass.Repo with { Material = GlassHqMaterial.Clear }).Preset);
        Assert.Equal(120, GlassHqGlass.Repo.With("refraction", 1e9).Refraction);
    }
}

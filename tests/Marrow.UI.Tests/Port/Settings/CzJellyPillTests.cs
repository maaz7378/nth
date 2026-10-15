// ED-NEW-jelly-pill-1 (row #87 phase 3): every Settings segmented selector's pill is a drop of liquid glass that can be dragged.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Marrow.UI.Port;
using Marrow.UI.Port.Settings;
using Marrow.UI.Tests.Fixtures;
using Xunit;

namespace Marrow.UI.Tests.Port.Settings;

[Collection("Headless")]
public sealed class CzJellyPillTests(HeadlessSession headless)
{
    private static (Window, CzSegmentedControl) Show()
    {
        var control = new CzSegmentedControl("Look", [("a", "Clear"), ("b", "Regular"), ("c", "Tinted")], "a")
        {
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
        };
        var window = new Window { Content = control, Width = 400, Height = 100 };
        window.Show();
        GlassScene.Drain();
        return (window, control);
    }

    [Fact]
    [Trait("Decision", "ED-NEW-jelly-pill-1")]
    public void The_pill_is_a_liquid_glass_drop_on_the_glass_under_it() => headless.OnUiThread(() =>
    {
        var (window, control) = Show();
        try
        {
            var effect = SwiftGlass.GetEffect(control.Drop);
            Assert.NotNull(effect);
            Assert.Equal(GlassSurface.Inner, effect!.Surface);
            Assert.Equal(control.Pill, control.Drop.Bounds);
        }
        finally { window.Close(); }
    });

    [Fact]
    [Trait("Decision", "ED-NEW-jelly-pill-1")]
    public void Dragging_the_drop_onto_a_segment_picks_it() => headless.OnUiThread(() =>
    {
        var (window, control) = Show();
        try
        {
            var picked = "";
            control.SelectionChanged += id => picked = id;
            var from = control.Pill.Center;
            var to = control.Buttons[2].Bounds.Center;
            window.MouseDown(from, MouseButton.Left, RawInputModifiers.None);
            window.MouseMove(new Point((from.X + to.X) / 2, from.Y), RawInputModifiers.LeftMouseButton);
            Assert.True(control.Dragging);
            window.MouseMove(to, RawInputModifiers.LeftMouseButton);
            window.MouseUp(to, MouseButton.Left, RawInputModifiers.None);
            Assert.False(control.Dragging);
            Assert.Equal("c", control.SelectedId);
            Assert.Equal("c", picked);
        }
        finally { window.Close(); }
    });

    [Fact]
    [Trait("Decision", "ED-NEW-jelly-pill-1")]
    public void The_stretch_keeps_the_drops_volume_and_is_none_at_rest()
    {
        Assert.Equal((1.0, 1.0), CzSegmentedControl.Stretch(0));
        var (x, y) = CzSegmentedControl.Stretch(1);
        Assert.True(x > 1.1);
        Assert.Equal(1, x * y, 9);
    }
}

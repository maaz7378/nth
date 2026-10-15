// ED-NEW-glass-controls-1 (row #87; overnight brief item 5): every Settings slider's knob is a liquid glass thumb over its track: a third-layer
// lens that follows the value and stretches along its travel with the drag's speed, relaxing when it stops; Reduce Motion never stretches it.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Marrow.Testing;
using Marrow.UI.Port;
using Marrow.UI.Port.Settings;
using Marrow.UI.Tests.Fixtures;
using Xunit;

namespace Marrow.UI.Tests.Port.Settings;

[Collection("Headless")]
public sealed class GlassThumbTests(HeadlessSession headless)
{
    [Fact]
    [Trait("Decision", "ED-NEW-glass-controls-1")]
    public void A_slider_drag_moves_the_glass_thumb_with_the_value_and_stretches_it_with_speed() => headless.OnUiThread(() =>
    {
        var frames = new ManualFrameSource();
        var time = new ManualTimeProvider();
        using var source = FrameClock.Source.Override(frames);
        using var clock = Marrow.Core.Port.SwiftClock.Provider.Override(time);
        var slider = new CzSlider("Glass intensity", 0, 2, 0.05, 1)
        {
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
        };
        var window = new Window { Content = slider, Width = 400, Height = 100 };
        window.Show();
        void Step(int n = 1) { for (var i = 0; i < n; i++) { time.Advance(TimeSpan.FromMilliseconds(16)); frames.Advance(TimeSpan.FromMilliseconds(16)); } GlassScene.Drain(); }
        try
        {
            Step(60);
            var thumb = slider.Rail.Thumb;
            var effect = SwiftGlass.GetEffect(thumb);
            Assert.NotNull(effect);
            Assert.Equal(GlassSurface.Inner, effect!.Surface);
            Assert.Equal(3, SwiftGlass.GetLayer(thumb));
            Assert.Equal(slider.Rail.KnobRect, thumb.Bounds);
            var start = thumb.Bounds.Center.X;
            var rail = slider.Rail.TranslatePoint(default, window)!.Value;
            var from = new Point(rail.X + slider.Rail.KnobRect.Center.X, rail.Y + 11);
            window.MouseDown(from, MouseButton.Left, RawInputModifiers.None);
            Step();
            window.MouseMove(new Point(from.X + 50, from.Y), RawInputModifiers.LeftMouseButton);
            Step();
            Assert.True(thumb.Bounds.Center.X > start + 40, "the thumb follows the drag");
            Assert.True(thumb.Jelly > 0.1, $"a fast drag stretches it ({thumb.Jelly:F2})");
            window.MouseUp(new Point(from.X + 50, from.Y), MouseButton.Left, RawInputModifiers.None);
            Step(90);
            Assert.Equal(0, thumb.Jelly, 2);
            Assert.True(slider.Value > 1.4);
        }
        finally { window.Close(); }
    });

    [Fact]
    [Trait("Decision", "ED-NEW-glass-controls-1")]
    public void The_stretch_keeps_the_area_and_grows_with_speed()
    {
        Assert.Equal((1.0, 1.0), GlassThumb.Stretch(0));
        var (along, across) = GlassThumb.Stretch(1);
        Assert.True(along > 1.2 && across < 1);
        Assert.Equal(1, along * across, 9);
        Assert.True(GlassThumb.StretchFor(200) < GlassThumb.StretchFor(500));
        Assert.Equal(1.2, GlassThumb.StretchFor(1e6));
    }
}

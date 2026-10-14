// ED-NEW-glass-alive-4 (row #87: "a jelly scroll ... plus an elastic edge"): a scroll view pushed past its end gives a little and springs
// back. The offset never leaves its range; the content moves by a render transform that comes off at rest, so a still window draws nothing.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Media;
using Marrow.UI.Port;
using Marrow.UI.Tests.Fixtures;
using Xunit;

namespace Marrow.UI.Tests.Port;

[Collection("Headless")]
public sealed class ElasticEdgeTests(HeadlessSession headless)
{
    [Fact]
    [Trait("Decision", "ED-NEW-glass-alive-4")] // the band: gives less the further it is pushed, never the view's length, odd in the push
    public void The_band_gives_less_the_further_it_is_pushed()
    {
        Assert.Equal(0, ElasticStretch.Rubber(0, 300));
        var small = ElasticStretch.Rubber(10, 300);
        Assert.InRange(small, 5.3, 5.5); // about the resistance's share of a small push
        var a = ElasticStretch.Rubber(100, 300);
        var b = ElasticStretch.Rubber(200, 300);
        Assert.True(a < 100 && b > a && b - a < a, $"100 gives {a:F1}, 200 gives {b:F1}");
        Assert.True(ElasticStretch.Rubber(1e6, 300) < 300);
        Assert.True(ElasticStretch.Rubber(1e6, 5000) < ElasticStretch.MaxDimension); // a long view is measured at most 600 points
        Assert.Equal(-ElasticStretch.Rubber(70, 300), ElasticStretch.Rubber(-70, 300), 9);
    }

    [Fact]
    [Trait("Decision", "ED-NEW-glass-alive-4")] // it follows the push smoothly, holds while pushed, springs back with no overshoot and rests at exactly zero
    public void The_stretch_follows_holds_then_springs_back_to_rest()
    {
        var s = new ElasticStretch();
        s.Pull(100, 300, 10.0);
        Assert.Equal(0, s.Shown); // nothing at the push's own instant
        var target = ElasticStretch.Rubber(100, 300);
        var shown = new List<double>();
        for (var k = 1; k <= 240; k++)
        {
            var moving = s.Step(10.0 + k / 240.0);
            shown.Add(s.Shown);
            if (!moving) break;
        }
        Assert.True(shown[0] > 0 && shown[0] < target * 0.2, $"the first frame shows {shown[0]:F2} of {target:F2}: no jump");
        Assert.True(shown.Max() <= target + 1e-9 && shown.Max() > target * 0.85, $"it reaches {shown.Max():F2} of {target:F2}");
        Assert.All(shown, v => Assert.True(v >= 0, "it never springs past zero"));
        Assert.Equal(0, shown[^1]);
        Assert.False(s.IsMoving);
        Assert.True(shown.Count < 240, "it rests within a second");
        var steps = shown.Zip(shown.Skip(1), (p, n) => Math.Abs(n - p)).Max();
        Assert.True(steps < target * 0.2, $"no frame moves more than a fifth of the stretch ({steps:F2})");
    }

    [Fact]
    [Trait("Decision", "ED-NEW-glass-alive-4")] // a push during the spring back grabs it from where it shows, with no jump
    public void A_push_during_the_spring_back_grabs_it_where_it_is()
    {
        var s = new ElasticStretch();
        s.Pull(100, 300, 0);
        for (var k = 1; k <= 36; k++) s.Step(k / 240.0); // 150 ms: past the hold, springing back
        var before = s.Shown;
        s.Pull(1e-9, 300, 36 / 240.0);
        s.Step(37 / 240.0);
        Assert.True(Math.Abs(s.Shown - before) < before * 0.1, $"{before:F2} then {s.Shown:F2}");
    }

    [Fact]
    [Trait("Decision", "ED-NEW-glass-alive-4")] // in the app: the offset stays at its end, the content moves up, and at rest the transform is gone and no frame is asked for
    public void A_wheel_past_the_end_stretches_the_content_and_rests_with_no_transform() => headless.OnUiThread(() =>
    {
        using var rig = new SoftScrollRig();
        var most = rig.Scroll.Extent.Height - rig.Scroll.Viewport.Height;
        SoftScroll.For(rig.Scroll).Jump(new Vector(0, most));
        rig.Window.UpdateLayout();
        var content = (Control)((ScrollContentPresenter)rig.Scroll.Presenter!).Child!;
        rig.Wheel(new Vector(0, -1)); // down, past the bottom
        var lowest = 0.0;
        for (var k = 0; k < 24; k++)
        {
            rig.Frame(240);
            Assert.Equal(most, rig.Scroll.Offset.Y, 6);
            if (content.RenderTransform is TranslateTransform t) lowest = Math.Min(lowest, t.Y);
        }
        Assert.True(lowest < -10, $"the content moved up {-lowest:F1} points");
        rig.Run(1.0);
        Assert.Null(content.RenderTransform);
        Assert.False(ElasticEdge.IsMoving(rig.Scroll));
        Assert.Equal(0, rig.Frames.Pending);
        Assert.Equal(most, rig.Scroll.Offset.Y, 6);
    });

    [Fact]
    [Trait("Decision", "ED-NEW-glass-alive-4")] // Reduce Motion keeps the dead stop
    public void Reduce_motion_keeps_the_dead_stop() => headless.OnUiThread(() =>
    {
        using var rig = new SoftScrollRig(viewer => EnvironmentValues.SetAccessibilityReduceMotion(viewer, true));
        var content = (Control)((ScrollContentPresenter)rig.Scroll.Presenter!).Child!;
        rig.Wheel(new Vector(0, 1)); // up, past the top
        rig.Run(0.2);
        Assert.Null(content.RenderTransform);
        Assert.Equal(default, ElasticEdge.Stretch(rig.Scroll));
        Assert.Equal(0, rig.Scroll.Offset.Y);
    });
}

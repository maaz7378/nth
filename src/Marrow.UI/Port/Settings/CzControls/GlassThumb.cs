// PORT-ONLY ED-NEW-glass-controls-1 (row #87; Maaz, 2026-10-15, overnight brief item 5; Glass-HQ's slider and switch, MIT, ideas only): the thumb
// of every slider and switch is one drop of liquid glass, the third lens (on the control, on its card, on the window): it bends the track and the
// light under it, and while it moves it stretches along its travel and narrows across it with its speed, relaxing with a small wobble when it stops.
// Its owner places it (Place) and arranges it at Rect; it draws nothing of its own past the glass. Under Reduce Motion it never stretches.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Marrow.Core.Port;

namespace Marrow.UI.Port.Settings;

/// <summary>A control's liquid glass thumb.</summary>
public sealed class GlassThumb : Border
{
    private readonly ScaleTransform squash = new();
    private readonly CzNumber jelly;
    private (double X, ContinuousInstant At)? last;
    private readonly bool horizontal;

    /// <summary>The spring the stretch relaxes on.</summary>
    public static Animation Relax => Animation.Spring(response: 0.3, dampingFraction: 0.42);

    /// <param name="corner">The thumb's corner (pt); a capsule when it is half its height.</param>
    /// <param name="horizontal">The thumb travels across (a slider, a switch) rather than down.</param>
    public GlassThumb(double corner, bool horizontal = true)
    {
        this.horizontal = horizontal;
        Background = Brushes.Transparent;
        CornerRadius = new CornerRadius(corner);
        IsHitTestVisible = false;
        RenderTransform = squash;
        RenderTransformOrigin = RelativePoint.Center;
        SwiftGlass.SetEffect(this, GlassEffect.Regular(GlassShape.RoundedRectangle(corner, RoundedCornerStyle.Continuous)).OnSurface(GlassSurface.Inner));
        SwiftGlass.SetLayer(this, 3); // ED-NEW-glass-layers-1: a control on a card on the window
        jelly = new CzNumber(new CzMotion(this), 0, _ => Restretch());
    }

    /// <summary>Where the owner arranges the thumb.</summary>
    public Rect Rect { get; private set; }

    /// <summary>How stretched it is now (0 at rest).</summary>
    public double Jelly => jelly.Value;

    /// <summary>The scale at a stretch <paramref name="j"/>: longer along the travel, narrower across it, its area kept.</summary>
    public static (double Along, double Across) Stretch(double j) => (1 + 0.3 * j, 1 / (1 + 0.3 * j));

    /// <summary>The stretch a speed (pt per second) asks for.</summary>
    public static double StretchFor(double speed) => Math.Min(1.2, Math.Abs(speed) / 700);

    /// <summary>Moves the thumb to <paramref name="rect"/>; its speed since the last place stretches it.</summary>
    public void Place(Rect rect)
    {
        var now = SwiftClock.Now;
        var at = horizontal ? rect.Center.X : rect.Center.Y;
        if (last is { } l && !EnvironmentValues.GetAccessibilityReduceMotion(this))
        {
            // two places in one instant (a layout pass inside the input that moved it) count as one 60 Hz frame apart
            var dt = Math.Max((now - l.At).TotalSeconds, 1 / 60.0);
            if (dt < 0.25)
            {
                var want = StretchFor((at - l.X) / dt);
                if (want > jelly.Value + 0.01) { jelly.Jump(want); jelly.To(0, Relax); }
            }
        }
        last = (at, now);
        Rect = rect;
    }

    private void Restretch()
    {
        var (along, across) = EnvironmentValues.GetAccessibilityReduceMotion(this) ? (1.0, 1.0) : Stretch(Math.Clamp(jelly.Value, -0.3, 1.3));
        (squash.ScaleX, squash.ScaleY) = horizontal ? (along, across) : (across, along);
    }
}

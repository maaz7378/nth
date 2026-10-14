// PORT-ONLY (ED-NEW-glass-alive-4; row #87: "a jelly scroll ... plus an elastic edge"): a scroll view pushed past its end gives a little and
// springs back, instead of stopping dead (SoftScroll's file note, "the ends"). The offset never leaves its range: the content is moved by a
// render transform on the presenter's child (no layout, no new pass), and the transform is taken off when it rests, so a still window draws
// nothing. Reduce Motion, or no frame clock, keeps the dead stop.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Media;

namespace Marrow.UI.Port;

/// <summary>
/// PORT-ONLY. The stretch on one axis: a rubber band over the distance pushed past the end, <c>(1 - 1 / (x * 0.55 / d + 1)) * d</c> for a
/// view <c>d</c> points long (at most <see cref="MaxDimension"/>), so it gives less the further it is pushed and never reaches <c>d</c>.
/// The shown stretch follows it through a 30 ms lag; 80 ms after the last push it springs back, critically damped (90 ms), from rest, and
/// rests at zero once under a quarter point. Times are the Swift clock's seconds, so it is the same curve at any refresh rate.
/// </summary>
public sealed class ElasticStretch
{
    /// <summary>The band's resistance (the share of a small push that shows).</summary>
    public const double Resistance = 0.55;

    /// <summary>The longest view the band is measured against, in points.</summary>
    public const double MaxDimension = 600;

    /// <summary>The lag the shown stretch follows the band with, in seconds.</summary>
    public const double Follow = 0.03;

    /// <summary>How long after the last push the band lets go, in seconds.</summary>
    public const double Hold = 0.08;

    /// <summary>The spring back's time constant, in seconds (critically damped).</summary>
    public const double Return = 0.09;

    /// <summary>Under this many points the stretch is at rest.</summary>
    public const double Rest = 0.25;

    private double pull, target, dimension = MaxDimension, lastPull, stepped, releaseAt, releaseFrom;
    private bool releasing;

    /// <summary>The stretch shown, in points, signed as the push (positive: past the far end).</summary>
    public double Shown { get; private set; }

    /// <summary>Whether it is stretched or being pulled (frames are wanted).</summary>
    public bool IsMoving { get; private set; }

    /// <summary>The band's stretch for <paramref name="pushed"/> points past the end of a view <paramref name="length"/> points long.</summary>
    public static double Rubber(double pushed, double length)
    {
        var d = Math.Clamp(length, 1, MaxDimension);
        return Math.Sign(pushed) * (1 - 1 / (Math.Abs(pushed) * Resistance / d + 1)) * d;
    }

    // The push that shows `stretch` (the band's inverse): a grab during the spring back starts from what is shown.
    private static double Unrubber(double stretch, double length)
    {
        var d = Math.Clamp(length, 1, MaxDimension);
        var s = Math.Min(Math.Abs(stretch), d * 0.999);
        return Math.Sign(stretch) * d / Resistance * (s / (d - s));
    }

    /// <summary>A push of <paramref name="distance"/> points past the end of a view <paramref name="length"/> points long, at <paramref name="now"/>.</summary>
    public void Pull(double distance, double length, double now)
    {
        if (distance == 0 || !double.IsFinite(distance)) return;
        if (!IsMoving)
        {
            stepped = now;
            Shown = 0;
            pull = 0;
        }
        else
        {
            Advance(now);
            if (releasing || (target != 0 && Math.Sign(distance) != Math.Sign(target))) pull = Unrubber(Shown, length);
        }
        releasing = false;
        dimension = length;
        pull += distance;
        target = Rubber(pull, dimension);
        lastPull = now;
        IsMoving = true;
    }

    /// <summary>Moves the stretch to <paramref name="now"/>; false once it rests at zero.</summary>
    public bool Step(double now)
    {
        if (!IsMoving) return false;
        Advance(now);
        if (releasing && Math.Abs(Shown) < Rest)
        {
            Shown = pull = target = 0;
            releasing = false;
            IsMoving = false;
        }
        return IsMoving;
    }

    /// <summary>Lets go at once: no stretch, at rest.</summary>
    public void Clear()
    {
        Shown = pull = target = 0;
        releasing = false;
        IsMoving = false;
    }

    private void Advance(double now)
    {
        if (now <= stepped) return;
        if (!releasing)
        {
            var letGo = lastPull + Hold;
            var until = Math.Min(now, letGo);
            if (until > stepped) Shown = target + (Shown - target) * Math.Exp(-(until - stepped) / Follow);
            stepped = until;
            if (now < letGo) return;
            releasing = true;
            releaseAt = letGo;
            releaseFrom = Shown;
        }
        var u = now - releaseAt;
        Shown = releaseFrom * (1 + u / Return) * Math.Exp(-u / Return);
        stepped = now;
    }
}

/// <summary>PORT-ONLY (ED-NEW-glass-alive-4). The elastic edge on a ScrollViewer: <see cref="Pull"/> from the wheel that found no scroll view
/// left to move, the content moved by the stretch on the frame clock until it rests.</summary>
public static class ElasticEdge
{
    private sealed class State(ScrollViewer viewer)
    {
        public readonly ScrollViewer Viewer = viewer;
        public readonly ElasticStretch X = new();
        public readonly ElasticStretch Y = new();
        public readonly TranslateTransform Shift = new();
        public Control? Moved;
        public IFrameSource? Source;
        public bool Outstanding;
        public Action<TimeSpan>? Frame;
    }

    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<ScrollViewer, State> States = new();

    /// <summary>The stretch shown on <paramref name="viewer"/>, in points per axis (positive: past the far end).</summary>
    public static Vector Stretch(ScrollViewer viewer) => States.TryGetValue(viewer, out var s) ? new Vector(s.X.Shown, s.Y.Shown) : default;

    /// <summary>Whether <paramref name="viewer"/> is stretched or springing back.</summary>
    public static bool IsMoving(ScrollViewer viewer) => States.TryGetValue(viewer, out var s) && (s.X.IsMoving || s.Y.IsMoving);

    /// <summary>A push of <paramref name="distance"/> points per axis past <paramref name="viewer"/>'s end. Nothing under Reduce Motion or
    /// with no frame clock, and nothing on a view whose content carries a transform of its own.</summary>
    public static void Pull(ScrollViewer viewer, Vector distance)
    {
        if (EnvironmentValues.GetAccessibilityReduceMotion(viewer) || !FrameClock.IsBound) return;
        if (viewer.Presenter is not ScrollContentPresenter { Child: Control content }) return;
        var state = States.GetValue(viewer, v => new State(v));
        if (content.RenderTransform is { } own && !ReferenceEquals(own, state.Shift)) return;
        if (state.Moved is { } before && before != content) Release(state);
        var now = ScrollDriver.Now;
        state.X.Pull(distance.X, viewer.Viewport.Width, now);
        state.Y.Pull(distance.Y, viewer.Viewport.Height, now);
        if (!state.X.IsMoving && !state.Y.IsMoving) return;
        state.Moved = content;
        if (state.Frame is null)
        {
            state.Frame = _ => OnFrame(state);
            viewer.DetachedFromVisualTree += (_, _) => Stop(state);
        }
        Request(state);
    }

    private static void Request(State state)
    {
        if (state.Outstanding) return;
        state.Source ??= FrameClock.Source.Current;
        state.Outstanding = true;
        state.Source.RequestFrame(state.Frame!);
    }

    private static void OnFrame(State state)
    {
        state.Outstanding = false;
        var now = ScrollDriver.Now;
        var moving = state.X.Step(now) | state.Y.Step(now);
        if (state.Moved is not { } content) return;
        if (!moving)
        {
            Release(state);
            return;
        }
        state.Shift.X = -state.X.Shown;
        state.Shift.Y = -state.Y.Shown;
        if (!ReferenceEquals(content.RenderTransform, state.Shift)) content.RenderTransform = state.Shift;
        Request(state);
    }

    // At rest: the transform comes off, so the content draws as it did before the push.
    private static void Release(State state)
    {
        if (state.Moved is { } content && ReferenceEquals(content.RenderTransform, state.Shift)) content.RenderTransform = null;
        state.Moved = null;
        state.Shift.X = state.Shift.Y = 0;
        state.Source = null;
    }

    private static void Stop(State state)
    {
        if (state.Outstanding && state.Frame is not null) state.Source?.CancelFrame(state.Frame);
        state.Outstanding = false;
        state.X.Clear();
        state.Y.Clear();
        Release(state);
    }
}

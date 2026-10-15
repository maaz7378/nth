// PORT-ONLY ED-NEW-jelly-pill-2 (row #87; Maaz, 2026-10-15, overnight brief item 4; plasma-ui's jelly selector, MIT, ideas only): the Settings
// navigation's selected highlight is a drop of liquid glass over the list, the third lens (on the list, on the sheet's glass, on the window's). Press
// the selected row and drag: the drop follows the pointer (captured, so a drag past the list never leaves it stuck), stretching along its travel and
// squashing across it with its speed; let go and it springs to the nearest row with a little overshoot and wobble, and that page opens. A click on
// another row, or the arrow keys, send it there on the same spring. Under Reduce Motion it moves at once and never stretches. It runs on the shared
// frame clock (CzMotion) and only while it moves.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Marrow.Core.Port;

namespace Marrow.UI.Port.Settings;

/// <summary>A list whose selected row is marked by a draggable liquid glass drop.</summary>
public sealed class JellyNav : Panel
{
    private readonly Control list;
    private readonly Func<IReadOnlyList<Control>> rows;
    private readonly Func<Control?> selected;
    private readonly Action<Control> pick;
    private readonly Border drop;
    private readonly ScaleTransform squash = new();
    private readonly CzNumber top, height, jelly;
    private bool placed;
    private Point? grab;
    private double grabTop;
    private (double Y, ContinuousInstant At)? lastMove;
    private (double Top, ContinuousInstant At)? lastFrame;

    /// <summary>The spring the drop travels and lands on: a little overshoot and a wobble that dies in about half a second.</summary>
    public static Animation Land => Animation.Spring(response: 0.38, dampingFraction: 0.58);

    /// <summary>The spring the stretch relaxes on.</summary>
    public static Animation Relax => Animation.Spring(response: 0.32, dampingFraction: 0.45);

    /// <param name="list">The rows' panel, drawn under the drop.</param>
    /// <param name="rows">The rows in order.</param>
    /// <param name="selected">The selected row, if it is listed.</param>
    /// <param name="pick">Picks a row (as its click would).</param>
    /// <param name="corner">The drop's corner (pt).</param>
    public JellyNav(Control list, Func<IReadOnlyList<Control>> rows, Func<Control?> selected, Action<Control> pick, double corner = 9)
    {
        this.list = list;
        this.rows = rows;
        this.selected = selected;
        this.pick = pick;
        drop = new Border
        {
            Background = Brushes.Transparent, CornerRadius = new CornerRadius(corner), IsHitTestVisible = false,
            RenderTransform = squash, RenderTransformOrigin = RelativePoint.Center, Name = "SettingsNavDrop",
        };
        SwiftGlass.SetEffect(drop, GlassEffect.Regular(GlassShape.RoundedRectangle(corner, RoundedCornerStyle.Continuous)).OnSurface(GlassSurface.Inner));
        SwiftGlass.SetLayer(drop, 3); // ED-NEW-glass-layers-1: on the list, on the sheet, on the window
        Children.Add(list);
        Children.Add(drop); // over the rows: it lenses their words
        var motion = new CzMotion(this);
        top = new CzNumber(motion, 0, _ => Moved());
        height = new CzNumber(motion, 0, _ => Moved());
        jelly = new CzNumber(motion, 0, _ => Moved());
        AddHandler(PointerPressedEvent, Pressed, Avalonia.Interactivity.RoutingStrategies.Tunnel, handledEventsToo: true);
        AddHandler(KeyDownEvent, KeyPressed, Avalonia.Interactivity.RoutingStrategies.Tunnel);
    }

    /// <summary>The drop's rectangle now (before its stretch).</summary>
    public Rect Drop => new(list.Margin.Left, top.Value, Math.Max(0, Bounds.Width - list.Margin.Left - list.Margin.Right), height.Value);

    /// <summary>The drop control (its glass).</summary>
    public Control DropControl => drop;

    /// <summary>How stretched the drop is now (0 at rest).</summary>
    public double Jelly => jelly.Value;

    /// <summary>Whether the drop is being dragged.</summary>
    public bool Dragging => grab is not null;

    /// <summary>The drop's scale at a stretch <paramref name="j"/>: taller along the travel, narrower across it, its area kept.</summary>
    public static (double X, double Y) Stretch(double j) => (1 / (1 + 0.24 * j), 1 + 0.24 * j);

    /// <summary>The stretch a speed (pt per second) asks for.</summary>
    public static double StretchFor(double speed) => Math.Min(1.3, Math.Abs(speed) / 1100);

    private bool Still => EnvironmentValues.GetAccessibilityReduceMotion(this);

    /// <summary>The selection changed (a click, a key, a search): the drop goes to the selected row on the landing spring.</summary>
    public void Sync()
    {
        InvalidateArrange();
        if (!Dragging) Place(animate: true);
    }

    private void Moved()
    {
        // the drop's own speed while it springs: its stretch follows it (the overshoot's wobble included)
        var now = SwiftClock.Now;
        if (!Dragging && lastFrame is { } f && !Still)
        {
            var dt = (now - f.At).TotalSeconds;
            if (dt > 0.001 && dt < 0.1)
            {
                var want = StretchFor((top.Value - f.Top) / dt);
                if (want > jelly.Value + 0.01) { jelly.Jump(want); jelly.To(0, Relax); }
            }
        }
        lastFrame = (top.Value, now);
        (squash.ScaleX, squash.ScaleY) = Stretch(Still ? 0 : Math.Clamp(jelly.Value, -0.3, 1.4));
        InvalidateArrange();
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        list.Arrange(new Rect(finalSize));
        if (!Dragging) Place(animate: placed);
        var target = selected();
        drop.IsVisible = target is not null && height.Value > 0;
        drop.Arrange(Drop);
        return finalSize;
    }

    private void Place(bool animate)
    {
        if (selected() is not { } row || row.Bounds.Height <= 0 || row.TranslatePoint(default, this) is not { } at) return;
        if (!placed || !animate || Still) { top.Jump(at.Y); height.Jump(row.Bounds.Height); placed = true; return; }
        if (top.Target == at.Y && height.Target == row.Bounds.Height) return;
        top.To(at.Y, Land);
        height.To(row.Bounds.Height, Land);
    }

    // tunnelled, so a press on the selected row grabs the drop before the row's button sees it
    private void Pressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed || selected() is null) return;
        var at = e.GetPosition(this);
        if (!Drop.Inflate(new Thickness(0, 2)).Contains(at)) return;
        grab = at;
        grabTop = top.Value;
        lastMove = (at.Y, SwiftClock.Now);
        e.Pointer.Capture(this);
        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (grab is not { } from) return;
        var y = e.GetPosition(this).Y;
        var all = rows();
        var first = all.Count > 0 && all[0].TranslatePoint(default, this) is { } a ? a.Y : 0;
        var last = all.Count > 0 && all[^1].TranslatePoint(default, this) is { } b ? b.Y : 0;
        top.Jump(Math.Clamp(grabTop + y - from.Y, first - 6, last + 6));
        // the height follows the row under the drop's middle
        if (RowAt(top.Value + height.Value / 2) is { } under) height.To(under.Bounds.Height, CzMotion.Ez(.2));
        var now = SwiftClock.Now;
        if (!Still && lastMove is { } m)
        {
            var dt = (now - m.At).TotalSeconds;
            if (dt > 0.001)
            {
                jelly.Jump(Math.Max(jelly.Value * 0.6, StretchFor((y - m.Y) / dt)));
                jelly.To(0, Relax);
            }
        }
        lastMove = (y, now);
        e.Handled = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (grab is null) return;
        LetGo(e.Pointer);
        e.Handled = true;
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        base.OnPointerCaptureLost(e);
        if (grab is not null) LetGo(null);
    }

    private Control? RowAt(double y) => rows().Where(r => r.Bounds.Height > 0).MinBy(r =>
        r.TranslatePoint(default, this) is { } at ? Math.Abs(at.Y + r.Bounds.Height / 2 - y) : double.MaxValue);

    // Lets go: the drop lands on the row nearest its middle, which opens.
    private void LetGo(IPointer? pointer)
    {
        grab = null;
        lastMove = null;
        pointer?.Capture(null);
        var under = RowAt(top.Value + height.Value / 2);
        if (!Still) jelly.To(0, Relax);
        if (under is not null && !ReferenceEquals(under, selected())) pick(under);
        Place(animate: true);
    }

    private void KeyPressed(object? sender, KeyEventArgs e)
    {
        if (e.Key is not (Key.Up or Key.Down) || e.KeyModifiers != KeyModifiers.None) return;
        var all = rows();
        var at = selected() is { } s ? IndexOf(all, s) : -1;
        var next = at < 0 ? 0 : Math.Clamp(at + (e.Key == Key.Down ? 1 : -1), 0, all.Count - 1);
        if (all.Count == 0 || next == at) return;
        pick(all[next]);
        e.Handled = true;
    }

    private static int IndexOf(IReadOnlyList<Control> all, Control row)
    {
        for (var i = 0; i < all.Count; i++) if (ReferenceEquals(all[i], row)) return i;
        return -1;
    }
}

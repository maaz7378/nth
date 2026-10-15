// PORT-ONLY CZ-1: measured moving pill, DESIGN gen.py:228-240; cz.js:420-436.
// ED-NEW-jelly-pill-1 (row #87 phase 3; Maaz, 2026-10-15; plasma-ui's jelly selector, MIT, ideas only): the pill is a drop of liquid glass, a
// third lens over the Settings sheet's glass and the card's: it stretches along its travel and settles with a wobble when the pick moves, and it
// can be dragged along the track (it lands on the segment under its middle, which picks it). Keyboard picks move it the same way; under Reduce
// Motion it moves at once and never stretches. Every segmented selector of every Settings page is this control.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Marrow.Core.Port;
namespace Marrow.UI.Port.Settings;

public sealed class CzSegmentedControl : CzRadioGroup
{
    private readonly CzNumber left, width, jelly;
    private bool placed;
    private readonly CzSegmentTrack track;
    private readonly Border drop;
    private readonly ScaleTransform squash = new();
    private Point? grab;
    private double grabLeft;
    public Rect Pill => new(left.Value, 2, width.Value, Small ? 22 : 24);
    public bool Small { get; }

    /// <summary>ED-NEW-jelly-pill-1: the liquid glass drop that is the pill.</summary>
    public Control Drop => drop;

    /// <summary>ED-NEW-jelly-pill-1: how stretched the drop is now (0 at rest, 1 the most).</summary>
    public double Jelly => jelly.Value;

    /// <summary>ED-NEW-jelly-pill-1: the drop is being dragged.</summary>
    public bool Dragging => grab is not null;

    public CzSegmentedControl(string name, IReadOnlyList<(string Id, string Text)> options, string selected, bool small = false) : base(name, options, selected)
    {
        Small = small; Height = small ? 26 : 28; Margin = new Thickness(0);
        track = new CzSegmentTrack(this); Children.Insert(0, track);
        drop = new Border { Background = Brushes.Transparent, CornerRadius = new CornerRadius(small ? 11 : 12), IsHitTestVisible = false, RenderTransform = squash, RenderTransformOrigin = RelativePoint.Center };
        SwiftGlass.SetEffect(drop, GlassEffect.Regular(GlassShape.Capsule(RoundedCornerStyle.Continuous)).OnSurface(GlassSurface.Inner));
        Children.Insert(1, drop);
        var motion = new CzMotion(this);
        left = new CzNumber(motion, 2, _ => Moved()); width = new CzNumber(motion, 0, _ => Moved());
        jelly = new CzNumber(motion, 0, _ => Moved());
        foreach (var button in Buttons)
        {
            button.Height = small ? 22 : 24; button.Padding = new Thickness(small ? 9 : 11, 0); button.CornerRadius = new CornerRadius(small ? 11 : 12);
            if (button.Child is Avalonia.Controls.TextBlock text) text.FontSize = small ? 11.5 : 12;
        }
        SelectionChanged += _ => Place();
        AddHandler(PointerPressedEvent, Pressed, Avalonia.Interactivity.RoutingStrategies.Tunnel, handledEventsToo: true);
    }

    /// <summary>The drop's stretch at <paramref name="j"/>: wider along the travel, flatter across it (the volume kept).</summary>
    public static (double X, double Y) Stretch(double j) => (1 + 0.16 * j, 1 / (1 + 0.16 * j));

    private void Moved()
    {
        track.InvalidateVisual();
        (squash.ScaleX, squash.ScaleY) = Stretch(Math.Clamp(jelly.Value, 0, 1.4));
        InvalidateArrange();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var size = base.MeasureOverride(new Size(Math.Max(0, availableSize.Width - 4), availableSize.Height));
        return new Size(size.Width + 4, Small ? 26 : 28);
    }
    protected override Size ArrangeOverride(Size finalSize)
    {
        track.Arrange(new Rect(finalSize));
        double x = 2;
        foreach (var button in Buttons) { button.Arrange(new Rect(x, 2, button.DesiredSize.Width, Small ? 22 : 24)); x += button.DesiredSize.Width; }
        if (!Dragging) Place(arranging: true);
        drop.IsVisible = SelectedId.Length > 0 && width.Value > 0;
        drop.Arrange(Pill);
        return finalSize;
    }
    private void Place(bool arranging = false)
    {
        var button = Buttons.FirstOrDefault(b => b.Picked);
        if (button is null || button.Bounds.Width == 0) return;
        if (!placed) { left.Jump(button.Bounds.X); width.Jump(button.Bounds.Width); placed = true; return; }
        if (left.Target == button.Bounds.X && width.Target == button.Bounds.Width) return;
        var travel = Math.Abs(button.Bounds.X - left.Value);
        left.To(button.Bounds.X, CzMotion.Ez(.3)); width.To(button.Bounds.Width, CzMotion.Ez(.3));
        if (!arranging && travel > 1 && !EnvironmentValues.GetAccessibilityReduceMotion(this))
        {
            jelly.Jump(Math.Min(1, travel / 60));
            jelly.To(0, Animation.Spring(response: 0.42, dampingFraction: 0.45));
        }
    }

    // tunnelled, so a press on the segment button under the drop still grabs the drop
    private void Pressed(object? sender, PointerPressedEventArgs e)
    {
        if (Dimmed || SelectedId.Length == 0 || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;
        var at = e.GetPosition(this);
        if (!Pill.Inflate(2).Contains(at)) return;
        grab = at; grabLeft = left.Value;
        e.Pointer.Capture(this);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (grab is not { } from) return;
        var dx = e.GetPosition(this).X - from.X;
        var last = Buttons[^1].Bounds;
        left.Jump(Math.Clamp(grabLeft + dx, 2, Math.Max(2, last.Right - width.Value)));
        if (!EnvironmentValues.GetAccessibilityReduceMotion(this)) jelly.Jump(Math.Min(1, Math.Abs(dx) / 80));
        // the width follows the segment under the drop's middle
        var middle = left.Value + width.Value / 2;
        if (Buttons.FirstOrDefault(b => middle >= b.Bounds.Left && middle < b.Bounds.Right) is { } under) width.To(under.Bounds.Width, CzMotion.Ez(.2));
        e.Handled = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (grab is null) return;
        Drop_(e.Pointer);
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        base.OnPointerCaptureLost(e);
        if (grab is not null) Drop_(null);
    }

    // Lets go of the drop: it lands on the segment under its middle (picking it) and settles.
    private void Drop_(IPointer? pointer)
    {
        grab = null;
        pointer?.Capture(null);
        var middle = left.Value + width.Value / 2;
        var under = Buttons.MinBy(b => Math.Abs(b.Bounds.Center.X - middle))!;
        if (!EnvironmentValues.GetAccessibilityReduceMotion(this)) jelly.To(0, Animation.Spring(response: 0.42, dampingFraction: 0.45));
        if (under.Id != SelectedId) Select(under.Id, focus: true);
        else Place();
    }
}

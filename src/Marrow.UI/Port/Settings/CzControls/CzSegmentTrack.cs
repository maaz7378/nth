// PORT-ONLY CZ-1: separate track visual below the measured segmented buttons.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
namespace Marrow.UI.Port.Settings;

internal sealed class CzSegmentTrack : Control
{
    private readonly CzSegmentedControl owner;
    public CzSegmentTrack(CzSegmentedControl owner)
    {
        this.owner = owner;
        ActualThemeVariantChanged += (_, _) => InvalidateVisual();
    }
    public override void Render(DrawingContext context)
    {
        var radius = owner.Small ? 13 : 14;
        context.DrawRectangle(CzTheme.Brush(CzTheme.Token(owner, "cap")), null, new Rect(Bounds.Size), radius, radius);
        var r = owner.Small ? 11 : 12;
        // ED-NEW-jelly-pill-1: under liquid glass the pill is the glass drop (CzSegmentedControl.Drop), so only its shadow is drawn here; the
        // classic glass keeps the flat fill
        var fill = GlassLooks.Tuning.Drawn == Marrow.Core.Port.Settings.GlassEngine.Canvas || GlassLooks.Tuning.ReduceTransparency == true
            ? CzTheme.Brush(CzTheme.Token(owner, "pill")) : Brushes.Transparent;
        context.DrawRectangle(fill, null, new RoundedRect(owner.Pill, r), BoxShadows.Parse("0 1 2 0 #2E000000"));
        context.DrawRectangle(null, new Pen(CzTheme.Brush(CzTheme.Token(owner, "rim")), .5), owner.Pill.Deflate(.25), r - .25, r - .25);
    }
}

using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NotEngine.Editor.CustomControls;

/// <summary>
/// Internal nano TextBox
/// </summary>
public class NanoTextBox : TextBox
{
    static NanoTextBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(NanoTextBox), new FrameworkPropertyMetadata(typeof(TextBox)));
    }
    private Point lastRenderPoint = new Point(0, 0);

    /// <summary>
    /// Identifies the SelectionForeground dependency property.
    /// </summary>
    public static readonly DependencyProperty SelectionForegroundProperty =
        DependencyProperty.Register(
            "SelectionForeground",
            typeof(Brush),
            typeof(NanoTextBox),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Identifies the BaseBackground dependency property.
    /// </summary>
    public static DependencyProperty BaseBackgroundProperty =
        DependencyProperty.Register(
            "BaseBackground",
            typeof(Brush),
            typeof(NanoTextBox),
            new FrameworkPropertyMetadata(Panel.BackgroundProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.None));

    /// <summary>
    /// Identifies the BaseForeground dependency property.
    /// </summary>
    public static DependencyProperty BaseForegroundProperty =
        DependencyProperty.Register(
            "BaseForeground",
            typeof(Brush),
            typeof(NanoTextBox),
            new FrameworkPropertyMetadata(SystemColors.ControlTextBrush, FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>
    /// Identifies the BaseSelectionBrush dependency property.
    /// </summary>
    public static readonly DependencyProperty BaseSelectionBrushProperty =
        DependencyProperty.Register(
            "BaseSelectionBrush",
            typeof(Brush),
            typeof(NanoTextBox),
            new FrameworkPropertyMetadata(new SolidColorBrush(SystemColors.HighlightColor), FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Ctor
    /// </summary>
    public NanoTextBox()
    {
        Background = new SolidColorBrush(Colors.Transparent);
        Foreground = new SolidColorBrush(Colors.Transparent);
        SelectionBrush = new SolidColorBrush(Colors.Transparent);
    }

    /// <summary>
    /// Gets or sets the brush that highlights selected text.
    /// </summary>
    /// 
    /// <returns>
    /// The brush that highlights selected text.
    /// </returns>
    public Brush SelectionForeground
    {
        get { return (Brush)GetValue(SelectionForegroundProperty); }
        set { SetValue(SelectionForegroundProperty, value); }
    }


    /// <summary>
    /// Gets or sets a brush that describes the background of a control.
    /// </summary>
    /// 
    /// <returns>
    /// The brush that is used to fill the background of the control. The default is <see cref="P:System.Windows.Media.Brushes.Transparent"/>.
    /// </returns>
    [Bindable(true)]
    public Brush BaseBackground
    {
        get { return (Brush)GetValue(BaseBackgroundProperty); }
        set { SetValue(BaseBackgroundProperty, value); }
    }

    /// <summary>
    /// Gets or sets a brush that describes the foreground color.
    /// </summary>
    /// 
    /// <returns>
    /// The brush that paints the foreground of the control. The default value is the system dialog font color.
    /// </returns>
    [Bindable(true)]
    public Brush BaseForeground
    {
        get { return (Brush)GetValue(BaseForegroundProperty); }
        set { SetValue(BaseForegroundProperty, value); }
    }

    /// <summary>
    /// Gets or sets the brush that highlights selected text.
    /// </summary>
    /// 
    /// <returns>
    /// The brush that highlights selected text.
    /// </returns>
    public Brush BaseSelectionBrush
    {
        get { return (Brush)GetValue(BaseSelectionBrushProperty); }
        set { SetValue(BaseSelectionBrushProperty, value); }
    }

    protected override void OnTextInput(TextCompositionEventArgs e)
    {
        base.OnTextInput(e);
        InvalidateVisual();
    }

    protected override void OnPreviewKeyUp(KeyEventArgs e)
    {
        base.OnPreviewKeyUp(e);
        InvalidateVisual();
    }

    protected override void OnSelectionChanged(RoutedEventArgs e)
    {
        base.OnSelectionChanged(e);
        InvalidateVisual();
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);
        InvalidateVisual();
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        drawingContext.PushClip(new RectangleGeometry(new Rect(0, 0, ActualWidth, ActualHeight)));
        drawingContext.DrawRectangle(BaseBackground, null, new Rect(0, 0, ActualWidth, ActualHeight));

        if (string.IsNullOrEmpty(Text))
            return;

        var firstChar = 0;
        var cRect = GetRectFromCharacterIndex(firstChar);
        var renderPoint = new Point(0, 0);
        if (!double.IsInfinity(cRect.Top))
        {
            renderPoint.X = cRect.Left;
            renderPoint.Y = cRect.Top;
        }
        else
        {
            renderPoint.X = HorizontalOffset > 0
                ? -HorizontalOffset
                : lastRenderPoint.X;

            renderPoint.Y = VerticalOffset > 0
                ? -VerticalOffset
                : lastRenderPoint.Y;
        }

        var formattedText = new FormattedText(
            Text,
            CultureInfo.CurrentUICulture,
            FlowDirection.LeftToRight,
            new Typeface(FontFamily.Source),
            FontSize,
            BaseForeground);

        if (AcceptsReturn || TextWrapping != TextWrapping.NoWrap)
        {
            formattedText.MaxTextWidth = ActualWidth - Margin.Left - Margin.Right;
            formattedText.Trimming = TextTrimming.None;
        }

        if (IsFocused)
        {
            var selectionForeground = SelectionForeground;
            if (selectionForeground != null)
            {
                if(SelectionLength!=0)
                    formattedText.SetForegroundBrush(selectionForeground, SelectionStart, SelectionLength);
            }

            if (SelectionLength != 0)
            {
                var highlightGeometry = formattedText.BuildHighlightGeometry(new Point(0, 0), SelectionStart,
                    SelectionLength);
                if (highlightGeometry != null)
                {
                    highlightGeometry.Transform = new TranslateTransform(renderPoint.X, renderPoint.Y);
                    drawingContext.DrawGeometry(BaseSelectionBrush, null, highlightGeometry);
                }
            }
               
        }

        drawingContext.DrawText(formattedText, renderPoint);
        lastRenderPoint = renderPoint;
    }
}
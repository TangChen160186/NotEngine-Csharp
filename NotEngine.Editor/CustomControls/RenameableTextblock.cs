using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NotEngine.Editor.CustomControls;

public class RenameableTextblock : Control
{
    static RenameableTextblock()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(RenameableTextblock), new FrameworkPropertyMetadata(typeof(RenameableTextblock)));
    }

    // Text DependencyProperty
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(RenameableTextblock), new PropertyMetadata(string.Empty));

    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    // CanEditing DependencyProperty
    public static readonly DependencyProperty CanEditingProperty =
        DependencyProperty.Register(nameof(CanEditing), typeof(bool), typeof(RenameableTextblock),
            new PropertyMetadata(true));

    public bool CanEditing
    {
        get { return (bool)GetValue(CanEditingProperty); }
        set { SetValue(CanEditingProperty, value); }
    }


    // IsEditing DependencyProperty
    public static readonly DependencyProperty IsEditingProperty =
        DependencyProperty.Register(nameof(IsEditing), typeof(bool), typeof(RenameableTextblock),
            new PropertyMetadata(false, OnIsEditingChanged));

    public bool IsEditing
    {
        get { return (bool)GetValue(IsEditingProperty); }
        set { SetValue(IsEditingProperty, value); }
    }

    private static void OnIsEditingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (RenameableTextblock)d;
        var newValue = (bool)e.NewValue;
      
        if (newValue)
        {
            // 开始编辑，选择 TextBox 并选中所有文本
            control.FocusAndSelectTextBox();
        }
    }

    private TextBox _textBox;

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _textBox = GetTemplateChild("PART_TextBox") as TextBox;

        // 添加事件以支持从 TextBlock 切换到编辑模式
        var textBlock = GetTemplateChild("PART_TextBlock") as TextBlock;
        if (_textBox != null)
        {
            _textBox.LostFocus += (s, e) => IsEditing = false;
            _textBox.KeyDown += (s, e) =>
            {
                if (e.Key == Key.Enter || e.Key == Key.Escape)
                {
                    IsEditing = false;
                    MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }
            };

            _textBox.LostFocus += TextBoxLostFocus;
        }
    }

    private void TextBoxLostFocus(object sender, RoutedEventArgs e)
    {
        IsEditing = false;
    }

    private void FocusAndSelectTextBox()
    {
        if (_textBox != null)
        {
            _textBox.Dispatcher.BeginInvoke(new Action(() =>
            {

                _textBox.Focus();    // 聚焦 TextBox
                
                _textBox.SelectAll(); // 选中所有文本
            }), System.Windows.Threading.DispatcherPriority.Background);
        }
    }
}
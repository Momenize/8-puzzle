using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MsgBox;

partial class MessageBox : Window
{
    public enum MessageBoxButtons
    {
        Ok
    }

    public enum MessageBoxResult
    {
        Ok
    }

    public MessageBox()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public static Task<MessageBoxResult> Show(Window parent, string text, string title, MessageBoxButtons buttons)
    {
        var msgbox = new MessageBox()
        {
            Title = title
        };
        msgbox.FindControl<TextBlock>("Text").Text = text;
        var buttonPanel = msgbox.FindControl<StackPanel>("Buttons");

        var res = MessageBoxResult.Ok;

        void AddButton(string caption, MessageBoxResult r, bool def = false)
        {
            var btn = new Button {Content = caption};
            btn.Click += (_, __) => { 
                res = r;
                msgbox.Close();
            };
            buttonPanel.Children.Add(btn);
            if (def)
                res = r;
        }

        if (buttons == MessageBoxButtons.Ok)
            AddButton("Ok", MessageBoxResult.Ok, true);


        var tcs = new TaskCompletionSource<MessageBoxResult>();
        msgbox.Closed += delegate { tcs.TrySetResult(res); };
        if (parent != null)
            msgbox.ShowDialog(parent);
        else msgbox.Show();
        return tcs.Task;
    }


}
using System.Collections.Generic;

namespace Keyboard;

public delegate void ActionDelegate();

class KeyboardShortcut
{
    public List<char> Keys { get; }
    public ActionDelegate? Action { get; private set; }

    public KeyboardShortcut()
    {
        Keys = new List<char>();
    }

    public KeyboardShortcut(List<char> keys, ActionDelegate action)
    {
        Keys = keys;
        Action += action;
    }

    public void AddKey(char c)
    {
        Keys.Add(c);
    }

    public void AddAction(ActionDelegate action)
    {
        Action += action;
    }
}
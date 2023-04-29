using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Keyboard;
class KeyboardHandler : IDisposable
{
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_SYSKEYDOWN = 0x0104;
    private DateTime _baseTime = DateTime.UtcNow.AddMilliseconds(-2*MaxTimeDelta);
    private const int MaxTimeDelta = 250;
    private IntPtr _hookId = IntPtr.Zero;
    private LowLevelKeyboardProc? _hookProc;
    private bool _disposed = false;

    private readonly List<KeyboardShortcut> _keyboardShortcuts = new List<KeyboardShortcut>();
    private const int _keyCount = 0xff;
    private int[] _lastPressed = new int[_keyCount];

    public KeyboardHandler()
    {
        _hookId = SetHook(HookCallback);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed ) return;
        if(disposing) {
            UnhookWindowsHookEx(_hookId);
        }
        _disposed = true;
    }

    public void AddShortcut(KeyboardShortcut shortcut)
    {
        _keyboardShortcuts.Add(shortcut);
    }

    private void OnKeyPress(char pressedKey)
    {
        if (pressedKey >= _keyCount || pressedKey < 0)
            return;
        var timeSpan = (int) (DateTime.UtcNow - _baseTime).TotalMilliseconds;
        _lastPressed[pressedKey] = timeSpan;
        foreach (var shortcut in _keyboardShortcuts)
        {
            bool actionDetected = true;
            foreach (var key in shortcut.Keys)
            {
                if (timeSpan - _lastPressed[key] > MaxTimeDelta)
                {
                    actionDetected = false;
                    break;
                }
            }
            if (actionDetected)
                shortcut.Action!();
        }
    }

    private IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
        {
            _hookProc = proc; // save proc to handler object so that the garbage collector doesn't delete it
            return SetWindowsHookEx(WH_KEYBOARD_LL, _hookProc, GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr) WM_SYSKEYDOWN)
        {
            var vkCode = Marshal.ReadInt32(lParam);
            OnKeyPress((char) vkCode);
        }
        return CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook,
        LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
        IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);
}
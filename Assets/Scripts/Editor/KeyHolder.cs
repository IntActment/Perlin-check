using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEditor;
using UnityEngine;
public class KeyHolder
{
    private Dictionary<KeyCode, bool> m_pressedKeys = new Dictionary<KeyCode, bool>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsKeyPressed(KeyCode code)
    {
        if (m_pressedKeys.TryGetValue(code, out bool val))
        {
            return val;
        }

        return false;
    }

    public bool IsShiftPressed => IsKeyPressed(KeyCode.LeftShift) || IsKeyPressed(KeyCode.RightShift);

    public bool IsCtrlPressed => IsKeyPressed(KeyCode.LeftControl) || IsKeyPressed(KeyCode.RightControl);

    public bool IsAltPressed => IsKeyPressed(KeyCode.LeftAlt) || IsKeyPressed(KeyCode.RightAlt);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool UpdateState()
    {
        var ev = Event.current;

        if ((null != ev)
                && (ev.isKey)
                && (ev.keyCode != KeyCode.None))
        {
            bool pressed;

            if (ev.type == EventType.KeyDown)
            {
                m_pressedKeys.TryGetValue(ev.keyCode, out pressed);

                if (false == pressed)
                {
                    m_pressedKeys[ev.keyCode] = true;

                    //Debug.Log($"Down: {ev.keyCode}");
                }
            }
            else if (ev.type == EventType.KeyUp)
            {
                m_pressedKeys.TryGetValue(ev.keyCode, out pressed);

                if (true == pressed)
                {
                    m_pressedKeys[ev.keyCode] = false;

                    //Debug.Log($"Up: {ev.keyCode}");
                }
            }
        }

        return true;
    }
}


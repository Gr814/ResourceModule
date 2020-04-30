using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class RefKeeper: MonoBehaviour
{
    private Action m_break_call_back;

    void OnDestroy()
    {
        m_break_call_back?.Invoke();
    }

    internal void SetBreakCallBack(Action breakLink)
    {
        m_break_call_back = breakLink;
    }
}

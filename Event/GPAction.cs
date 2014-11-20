using UnityEngine;
using System.Collections;

public abstract class GPAction
{
    private bool _hasEnded;

    public bool HasEnded
    {
        get { return _hasEnded; }
        set { _hasEnded = value; }
    }

    public virtual void OnTrigger()
    {
    }

    public virtual void OnUpdate()
    {
    }

    public virtual void OnDestroy()
    {
    }
}

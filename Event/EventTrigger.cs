using UnityEngine;
using System.Collections;

public class EventTrigger : MonoBehaviour {

    public string _eventName;

    public void Trigger()
    {
        if(_eventName == "" ||_eventName == null)
        {
            Debug.LogError("EVent name can't be null, an error must have occured!");
            return;
        }

        EventManager.Instance.PlayEvent(_eventName);
    }
}

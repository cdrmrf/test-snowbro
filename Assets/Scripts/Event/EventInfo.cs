using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.Events;


public class EventInfo<T> : IEvent {
    
    public UnityAction<T> actions;

    public EventInfo(UnityAction<T> action) {
        actions += action;
    }

}
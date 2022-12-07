using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.Events;

public class EventInfoNoParam : IEvent {
                
    public UnityAction actions;

    public EventInfoNoParam(UnityAction action) {
        actions += action;
    }

}
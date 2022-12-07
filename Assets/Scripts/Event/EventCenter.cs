using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class EventCenter : Singleton<EventCenter> {

    private Dictionary<EventEnum, IEvent> _eventDic = new Dictionary<EventEnum, IEvent>();
    private Dictionary<EventEnum, IEvent> _eventDicNoParam = new Dictionary<EventEnum, IEvent>();

    // 添加带参数事件的监听
    public void AddEventListener<T>(EventEnum name, UnityAction<T> action) {
        if (_eventDic.ContainsKey(name)) {
            // 旧事件
            (_eventDic[name] as EventInfo<T>).actions += action;
        } else {
            // 新事件
            _eventDic.Add(name, new EventInfo<T>(action));            
        }
    }

    // 添加无参数事件的监听
    public void AddEventListener2(EventEnum name, UnityAction action) {
        if (_eventDicNoParam.ContainsKey(name)) {
            // 旧事件
            (_eventDicNoParam[name] as EventInfoNoParam).actions += action;
        } else {
            // 新事件
            _eventDicNoParam.Add(name, new EventInfoNoParam(action));            
        }
    }

    // 分发带参数的事件
    public void EventTrigger<T>(EventEnum name, T info) {
        if (_eventDic.ContainsKey(name)) {
            if ((_eventDic[name] as EventInfo<T>).actions != null) {
                (_eventDic[name] as EventInfo<T>).actions.Invoke(info);
            }
        }
    }
    // 分发无参数的事件
    public void EventTrigger2(EventEnum name) {
        if (_eventDicNoParam.ContainsKey(name)) {
            if ((_eventDicNoParam[name] as EventInfoNoParam).actions != null) {
                (_eventDicNoParam[name] as EventInfoNoParam).actions.Invoke();
            }   
        }
    }

    // 清空事件监听
    // 主要用于场景切换时防止内存泄漏
    public void Clear() {
        _eventDic.Clear();
    }

    // 移除带参数事件的监听
    public void RemoveEventListener<T>(EventEnum name, UnityAction<T> action) {
        if (_eventDic.ContainsKey(name)) {
            (_eventDic[name] as EventInfo<T>).actions -= action;
        }
    }

    // 移除带参数事件的监听
    public void RemoveEventListener2(EventEnum name, UnityAction action) {
        if (_eventDic.ContainsKey(name)) {
            (_eventDic[name] as EventInfoNoParam).actions -= action;
        }
    }
    
}
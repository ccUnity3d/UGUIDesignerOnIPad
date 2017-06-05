using System;
using System.Collections.Generic;
using foundation;

public class TickManager
{
    private List<Action> list = new List<Action>();
    private List<Action> fixedUpdateList = new List<Action>();
    private List<Action> gizmosList = new List<Action>();
    public TickManager()
    {

    }


    private static Stack<List<Action>> actionListPool = new Stack<List<Action>>();

    private static List<Action> getActionList()
    {
        if (actionListPool.Count > 0)
        {
            List<Action> temp = actionListPool.Pop();
            temp.Clear();

            return temp;
        }

        return new List<Action>();
    }

    private static void recycle(List<Action> node)
    {
        if (actionListPool.Count < 300)
        {
            actionListPool.Push(node);
        }
    }


    private static TickManager ins;

    public static TickManager getInstance()
    {
        if (ins == null)
        {
            ins = new TickManager();
        }
        return ins;
    }

    public static bool add(Action handle, bool Fixed = false)
    {
        return getInstance()._add(handle, Fixed);
    }

    public static bool add(ITickable ticker, bool Fixed = false)
    {
        return getInstance()._add(ticker.tick, Fixed);
    }

    public static bool addGizmos(Action action)
    {
        return getInstance()._addGizmos(action);
    }

    public static bool remove(Action handle)
    {
        return getInstance()._remove(handle);
    }

    public static bool remove(ITickable ticker)
    {
        return getInstance()._remove(ticker.tick);
    }

    public static bool removeGizmos(Action action)
    {
        return getInstance()._removeGizmos(action);
    }

    private bool _add(Action handle, bool isFixed = false)
    {
        if (handle == null)
        {
            return false;
        }

        List<Action> temp;

        if (isFixed == false)
        {
            temp = list;
        }
        else
        {
            temp = fixedUpdateList;
        }

        if (temp.Contains(handle) == true)
        {
            return false;
        }
        temp.Add(handle);

        return true;
    }


    private bool _remove(Action handle)
    {
        if (fixedUpdateList.Contains(handle))
        {
            fixedUpdateList.Remove(handle);
            return true;
        }
        if (list.Contains(handle))
        {
            list.Remove(handle);
            return true;
        }

        return false;
    }

    private bool _addGizmos(Action handle)
    {
        if (handle == null || gizmosList.Contains(handle))
        {
            return false;
        }
        gizmosList.Add(handle);
        return true;
    }

    private bool _removeGizmos(Action handle)
    {
        if (handle == null || !gizmosList.Contains(handle))
        {
            return false;
        }
        gizmosList.Remove(handle);
        return true;
    }

    public void tick()
    {
        List<Action> temp = getActionList();
        int len = list.Count;

        for (int i = 0; i < len; i++)
        {
            temp.Add(list[i]);
        }

        for (int i = 0; i < len; i++)
        {
            temp[i]();
        }

        recycle(temp);
    }


    public void fixedTick()
    {
        List<Action> temp = getActionList();
        int len = fixedUpdateList.Count;
        for (int i = 0; i < len; i++)
        {
            temp.Add(fixedUpdateList[i]);
        }
        for (int i = 0; i < len; i++)
        {
            temp[i]();
        }

        recycle(temp);
    }

    public void gizmos()
    {
        for (int i = 0; i < gizmosList.Count; i++)
        {
            gizmosList[i]();
        }
    }

}



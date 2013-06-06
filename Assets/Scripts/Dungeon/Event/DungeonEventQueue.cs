using System;
using System.Collections.Generic;

public class DungeonEventQueue
{
    //We use a list instead of a queue to more easilly iterate and find events already in the queue
    private List<DungeonEvent> queue = new List<DungeonEvent>();

    public bool IsEmpty
    {
        get { return Count == 0; }
    }

    public int Count
    {
        get { return queue.Count; }
    }

    public void Clear()
    {
        queue.Clear();
    }

    public void EnqueueEvent(DungeonEvent dungeonEvent)
    {
        queue.Add(dungeonEvent);
    }

    public DungeonEvent DequeueEvent()
    {
        if (queue.Count == 0)
            return null;

        DungeonEvent dungeonEvent = queue[0];
        queue.RemoveAt(0);

        return dungeonEvent;
    }

}



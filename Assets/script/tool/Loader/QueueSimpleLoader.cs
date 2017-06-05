
using System;
using System.Collections.Generic;

public class QueueSimpleLoader : MyEventDispatcher {

    public object bringData;
    public List<QueueItem> loaderQueue = new List<QueueItem>();
    private int count = 1;
    private float temptotal;
    private MainPageUIController mainPageUIController
    {
        get {
            return MainPageUIController.Instance;
        }
    }
    public int getCount
    {
        get
        {
            return count;
        }
    }
    public QueueSimpleLoader(object data)
    {
        this.bringData = data;
        SimpleLoader.StaticEventDispatcher.addEventListener(LoadEvent.Cancel, CancelQueueLoad);
    }

    private void CancelQueueLoad(MyEvent obj)
    {
        mainPageUIController.dispatchEvent(new LoadEvent(LoadEvent.CloseLoadingPage));
        this.ClearListioner();
        loaderQueue.Clear();
    }

    public float progress
    {
        get {
            temptotal = 0;
            for (int i = 0; i < loaderQueue.Count; i++)
            {
                temptotal += loaderQueue[i].progress;
            }
            return temptotal / count;
        }
    }

    public void AddQueueItem(SimpleLoader loader)
    {
        QueueItem item = new QueueItem(loader);
        item.addEventListener(LoadEvent.Progress, Progress);
        loaderQueue.Add(item);
        count = loaderQueue.Count;
    }

    private void Progress(MyEvent obj)
    {
        this.dispatchEvent(new LoadEvent(LoadEvent.QueueProgress, progress));
        if (progress == 1)
        {
            this.dispatchEvent(new LoadEvent(LoadEvent.QueueComplete, this));
        }
    }

    //public void AddQueueItem(SimpleLoader loader)
    //{
    //    loaderQueue.Add(loader);
    //}

    public class QueueItem : MyEventDispatcher
    {
        public float progress;
        private SimpleLoader loader;
        public SimpleLoadedState state
        {
            get {
                if (loader == null) return SimpleLoadedState.None;
                return loader.state;
            }
        }

        public QueueItem(SimpleLoader loader)
        {
            this.loader = loader;
            loader.addEventListener(LoadEvent.Complete, LoadComplete);
            loader.addEventListener(LoadEvent.Progress, LoadProgress);
        }

        private void LoadProgress(MyEvent obj)
        {
            progress = (float)obj.data;
            this.dispatchEvent(new LoadEvent(LoadEvent.Progress, loader.progress));
        }

        private void LoadComplete(MyEvent obj)
        {
            loader.removeEventListener(LoadEvent.Complete, LoadComplete);
            loader.removeEventListener(LoadEvent.Progress, LoadProgress);
            progress = 1;
            this.dispatchEvent(new LoadEvent(LoadEvent.Progress, progress));
        }
    }
}

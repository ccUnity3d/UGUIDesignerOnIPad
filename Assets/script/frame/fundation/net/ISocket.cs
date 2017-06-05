using System;

namespace foundation
{
    public interface ISocket
    {
        bool connected { get; }

        void send(AbstractMessage msg);
        void send(AbstractMessage msg, int CMD, Action<AbstractMessage> handler);

        void addListener(int CMD, Action<AbstractMessage> handler);

        void removeListener(int CMD, Action<AbstractMessage> handler);

        void close();
    }
}

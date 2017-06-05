
    public abstract class AbstractMessage : IErrorMessage
    {
        public abstract void encode(Data data);

        public abstract void decode(Data data);

        public abstract AbstractMessage create();

        public abstract int getMessageType();

        public virtual int GetReturnFlag()
        {
            return 0;
        }

        public virtual string[] GetParamList()
        {
            return null;
        }
    }
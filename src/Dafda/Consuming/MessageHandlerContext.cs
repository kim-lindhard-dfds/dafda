namespace Dafda.Consuming
{
    /// <summary>
    /// Contains metadata for a message.
    /// </summary>
    public class MessageHandlerContext
    {
        /// <summary>
        /// An empty MessageHandlerContext
        /// </summary>
        public static readonly MessageHandlerContext Empty = new MessageHandlerContext();

        private readonly Metadata _metadata;

        /// <summary>
        /// Initialize an instance of the MessageHandlerContext
        /// </summary>
        public MessageHandlerContext() : this(new Metadata())
        {
        }

        internal MessageHandlerContext(Metadata metadata)
        {
            _metadata = metadata;
        }

        /// <summary>
        /// The message identifier.
        /// </summary>
        public virtual string MessageId => _metadata.MessageId;

        /// <summary>
        /// The message type.
        /// </summary>
        public virtual string MessageType => _metadata.Type;

        /// <summary>
        /// Access to message metadata values.
        /// </summary>
        /// <param name="key">A metadata name</param>
        public virtual string this[string key] => _metadata[key];
    }
}
using SharpMC.Players;

namespace SharpMC.Util
{
    public class ChatHandler
    {
        public ChatHandler()
        {
            CommandPrefix = '/';
        }

        /// <summary>
        /// Prepares the message for chat.
        /// </summary>
        /// <param name="source">The player that send the message.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public virtual string PrepareMessage(Player source, string message)
        {
            return $"<{source.Username}> {message}";
        }

        public virtual char CommandPrefix { get; }
    }
}
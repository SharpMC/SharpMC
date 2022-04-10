using Newtonsoft.Json;

namespace SharpMC.Util
{
    public static class ChatHelper
    {
        public static string EncodeChatMessage(string message)
        {
            return JsonConvert.SerializeObject(new ChatMessage(message));
        }

        private struct ChatMessage
        {
            public string text;

            public ChatMessage(string msg)
            {
                text = msg;
            }
        }
    }
}
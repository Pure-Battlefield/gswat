using System;
using core.TableStoreEntities;

namespace core.Utilities
{
    public static class ChatMessageCleaner
    {
        public static ChatMessageEntity CleanPlayerTargets(ChatMessageEntity input)
        {
            var targets = input.MessageType;
            if (!targets.Contains("squad"))
            {
                return input;
            }
            //Extract the single digit squad id after the word squad.  
            var teamId = targets.Substring(5, 1);
            //Extract the squad id (from teamId to end of string).  
            var squadId = targets.Substring(6);

            input.MessageType = String.Format("team{0}squad{1}", teamId, squadId);
            return input;
        }
    }
}
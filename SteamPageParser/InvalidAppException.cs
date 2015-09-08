using System;

namespace SteamPageParser
{
    public class InvalidAppException : Exception
    {
        public InvalidAppException(int appId)
        {
            AppId = appId;
        }
        
        public int AppId { get; }
    }
}
using System;

namespace Core
{
    public static class LimitSizeTranfer
    {
        public static int maxSize;
        public static bool isMaxSize = false;
        public static readonly object _lock = new Object();
    }
}

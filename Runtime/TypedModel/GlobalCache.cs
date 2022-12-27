using System;

namespace NodeApi.TypedModel;

public partial class GlobalCache
{
    [ThreadStatic] private static GlobalCache? s_instance;
    private JSValue?[] _entries = new JSValue?[CacheId.Count];

    private static JSValue GetValue(CacheId cacheId)
    {
        if (s_instance is null)
        {
            s_instance = new GlobalCache();
        }

        JSValue?[] entries = s_instance._entries;
        if (entries[cacheId.Index] is JSValue value && !value.Scope.IsDisposed)
        {
            return value;
        }

        value = cacheId.CreateValue();
        entries[cacheId.Index] = value;
        return value;
    }

    private partial class CacheId
    {
        public readonly int Index;
        public readonly Func<JSValue> CreateValue;

        public static int Count { get; private set; }

        private CacheId(string propertyName)
        {
            Index = Count++;
            CreateValue = () => JSValue.Global.GetProperty(propertyName);
        }
    }
}

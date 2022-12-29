using System;

namespace NodeApi.TypedModel;

// Keeps a list of pre-allocated strings
public partial class NameTable
{
    [ThreadStatic] private static NameTable? s_instance;
    private readonly JSValue?[] _entries = new JSValue?[CacheId.Count];

    private static JSValue GetString(CacheId cacheId)
    {
        s_instance ??= new NameTable();

        JSValue?[] entries = s_instance._entries;
        if (entries[cacheId.Index] is JSValue value && !value.Scope.IsDisposed)
        {
            return value;
        }

        // TODO: implement Module Cache

        value = JSNativeApi.CreateStringUtf16(cacheId.Name);
        entries[cacheId.Index] = value;
        return value;
    }

    private partial class CacheId
    {
        public readonly int Index;
        public readonly string Name;

        public static int Count { get; private set; }

        private CacheId(string name)
        {
            Index = Count++;
            Name = name;
        }
    }
}

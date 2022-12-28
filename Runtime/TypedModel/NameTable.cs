using System;

namespace NodeApi.TypedModel;

// Keeps a list of pre-allocated strings
public partial class NameTable
{
    [ThreadStatic] private static NameTable? s_instance;
    private JSValue?[] _entries = new JSValue?[CacheId.Count];

    private static JSValue GetStringName(CacheId cacheId)
    {
        if (s_instance is null)
        {
            s_instance = new NameTable();
        }

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

        private CacheId(string propertyName)
        {
            Index = Count++;
            Name = propertyName;
        }
    }
}

public partial class NameTable
{ 
    private partial class CacheId
    {
        public static readonly CacheId abs = new CacheId(nameof(abs));
    }

    public static JSValue abs => GetStringName(CacheId.abs);
}

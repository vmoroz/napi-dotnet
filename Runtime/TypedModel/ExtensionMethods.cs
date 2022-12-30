using System;
using static NodeApi.JSNativeApi.Interop;

namespace NodeApi.TypedModel;

public static class ExtensionMethods
{
    public static JSValue Call<T>(this JSValue thisValue, JSValue thisArg, params T[] args)
        where T : struct, ITypedValue<T>
    {
        Span<napi_value> argv = stackalloc napi_value[args.Length];
        for (int i = 0; i < argv.Length; ++i)
        {
            argv[i] = (napi_value)(JSValue)args[i];
        }
        return thisValue.Call((napi_value)thisArg, argv);
    }

    public static JSValue Call<T>(this JSValue thisValue, JSValue thisArg, JSValue arg0, params T[] args)
        where T : struct, ITypedValue<T>
    {
        Span<napi_value> argv = stackalloc napi_value[1 + args.Length];
        argv[0] = (napi_value)arg0;
        for (int i = 0, j = 1; j < argv.Length; ++i, ++j)
        {
            argv[j] = (napi_value)(JSValue)args[i];
        }
        return thisValue.Call((napi_value)thisArg, argv);
    }

    public static JSValue Call<T>(this JSValue thisValue, JSValue thisArg, JSValue arg0, JSValue arg1, params T[] args)
        where T : struct, ITypedValue<T>
    {
        Span<napi_value> argv = stackalloc napi_value[2 + args.Length];
        argv[0] = (napi_value)arg0;
        argv[1] = (napi_value)arg1;
        for (int i = 0, j = 2; j < argv.Length; ++i, ++j)
        {
            argv[j] = (napi_value)(JSValue)args[i];
        }
        return thisValue.Call((napi_value)thisArg, argv);
    }

    public static JSValue Call<T>(this JSValue thisValue, JSValue thisArg, JSValue arg0, JSValue arg1, JSValue arg2, params T[] args)
        where T : struct, ITypedValue<T>
    {
        Span<napi_value> argv = stackalloc napi_value[3 + args.Length];
        argv[0] = (napi_value)arg0;
        argv[1] = (napi_value)arg1;
        argv[2] = (napi_value)arg2;
        for (int i = 0, j = 3; j < argv.Length; ++i, ++j)
        {
            argv[j] = (napi_value)(JSValue)args[i];
        }
        return thisValue.Call((napi_value)thisArg, argv);
    }

    public static JSValue CallAsConstructor<T>(this JSValue thisValue, params T[] args)
    where T : struct, ITypedValue<T>
    {
        Span<napi_value> argv = stackalloc napi_value[args.Length];
        for (int i = 0; i < argv.Length; ++i)
        {
            argv[i] = (napi_value)(JSValue)args[i];
        }
        return thisValue.CallAsConstructor(argv);
    }

    public static JSValue CallAsConstructor<T>(this JSValue thisValue, JSValue arg0, params T[] args)
        where T : struct, ITypedValue<T>
    {
        Span<napi_value> argv = stackalloc napi_value[1 + args.Length];
        argv[0] = (napi_value)arg0;
        for (int i = 0, j = 1; j < argv.Length; ++i, ++j)
        {
            argv[j] = (napi_value)(JSValue)args[i];
        }
        return thisValue.CallAsConstructor(argv);
    }

    public static JSValue CallAsConstructor<T>(this JSValue thisValue, JSValue arg0, JSValue arg1, params T[] args)
        where T : struct, ITypedValue<T>
    {
        Span<napi_value> argv = stackalloc napi_value[2 + args.Length];
        argv[0] = (napi_value)arg0;
        argv[1] = (napi_value)arg1;
        for (int i = 0, j = 2; j < argv.Length; ++i, ++j)
        {
            argv[j] = (napi_value)(JSValue)args[i];
        }
        return thisValue.CallAsConstructor(argv);
    }

    public static JSValue CallAsConstructor<T>(this JSValue thisValue, JSValue arg0, JSValue arg1, JSValue arg2, params T[] args)
        where T : struct, ITypedValue<T>
    {
        Span<napi_value> argv = stackalloc napi_value[3 + args.Length];
        argv[0] = (napi_value)arg0;
        argv[1] = (napi_value)arg1;
        argv[2] = (napi_value)arg2;
        for (int i = 0, j = 3; j < argv.Length; ++i, ++j)
        {
            argv[j] = (napi_value)(JSValue)args[i];
        }
        return thisValue.CallAsConstructor(argv);
    }

    public static JSValue CallMethod<T>(this JSValue thisValue, JSValue methodName, params T[] args)
        where T : struct, ITypedValue<T>
    {
        Span<napi_value> argv = stackalloc napi_value[args.Length];
        for (int i = 0; i < argv.Length; ++i)
        {
            argv[i] = (napi_value)(JSValue)args[i];
        }
        return thisValue.CallMethod(methodName, argv);
    }


    public static JSValue CallMethod<T>(this JSValue thisValue, JSValue methodName, JSValue arg0, params T[] args)
        where T : struct, ITypedValue<T>
    {
        Span<napi_value> argv = stackalloc napi_value[1 + args.Length];
        argv[0] = (napi_value)arg0;
        for (int i = 0, j = 1; j < argv.Length; ++i, ++j)
        {
            argv[j] = (napi_value)(JSValue)args[i];
        }
        return thisValue.CallMethod(methodName, argv);
    }

    public static JSValue CallMethod<T>(this JSValue thisValue, JSValue methodName, JSValue arg0, JSValue arg1, params T[] args)
        where T : struct, ITypedValue<T>
    {
        Span<napi_value> argv = stackalloc napi_value[2 + args.Length];
        argv[0] = (napi_value)arg0;
        argv[1] = (napi_value)arg1;
        for (int i = 0, j = 2; j < argv.Length; ++i, ++j)
        {
            argv[j] = (napi_value)(JSValue)args[i];
        }
        return thisValue.CallMethod(methodName, argv);
    }

    public static JSValue CallMethod<T>(this JSValue thisValue, JSValue methodName, JSValue arg0, JSValue arg1, JSValue arg2, params T[] args)
        where T : struct, ITypedValue<T>
    {
        Span<napi_value> argv = stackalloc napi_value[3 + args.Length];
        argv[0] = (napi_value)arg0;
        argv[1] = (napi_value)arg1;
        argv[2] = (napi_value)arg2;
        for (int i = 0, j = 3; j < argv.Length; ++i, ++j)
        {
            argv[j] = (napi_value)(JSValue)args[i];
        }
        return thisValue.CallMethod(methodName, argv);
    }
}


using System.Linq;

namespace NodeApi.TypedModel;

public static class ExtensionMethods
{
    public static JSValue CallMethod(this JSValue thisValue, JSValue name)
        => thisValue.GetProperty(name).Call(thisValue);

    public static JSValue CallMethod(this JSValue thisValue, JSValue name, JSValue arg0)
        => thisValue.GetProperty(name).Call(thisValue, arg0);

    public static JSValue CallMethod(this JSValue thisValue, JSValue name, JSValue arg0, JSValue arg1)
        => thisValue.GetProperty(name).Call(thisValue, arg0, arg1);

    public static JSValue CallMethod(this JSValue thisValue, JSValue name, JSValue arg0, JSValue arg1, JSValue arg2)
        => thisValue.GetProperty(name).Call(thisValue, arg0, arg1, arg2);

    public static JSValue CallMethod(this JSValue thisValue, JSValue name, params JSValue[] args)
        => thisValue.GetProperty(name).Call(thisValue, args);

    public static JSValue CallMethod<T>(this JSValue thisValue, JSValue name, params T[] args)
        where T : struct, ITypedValue<T>
            => thisValue.GetProperty(name).Call(thisValue, args.Select(a => (JSValue)a).ToArray());

    public static JSValue CallMethod<T>(this JSValue thisValue, JSValue name, JSValue arg0, params T[] args)
        where T : struct, ITypedValue<T>
        => thisValue.GetProperty(name).Call(thisValue, arg0, args.Select(a => (JSValue)a).ToArray());

    public static JSValue CallMethod<T>(this JSValue thisValue, JSValue name, JSValue arg0, JSValue arg1, params T[] args)
        where T : struct, ITypedValue<T>
        => thisValue.GetProperty(name).Call(thisValue, arg0, arg1, args.Select(a => (JSValue)a).ToArray());

    public static JSValue CallMethod<T>(this JSValue thisValue, JSValue name, JSValue arg0, JSValue arg1, JSValue arg2, params T[] args)
        where T : struct, ITypedValue<T>
        => thisValue.GetProperty(name).Call(thisValue, arg0, arg1, arg2, args.Select(a => (JSValue)a).ToArray());

    public static JSValue Call<T>(this JSValue thisValue, params T[] args)
        where T : struct, ITypedValue<T>
        => JSNativeApi.Call(thisValue, thisValue, args.Select(a => (JSValue)a).ToArray());

    public static JSValue Call<T>(this JSValue thisValue, JSValue arg0, params T[] args)
        where T : struct, ITypedValue<T>
        => JSNativeApi.Call(thisValue, thisValue, arg0, args.Select(a => (JSValue)a).ToArray());

    public static JSValue Call<T>(this JSValue thisValue, JSValue arg0, JSValue arg1, params T[] args)
        where T : struct, ITypedValue<T>
        => JSNativeApi.Call(thisValue, thisValue, arg0, arg1, args.Select(a => (JSValue)a).ToArray());

    public static JSValue Call<T>(this JSValue thisValue, JSValue arg0, JSValue arg1, JSValue arg2, params T[] args)
        where T : struct, ITypedValue<T>
        => JSNativeApi.Call(thisValue, thisValue, arg0, arg1, arg2, args.Select(a => (JSValue)a).ToArray());

    public static JSValue CallAsConstructor<T>(this JSValue thisValue, params T[] args)
    where T : struct, ITypedValue<T>
        => JSNativeApi.CallAsConstructor(thisValue, args.Select(a => (JSValue)a).ToArray());

    public static JSValue CallAsConstructor<T>(this JSValue thisValue, JSValue arg0, params T[] args)
        where T : struct, ITypedValue<T>
        => JSNativeApi.CallAsConstructor(thisValue, arg0, args.Select(a => (JSValue)a).ToArray());

    public static JSValue CallAsConstructor<T>(this JSValue thisValue, JSValue arg0, JSValue arg1, params T[] args)
        where T : struct, ITypedValue<T>
        => JSNativeApi.CallAsConstructor(thisValue, arg0, arg1, args.Select(a => (JSValue)a).ToArray());

    public static JSValue CallAsConstructor<T>(this JSValue thisValue, JSValue arg0, JSValue arg1, JSValue arg2, params T[] args)
        where T : struct, ITypedValue<T>
        => JSNativeApi.CallAsConstructor(thisValue, arg0, arg1, arg2, args.Select(a => (JSValue)a).ToArray());
}


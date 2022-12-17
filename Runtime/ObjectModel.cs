// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace NodeApi;

//Undefined,
//Null,
//Boolean,
//Number,
//String,
//Symbol,
//Object,
//Function,
//External,
//BigInt,

public interface IJsUnknown<TSelf>
    where TSelf : IJsUnknown<TSelf>
{
    static abstract TSelf FromJsValue(JSValue value);
    static abstract JSValue ToJsValue(TSelf self);

    JsString As() => JsString.FromJsValue(JSNativeApi.CreateObject());
}

public interface IJsUndefined<TSelf> : IJsUnknown<TSelf>
    where TSelf : struct, IJsUndefined<TSelf>
{
}

public interface IJsNull<TSelf> : IJsUnknown<TSelf>
    where TSelf : IJsNull<TSelf>
{
}

public interface IJsBoolean<TSelf> : IJsUnknown<TSelf>
    where TSelf : IJsBoolean<TSelf>
{
}

public interface IJsNumber<TSelf> : IJsUnknown<TSelf>
    where TSelf : IJsNumber<TSelf>
{
}

public interface IJsName<TSelf> : IJsUnknown<TSelf>
    where TSelf : IJsName<TSelf>
{
}

public interface IJsString<TSelf> : IJsName<TSelf>
    where TSelf : IJsString<TSelf>
{
}

public interface IJsSymbol<TSelf> : IJsName<TSelf>
    where TSelf : IJsSymbol<TSelf>
{
}

public interface IJsObject<TSelf> : IJsUnknown<TSelf>
    where TSelf : IJsObject<TSelf>
{
}

public interface IJsExternal<TSelf> : IJsUnknown<TSelf>
    where TSelf : IJsExternal<TSelf>
{
}

public interface IJsBigInt<TSelf> : IJsUnknown<TSelf>
    where TSelf : IJsBigInt<TSelf>
{
}

public interface IJsFunction<TSelf> : IJsObject<TSelf>
    where TSelf : IJsFunction<TSelf>
{
}

public interface IJsArray<TSelf> : IJsObject<TSelf>
    where TSelf : IJsArray<TSelf>
{
}

public interface IJsOneOf<TSelf> : IJsUnknown<TSelf>
    where TSelf : IJsOneOf<TSelf>
{
}

public struct JsUnknown : IJsUnknown<JsUnknown>
{
    private JSValue JsValue { get; init; }

    public static JsUnknown FromJsValue(JSValue value) => new JsUnknown { JsValue = value };
    public static JSValue ToJsValue(JsUnknown value) => value.JsValue;
}

public struct JsUndefined : IJsUndefined<JsUndefined>
{
    private JSValue JsValue { get; init; }

    public static JsUndefined FromJsValue(JSValue value) => new JsUndefined { JsValue = value };
    public static JSValue ToJsValue(JsUndefined value) => value.JsValue;

    public static implicit operator JsUnknown(JsUndefined value) => JsUnknown.FromJsValue(value.JsValue);
}

public struct JsNull : IJsNull<JsNull>
{
    private JSValue JsValue { get; init; }

    public static JsNull FromJsValue(JSValue value) => new JsNull { JsValue = value };
    public static JSValue ToJsValue(JsNull value) => value.JsValue;

    public static implicit operator JsUnknown(JsNull value) => JsUnknown.FromJsValue(value.JsValue);
}

public struct JsBoolean : IJsBoolean<JsBoolean>
{
    private JSValue JsValue { get; init; }

    public static JsBoolean FromJsValue(JSValue value) => new JsBoolean { JsValue = value };
    public static JSValue ToJsValue(JsBoolean value) => value.JsValue;

    public static implicit operator JsUnknown(JsBoolean value) => JsUnknown.FromJsValue(value.JsValue);
}

public struct JsNumber : IJsNumber<JsNumber>
{
    private JSValue JsValue { get; init; }

    public static JsNumber FromJsValue(JSValue value) => new JsNumber { JsValue = value };
    public static JSValue ToJsValue(JsNumber value) => value.JsValue;

    public static implicit operator JsUnknown(JsNumber value) => JsUnknown.FromJsValue(value.JsValue);
}

public struct JsName : IJsName<JsName>
{
    private JSValue JsValue { get; init; }

    public static JsName FromJsValue(JSValue value) => new JsName { JsValue = value };
    public static JSValue ToJsValue(JsName value) => value.JsValue;

    public static implicit operator JsUnknown(JsName value) => JsUnknown.FromJsValue(value.JsValue);
}

public struct JsString : IJsString<JsString>
{
    private JSValue JsValue { get; init; }

    public static JsString FromJsValue(JSValue value) => new JsString { JsValue = value };
    public static JSValue ToJsValue(JsString value) => value.JsValue;

    public static implicit operator JsUnknown(JsString value) => JsUnknown.FromJsValue(value.JsValue);
    public static implicit operator JsName(JsString value) => JsName.FromJsValue(value.JsValue);
}

public struct JsSymbol : IJsSymbol<JsSymbol>
{
    private JSValue JsValue { get; init; }

    public static JsSymbol FromJsValue(JSValue value) => new JsSymbol { JsValue = value };
    public static JSValue ToJsValue(JsSymbol value) => value.JsValue;

    public static implicit operator JsUnknown(JsSymbol value) => JsUnknown.FromJsValue(value.JsValue);
    public static implicit operator JsName(JsSymbol value) => JsName.FromJsValue(value.JsValue);
}

public struct JsObject : IJsObject<JsObject>
{
    private JSValue JsValue { get; init; }

    public static JsObject FromJsValue(JSValue value) => new JsObject { JsValue = value };
    public static JSValue ToJsValue(JsObject value) => value.JsValue;

    public static implicit operator JsUnknown(JsObject value) => JsUnknown.FromJsValue(value.JsValue);
}

public struct JsFunction : IJsFunction<JsFunction>
{
    private JSValue JsValue { get; init; }

    public static JsFunction FromJsValue(JSValue value) => new JsFunction { JsValue = value };
    public static JSValue ToJsValue(JsFunction value) => value.JsValue;

    public static implicit operator JsUnknown(JsFunction value) => JsUnknown.FromJsValue(value.JsValue);
    public static implicit operator JsObject(JsFunction value) => JsObject.FromJsValue(value.JsValue);
}

public struct JsExternal : IJsExternal<JsExternal>
{
    private JSValue JsValue { get; init; }

    public static JsExternal FromJsValue(JSValue value) => new JsExternal { JsValue = value };
    public static JSValue ToJsValue(JsExternal value) => value.JsValue;

    public static implicit operator JsUnknown(JsExternal value) => JsUnknown.FromJsValue(value.JsValue);
}

public struct JsBigInt : IJsBigInt<JsBigInt>
{
    private JSValue JsValue { get; init; }

    public static JsBigInt FromJsValue(JSValue value) => new JsBigInt { JsValue = value };
    public static JSValue ToJsValue(JsBigInt value) => value.JsValue;

    public static implicit operator JsUnknown(JsBigInt value) => JsUnknown.FromJsValue(value.JsValue);
}

public struct JsOneOf<T1, T2> : IJsOneOf<JsOneOf<T1, T2>>
    where T1 : struct, IJsUnknown<T1>
    where T2 : struct, IJsUnknown<T2>
{
    private JSValue JsValue { get; init; }

    public static JsOneOf<T1, T2> FromJsValue(JSValue value) => new JsOneOf<T1, T2> { JsValue = value };
    public static JSValue ToJsValue(JsOneOf<T1, T2> value) => value.JsValue;

    public static implicit operator JsUnknown(JsOneOf<T1, T2> value) => JsUnknown.FromJsValue(value.JsValue);
    public static implicit operator JsOneOf<T1, T2>(T1 value) => FromJsValue(T1.ToJsValue(value));
    public static implicit operator JsOneOf<T1, T2>(T2 value) => FromJsValue(T2.ToJsValue(value));
}

public static class JsUnknownExtensions
{
    public static JSValueType TypeOf<T>(this T thisValue) where T : IJsUnknown<T>
    {
        return T.ToJsValue(thisValue).TypeOf();
    }

    public static bool IsUndefined<T>(this T thisValue) where T : IJsUnknown<T> => thisValue.TypeOf() == JSValueType.Undefined;
    public static bool IsNull<T>(this T thisValue) where T : IJsUnknown<T> => thisValue.TypeOf() == JSValueType.Null;
    public static bool IsBoolean<T>(this T thisValue) where T : IJsUnknown<T> => thisValue.TypeOf() == JSValueType.Boolean;
    public static bool IsNumber<T>(this T thisValue) where T : IJsUnknown<T> => thisValue.TypeOf() == JSValueType.Number;
    public static bool IsString<T>(this T thisValue) where T : IJsUnknown<T> => thisValue.TypeOf() == JSValueType.String;
    public static bool IsSymbol<T>(this T thisValue) where T : IJsUnknown<T> => thisValue.TypeOf() == JSValueType.Symbol;
    public static bool IsObject<T>(this T thisValue) where T : IJsUnknown<T> => thisValue.TypeOf() == JSValueType.Object;
    public static bool IsFunction<T>(this T thisValue) where T : IJsUnknown<T> => thisValue.TypeOf() == JSValueType.Function;
    public static bool IsExternal<T>(this T thisValue) where T : IJsUnknown<T> => thisValue.TypeOf() == JSValueType.External;
    public static bool IsBigInt<T>(this T thisValue) where T : IJsUnknown<T> => thisValue.TypeOf() == JSValueType.BigInt;
}

public static class JsObjectExtensions
{
    //public static bool HasProperty<T>(this T thisObject) where T : struct, IJsObject<T>
    //{
    //    return ((JSValue)thisObject).HasProperty("aaaa");
    //}

    //public static bool HasProperty<T>(this T thisObject, JsName name) where T : struct, IJsObject<T>
    //{
    //    return ((JSValue)thisObject).HasProperty(name);
    //}

    //public static bool HasOwnProperty<T>(this T thisObject, JsName name) where T : struct, IJsObject<T>
    //{
    //    return ((JSValue)thisObject).HasProperty(name);
    //}

    //public static JsUnknown GetProperty<T>(this T thisObject, JsName name) where T : struct, IJsObject<T>
    //{
    //    return ((JSValue)thisObject).GetProperty(name);
    //}

    //public static void SetProperty<T>(this T thisObject, JsName name, JsUnknown value) where T : struct, IJsObject<T>
    //{
    //    ((JSValue)thisObject).SetProperty(name, value);
    //}

    //public static void DeleteProperty<T>(this T thisObject, JsName name) where T : struct, IJsObject<T>
    //{
    //    ((JSValue)thisObject).DeleteProperty(name);
    //}

    //public static void Freeze<T>(this T obj) where T : struct, IJsObject<T>
    //{
    //    ((JSValue)obj).Freeze();
    //}

    //public static void Seal<T>(this T obj) where T : struct, IJsObject<T>
    //{
    //    ((JSValue)obj).Seal();
    //}
}

public interface IBar
{
    public int Value1() => 1;
}

public struct Bar : IBar
{
    public int Value2() => 2;
}

public static class Foo
{
    public static void TestMe()
    {
        JSValue x = JSNativeApi.CreateObject();
        JsObject obj = JsObject.FromJsValue(x);
        JsFunction fun = JsFunction.FromJsValue(x);

        obj.IsString();

        JsUnknown unk = obj;

        var y = JsString.FromJsValue(x);
        JsOneOf<JsString, JsNumber> z = y;

        Bar b = new Bar();
        b.Value2();
        b.Value1();
        //JsUnknown ii = z;
        //var k = ii.As();

        //obj.Freeze();
        //fun.Freeze();

        //obj.HasProperty();

    }
}

public struct JsArray
{
}

public struct JsDate
{
}

public struct JsArrayBuffer
{
}

public struct JsTypedArray
{
}

public struct JsDataView
{
}

public struct JsPromise
{
}

public struct JsBuffer
{
}

public struct JsError
{
}

public struct JsTypeError
{
}

public struct JsRangeError
{
}

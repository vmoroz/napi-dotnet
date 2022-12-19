using System;
using System.Collections;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static NodeApi.JSNativeApi.Interop;

namespace NodeApi;

public interface IJSUnknown<TSelf>
    where TSelf : struct, IJSUnknown<TSelf>
{
    static abstract TSelf FromJSValue(JSValue value);
    static abstract JSValue ToJSValue(TSelf self);
}

public interface IJSBoolean<TSelf> : IJSUnknown<TSelf>
    where TSelf : struct, IJSBoolean<TSelf>
{
}

public interface IJSNumber<TSelf> : IJSUnknown<TSelf>
    where TSelf : struct, IJSNumber<TSelf>
{
}

public interface IJSName<TSelf> : IJSUnknown<TSelf>
    where TSelf : struct, IJSName<TSelf>
{
}

public interface IJSString<TSelf> : IJSName<TSelf>
    where TSelf : struct, IJSString<TSelf>
{
}

public interface IJSSymbol<TSelf> : IJSName<TSelf>
    where TSelf : struct, IJSSymbol<TSelf>
{
}

public interface IJSObject<TSelf> : IJSUnknown<TSelf>
    where TSelf : struct, IJSObject<TSelf>
{
}

public interface IJSExternal<TSelf> : IJSUnknown<TSelf>
    where TSelf : struct, IJSExternal<TSelf>
{
}

public interface IJSBigInt<TSelf> : IJSUnknown<TSelf>
    where TSelf : struct, IJSBigInt<TSelf>
{
}

public interface IJSFunction<TSelf> : IJSObject<TSelf>
    where TSelf : struct, IJSFunction<TSelf>
{
}

public interface IJSArray<TSelf> : IJSObject<TSelf>
    where TSelf : struct, IJSArray<TSelf>
{
}

public struct JSUnknown : IJSUnknown<JSUnknown>
{
    private JSValue _value;

    public static JSUnknown FromJSValue(JSValue value) => new() { _value = value };
    public static JSValue ToJSValue(JSUnknown value) => value._value;

    public static implicit operator JSUnknown(JSValue value) => new() { _value = value };
}

public struct JSBoolean : IJSBoolean<JSBoolean>
{
    private JSValue _value;

    public JSBoolean(bool value) => _value = JSNativeApi.GetBoolean(value);

    public static JSBoolean FromJSValue(JSValue value) => new() { _value = value };
    public static JSValue ToJSValue(JSBoolean value) => value._value;

    public static implicit operator JSUnknown(JSBoolean value) => JSUnknown.FromJSValue(value._value);

    public static explicit operator bool(JSBoolean value) => value._value.GetValueBool();
    public static implicit operator JSBoolean(bool value) => new(value);
}

public struct JSNumber : IJSNumber<JSNumber>
{
    private JSValue _value;

    public JSNumber(sbyte value) => _value = JSNativeApi.CreateNumber(value);
    public JSNumber(byte value) => _value = JSNativeApi.CreateNumber(value);
    public JSNumber(short value) => _value = JSNativeApi.CreateNumber(value);
    public JSNumber(ushort value) => _value = JSNativeApi.CreateNumber(value);
    public JSNumber(int value) => _value = JSNativeApi.CreateNumber(value);
    public JSNumber(uint value) => _value = JSNativeApi.CreateNumber(value);
    public JSNumber(long value) => _value = JSNativeApi.CreateNumber(value);
    public JSNumber(ulong value) => _value = JSNativeApi.CreateNumber(value);
    public JSNumber(float value) => _value = JSNativeApi.CreateNumber(value);
    public JSNumber(double value) => _value = JSNativeApi.CreateNumber(value);

    public static JSNumber FromJSValue(JSValue value) => new() { _value = value };
    public static JSValue ToJSValue(JSNumber value) => value._value;

    public static implicit operator JSUnknown(JSNumber value) => JSUnknown.FromJSValue(value._value);

    public static implicit operator JSNumber(sbyte value) => new(value);
    public static implicit operator JSNumber(byte value) => new(value);
    public static implicit operator JSNumber(short value) => new(value);
    public static implicit operator JSNumber(ushort value) => new(value);
    public static implicit operator JSNumber(int value) => new(value);
    public static implicit operator JSNumber(uint value) => new(value);
    public static implicit operator JSNumber(long value) => new(value);
    public static implicit operator JSNumber(ulong value) => new(value);
    public static implicit operator JSNumber(float value) => new(value);
    public static implicit operator JSNumber(double value) => new(value);

    public static explicit operator sbyte(JSNumber value) => (sbyte)value._value.GetValueInt32();
    public static explicit operator byte(JSNumber value) => (byte)value._value.GetValueUInt32();
    public static explicit operator short(JSNumber value) => (short)value._value.GetValueInt32();
    public static explicit operator ushort(JSNumber value) => (ushort)value._value.GetValueUInt32();
    public static explicit operator int(JSNumber value) => value._value.GetValueInt32();
    public static explicit operator uint(JSNumber value) => value._value.GetValueUInt32();
    public static explicit operator long(JSNumber value) => value._value.GetValueInt64();
    public static explicit operator ulong(JSNumber value) => (ulong)value._value.GetValueInt64();
    public static explicit operator float(JSNumber value) => (float)value._value.GetValueDouble();
    public static explicit operator double(JSNumber value) => value._value.GetValueDouble();
}

public struct JSName : IJSName<JSName>
{
    private JSValue _value;

    public static JSName FromJSValue(JSValue value) => new() { _value = value };
    public static JSValue ToJSValue(JSName value) => value._value;

    public static implicit operator JSUnknown(JSName value) => JSUnknown.FromJSValue(value._value);
}

public struct JSString : IJSString<JSString>
{
    private JSValue _value;

    public JSString(string value) => _value = JSNativeApi.CreateStringUtf16(value);

    public static JSString FromJSValue(JSValue value) => new() { _value = value };
    public static JSValue ToJSValue(JSString value) => value._value;

    public static JSString FromLatin1(ReadOnlySpan<byte> value) => FromJSValue(JSNativeApi.CreateStringLatin1(value));
    public byte[] ToLatin1() => _value.GetValueStringLatin1();
    public int ToLatin1(Span<byte> data) => _value.GetValueStringLatin1(data);

    public static JSString FromUtf8(ReadOnlySpan<byte> value) => FromJSValue(JSNativeApi.CreateStringUtf8(value));
    public byte[] ToUtf8() => _value.GetValueStringUtf8();
    public int ToUtf8(Span<byte> data) => _value.GetValueStringUtf8(data);

    public static JSString FromUtf16(ReadOnlySpan<char> value) => FromJSValue(JSNativeApi.CreateStringUtf16(value));
    public char[] ToUtf16() => _value.GetValueStringUtf16AsCharArray();
    public int ToUtf16(Span<char> data) => _value.GetValueStringUtf16(data);

    public static implicit operator JSUnknown(JSString value) => JSUnknown.FromJSValue(value._value);
    public static implicit operator JSName(JSString value) => JSName.FromJSValue(value._value);

    public static implicit operator JSString(string value) => new(value);
    public static implicit operator JSString(ReadOnlySpan<char> value) => FromUtf16(value);
    public static implicit operator JSString(ReadOnlySpan<byte> value) => FromUtf8(value);

    public static explicit operator string(JSString value) => value._value.GetValueStringUtf16();
    public static explicit operator char[](JSString value) => value._value.GetValueStringUtf16AsCharArray();
    public static explicit operator byte[](JSString value) => value._value.GetValueStringUtf8();
}

// TODO: Add more methods
public struct JSSymbol : IJSSymbol<JSSymbol>
{
    private JSValue _value;

    public JSSymbol(JSValue description) => JSNativeApi.CreateSymbol(description);

    public static JSSymbol SymbolFor(ReadOnlySpan<byte> utf8Name) => new() { _value = JSNativeApi.SymbolFor(utf8Name) };

    public static JSSymbol FromJSValue(JSValue value) => new() { _value = value };
    public static JSValue ToJSValue(JSSymbol value) => value._value;

    public static implicit operator JSUnknown(JSSymbol value) => JSUnknown.FromJSValue(value._value);
    public static implicit operator JSName(JSSymbol value) => JSName.FromJSValue(value._value);
}

public struct JSObject : IJSObject<JSObject>, IEnumerable<(JSName name, JSUnknown value)>
{
    private JSValue _value;

    public JSObject() => JSNativeApi.CreateObject();

    public static JSObject FromJSValue(JSValue value) => new() { _value = value };
    public static JSValue ToJSValue(JSObject value) => value._value;

    public PropertyEnumerator GetEnumerator()
        => new PropertyEnumerator(_value);

    IEnumerator<(JSName name, JSUnknown value)> IEnumerable<(JSName name, JSUnknown value)>.GetEnumerator()
        => new PropertyEnumerator(_value);

    IEnumerator IEnumerable.GetEnumerator()
        => new PropertyEnumerator(_value);

    public static implicit operator JSUnknown(JSObject value) => JSUnknown.FromJSValue(value._value);

    public static JSObject? FromJSValue(JSValue? value)
        => value != null ? new JSObject?(new() { _value = value.Value }) : null;

    public struct PropertyEnumerator : IEnumerator<(JSName name, JSUnknown value)>, IEnumerator
    {
        private readonly JSValue _value;
        private readonly JSValue _names;
        private readonly int _count;
        private int _index;
        private (JSName name, JSUnknown value)? _current;

        internal PropertyEnumerator(JSValue value)
        {
            _value = value;
            JSValueType valueType = value.TypeOf();
            if (valueType == JSValueType.Object || valueType == JSValueType.Function)
            {
                JSValue names = value.GetPropertyNames();
                _names = names;
                _count = names.GetArrayLength();
            }
            else
            {
                _names = JSValue.Undefined;
                _count = 0;
            }
            _index = 0;
            _current = default;
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (_index < _count)
            {
                JSValue name = _names.GetElement(_index);
                _current = (JSName.FromJSValue(name), _value.GetProperty(name));
                _index++;
                return true;
            }

            _index = _count + 1;
            _current = default;
            return false;
        }

        public (JSName name, JSUnknown value) Current
            => _current ?? throw new InvalidOperationException("Unexpected enumerator state");

        object? IEnumerator.Current
        {
            get
            {
                if (_index == 0 || _index == _count + 1)
                {
                    throw new InvalidOperationException("Invalid enumerator state");
                }
                return Current;
            }
        }

        void IEnumerator.Reset()
        {
            _index = 0;
            _current = default;
        }
    }
}

public struct JSFunction : IJSFunction<JSFunction>
{
    private JSValue _value;

    public static JSFunction FromJSValue(JSValue value) => new() { _value = value };
    public static JSValue ToJSValue(JSFunction value) => value._value;

    public static implicit operator JSUnknown(JSFunction value) => JSUnknown.FromJSValue(value._value);
    public static implicit operator JSObject(JSFunction value) => JSObject.FromJSValue(value._value);
}

public struct JSExternal : IJSExternal<JSExternal>
{
    private JSValue _value;

    public static JSExternal FromJSValue(JSValue value) => new() { _value = value };
    public static JSValue ToJSValue(JSExternal value) => value._value;

    public static implicit operator JSUnknown(JSExternal value) => JSUnknown.FromJSValue(value._value);
}

public struct JSBigInt : IJSBigInt<JSBigInt>
{
    private JSValue _value;

    public static JSBigInt FromJSValue(JSValue value) => new() { _value = value };
    public static JSValue ToJSValue(JSBigInt value) => value._value;

    public static implicit operator JSUnknown(JSBigInt value) => JSUnknown.FromJSValue(value._value);
}

public struct JSArray<T> : IJSArray<JSArray<T>>
    where T : struct, IJSUnknown<T>
{
    private JSValue _value;

    public static JSArray<T> FromJSValue(JSValue value) => new() { _value = value };
    public static JSValue ToJSValue(JSArray<T> value) => value._value;

    public static implicit operator JSUnknown(JSArray<T> value) => JSUnknown.FromJSValue(value._value);
    public static implicit operator JSObject(JSArray<T> value) => JSObject.FromJSValue(value._value);
}

public static class JSUnknownExtensions
{
    public static JSValueType TypeOf<T>(this T thisValue) where T : struct, IJSUnknown<T>
        => T.ToJSValue(thisValue).TypeOf();

    public static bool IsUndefined<T>(this T thisValue) where T : struct, IJSUnknown<T> => thisValue.TypeOf() == JSValueType.Undefined;
    public static bool IsNull<T>(this T thisValue) where T : struct, IJSUnknown<T> => thisValue.TypeOf() == JSValueType.Null;
    public static bool IsBoolean<T>(this T thisValue) where T : struct, IJSUnknown<T> => thisValue.TypeOf() == JSValueType.Boolean;
    public static bool IsNumber<T>(this T thisValue) where T : struct, IJSUnknown<T> => thisValue.TypeOf() == JSValueType.Number;
    public static bool IsString<T>(this T thisValue) where T : struct, IJSUnknown<T> => thisValue.TypeOf() == JSValueType.String;
    public static bool IsSymbol<T>(this T thisValue) where T : struct, IJSUnknown<T> => thisValue.TypeOf() == JSValueType.Symbol;
    public static bool IsObject<T>(this T thisValue) where T : struct, IJSUnknown<T> => thisValue.TypeOf() == JSValueType.Object;
    public static bool IsFunction<T>(this T thisValue) where T : struct, IJSUnknown<T> => thisValue.TypeOf() == JSValueType.Function;
    public static bool IsExternal<T>(this T thisValue) where T : struct, IJSUnknown<T> => thisValue.TypeOf() == JSValueType.External;
    public static bool IsBigInt<T>(this T thisValue) where T : struct, IJSUnknown<T> => thisValue.TypeOf() == JSValueType.BigInt;

    public static JSBoolean ToJSBoolean<T>(this T thisValue) where T : struct, IJSUnknown<T>
        => JSBoolean.FromJSValue(T.ToJSValue(thisValue).CoerceToBoolean());

    public static JSNumber ToJSNumber<T>(this T thisValue) where T : struct, IJSUnknown<T>
        => JSNumber.FromJSValue(T.ToJSValue(thisValue).CoerceToNumber());

    public static JSObject ToJSObject<T>(this T thisValue) where T : struct, IJSUnknown<T>
        => JSObject.FromJSValue(T.ToJSValue(thisValue).CoerceToObject());

    public static JSString ToJSString<T>(this T thisValue) where T : struct, IJSUnknown<T>
        => JSString.FromJSValue(T.ToJSValue(thisValue).CoerceToString());
}

public static class JSObjectExtensions
{
    public static JSObject? GetPrototypeOf<T>(this T thisObject) where T : struct, IJSObject<T>
    {
        JSValue prototype = T.ToJSValue(thisObject).GetPrototype();
        return prototype.TypeOf() == JSValueType.Object ? new JSObject?(JSObject.FromJSValue(prototype)) : null;
    }

    public static JSArray<JSString> Keys<T>(this T thisObject) where T : struct, IJSObject<T>
        => JSArray<JSString>.FromJSValue(T.ToJSValue(thisObject).GetPropertyNames());

    public static JSArray<JSString> GetOwnPropertyNames<T>(this T thisObject) where T : struct, IJSObject<T>
        => JSArray<JSString>.FromJSValue(T.ToJSValue(thisObject).GetAllPropertyNames(
            JSKeyCollectionMode.OwnOnly, JSKeyFilter.SkipSymbols, JSKeyConversion.NumbersToStrings));

    public static JSArray<JSString> GetOwnPropertySymbols<T>(this T thisObject) where T : struct, IJSObject<T>
        => JSArray<JSString>.FromJSValue(T.ToJSValue(thisObject).GetAllPropertyNames(
            JSKeyCollectionMode.OwnOnly, JSKeyFilter.SkipStrings, JSKeyConversion.NumbersToStrings));

    public static JSArray<JSString> GetAllPropertySymbols<T>(this T thisObject, JSKeyCollectionMode mode, JSKeyFilter filter, JSKeyConversion conversion)
        where T : struct, IJSObject<T>
        => JSArray<JSString>.FromJSValue(T.ToJSValue(thisObject).GetAllPropertyNames(mode, filter, conversion));

    public static bool HasProperty<T>(this T thisObject, JSName key) where T : struct, IJSObject<T>
        => T.ToJSValue(thisObject).HasProperty(JSName.ToJSValue(key));

    public static bool HasOwnProperty<T>(this T thisObject, JSName key) where T : struct, IJSObject<T>
        => T.ToJSValue(thisObject).HasOwnProperty(JSName.ToJSValue(key));

    public static JSUnknown GetProperty<T>(this T thisObject, JSName key) where T : struct, IJSObject<T>
        => JSUnknown.FromJSValue(T.ToJSValue(thisObject).GetProperty(JSName.ToJSValue(key)));

    public static void SetProperty<T>(this T thisObject, JSName key, JSValue value) where T : struct, IJSObject<T>
        => T.ToJSValue(thisObject).SetProperty(JSName.ToJSValue(key), value);

    public static bool DeleteProperty<T>(this T thisObject, JSName key) where T : struct, IJSObject<T>
        => T.ToJSValue(thisObject).DeleteProperty(JSName.ToJSValue(key));

    public static void Freeze<T>(this T thisObject, JSName key) where T : struct, IJSObject<T>
    {
        T.ToJSValue(thisObject).Freeze();
    }

    public static void Seal<T>(this T thisObject, JSName key) where T : struct, IJSObject<T>
    {
        T.ToJSValue(thisObject).Seal();
    }
}

public static class Foo
{
    public static void TestMe()
    {
        JSValue x = JSNativeApi.CreateObject();
        JSObject obj = JSObject.FromJSValue(x);
        JSFunction fun = JSFunction.FromJSValue(x);

        //obj.IsString();

        JSUnknown unk = obj;

        //JsUnknown ii = z;
        //var k = ii.As();

        //obj.Freeze();
        //fun.Freeze();

        //obj.HasProperty();

        //JSObject obj1 = new JSObject()
        //{
        //    ["foo"] = 5,
        //    ["bar"] = "sss",
        //};
    }
}

public struct JSDate
{
}

public struct JSArrayBuffer
{
}

public struct JSTypedArray
{
}

public struct JSDataView
{
}

public struct JSPromise
{
}

public struct JSBuffer
{
}

public struct JSError
{
}

public struct JSTypeError
{
}

public struct JSRangeError
{
}

using System;
using static NodeApi.JSNativeApi.Interop;

namespace NodeApi;

public struct JSValue
{
    private napi_value _handle;

    public JSValueScope Scope { get; }

    public JSValue(JSValueScope scope, napi_value handle)
    {
        if (handle.Handle == nint.Zero)
        {
            throw new ArgumentException($"{nameof(handle)} must not be null.");
        }
        Scope = scope;
        _handle = handle;
    }

    public JSValue(napi_value handle)
    {
        if (handle.Handle == nint.Zero)
        {
            throw new ArgumentException($"{nameof(handle)} must not be null.");
        }
        Scope = JSValueScope.Current ?? throw new InvalidOperationException("No current scope");
        _handle = handle;
    }

    public napi_value? Handle => !Scope.IsDisposed ? _handle : null;

    public napi_value GetCheckedHandle()
        => Handle ?? throw new InvalidOperationException("The value handle is invalid because its scope is closed");

    public static JSValue Undefined => JSNativeApi.GetUndefined();
    public static JSValue Null => JSNativeApi.GetNull();
    public static JSValue Global => JSNativeApi.GetGlobal();
    public static JSValue True => JSNativeApi.GetBoolean(true);
    public static JSValue False => JSNativeApi.GetBoolean(false);
    public static JSValue GetBoolean(bool value) => JSNativeApi.GetBoolean(value);

    public JSObject Properties => (JSObject)this;

    public JSArray Items => (JSArray)this;

    public JSValue this[JSValue name]
    {
        get => this.GetProperty(name);
        set => this.SetProperty(name, value);
    }

    public JSValue this[string name]
    {
        get => this.GetProperty(name);
        set => this.SetProperty(name, value);
    }

    public JSValue this[int index]
    {
        get => this.GetElement(index);
        set => this.SetElement(index, value);
    }

    public static implicit operator JSValue(bool value) => JSNativeApi.GetBoolean(value);
    public static implicit operator JSValue(sbyte value) => JSNativeApi.CreateNumber(value);
    public static implicit operator JSValue(byte value) => JSNativeApi.CreateNumber(value);
    public static implicit operator JSValue(short value) => JSNativeApi.CreateNumber(value);
    public static implicit operator JSValue(ushort value) => JSNativeApi.CreateNumber(value);
    public static implicit operator JSValue(int value) => JSNativeApi.CreateNumber(value);
    public static implicit operator JSValue(uint value) => JSNativeApi.CreateNumber(value);
    public static implicit operator JSValue(long value) => JSNativeApi.CreateNumber(value);
    public static implicit operator JSValue(ulong value) => JSNativeApi.CreateNumber(value);
    public static implicit operator JSValue(float value) => JSNativeApi.CreateNumber(value);
    public static implicit operator JSValue(double value) => JSNativeApi.CreateNumber(value);
    public static implicit operator JSValue(string value) => JSNativeApi.CreateStringUtf16(value);
    public static implicit operator JSValue(char[] value) => JSNativeApi.CreateStringUtf16(value);
    public static implicit operator JSValue(Span<char> value) => JSNativeApi.CreateStringUtf16(value);
    public static implicit operator JSValue(ReadOnlySpan<char> value) => JSNativeApi.CreateStringUtf16(value);
    public static implicit operator JSValue(byte[] value) => JSNativeApi.CreateStringUtf8(value);
    public static implicit operator JSValue(Span<byte> value) => JSNativeApi.CreateStringUtf8(value);
    public static implicit operator JSValue(ReadOnlySpan<byte> value) => JSNativeApi.CreateStringUtf8(value);
    public static implicit operator JSValue(JSCallback callback) => JSNativeApi.CreateFunction("Unknown", callback);

    public static explicit operator bool(JSValue value) => value.GetValueBool();
    public static explicit operator sbyte(JSValue value) => (sbyte)value.GetValueInt32();
    public static explicit operator byte(JSValue value) => (byte)value.GetValueUInt32();
    public static explicit operator short(JSValue value) => (short)value.GetValueInt32();
    public static explicit operator ushort(JSValue value) => (ushort)value.GetValueUInt32();
    public static explicit operator int(JSValue value) => value.GetValueInt32();
    public static explicit operator uint(JSValue value) => value.GetValueUInt32();
    public static explicit operator long(JSValue value) => value.GetValueInt64();
    public static explicit operator ulong(JSValue value) => (ulong)value.GetValueInt64();
    public static explicit operator float(JSValue value) => (float)value.GetValueDouble();
    public static explicit operator double(JSValue value) => value.GetValueDouble();
    public static explicit operator string(JSValue value) => value.GetValueStringUtf16();
    public static explicit operator char[](JSValue value) => value.GetValueStringUtf16AsCharArray();
    public static explicit operator byte[](JSValue value) => value.GetValueStringUtf8();

    public static explicit operator napi_value(JSValue value) => value.GetCheckedHandle();
    public static implicit operator JSValue(napi_value handle) => new(handle);

    public static explicit operator napi_value(JSValue? value) => value?.Handle ?? new napi_value(nint.Zero);
    public static implicit operator JSValue?(napi_value handle) => handle.Handle != nint.Zero ? new JSValue?(new JSValue(handle)) : null;
}

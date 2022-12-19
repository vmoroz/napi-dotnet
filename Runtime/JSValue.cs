using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using static NodeApi.JSNativeApi.Interop;

namespace NodeApi;

public struct JSValue
{
    private napi_value _handle;

    public JSValueScope Scope { get; }

    public JSValue(JSValueScope scope, napi_value handle)
    {
        Contract.Requires(handle.Handle != nint.Zero, "handle must be not null");
        Scope = scope;
        _handle = handle;
    }

    public JSValue(napi_value handle)
    {
        Contract.Requires(handle.Handle != nint.Zero, "handle must be not null");
        Scope = JSValueScope.Current ?? throw new InvalidOperationException("No current scope");
        _handle = handle;
    }

    public napi_value GetCheckedHandle()
    {
        if (Scope.IsDisposed)
        {
            throw new InvalidOperationException("The value handle is invalid because its scope is closed");
        }
        return _handle;
    }

    public static JSValue Undefined => JSNativeApi.GetUndefined();
    public static JSValue Null => JSNativeApi.GetNull();
    public static JSValue Global => JSNativeApi.GetGlobal();
    public static JSValue True => JSNativeApi.GetBoolean(true);
    public static JSValue False => JSNativeApi.GetBoolean(false);
    public static JSValue GetBoolean(bool value) => JSNativeApi.GetBoolean(value);

    public EnumerableProperties Properties => new EnumerableProperties(this);

    public EnumerableItems Items => new EnumerableItems(this);

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
    public static implicit operator JSValue(ReadOnlySpan<byte> value) => JSNativeApi.CreateStringUtf8(value);
    public static implicit operator JSValue(byte[] value) => JSNativeApi.CreateStringUtf8(value);
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

    public JSValue this[JSValue name]
    {
        get => this.GetProperty(name);
        set => this.SetProperty(name, value);
    }

    public JSValue this[string name]
    {
        get => this.GetProperty(name);
        set { this.SetProperty(name, value); }
    }

    public JSValue this[int index]
    {
        get { return this.GetElement(index); }
        set { this.SetElement(index, value); }
    }

    public static explicit operator napi_value(JSValue value) => value.GetCheckedHandle();
    public static explicit operator napi_value(JSValue? value) => value != null ? value.Value.GetCheckedHandle() : new napi_value(nint.Zero);

    public static implicit operator JSValue(napi_value handle) => new(handle);
    public static implicit operator JSValue?(napi_value handle) => handle.Handle != nint.Zero ? new JSValue?(new JSValue(handle)) : null;

    public struct EnumerableProperties : IEnumerable<(JSValue name, JSValue value)>, IEnumerable
    {
        private JSValue _value;

        internal EnumerableProperties(JSValue value)
        {
            _value = value;
        }

        public PropertyEnumerator GetEnumerator()
            => new PropertyEnumerator(_value);

        IEnumerator<(JSValue name, JSValue value)> IEnumerable<(JSValue name, JSValue value)>.GetEnumerator()
            => new PropertyEnumerator(_value);

        IEnumerator IEnumerable.GetEnumerator()
            => new PropertyEnumerator(_value);
    }

    public struct PropertyEnumerator : IEnumerator<(JSValue name, JSValue value)>, IEnumerator
    {
        private readonly JSValue _value;
        private readonly JSValue _names;
        private readonly int _count;
        private int _index;
        private (JSValue name, JSValue value)? _current;

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
                _names = Undefined;
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
                _current = (name, _value.GetProperty(name));
                _index++;
                return true;
            }

            _index = _count + 1;
            _current = default;
            return false;
        }

        public (JSValue name, JSValue value) Current
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

    public struct EnumerableItems : IEnumerable<JSValue>, IEnumerable
    {
        private JSValue _value;

        internal EnumerableItems(JSValue value)
        {
            _value = value;
        }

        public ItemEnumerator GetEnumerator()
            => new ItemEnumerator(_value);

        IEnumerator<JSValue> IEnumerable<JSValue>.GetEnumerator()
            => new ItemEnumerator(_value);

        IEnumerator IEnumerable.GetEnumerator()
            => new ItemEnumerator(_value);
    }

    public struct ItemEnumerator : IEnumerator<JSValue>, IEnumerator
    {
        private readonly JSValue _value;
        private readonly int _count;
        private int _index;
        private JSValue? _current;

        internal ItemEnumerator(JSValue value)
        {
            _value = value;
            if (value.IsArray())
            {
                _count = value.GetArrayLength();
            }
            else
            {
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
                _current = _value.GetElement(_index);
                _index++;
                return true;
            }

            _index = _count + 1;
            _current = default;
            return false;
        }

        public JSValue Current
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

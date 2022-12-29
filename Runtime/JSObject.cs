using System.Collections;
using System.Collections.Generic;

namespace NodeApi;

public struct JSObject : IEnumerable<(JSValue name, JSValue value)>, IEnumerable
{
    JSValue _value;

    public static explicit operator JSObject(JSValue value) => new JSObject { _value = value };
    public static implicit operator JSValue(JSObject obj) => obj._value;

    public JSObject()
    {
        _value = JSNativeApi.CreateObject();
    }

    public static JSObject Global => (JSObject)JSNativeApi.GetGlobal();

    public JSValue this[JSValue name]
    {
        get => _value.GetProperty(name);
        set => _value.SetProperty(name, value);
    }

    public JSValue this[string name]
    {
        get => _value.GetProperty(name);
        set => _value.SetProperty(name, value);
    }

    public void Add(JSValue name, JSValue value)
        => _value.SetProperty(name, value);

    public JSObjectPropertyEnumerator GetEnumerator()
        => new JSObjectPropertyEnumerator(_value);

    IEnumerator<(JSValue name, JSValue value)> IEnumerable<(JSValue name, JSValue value)>.GetEnumerator()
        => new JSObjectPropertyEnumerator(_value);

    IEnumerator IEnumerable.GetEnumerator()
        => new JSObjectPropertyEnumerator(_value);
}

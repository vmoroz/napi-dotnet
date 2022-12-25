using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NodeApi.EcmaScript;

// ECMAScript APIs as they defined in
// https://github.com/microsoft/TypeScript/blob/main/src/lib/es5.d.ts

//TODO: type aliases
//TODO: interfaces
//TODO: type assertions
//TODO: literal types
//TODO: as const - converts to literals
//TODO: Non-null (postfix !) - can we use C# Nullable?
//TODO: Enums
//TODO: Narrowing
//TODO:   typeof -> "string" "number" "bigint" "boolean" "symbol" "undefined" "object" "function" (where is null? typeof null is actually "object")
//TODO:   Truthiness
//TODO:   Equality Narrowing
//TODO:   The in operator narrowing
//TODO:   instanceof narrowing
//TODO:   type predicates
//TODO:   Discriminated unions
//TODO:   the never type
//TODO: Function Call Signatures
//TODO: Function Construct Signatures
//TODO: Generic Functions
//TODO: Generic constraints
//TODO: Can we use anonymous types for in-place type definitions?
//TODO: Functions with variable parameters
//TODO: Can we adopt C# delegate types?
//TODO: Function overloads
//TODO: Special types: void, object, unknown, never . Functions are objects
//TODO: Special type: Function - it is like any that can always be called
//TODO: Rest Parameters, Rest arguments
//TODO: Parameter Destructuring
//TODO: Function void return type means that it can return anything, but it is ignored

//TODO: We can use tuple types to represent in-place object types. E.g TsObject<(TsString foo, TsOptional<TsString> bar)>

public partial struct PropertyDescriptorMap : IPropertyDescriptorMap<PropertyDescriptorMap>
{
}

public partial interface IGlobal<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, IGlobal<TSelf>
{
    Number NaN { get; set; }
    Number Infinity { get; set; }
    Any eval(String text);
    Number parseInt(String value, Number? radix = null);
    Number parseFloat(String value);
    Boolean isNaN(Number value);
    Boolean isFinite(Number value);
    String decodeURI(String value);
    String decodeURIComponent(String value);
    String encodeURI(String value);
    String encodeURIComponent(Union<String, Number, Boolean> value);
}

public partial struct Global : IGlobal<Global>
{
}

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
        where T : struct, IJSValueHolder<T>
            => thisValue.GetProperty(name).Call(thisValue, args.Select(a => (JSValue)a).ToArray());
}

public partial interface ISymbol<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, ISymbol<TSelf>
{
    String toString();
    Symbol valueOf();
}

public partial struct Symbol : ISymbol<Symbol>
{
}

// declare type PropertyKey = string | number | symbol;
public partial struct PropertyKey : IJSValueHolder<PropertyKey>
{
    public static implicit operator PropertyKey(String value) => new PropertyKey { _value = (JSValue)value };
    public static implicit operator PropertyKey(Number value) => new PropertyKey { _value = (JSValue)value };
    public static implicit operator PropertyKey(Symbol value) => new PropertyKey { _value = (JSValue)value };

    public static explicit operator String(PropertyKey value) => (String)value._value;
    public static explicit operator Number(PropertyKey value) => (Number)value._value;
    public static explicit operator Symbol(PropertyKey value) => (Symbol)value._value;
}

public interface IPropertyDescriptor<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, IPropertyDescriptor<TSelf>
{
    Boolean? configurable { get; set; }
    Boolean? enumerable { get; set; }
    Boolean? writable { get; set; }
    Any? value { get; set; }
    Function<Any>? get { get; set; }
    Function<Any, Void>? set { get; set; }
}

public partial interface IPropertyDescriptorMap<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, IPropertyDescriptorMap<TSelf>
{
    PropertyDescriptor this[PropertyKey key] { get; set; }
}

public partial struct PropertyDescriptor : IPropertyDescriptor<PropertyDescriptor>
{
}

public partial struct Nullable<T> : IJSValueHolder<Nullable<T>>
{
}

public partial interface IObject<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, IObject<TSelf>
{
    Function constructor { get; set; }
    String toString();
    String toLocaleString();
    Object valueOf();
    Boolean hasOwnProperty(PropertyKey key);
    Boolean isPrototypeOf(Object value);
    Boolean propertyIsEnumerable(PropertyKey key);
}

// interface Object
public partial struct Object : IObject<Object>
{
}

public partial struct Global
{
    public static Global Instance => (Global)JSValue.Global;
}

/**
 * Marker for contextual 'this' type
 */
// interface ThisType<T> { }
public partial struct ThisType<T> : IJSValueHolder<ThisType<T>>
{
}

public partial struct Intersect<T1, T2> : IJSValueHolder<Intersect<T1, T2>>
{
}

public partial struct Readonly<T> : IJSValueHolder<Readonly<T>>
{
}

public interface IJSValueHolder<TSelf> where TSelf : struct, IJSValueHolder<TSelf>
{
    public static abstract explicit operator TSelf(JSValue value);
    public static abstract implicit operator JSValue(TSelf value);

    // Map Undefined to Nullable
    public static abstract explicit operator TSelf?(JSValue value);
    public static abstract implicit operator JSValue(TSelf? value);
}

public interface IFunction<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, IFunction<TSelf>
{
}

public partial interface IObjectConstructor<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, IObjectConstructor<TSelf>
{
    Object New(Any? value);
    Any Call();
    Any Call(Any value);
    Object prototype { get; }
    Any getPrototypeOf(Any obj);
    PropertyDescriptor? getOwnPropertyDescriptor(Any obj, PropertyKey key);
    Array<String> getOwnPropertyNames(Any obj);
    Any create(Nullable<Object> obj);
    Any create(Nullable<Object> obj, Intersect<PropertyDescriptorMap, ThisType<Any>> properties);
    T defineProperty<T>(T obj, PropertyKey key, Intersect<PropertyDescriptor, ThisType<Any>> attributes) where T : struct, IJSValueHolder<T>;
    T defineProperties<T>(T obj, Intersect<PropertyDescriptorMap, ThisType<Any>> properties) where T : struct, IJSValueHolder<T>;
    T seal<T>(T obj) where T : struct, IJSValueHolder<T>;
    Readonly<T> freeze<T>(T obj) where T : struct, IJSValueHolder<T>;
    T preventExtensions<T>(T obj) where T : struct, IJSValueHolder<T>;
    Boolean isSealed(Any obj);
    Boolean isFrozen(Any obj);
    Boolean isExtensible(Any obj);
    Array<String> keys(Object obj);
}

public partial struct ObjectConstructor : IObjectConstructor<ObjectConstructor>
{
}

public partial interface IGlobal<TSelf>
{
    ObjectConstructor Object { get; set; }
}

public partial interface IString<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, IString<TSelf>
{
    String toString();
    String charAt(Number index);
    String charCodeAt(Number index);
    String concat(params String[] strings);
    Number indexOf(String searchString, Number? position);
    Number lastIndexOf(String searchString, Number? position);
    Number localeCompare(String value);

    // match(regexp: string | RegExp): RegExpMatchArray | null;
    // replace(searchValue: string | RegExp, replaceValue: string): string;
    // replace(searchValue: string | RegExp, replacer: (substring: string, ...args: any[]) => string): string;
    // search(regexp: string | RegExp): number;

    String slice(Number? start = null, Number? end = null);

    // split(separator: string | RegExp, limit?: number): string[];
    String substring(Number start, Number? end = null);
    String toLowerCase();

    String toLocaleLowerCase(Union<String, Array<String>>? locales = null);
    String toUpperCase();
    String toLocaleUpperCase(Union<String, Array<String>>? locales = null);
    String trim();
    Number length { get; }
    String valueOf();
    String this[Number index] { get; }
}

public partial struct String : IString<String>
{
}

public partial interface IStringConstructor<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, IStringConstructor<TSelf>
{
    String New(Any? value = null);
    String Call(Any? value = null);
    String prototype { get; }
    String fromCharCode(params Number[] codes);
}

public partial struct StringConstructor : IStringConstructor<StringConstructor>
{
}

public partial interface IGlobal<TSelf>
{
    StringConstructor String { get; set; }
}

public partial interface IBoolean<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, IBoolean<TSelf>
{
    Boolean valueOf();
}

public partial struct Boolean : IBoolean<Boolean>
{
}

public partial interface IBooleanConstructor<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, IBooleanConstructor<TSelf>
{
    Boolean New(Any? value = null);
    Boolean Call<T>(T? value) where T : struct, IJSValueHolder<T>;
    Boolean prototype { get; }
}

public partial struct BooleanConstructor : IBooleanConstructor<BooleanConstructor>
{
}

public partial interface IGlobal
{
    BooleanConstructor Boolean { get; set; }
}

public partial interface INumber<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, INumber<TSelf>
{
    String toString(Number? radix = null);
    String toFixed(Number? fractionDigits = null);
    String toExponential(Number? fractionDigits = null);
    String toPrecision(Number? precision = null);
    Number valueOf();
}

public partial struct Number : INumber<Number>
{
}

public partial interface INumberConstructor<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, INumberConstructor<TSelf>
{
    Number New(Any? value = null);
    Number Call(Any? value = null);
    Number prototype { get; }
    Number MAX_VALUE { get; }
    Number MIN_VALUE { get; }
    Number NaN { get; }
    Number NEGATIVE_INFINITY { get; }
    Number POSITIVE_INFINITY { get; }
}

public partial struct NumberConstructor : INumberConstructor<NumberConstructor>
{
}

public partial interface IGlobal
{
    NumberConstructor Number { get; set; }
}

/**
 * Represents a raw buffer of binary data, which is used to store data for the
 * different typed arrays. ArrayBuffers cannot be read from or written to directly,
 * but can be passed to a typed array or DataView Object to interpret the raw
 * buffer as needed.
 */
// interface ArrayBuffer
public partial struct ArrayBuffer : IJSValueHolder<ArrayBuffer>
{
    /**
     * Read-only. The length of the ArrayBuffer (in bytes).
     */
    //readonly byteLength: number;
    public Number ByteLength => (Number)_value.GetProperty(NameTable.byteLength);


    /**
     * Returns a section of an ArrayBuffer.
     */
    //slice(begin: number, end?: number): ArrayBuffer;
    public ArrayBuffer Slice(Number begin, Number? end = null)
        => (ArrayBuffer)_value.CallMethod(NameTable.slice, begin, end);
}


public partial class NameTable
{
    public static JSValue byteLength => GetStringName(nameof(byteLength));

    // TODO: Implement
    public static JSValue GetStringName(string value) => JSValue.Null;
}

public partial struct Union<T1, T2> : IJSValueHolder<Union<T1, T2>>
{
}

public partial struct Union<T1, T2, T3> : IJSValueHolder<Union<T1, T2, T3>>
{
}

public partial struct Number : IJSValueHolder<Number>
{
}

public partial struct Function : IFunction<Function>
{
}

public partial struct Function<TResult> : IFunction<Function<TResult>>
{
}

public partial struct Function<TArg1, TResult> : IFunction<Function<TArg1, TResult>>
{
}

public partial struct Void : IJSValueHolder<Void>
{
}

public partial struct Any : IJSValueHolder<Any>
{
}

public partial struct Array<T> : IJSValueHolder<Array<T>>
{
}

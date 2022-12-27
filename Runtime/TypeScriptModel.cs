using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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

[TypedInterface]
public partial interface IGlobal
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
    String encodeURIComponent(OneOf<String, Number, Boolean> value);
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

    public static JSValue CallMethod<T>(this JSValue thisValue, JSValue name, JSValue arg0, params T[] args)
        where T : struct, IJSValueHolder<T>
        => thisValue.GetProperty(name).Call(thisValue, arg0, args.Select(a => (JSValue)a).ToArray());

    public static JSValue CallMethod<T>(this JSValue thisValue, JSValue name, JSValue arg0, JSValue arg1, params T[] args)
        where T : struct, IJSValueHolder<T>
        => thisValue.GetProperty(name).Call(thisValue, arg0, arg1, args.Select(a => (JSValue)a).ToArray());

    public static JSValue CallMethod<T>(this JSValue thisValue, JSValue name, JSValue arg0, JSValue arg1, JSValue arg2, params T[] args)
        where T : struct, IJSValueHolder<T>
        => thisValue.GetProperty(name).Call(thisValue, arg0, arg1, arg2, args.Select(a => (JSValue)a).ToArray());

    public static JSValue Call<T>(this JSValue thisValue, params T[] args)
        where T : struct, IJSValueHolder<T>
        => JSNativeApi.Call(thisValue, thisValue, args.Select(a => (JSValue)a).ToArray());

    public static JSValue Call<T>(this JSValue thisValue, JSValue arg0, params T[] args)
        where T : struct, IJSValueHolder<T>
        => JSNativeApi.Call(thisValue, thisValue, arg0, args.Select(a => (JSValue)a).ToArray());

    public static JSValue Call<T>(this JSValue thisValue, JSValue arg0, JSValue arg1, params T[] args)
        where T : struct, IJSValueHolder<T>
        => JSNativeApi.Call(thisValue, thisValue, arg0, arg1, args.Select(a => (JSValue)a).ToArray());

    public static JSValue Call<T>(this JSValue thisValue, JSValue arg0, JSValue arg1, JSValue arg2, params T[] args)
        where T : struct, IJSValueHolder<T>
        => JSNativeApi.Call(thisValue, thisValue, arg0, arg1, arg2, args.Select(a => (JSValue)a).ToArray());

    public static JSValue CallAsConstructor<T>(this JSValue thisValue, params T[] args)
    where T : struct, IJSValueHolder<T>
        => JSNativeApi.CallAsConstructor(thisValue, args.Select(a => (JSValue)a).ToArray());

    public static JSValue CallAsConstructor<T>(this JSValue thisValue, JSValue arg0, params T[] args)
        where T : struct, IJSValueHolder<T>
        => JSNativeApi.CallAsConstructor(thisValue, arg0, args.Select(a => (JSValue)a).ToArray());

    public static JSValue CallAsConstructor<T>(this JSValue thisValue, JSValue arg0, JSValue arg1, params T[] args)
        where T : struct, IJSValueHolder<T>
        => JSNativeApi.CallAsConstructor(thisValue, arg0, arg1, args.Select(a => (JSValue)a).ToArray());

    public static JSValue CallAsConstructor<T>(this JSValue thisValue, JSValue arg0, JSValue arg1, JSValue arg2, params T[] args)
        where T : struct, IJSValueHolder<T>
        => JSNativeApi.CallAsConstructor(thisValue, arg0, arg1, arg2, args.Select(a => (JSValue)a).ToArray());
}

[TypedInterface]
public partial interface ISymbol
{
    String toString();
    Symbol valueOf();
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

[TypedInterface]
public partial interface IPropertyDescriptor
{
    Boolean? configurable { get; set; }
    Boolean? enumerable { get; set; }
    Boolean? writable { get; set; }
    Any? value { get; set; }
    Function<Any>? get { get; set; }
    Function<Any, Void>? set { get; set; }
}

[TypedInterface]
public partial interface IPropertyDescriptorMap
{
    PropertyDescriptor this[PropertyKey key] { get; set; }
}

[TypedInterface]
public partial interface IObject
{
    Function constructor { get; set; }
    String toString();
    String toLocaleString();
    Object valueOf();
    Boolean hasOwnProperty(PropertyKey key);
    Boolean isPrototypeOf(Object value);
    Boolean propertyIsEnumerable(PropertyKey key);
}

[TypedInterface]
public partial interface IObjectConstructor
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

public partial interface IGlobal
{
    ObjectConstructor Object { get; set; }
}

[TypedInterface]
public partial interface IString
{
    String toString();
    String charAt(Number index);
    String charCodeAt(Number index);
    String concat(params String[] strings);
    Number indexOf(String searchString, Number? position);
    Number lastIndexOf(String searchString, Number? position);
    Number localeCompare(String value);

    Nullable<RegExpMatchArray> match(OneOf<String, RegExp> regexp);
    String replace(OneOf<String, RegExp> searchValue, String replaceValue);
    String replace(OneOf<String, RegExp> searchValue, Function<String /*substring*/ /*...args: any[]*/, String> replacer);
    Number search(OneOf<String, RegExp> regexp);
    String slice(Number? start = null, Number? end = null);
    Array<String> split(OneOf<String, RegExp> separator, Number? limit = null);
    String substring(Number start, Number? end = null);
    String toLowerCase();
    String toLocaleLowerCase(OneOf<String, Array<String>>? locales = null);
    String toUpperCase();
    String toLocaleUpperCase(OneOf<String, Array<String>>? locales = null);
    String trim();
    Number length { get; }
    String valueOf();
    String this[Number index] { get; }
}

[TypedInterface]
public partial interface IStringConstructor
{
    String New(Any? value = null);
    String Call(Any? value = null);
    String prototype { get; }
    String fromCharCode(params Number[] codes);
}

public partial interface IGlobal
{
    StringConstructor String { get; set; }
}

[TypedInterface]
public partial interface IBoolean
{
    Boolean valueOf();
}

[TypedInterface]
public partial interface IBooleanConstructor
{
    Boolean New(Any? value = null);
    Boolean Call<T>(T? value) where T : struct, IJSValueHolder<T>;
    Boolean prototype { get; }
}

public partial interface IGlobal
{
    BooleanConstructor Boolean { get; set; }
}

public partial interface INumber<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, INumber<TSelf>
{
    String toString(@number? radix = null);
    String toFixed(@number? fractionDigits = null);
    String toExponential(@number? fractionDigits = null);
    String toPrecision(@number? precision = null);
    @number valueOf();
}

public partial interface INumberConstructor : ITypedConstructor<NumberConstructor, Number>
{
    Number New(Any? value = null);
    @number Call(Any? value = null);
    Number prototype { get; }
    @number MAX_VALUE { get; }
    @number MIN_VALUE { get; }
    @number NaN { get; }
    @number NEGATIVE_INFINITY { get; }
    @number POSITIVE_INFINITY { get; }
}

public partial struct @number : INumber<@number> { }
public partial struct Number : INumber<Number> { }
public partial struct NumberConstructor : INumberConstructor
{
    public static NumberConstructor Instance => GlobalCache.Number;
}

public partial interface IGlobal
{
    NumberConstructor Number { get; set; }
}

public partial interface IGlobalCache
{
    static abstract NumberConstructor Number { get; }
}

public partial class GlobalCache : IGlobalCache
{
    //TODO: implement generated
    public static NumberConstructor Number { get { return new NumberConstructor(); } }
}

//TODO: Add import types and template strings array

[TypedInterface]
public partial interface IMath
{
    Number E { get; }
    Number LN10 { get; }
    Number LN2 { get; }
    Number LOG2E { get; }
    Number LOG10E { get; }
    Number PI { get; }
    Number SQRT1_2 { get; }
    Number SQRT2 { get; }
    Number abs(Number x);
    Number acos(Number x);
    Number asin(Number x);
    Number atan(Number x);
    Number atan2(Number y, Number x);
    Number ceil(Number x);
    Number cos(Number x);
    Number exp(Number x);
    Number floor(Number x);
    Number log(Number x);
    Number max(params Number[] values);
    Number min(params Number[] values);
    Number pow(Number x, Number y);
    Number random();
    Number round(Number x);
    Number sin(Number x);
    Number sqrt(Number x);
    Number tan(Number x);
}

public partial interface IGlobal
{
    Math Math { get; set; }
}

[TypedInterface]
public partial interface IDate
{
    String toString();
    String toDateString();
    String toTimeString();
    String toLocaleString();
    String toLocaleDateString();
    String toLocaleTimeString();
    Number valueOf();
    Number getTime();
    Number getFullYear();
    Number getUTCFullYear();
    Number getMonth();
    Number getUTCMonth();
    Number getDate();
    Number getUTCDate();
    Number getDay();
    Number getUTCDay();
    Number getHours();
    Number getUTCHours();
    Number getMinutes();
    Number getUTCMinutes();
    Number getSeconds();
    Number getUTCSeconds();
    Number getMilliseconds();
    Number getUTCMilliseconds();
    Number getTimezoneOffset();
    Number setTime(Number time);
    Number setMilliseconds(Number ms);
    Number setUTCMilliseconds(Number ms);
    Number setSeconds(Number sec, Number? ms = null);
    Number setUTCSeconds(Number sec, Number? ms = null);
    Number setMinutes(Number min, Number? sec = null, Number? ms = null);
    Number setUTCMinutes(Number min, Number? sec = null, Number? ms = null);
    Number setHours(Number hours, Number? min = null, Number? sec = null, Number? ms = null);
    Number setUTCHours(Number hours, Number? min = null, Number? sec = null, Number? ms = null);
    Number setDate(Number date);
    Number setUTCDate(Number date);
    Number setMonth(Number month, Number? date);
    Number setUTCMonth(Number month, Number? date = null);
    Number setFullYear(Number year, Number? month = null, Number? date = null);
    Number setUTCFullYear(Number year, Number? month = null, Number? date = null);
    String toUTCString();
    String toISOString();
    String toJSON(Any? key = null);
}

[TypedInterface]
public partial interface IDateConstructor
{
    Date New();
    Date New(Number value);
    Date New(String value);
    Date New(Number year, Number monthIndex, Number? date = null, Number? hours = null, Number? minutes = null, Number? seconds = null, Number? ms = null);
    String Call();
    Date prototype { get; }
    Number parse(string s);
    Number UTC(Number year, Number monthIndex, Number? date = null, Number? hours = null, Number? minutes = null, Number? seconds = null, Number? ms = null);
    Number now();
}

public partial interface IGlobal
{
    DateConstructor Date { get; set; }
}

public partial interface IRegExpMatchArray<TSelf> : IArray<TSelf, String>
    where TSelf : struct, IRegExpMatchArray<TSelf>
{
    Number? index { get; set; }
    String? input { get; set; }
}

public partial struct RegExpMatchArray : IRegExpMatchArray<RegExpMatchArray>
{
}

public partial interface IRegExpExecArray<TSelf> : IArray<TSelf, String>
    where TSelf : struct, IRegExpExecArray<TSelf>
{
    Number? index { get; set; }
    String? input { get; set; }
}

public partial struct RegExpExecArray : IRegExpExecArray<RegExpExecArray>
{
}

[TypedInterface]
public partial interface IRegExp
{
    Nullable<RegExpExecArray> exec(String value);
    Boolean test(String value);
    String source { get; }
    Boolean global { get; }
    Boolean ignoreCase { get; }
    Boolean multiline { get; }
    Number lastIndex { get; set; }
}

[TypedInterface]
public partial interface IRegExpConstructor
{
    RegExp New(OneOf<RegExp, String> pattern);
    RegExp New(String pattern, String? flags = null);
    RegExp Call(OneOf<RegExp, String> pattern);
    RegExp Call(String pattern, String? flags = null);
    RegExp prototype { get; }
}

public partial interface IGlobal
{
    RegExpConstructor RegExp { get; set; }
}

public partial interface IError<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, IError<TSelf>
{
    String name { get; set; }
    String message { get; set; }
    String? stack { get; set; }
}

public partial interface IErrorConstructor<TSelf, TError> : IJSValueHolder<TSelf>
    where TSelf : struct, IErrorConstructor<TSelf, TError>
    where TError : struct, IError<TError>
{
    TError New(String? message = null);
    TError Call(String? message = null);
    TError prototype { get; }
}

public partial struct Error : IError<Error> { }
public partial struct EvalError : IError<EvalError> { }
public partial struct RangeError : IError<RangeError> { }
public partial struct ReferenceError : IError<ReferenceError> { }
public partial struct SyntaxError : IError<SyntaxError> { }
public partial struct TypeError : IError<TypeError> { }
public partial struct URIError : IError<URIError> { }

public partial struct ErrorConstructor : IErrorConstructor<ErrorConstructor, Error> { }
public partial struct EvalErrorConstructor : IErrorConstructor<EvalErrorConstructor, EvalError> { }
public partial struct RangeErrorConstructor : IErrorConstructor<RangeErrorConstructor, RangeError> { }
public partial struct ReferenceErrorConstructor : IErrorConstructor<ReferenceErrorConstructor, ReferenceError> { }
public partial struct SyntaxErrorConstructor : IErrorConstructor<SyntaxErrorConstructor, SyntaxError> { }
public partial struct TypeErrorConstructor : IErrorConstructor<TypeErrorConstructor, TypeError> { }
public partial struct URIErrorConstructor : IErrorConstructor<URIErrorConstructor, URIError> { }

public partial interface IGlobal
{
    ErrorConstructor Error { get; set; }
    EvalErrorConstructor EvalError { get; set; }
    RangeErrorConstructor RangeError { get; set; }
    ReferenceErrorConstructor ReferenceError { get; set; }
    SyntaxErrorConstructor SyntaxError { get; set; }
    TypeErrorConstructor TypeError { get; set; }
    URIErrorConstructor URIError { get; set; }
}

public partial interface IJSON<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, IJSON<TSelf>
{
    Any parse(String text, Function<Any /*this*/, String /*key*/, Any /*value*/, Any>? reviver = null);
    String stringify(Any value, Function<Any /*this*/, String /*key*/, Any /*value*/, Any>? replacer = null, OneOf<String, Number>? space = null);
    String stringify(Any value, Nullable<Array<OneOf<Number, String>>>? replacer = null, OneOf<String, Number>? space = null);
}

public partial struct JSON : IJSON<JSON>
{
}

public partial interface IGlobal
{
    JSON JSON { get; set; }
}

public partial interface IReadonlyArray<TSelf, T> : IJSValueHolder<TSelf>
    where TSelf : struct, IReadonlyArray<TSelf, T>
    where T : struct, IJSValueHolder<T>
{
    Number length { get; }
    String toString();
    String toLocaleString();
    Array<T> concat(params ConcatArray<T>[] items);
    Array<T> concat(params OneOf<T, ConcatArray<T>>[] items);
    String join(String? separator = null);
    Array<T> slice(Number? start = null, Number? end = null);
    Number indexOf(T searchElement, Number? fromIndex = null);
    Number lastIndexOf(T searchElement, Number? fromIndex = null);
    //TODO: How to simulate extends?
    //TODO: How to simulate "is"?
    //Original: every<S extends T>(predicate: (value: T, index: number, array: readonly T[]) => value is S, thisArg?: any): this is readonly S[];
    Boolean every<S>(Function<T /*value*/, Number /*index*/, ReadonlyArray<T> /*array*/, Unknown> predicate, Any? thisArg = null)
        where S : struct, IJSValueHolder<S>;
    Boolean every(Function<T /*value*/, Number /*index*/, ReadonlyArray<T> /*array*/, Unknown> predicate, Any? thisArg = null);
    Boolean some(Function<T /*value*/, Number /*index*/, ReadonlyArray<T> /*array*/, Unknown> predicate, Any? thisArg = null);
    Void forEach(Function<T /*value*/, Number /*index*/, ReadonlyArray<T> /*array*/, Void> callbackfn, Any? thisArg = null);
    Array<U> map<U>(Function<T /*value*/, Number /*index*/, ReadonlyArray<T> /*array*/, U> callbackfn, Any? thisArg = null)
        where U : struct, IJSValueHolder<U>;
    //TODO: How to simulate extends?
    //TODO: How to simulate "is"?
    //Original: filter<S extends T>(predicate: (value: T, index: number, array: readonly T[]) => value is S, thisArg?: any): S[];
    Array<S> filter<S>(Function<T /*value*/, Number /*index*/, ReadonlyArray<T> /*array*/, Unknown> predicate, Any? thisArg = null)
        where S : struct, IJSValueHolder<S>;
    Array<T> filter(Function<T /*value*/, Number /*index*/, ReadonlyArray<T> /*array*/, Unknown> predicate, Any? thisArg = null);
    T reduce(Function<T /*previousValue*/, T /*currentValue*/, Number /*currentIndex*/, ReadonlyArray<T> /*array*/, T> callbackfn);
    T reduce(Function<T /*previousValue*/, T /*currentValue*/, Number /*currentIndex*/, ReadonlyArray<T> /*array*/, T> callbackfn, T initialValue);
    U reduce<U>(Function<U /*previousValue*/, T /*currentValue*/, Number /*currentIndex*/, ReadonlyArray<T> /*array*/, U> callbackfn, U initialValue)
        where U : struct, IJSValueHolder<U>;
    T reduceRight(Function<T /*previousValue*/, T /*currentValue*/, Number /*currentIndex*/, ReadonlyArray<T> /*array*/, T> callbackfn);
    T reduceRight(Function<T /*previousValue*/, T /*currentValue*/, Number /*currentIndex*/, ReadonlyArray<T> /*array*/, T> callbackfn, T initialValue);
    U reduceRight<U>(Function<U /*previousValue*/, T /*currentValue*/, Number /*currentIndex*/, ReadonlyArray<T> /*array*/, U> callbackfn, U initialValue)
        where U : struct, IJSValueHolder<U>;
    T this[Number index] { get; }
}

public partial struct ReadonlyArray<T> : IReadonlyArray<ReadonlyArray<T>, T>
    where T : struct, IJSValueHolder<T>
{
}

public partial interface IConcatArray<TSelf, T> : IJSValueHolder<TSelf>
    where TSelf : struct, IConcatArray<TSelf, T>
    where T : struct, IJSValueHolder<T>
{
    Number length { get; }
    T this[Number n] { get; }
    String join(String? separator = null);
    Array<T> slice(Number? start = null, Number? end = null);
}

public partial struct ConcatArray<T> : IConcatArray<ConcatArray<T>, T>
    where T : struct, IJSValueHolder<T>
{
}

public partial interface IArray<TSelf, T> : IJSValueHolder<TSelf>
    where TSelf : struct, IArray<TSelf, T>
    where T : struct, IJSValueHolder<T>
{
    Number length { get; set; }
    String toString();
    String toLocaleString();
    T? pop();
    Number push(params T[] items);
    Array<T> concat(params ConcatArray<T>[] items);
    Array<T> concat(params OneOf<T, ConcatArray<T>>[] items);
    String join(String? separator = null);
    Array<T> reverse();
    T? shift();
    Array<T> slice(Number? start = null, Number? end = null);
    //TODO: how to return "this"? Should it be ref?
    Array<T> sort(Function<T /*a*/, T/*b*/, Number>? compareFn = null);
    Array<T> splice(Number start, Number? deleteCount = null);
    Array<T> splice(Number start, Number deleteCount, params T[] items);
    Number unshift(params T[] items);
    Number indexOf(T searchElement, Number? fromIndex = null);
    Number lastIndexOf(T searchElement, Number? fromIndex = null);
    Boolean every<S>(Function<T /*value*/, Number /*index*/, Array<T> /*array*/, Unknown> predicate, Any? thisArg = null)
        where S : struct, IJSValueHolder<S>;
    Boolean every(Function<T /*value*/, Number /*index*/, Array<T> /*array*/, Unknown> predicate, Any? thisArg = null);
    Boolean some(Function<T /*value*/, Number /*index*/, Array<T> /*array*/, Unknown> predicate, Any? thisArg = null);
    Void forEach(Function<T /*value*/, Number /*index*/, Array<T> /*array*/, Void> callbackfn, Any? thisArg = null);
    Array<U> map<U>(Function<T /*value*/, Number /*index*/, Array<T> /*array*/, U> callbackfn, Any? thisArg = null)
        where U : struct, IJSValueHolder<U>;
    Array<S> filter<S>(Function<T /*value*/, Number /*index*/, Array<T> /*array*/, Unknown> predicate, Any? thisArg = null)
        where S : struct, IJSValueHolder<S>;
    Array<T> filter(Function<T /*value*/, Number /*index*/, Array<T> /*array*/, Unknown> predicate, Any? thisArg = null);
    T reduce(Function<T /*previousValue*/, T /*currentValue*/, Number /*currentIndex*/, Array<T> /*array*/, T> callbackfn);
    T reduce(Function<T /*previousValue*/, T /*currentValue*/, Number /*currentIndex*/, Array<T> /*array*/, T> callbackfn, T initialValue);
    U reduce<U>(Function<U /*previousValue*/, T /*currentValue*/, Number /*currentIndex*/, Array<T> /*array*/, U> callbackfn, U initialValue)
        where U : struct, IJSValueHolder<U>;
    T reduceRight(Function<T /*previousValue*/, T /*currentValue*/, Number /*currentIndex*/, Array<T> /*array*/, T> callbackfn);
    T reduceRight(Function<T /*previousValue*/, T /*currentValue*/, Number /*currentIndex*/, Array<T> /*array*/, T> callbackfn, T initialValue);
    U reduceRight<U>(Function<U /*previousValue*/, T /*currentValue*/, Number /*currentIndex*/, Array<T> /*array*/, U> callbackfn, U initialValue)
        where U : struct, IJSValueHolder<U>;
    T this[Number index] { get; set; }
}

public partial struct Array<T> : IArray<Array<T>, T>
    where T : struct, IJSValueHolder<T>
{
}

public partial interface IArrayConstructor<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, IArrayConstructor<TSelf>
{
    Array<Any> New(Number? arrayLength = null);
    Array<T> New<T>(Number arrayLength) where T : struct, IJSValueHolder<T>;
    Array<T> New<T>(params T[] items) where T : struct, IJSValueHolder<T>;
    Array<Any> Call(Number? arrayLength = null);
    Array<T> Call<T>(Number arrayLength) where T : struct, IJSValueHolder<T>;
    Array<T> Call<T>(params T[] items) where T : struct, IJSValueHolder<T>;
    //TODO: What does it mean that he result must be "arg is any[]"
    Boolean isArray(Any arg);
    Array<Any> prototype { get; }
}

public partial struct ArrayConstructor : IArrayConstructor<ArrayConstructor>
{
}

public partial interface IGlobal
{
    ArrayConstructor Array { get; set; }
}

public partial interface ITypedPropertyDescriptor<TSelf, T> : IJSValueHolder<TSelf>
    where TSelf : struct, ITypedPropertyDescriptor<TSelf, T>
    where T : struct, IJSValueHolder<T>
{
    Boolean? enumerable { get; set; }
    Boolean? configurable { get; set; }
    Boolean? writable { get; set; }
    T? value { get; set; }
    Function<T>? get { get; set; }
    Function<T /*value*/, Void>? set { get; set; }
}

//TODO: Add Promise

public partial interface IArrayLike<TSelf, T> : IJSValueHolder<TSelf>
    where TSelf : struct, IArrayLike<TSelf, T>
    where T : struct, IJSValueHolder<T>
{
    Number length { get; }
    T this[Number index] { get; }
}

public partial struct ArrayLike<T> : IArrayLike<ArrayLike<T>, T>
    where T : struct, IJSValueHolder<T>
{
}

//TODO: Add the utility types

public partial interface IArrayBuffer<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, IArrayBuffer<TSelf>
{
    Number byteLength { get; }
    ArrayBuffer Slice(Number begin, Number? end = null);
}

public partial struct ArrayBuffer : IArrayBuffer<ArrayBuffer>
{
}

public partial interface IArrayBufferTypes<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, IArrayBufferTypes<TSelf>
{
    ArrayBuffer ArrayBuffer { get; set; }
}

public partial struct ArrayBufferTypes : IArrayBufferTypes<ArrayBufferTypes>
{
}

//type ArrayBufferLike = ArrayBufferTypes[keyof ArrayBufferTypes];

public partial interface IArrayBufferConstructor<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, IArrayBufferConstructor<TSelf>
{
    ArrayBuffer prototype { get; }
    ArrayBuffer New(Number byteLength);
    //TODO: arg is ArrayBufferView
    Boolean isView(Any arg);
}

public partial struct ArrayBufferConstructor : IArrayBufferConstructor<ArrayBufferConstructor>
{
}

public partial interface IGlobal
{
    ArrayBufferConstructor ArrayBuffer { get; set; }
}

public partial interface IArrayBufferView<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, IArrayBufferView<TSelf>
{
    //TODO: ArrayBufferLike
    ArrayBuffer buffer { get; set; }
    Number byteLength { get; set; }
    Number byteOffset { get; set; }
}

public partial struct ArrayBufferView : IArrayBufferView<ArrayBufferView>
{
}

public partial interface IDataView<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, IDataView<TSelf>
{
    ArrayBuffer buffer { get; }
    Number byteLength { get; }
    Number byteOffset { get; }
    Number getFloat32(Number byteOffset, Boolean? littleEndian = null);
    Number getFloat64(Number byteOffset, Boolean? littleEndian = null);
    Number getInt8(Number byteOffset);
    Number getInt16(Number byteOffset, Boolean? littleEndian = null);
    Number getInt32(Number byteOffset, Boolean? littleEndian = null);
    Number getUint8(Number byteOffset);
    Number getUint16(Number byteOffset, Boolean? littleEndian = null);
    Number getUint32(Number byteOffset, Boolean? littleEndian = null);
    Void setFloat32(Number byteOffset, Number value, Boolean? littleEndian = null);
    Void setFloat64(Number byteOffset, Number value, Boolean? littleEndian = null);
    Void setInt8(Number byteOffset, Number value);
    Void setInt16(Number byteOffset, Number value, Boolean? littleEndian = null);
    Void setInt32(Number byteOffset, Number value, Boolean? littleEndian = null);
    Void setUint8(Number byteOffset, Number value);
    Void setUint16(Number byteOffset, Number value, Boolean? littleEndian = null);
    Void setUint32(Number byteOffset, Number value, Boolean? littleEndian = null);
}

public partial struct DataView : IDataView<DataView>
{
}

public partial interface IDataViewConstructor<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, IDataViewConstructor<TSelf>
{
    DataView prototype { get; }
    DataView New(ArrayBuffer buffer, Number? byteOffset = null, Number? byteLength = null);
}

public partial struct DataViewConstructor : IDataViewConstructor<DataViewConstructor>
{
}

public partial interface IGlobal
{
    DataViewConstructor DataView { get; set; }
}

public partial interface ITypedArray<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, ITypedArray<TSelf>
{
    Number BYTES_PER_ELEMENT { get; }
    ArrayBuffer buffer { get; }
    Number byteLength { get; }
    Number byteOffset { get; }
    TSelf copyWithin(Number target, Number start, Number? end = null);
    Boolean every(Function<Number /*value*/, Number /*index*/, TSelf /*array*/, Unknown> predicate, Any? thisArg = null);
    TSelf fill(Number value, Number? start = null, Number? end = null);
    TSelf filter(Function<Number /*value*/, Number /*index*/, TSelf /*array*/, Any> predicate, Any? thisArg = null);
    Number? find(Function<Number /*value*/, Number /*index*/, TSelf /*array*/, Boolean> predicate, Any? thisArg = null);
    Number findIndex(Function<Number /*value*/, Number /*index*/, TSelf /*array*/, Boolean> predicate, Any? thisArg = null);
    Void forEach(Function<Number /*value*/, Number /*index*/, TSelf /*array*/, Void> callbackfn, Any? thisArg = null);
    Number indexOf(Number searchElement, Number? fromIndex = null);
    String join(String? separator = null);
    Number lastIndexOf(Number searchElement, Number? fromIndex = null);
    Number length { get; }
    TSelf map(Function<Number /*value*/, Number /*index*/, TSelf /*array*/, Number> callbackfn, Any? thisArg = null);
    Number reduce(Function<Number /*previousValue*/, Number /*currentValue*/, Number/*currentIndex*/, TSelf /*array*/, Number> callbackfn);
    Number reduce(Function<Number /*previousValue*/, Number /*currentValue*/, Number/*currentIndex*/, TSelf /*array*/, Number> callbackfn, Number initialValue);
    U reduce<U>(Function<U /*previousValue*/, Number /*currentValue*/, Number /*currentIndex*/, TSelf /*array*/, U> callbackfn, U initialValue)
        where U : struct, IJSValueHolder<U>;
    Number reduceRight(Function<Number /*previousValue*/, Number /*currentValue*/, Number/*currentIndex*/, TSelf /*array*/, Number> callbackfn);
    Number reduceRight(Function<Number /*previousValue*/, Number /*currentValue*/, Number/*currentIndex*/, TSelf /*array*/, Number> callbackfn, Number initialValue);
    U reduceRight<U>(Function<Number /*previousValue*/, Number /*currentValue*/, Number/*currentIndex*/, TSelf /*array*/, U> callbackfn, U initialValue)
        where U : struct, IJSValueHolder<U>;
    TSelf reverse();
    Void set(ArrayLike<Number> array, Number? offset = null);
    TSelf slice(Number? start = null, Number? end = null);
    Boolean some(Function<Number /*value*/, Number /*index*/, TSelf /*array*/, Unknown> predicate, Any? thisArg = null);
    TSelf sort(Function<Number /*a*/, Number /*b*/, Number>? compareFn = null);
    TSelf subarray(Number? begin = null, Number? end = null);
    String toLocaleString();
    String toString();
    TSelf valueOf();
    Number this[Number index] { get; set; }
}

public partial interface ITypedArrayConstructor<TSelf, TTypedArray> : IJSValueHolder<TSelf>
    where TSelf : struct, ITypedArrayConstructor<TSelf, TTypedArray>
    where TTypedArray : struct, ITypedArray<TTypedArray>
{
    TTypedArray prototype { get; }
    TTypedArray New(Number length);
    TTypedArray New(OneOf<ArrayLike<Number>, ArrayBuffer> array);
    TTypedArray New(ArrayBuffer buffer, Number? byteOffset = null, Number? length = null);
    Number BYTES_PER_ELEMENT { get; }
    TTypedArray of(params Number[] items);
    TTypedArray from(ArrayLike<Number> arrayLike);
    TTypedArray from<T>(ArrayLike<T> arrayLike, Function<T /*v*/, Number /*k*/, Number> mapfn, Any? thisArg = null)
        where T : struct, IJSValueHolder<T>;
}

public partial struct Int8Array : ITypedArray<Int8Array> { }
public partial struct Uint8Array : ITypedArray<Uint8Array> { }
public partial struct Uint8ClampedArray : ITypedArray<Uint8ClampedArray> { }
public partial struct Int16Array : ITypedArray<Int16Array> { }
public partial struct Uint16Array : ITypedArray<Uint16Array> { }
public partial struct Int32Array : ITypedArray<Int32Array> { }
public partial struct Uint32Array : ITypedArray<Uint32Array> { }
public partial struct Float32Array : ITypedArray<Float32Array> { }
public partial struct Float64Array : ITypedArray<Float64Array> { }

public partial struct Int8ArrayConstructor : ITypedArrayConstructor<Int8ArrayConstructor, Int8Array> { }
public partial struct Uint8ArrayConstructor : ITypedArrayConstructor<Uint8ArrayConstructor, Int8Array> { }
public partial struct Uint8ClampedArrayConstructor : ITypedArrayConstructor<Uint8ClampedArrayConstructor, Uint8ClampedArray> { }
public partial struct Int16ArrayConstructor : ITypedArrayConstructor<Int16ArrayConstructor, Int16Array> { }
public partial struct Uint16ArrayConstructor : ITypedArrayConstructor<Uint16ArrayConstructor, Int16Array> { }
public partial struct Int32ArrayConstructor : ITypedArrayConstructor<Int32ArrayConstructor, Int32Array> { }
public partial struct Uint32ArrayConstructor : ITypedArrayConstructor<Uint32ArrayConstructor, Int32Array> { }
public partial struct Float32ArrayConstructor : ITypedArrayConstructor<Float32ArrayConstructor, Float32Array> { }
public partial struct Float64ArrayConstructor : ITypedArrayConstructor<Float64ArrayConstructor, Float64Array> { }

public partial interface IGlobal
{
    Int8ArrayConstructor Int8Array { get; set; }
    Uint8ArrayConstructor Uint8Array { get; set; }
    Uint8ClampedArrayConstructor Uint8ClampedArray { get; set; }
    Int16ArrayConstructor Int16Array { get; set; }
    Uint16ArrayConstructor Uint16Array { get; set; }
    Int32ArrayConstructor Int32Array { get; set; }
    Uint32ArrayConstructor Uint32Array { get; set; }
    Float32ArrayConstructor Float32Array { get; set; }
    Float64ArrayConstructor Float64Array { get; set; }
}

public partial class NameTable
{
    // TODO: Implement
    public static JSValue GetStringName(string value) => JSValue.Null;
}

public partial struct OneOf<T1, T2> : IJSValueHolder<OneOf<T1, T2>>
{
}

public partial struct OneOf<T1, T2, T3> : IJSValueHolder<OneOf<T1, T2, T3>>
{
}

public partial struct Function : IFunction<Function>
{
}

public partial struct Function<TResult> : IFunction<Function<TResult>>
    where TResult : struct, IJSValueHolder<TResult>
{
}

public partial struct Function<TArg0, TResult>
    : IFunction<Function<TArg0, TResult>>
    where TArg0 : struct, IJSValueHolder<TArg0>
    where TResult : struct, IJSValueHolder<TResult>
{
}


public partial struct Function<TArg0, TArg1, TResult>
    : IFunction<Function<TArg0, TArg1, TResult>>
    where TArg0 : struct, IJSValueHolder<TArg0>
    where TArg1 : struct, IJSValueHolder<TArg1>
    where TResult : struct, IJSValueHolder<TResult>
{
}

public partial struct Function<TArg0, TArg1, TArg2, TResult>
    : IFunction<Function<TArg0, TArg1, TArg2, TResult>>
    where TArg0 : struct, IJSValueHolder<TArg0>
    where TArg1 : struct, IJSValueHolder<TArg1>
    where TArg2 : struct, IJSValueHolder<TArg2>
    where TResult : struct, IJSValueHolder<TResult>
{
}

public partial struct Function<TArg0, TArg1, TArg2, TArg3, TResult>
    : IFunction<Function<TArg0, TArg1, TArg2, TArg3, TResult>>
    where TArg0 : struct, IJSValueHolder<TArg0>
    where TArg1 : struct, IJSValueHolder<TArg1>
    where TArg2 : struct, IJSValueHolder<TArg2>
    where TArg3 : struct, IJSValueHolder<TArg3>
    where TResult : struct, IJSValueHolder<TResult>
{
}

public partial struct Void : IJSValueHolder<Void>
{
}

public partial struct Any : IJSValueHolder<Any>
{
}

public partial struct Unknown : IJSValueHolder<Unknown>
{
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

public interface ITypedConstructor<TSelf, TObject> : IJSValueHolder<TSelf>
    where TSelf : struct, ITypedConstructor<TSelf, TObject>
    where TObject : struct, IJSValueHolder<TObject>
{
    static abstract TSelf Instance { get; }
}


public interface IFunction<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, IFunction<TSelf>
{
}

public partial struct Nullable<T> : IJSValueHolder<Nullable<T>>
{
}

[AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = true)]
sealed class TypedInterfaceAttribute : Attribute
{
    public string? Name { get; init; }

    [SetsRequiredMembers]
    public TypedInterfaceAttribute(string? name = null)
    {
        Name = name;
    }
}

[AttributeUsage(AttributeTargets.Struct, Inherited = false)]
sealed class TypedConstructorAttribute<T> : Attribute
{
    public Type ConstructorType { get; init; }

    [SetsRequiredMembers]
    public TypedConstructorAttribute()
    {
        ConstructorType = typeof(T);
    }
}

// To avoid conflicts with types in the System namespace
public partial struct Boolean { }
public partial struct Object { }
public partial struct Math { }
public partial struct String { }

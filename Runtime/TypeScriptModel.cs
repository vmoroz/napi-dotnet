using System.Linq;
using NodeApi.EcmaScript;

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
    String encodeURIComponent(OneOf<String, Number, Boolean> value);
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

//TODO: Add import types and template strings array

public partial interface IMath<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, IMath<TSelf>
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

public partial struct Math : IMath<Math>
{
}

public partial interface IGlobal
{
    Math Math { get; set; }
}

public partial interface IDate<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, IDate<TSelf>
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

public partial struct Date : IDate<Date>
{
}

public partial interface IDateConstructor<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, IDateConstructor<TSelf>
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

public partial struct DateConstructor : IDateConstructor<DateConstructor>
{
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

public partial interface IRegExp<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, IRegExp<TSelf>
{
    Nullable<RegExpExecArray> exec(String value);
    Boolean test(String value);
    String source { get; }
    Boolean global { get; }
    Boolean ignoreCase { get; }
    Boolean multiline { get; }
    Number lastIndex { get; set; }
}

public partial struct RegExp : IRegExp<RegExp>
{
}


public partial interface IRegExpConstructor<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, IRegExpConstructor<TSelf>
{
    RegExp New(OneOf<RegExp, String> pattern);
    RegExp New(String pattern, String? flags = null);
    RegExp Call(OneOf<RegExp, String> pattern);
    RegExp Call(String pattern, String? flags = null);
    RegExp prototype { get; }
}

public partial struct RegExpConstructor : IRegExpConstructor<RegExpConstructor>
{
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

public partial struct Error : IError<Error>
{
}

public partial interface IErrorConstructor<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, IErrorConstructor<TSelf>
{
    Error New(String? message = null);
    Error Call(String? message = null);
    Error prototype { get; }
}

public partial struct ErrorConstructor : IErrorConstructor<ErrorConstructor>
{
}

public partial interface IGlobal
{
    ErrorConstructor Error { get; set; }
}

public partial interface IEvalError<TSelf> : IError<TSelf>
    where TSelf : struct, IEvalError<TSelf>
{
}

public partial struct EvalError : IEvalError<EvalError>
{
}

public partial interface IEvalErrorConstructor<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, IEvalErrorConstructor<TSelf>
{
    EvalError New(String? message = null);
    EvalError Call(String? message = null);
    EvalError prototype { get; }
}

public partial struct EvalErrorConstructor : IEvalErrorConstructor<EvalErrorConstructor>
{
}

public partial interface IGlobal
{
    EvalErrorConstructor EvalError { get; set; }
}

public partial interface IRangeError<TSelf> : IError<TSelf>
    where TSelf : struct, IRangeError<TSelf>
{
}

public partial struct RangeError : IRangeError<RangeError>
{
}

public partial interface IRangeErrorConstructor<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, IRangeErrorConstructor<TSelf>
{
    RangeError New(String? message = null);
    RangeError Call(String? message = null);
    RangeError prototype { get; }
}

public partial struct RangeErrorConstructor : IRangeErrorConstructor<RangeErrorConstructor>
{
}

public partial interface IGlobal
{
    RangeErrorConstructor RangeError { get; set; }
}

public partial interface IReferenceError<TSelf> : IError<TSelf>
    where TSelf : struct, IReferenceError<TSelf>
{
}

public partial struct ReferenceError : IReferenceError<ReferenceError>
{
}

public partial interface IReferenceErrorConstructor<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, IReferenceErrorConstructor<TSelf>
{
    ReferenceError New(String? message = null);
    ReferenceError Call(String? message = null);
    ReferenceError prototype { get; }
}

public partial struct ReferenceErrorConstructor : IReferenceErrorConstructor<ReferenceErrorConstructor>
{
}

public partial interface IGlobal
{
    ReferenceErrorConstructor ReferenceError { get; set; }
}

public partial interface ISyntaxError<TSelf> : IError<TSelf>
    where TSelf : struct, ISyntaxError<TSelf>
{
}

public partial struct SyntaxError : ISyntaxError<SyntaxError>
{
}

public partial interface ISyntaxErrorConstructor<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, ISyntaxErrorConstructor<TSelf>
{
    SyntaxError New(String? message = null);
    SyntaxError Call(String? message = null);
    SyntaxError prototype { get; }
}

public partial struct SyntaxErrorConstructor : ISyntaxErrorConstructor<SyntaxErrorConstructor>
{
}

public partial interface IGlobal
{
    SyntaxErrorConstructor SyntaxError { get; set; }
}

public partial interface ITypeError<TSelf> : IError<TSelf>
    where TSelf : struct, ITypeError<TSelf>
{
}

public partial struct TypeError : ITypeError<TypeError>
{
}

public partial interface ITypeErrorConstructor<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, ITypeErrorConstructor<TSelf>
{
    TypeError New(String? message = null);
    TypeError Call(String? message = null);
    TypeError prototype { get; }
}

public partial struct TypeErrorConstructor : ITypeErrorConstructor<TypeErrorConstructor>
{
}

public partial interface IGlobal
{
    TypeErrorConstructor TypeError { get; set; }
}


public partial interface IURIError<TSelf> : IError<TSelf>
    where TSelf : struct, IURIError<TSelf>
{
}

public partial struct URIError : IURIError<URIError>
{
}

public partial interface IURIErrorConstructor<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, IURIErrorConstructor<TSelf>
{
    URIError New(String? message = null);
    URIError Call(String? message = null);
    URIError prototype { get; }
}

public partial struct URIErrorConstructor : IURIErrorConstructor<URIErrorConstructor>
{
}

public partial interface IGlobal
{
    URIErrorConstructor URIError { get; set; }
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






public partial interface IArrayBuffer<TSelf> : IJSValueHolder<TSelf>
    where TSelf : struct, IArrayBuffer<TSelf>
{
    Number byteLength { get; }
    ArrayBuffer Slice(Number begin, Number? end = null);
}

public partial struct ArrayBuffer : IArrayBuffer<ArrayBuffer>
{
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

public partial struct Number : IJSValueHolder<Number>
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

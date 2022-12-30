// ECMAScript APIs as they defined in
// https://github.com/microsoft/TypeScript/blob/main/src/lib/es5.d.ts

namespace NodeApi.TypedModel;

[TypedInterface]
public partial interface IGlobal
{
    @number NaN { get; set; }
    @number Infinity { get; set; }
    @any eval(@string text);
    @number parseInt(@string value, @number? radix = null);
    @number parseFloat(@string value);
    @boolean isNaN(@number value);
    @boolean isFinite(@number value);
    @string decodeURI(@string value);
    @string decodeURIComponent(@string value);
    @string encodeURI(@string value);
    @string encodeURIComponent(OneOf<@string, @number, @boolean> value);
}

public partial interface ISymbol<TSelf> : ITypedValue<TSelf>
    where TSelf : struct, ISymbol<TSelf>
{
    @string toString();
    @symbol valueOf();
}

public partial struct Symbol : ISymbol<Symbol> { }

//TODO: There must be a better way to declare type
// declare type PropertyKey = string | number | symbol;
public partial struct PropertyKey : ITypedValue<PropertyKey>
{
    public static implicit operator PropertyKey(@string value) => (PropertyKey)(JSValue)value;
    public static implicit operator PropertyKey(@number value) => (PropertyKey)(JSValue)value;
    public static implicit operator PropertyKey(Symbol value) => (PropertyKey)(JSValue)value;

    public static explicit operator @string(PropertyKey value) => (@string)value._value;
    public static explicit operator @number(PropertyKey value) => (@number)value._value;
    public static explicit operator Symbol(PropertyKey value) => (Symbol)value._value;
}

[TypedInterface]
public partial interface IPropertyDescriptor
{
    @boolean? configurable { get; set; }
    @boolean? enumerable { get; set; }
    @boolean? writable { get; set; }
    @any? value { get; set; }
    Function<@any>? get { get; set; }
    Function<@any, @void>? set { get; set; }
}

[TypedInterface]
public partial interface IPropertyDescriptorMap
{
    PropertyDescriptor this[PropertyKey key] { get; set; }
}

public partial interface IObject<TSelf> : ITypedValue<TSelf>
    where TSelf : struct, IObject<TSelf>
{
    Function constructor { get; set; }
    @string toString();
    @string toLocaleString();
    @object valueOf();
    @boolean hasOwnProperty(PropertyKey key);
    @boolean isPrototypeOf(@object value);
    @boolean propertyIsEnumerable(PropertyKey key);
}

public partial interface IObjectConstructor : ITypedConstructor<ObjectConstructor, Object>
{
    Object New(@any? value);
    @any Invoke();
    @any Invoke(@any value);
    Object prototype { get; }
    @any getPrototypeOf(@any obj);
    PropertyDescriptor? getOwnPropertyDescriptor(@any obj, PropertyKey key);
    Array<@string> getOwnPropertyNames(@any obj);
    @any create(Nullable<@object> obj);
    @any create(Nullable<@object> obj, Intersect<PropertyDescriptorMap, ThisType<@any>> properties);
    T defineProperty<T>(T obj, PropertyKey key, Intersect<PropertyDescriptor, ThisType<@any>> attributes) where T : struct, ITypedValue<T>;
    T defineProperties<T>(T obj, Intersect<PropertyDescriptorMap, ThisType<@any>> properties) where T : struct, ITypedValue<T>;
    T seal<T>(T obj) where T : struct, ITypedValue<T>;
    Readonly<T> freeze<T>(T obj) where T : struct, ITypedValue<T>;
    T preventExtensions<T>(T obj) where T : struct, ITypedValue<T>;
    @boolean isSealed(@any obj);
    @boolean isFrozen(@any obj);
    @boolean isExtensible(@any obj);
    Array<@string> keys(@object obj);
}

public partial struct Object : IObject<Object> { }

[GenerateInstanceInGlobalCache(nameof(Object))]
public partial struct ObjectConstructor : IObjectConstructor { }

public partial interface IGlobal
{
    ObjectConstructor Object { get; set; }
}

public partial interface IString<TSelf> : ITypedValue<TSelf>
    where TSelf : struct, IString<TSelf>
{
    @string toString();
    @string charAt(@number index);
    @string charCodeAt(@number index);
    @string concat(params @string[] strings);
    @number indexOf(@string searchString, @number? position);
    @number lastIndexOf(@string searchString, @number? position);
    @number localeCompare(@string value);
    Nullable<RegExpMatchArray> match(OneOf<@string, RegExp> regexp);
    @string replace(OneOf<@string, RegExp> searchValue, @string replaceValue);
    @string replace(OneOf<@string, RegExp> searchValue, Function<@string /*substring*/ /*...args: any[]*/, @string> replacer);
    @number search(OneOf<@string, RegExp> regexp);
    @string slice(@number? start = null, @number? end = null);
    Array<@string> split(OneOf<@string, RegExp> separator, @number? limit = null);
    @string substring(@number start, @number? end = null);
    @string toLowerCase();
    @string toLocaleLowerCase(OneOf<@string, Array<@string>>? locales = null);
    @string toUpperCase();
    @string toLocaleUpperCase(OneOf<@string, Array<@string>>? locales = null);
    @string trim();
    @number length { get; }
    @string valueOf();
    @string this[@number index] { get; }
}

public partial interface IStringConstructor : ITypedConstructor<StringConstructor, String>
{
    String New(@any? value = null);
    @string Invoke(@any? value = null);
    String prototype { get; }
    @string fromCharCode(params @number[] codes);
}

public partial struct String : IString<String> { }

[GenerateInstanceInGlobalCache(nameof(String))]
public partial struct StringConstructor : IStringConstructor { }

public partial interface IGlobal
{
    StringConstructor String { get; set; }
}

public partial interface IBoolean<TSelf> : ITypedValue<TSelf>
    where TSelf : struct, IBoolean<TSelf>
{
    @boolean valueOf();
}

public partial interface IBooleanConstructor : ITypedConstructor<BooleanConstructor, Boolean>
{
    Boolean New(@any? value = null);
    @boolean Invoke<T>(T? value) where T : struct, ITypedValue<T>;
    Boolean prototype { get; }
}

public partial struct Boolean : IBoolean<Boolean> { }

[GenerateInstanceInGlobalCache(nameof(Boolean))]
public partial struct BooleanConstructor : IBooleanConstructor { }

public partial interface IGlobal
{
    BooleanConstructor Boolean { get; set; }
}

public partial interface INumber<TSelf> : ITypedValue<TSelf>
    where TSelf : struct, INumber<TSelf>
{
    @string toString(@number? radix = null);
    @string toFixed(@number? fractionDigits = null);
    @string toExponential(@number? fractionDigits = null);
    @string toPrecision(@number? precision = null);
    @number valueOf();
}

public partial interface INumberConstructor : ITypedConstructor<NumberConstructor, Number>
{
    Number New(@any? value = null);
    @number Invoke(@any? value = null);
    Number prototype { get; }
    @number MAX_VALUE { get; }
    @number MIN_VALUE { get; }
    @number NaN { get; }
    @number NEGATIVE_INFINITY { get; }
    @number POSITIVE_INFINITY { get; }
}

public partial struct Number : INumber<Number> { }

[GenerateInstanceInGlobalCache(nameof(Number))]
public partial struct NumberConstructor : INumberConstructor { }

public partial interface IGlobal
{
    NumberConstructor Number { get; set; }
}

//TODO: Add import types and template strings array

[TypedInterface]
public partial interface IMath
{
    @number E { get; }
    @number LN10 { get; }
    @number LN2 { get; }
    @number LOG2E { get; }
    @number LOG10E { get; }
    @number PI { get; }
    @number SQRT1_2 { get; }
    @number SQRT2 { get; }
    @number abs(@number x);
    @number acos(@number x);
    @number asin(@number x);
    @number atan(@number x);
    @number atan2(@number y, @number x);
    @number ceil(@number x);
    @number cos(@number x);
    @number exp(@number x);
    @number floor(@number x);
    @number log(@number x);
    @number max(params @number[] values);
    @number min(params @number[] values);
    @number pow(@number x, @number y);
    @number random();
    @number round(@number x);
    @number sin(@number x);
    @number sqrt(@number x);
    @number tan(@number x);
}

public partial struct Math { }

public partial interface IGlobal
{
    Math Math { get; set; }
}

[TypedInterface]
public partial interface IDate
{
    @string toString();
    @string toDateString();
    @string toTimeString();
    @string toLocaleString();
    @string toLocaleDateString();
    @string toLocaleTimeString();
    @number valueOf();
    @number getTime();
    @number getFullYear();
    @number getUTCFullYear();
    @number getMonth();
    @number getUTCMonth();
    @number getDate();
    @number getUTCDate();
    @number getDay();
    @number getUTCDay();
    @number getHours();
    @number getUTCHours();
    @number getMinutes();
    @number getUTCMinutes();
    @number getSeconds();
    @number getUTCSeconds();
    @number getMilliseconds();
    @number getUTCMilliseconds();
    @number getTimezoneOffset();
    @number setTime(@number time);
    @number setMilliseconds(@number ms);
    @number setUTCMilliseconds(@number ms);
    @number setSeconds(@number sec, @number? ms = null);
    @number setUTCSeconds(@number sec, @number? ms = null);
    @number setMinutes(@number min, @number? sec = null, @number? ms = null);
    @number setUTCMinutes(@number min, @number? sec = null, @number? ms = null);
    @number setHours(@number hours, @number? min = null, @number? sec = null, @number? ms = null);
    @number setUTCHours(@number hours, @number? min = null, @number? sec = null, @number? ms = null);
    @number setDate(@number date);
    @number setUTCDate(@number date);
    @number setMonth(@number month, @number? date);
    @number setUTCMonth(@number month, @number? date = null);
    @number setFullYear(@number year, @number? month = null, @number? date = null);
    @number setUTCFullYear(@number year, @number? month = null, @number? date = null);
    @string toUTCString();
    @string toISOString();
    @string toJSON(@any? key = null);
}

[TypedInterface]
public partial interface IDateConstructor
{
    Date New();
    Date New(@number value);
    Date New(@string value);
    Date New(@number year, @number monthIndex, @number? date = null, @number? hours = null, @number? minutes = null, @number? seconds = null, @number? ms = null);
    @string Invoke();
    Date prototype { get; }
    @number parse(@string s);
    @number UTC(@number year, @number monthIndex, @number? date = null, @number? hours = null, @number? minutes = null, @number? seconds = null, @number? ms = null);
    @number now();
}

public partial interface IGlobal
{
    DateConstructor Date { get; set; }
}

public partial interface IRegExpMatchArray<TSelf> : IArray<TSelf, @string>
    where TSelf : struct, IRegExpMatchArray<TSelf>
{
    @number? index { get; set; }
    @string? input { get; set; }
}

public partial struct RegExpMatchArray : IRegExpMatchArray<RegExpMatchArray>
{
}

public partial interface IRegExpExecArray<TSelf> : IArray<TSelf, @string>
    where TSelf : struct, IRegExpExecArray<TSelf>
{
    @number? index { get; set; }
    @string? input { get; set; }
}

public partial struct RegExpExecArray : IRegExpExecArray<RegExpExecArray>
{
}

[TypedInterface]
public partial interface IRegExp
{
    Nullable<RegExpExecArray> exec(@string value);
    @boolean test(@string value);
    @string source { get; }
    @boolean global { get; }
    @boolean ignoreCase { get; }
    @boolean multiline { get; }
    @number lastIndex { get; set; }
}

[TypedInterface]
public partial interface IRegExpConstructor
{
    RegExp New(OneOf<RegExp, @string> pattern);
    RegExp New(@string pattern, @string? flags = null);
    RegExp Invoke(OneOf<RegExp, @string> pattern);
    RegExp Invoke(@string pattern, @string? flags = null);
    RegExp prototype { get; }
}

public partial interface IGlobal
{
    RegExpConstructor RegExp { get; set; }
}

public partial interface IError<TSelf> : ITypedValue<TSelf>
    where TSelf : struct, IError<TSelf>
{
    @string name { get; set; }
    @string message { get; set; }
    @string? stack { get; set; }
}

public partial interface IErrorConstructor<TSelf, TError> : ITypedValue<TSelf>
    where TSelf : struct, IErrorConstructor<TSelf, TError>
    where TError : struct, IError<TError>
{
    TError New(@string? message = null);
    TError Invoke(@string? message = null);
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

public partial interface IJSON<TSelf> : ITypedValue<TSelf>
    where TSelf : struct, IJSON<TSelf>
{
    @any parse(@string text, Function<@any /*this*/, @string /*key*/, @any /*value*/, @any>? reviver = null);
    @string stringify(@any value, Function<@any /*this*/, @string /*key*/, @any /*value*/, @any>? replacer = null, OneOf<@string, @number>? space = null);
    @string stringify(@any value, Nullable<Array<OneOf<@number, @string>>>? replacer = null, OneOf<@string, @number>? space = null);
}

public partial struct JSON : IJSON<JSON>
{
}

public partial interface IGlobal
{
    JSON JSON { get; set; }
}

public partial interface IReadonlyArray<TSelf, T> : ITypedValue<TSelf>
    where TSelf : struct, IReadonlyArray<TSelf, T>
    where T : struct, ITypedValue<T>
{
    @number length { get; }
    @string toString();
    @string toLocaleString();
    Array<T> concat(params ConcatArray<T>[] items);
    Array<T> concat(params OneOf<T, ConcatArray<T>>[] items);
    @string join(@string? separator = null);
    Array<T> slice(@number? start = null, @number? end = null);
    @number indexOf(T searchElement, @number? fromIndex = null);
    @number lastIndexOf(T searchElement, @number? fromIndex = null);
    //TODO: How to simulate extends?
    //TODO: How to simulate "is"?
    //Original: every<S extends T>(predicate: (value: T, index: number, array: readonly T[]) => value is S, thisArg?: any): this is readonly S[];
    @boolean every<S>(Function<T /*value*/, @number /*index*/, ReadonlyArray<T> /*array*/, @unknown> predicate, @any? thisArg = null)
        where S : struct, ITypedValue<S>;
    @boolean every(Function<T /*value*/, @number /*index*/, ReadonlyArray<T> /*array*/, @unknown> predicate, @any? thisArg = null);
    @boolean some(Function<T /*value*/, @number /*index*/, ReadonlyArray<T> /*array*/, @unknown> predicate, @any? thisArg = null);
    @void forEach(Function<T /*value*/, @number /*index*/, ReadonlyArray<T> /*array*/, @void> callbackfn, @any? thisArg = null);
    Array<U> map<U>(Function<T /*value*/, @number /*index*/, ReadonlyArray<T> /*array*/, U> callbackfn, @any? thisArg = null)
        where U : struct, ITypedValue<U>;
    //TODO: How to simulate extends?
    //TODO: How to simulate "is"?
    //Original: filter<S extends T>(predicate: (value: T, index: number, array: readonly T[]) => value is S, thisArg?: any): S[];
    Array<S> filter<S>(Function<T /*value*/, @number /*index*/, ReadonlyArray<T> /*array*/, @unknown> predicate, @any? thisArg = null)
        where S : struct, ITypedValue<S>;
    Array<T> filter(Function<T /*value*/, @number /*index*/, ReadonlyArray<T> /*array*/, @unknown> predicate, @any? thisArg = null);
    T reduce(Function<T /*previousValue*/, T /*currentValue*/, @number /*currentIndex*/, ReadonlyArray<T> /*array*/, T> callbackfn);
    T reduce(Function<T /*previousValue*/, T /*currentValue*/, @number /*currentIndex*/, ReadonlyArray<T> /*array*/, T> callbackfn, T initialValue);
    U reduce<U>(Function<U /*previousValue*/, T /*currentValue*/, @number /*currentIndex*/, ReadonlyArray<T> /*array*/, U> callbackfn, U initialValue)
        where U : struct, ITypedValue<U>;
    T reduceRight(Function<T /*previousValue*/, T /*currentValue*/, @number /*currentIndex*/, ReadonlyArray<T> /*array*/, T> callbackfn);
    T reduceRight(Function<T /*previousValue*/, T /*currentValue*/, @number /*currentIndex*/, ReadonlyArray<T> /*array*/, T> callbackfn, T initialValue);
    U reduceRight<U>(Function<U /*previousValue*/, T /*currentValue*/, @number /*currentIndex*/, ReadonlyArray<T> /*array*/, U> callbackfn, U initialValue)
        where U : struct, ITypedValue<U>;
    T this[@number index] { get; }
}

public partial struct ReadonlyArray<T> : IReadonlyArray<ReadonlyArray<T>, T>
    where T : struct, ITypedValue<T>
{
}

public partial interface IConcatArray<TSelf, T> : ITypedValue<TSelf>
    where TSelf : struct, IConcatArray<TSelf, T>
    where T : struct, ITypedValue<T>
{
    @number length { get; }
    T this[@number n] { get; }
    @string join(@string? separator = null);
    Array<T> slice(@number? start = null, @number? end = null);
}

public partial struct ConcatArray<T> : IConcatArray<ConcatArray<T>, T>
    where T : struct, ITypedValue<T>
{
}

public partial interface IArray<TSelf, T> : ITypedValue<TSelf>
    where TSelf : struct, IArray<TSelf, T>
    where T : struct, ITypedValue<T>
{
    @number length { get; set; }
    @string toString();
    @string toLocaleString();
    T? pop();
    @number push(params T[] items);
    Array<T> concat(params ConcatArray<T>[] items);
    Array<T> concat(params OneOf<T, ConcatArray<T>>[] items);
    @string join(@string? separator = null);
    Array<T> reverse();
    T? shift();
    Array<T> slice(@number? start = null, @number? end = null);
    //TODO: how to return "this"? Should it be ref?
    Array<T> sort(Function<T /*a*/, T/*b*/, @number>? compareFn = null);
    Array<T> splice(@number start, @number? deleteCount = null);
    Array<T> splice(@number start, @number deleteCount, params T[] items);
    @number unshift(params T[] items);
    @number indexOf(T searchElement, @number? fromIndex = null);
    @number lastIndexOf(T searchElement, @number? fromIndex = null);
    @boolean every<S>(Function<T /*value*/, @number /*index*/, Array<T> /*array*/, @unknown> predicate, @any? thisArg = null)
        where S : struct, ITypedValue<S>;
    @boolean every(Function<T /*value*/, @number /*index*/, Array<T> /*array*/, @unknown> predicate, @any? thisArg = null);
    @boolean some(Function<T /*value*/, @number /*index*/, Array<T> /*array*/, @unknown> predicate, @any? thisArg = null);
    @void forEach(Function<T /*value*/, @number /*index*/, Array<T> /*array*/, @void> callbackfn, @any? thisArg = null);
    Array<U> map<U>(Function<T /*value*/, @number /*index*/, Array<T> /*array*/, U> callbackfn, @any? thisArg = null)
        where U : struct, ITypedValue<U>;
    Array<S> filter<S>(Function<T /*value*/, @number /*index*/, Array<T> /*array*/, @unknown> predicate, @any? thisArg = null)
        where S : struct, ITypedValue<S>;
    Array<T> filter(Function<T /*value*/, @number /*index*/, Array<T> /*array*/, @unknown> predicate, @any? thisArg = null);
    T reduce(Function<T /*previousValue*/, T /*currentValue*/, @number /*currentIndex*/, Array<T> /*array*/, T> callbackfn);
    T reduce(Function<T /*previousValue*/, T /*currentValue*/, @number /*currentIndex*/, Array<T> /*array*/, T> callbackfn, T initialValue);
    U reduce<U>(Function<U /*previousValue*/, T /*currentValue*/, @number /*currentIndex*/, Array<T> /*array*/, U> callbackfn, U initialValue)
        where U : struct, ITypedValue<U>;
    T reduceRight(Function<T /*previousValue*/, T /*currentValue*/, @number /*currentIndex*/, Array<T> /*array*/, T> callbackfn);
    T reduceRight(Function<T /*previousValue*/, T /*currentValue*/, @number /*currentIndex*/, Array<T> /*array*/, T> callbackfn, T initialValue);
    U reduceRight<U>(Function<U /*previousValue*/, T /*currentValue*/, @number /*currentIndex*/, Array<T> /*array*/, U> callbackfn, U initialValue)
        where U : struct, ITypedValue<U>;
    T this[@number index] { get; set; }
}

public partial struct Array<T> : IArray<Array<T>, T>
    where T : struct, ITypedValue<T>
{
}

public partial interface IArrayConstructor<TSelf> : ITypedValue<TSelf>
    where TSelf : struct, IArrayConstructor<TSelf>
{
    Array<@any> New(@number? arrayLength = null);
    Array<T> New<T>(@number arrayLength) where T : struct, ITypedValue<T>;
    Array<T> New<T>(params T[] items) where T : struct, ITypedValue<T>;
    Array<@any> Invoke(@number? arrayLength = null);
    Array<T> Invoke<T>(@number arrayLength) where T : struct, ITypedValue<T>;
    Array<T> Invoke<T>(params T[] items) where T : struct, ITypedValue<T>;
    //TODO: What does it mean that he result must be "arg is any[]"
    @boolean isArray(@any arg);
    Array<@any> prototype { get; }
}

public partial struct ArrayConstructor : IArrayConstructor<ArrayConstructor>
{
}

public partial interface IGlobal
{
    ArrayConstructor Array { get; set; }
}

public partial interface ITypedPropertyDescriptor<TSelf, T> : ITypedValue<TSelf>
    where TSelf : struct, ITypedPropertyDescriptor<TSelf, T>
    where T : struct, ITypedValue<T>
{
    @boolean? enumerable { get; set; }
    @boolean? configurable { get; set; }
    @boolean? writable { get; set; }
    T? value { get; set; }
    Function<T>? get { get; set; }
    Function<T /*value*/, @void>? set { get; set; }
}

//TODO: Add Promise

public partial interface IArrayLike<TSelf, T> : ITypedValue<TSelf>
    where TSelf : struct, IArrayLike<TSelf, T>
    where T : struct, ITypedValue<T>
{
    @number length { get; }
    T this[@number index] { get; }
}

public partial struct ArrayLike<T> : IArrayLike<ArrayLike<T>, T>
    where T : struct, ITypedValue<T>
{
}

//TODO: Add the utility types

public partial interface IArrayBuffer<TSelf> : ITypedValue<TSelf>
    where TSelf : struct, IArrayBuffer<TSelf>
{
    @number byteLength { get; }
    ArrayBuffer Slice(@number begin, @number? end = null);
}

public partial struct ArrayBuffer : IArrayBuffer<ArrayBuffer>
{
}

public partial interface IArrayBufferTypes<TSelf> : ITypedValue<TSelf>
    where TSelf : struct, IArrayBufferTypes<TSelf>
{
    ArrayBuffer ArrayBuffer { get; set; }
}

public partial struct ArrayBufferTypes : IArrayBufferTypes<ArrayBufferTypes>
{
}

//type ArrayBufferLike = ArrayBufferTypes[keyof ArrayBufferTypes];

public partial interface IArrayBufferConstructor<TSelf> : ITypedValue<TSelf>
    where TSelf : struct, IArrayBufferConstructor<TSelf>
{
    ArrayBuffer prototype { get; }
    ArrayBuffer New(@number byteLength);
    //TODO: arg is ArrayBufferView
    @boolean isView(@any arg);
}

public partial struct ArrayBufferConstructor : IArrayBufferConstructor<ArrayBufferConstructor>
{
}

public partial interface IGlobal
{
    ArrayBufferConstructor ArrayBuffer { get; set; }
}

public partial interface IArrayBufferView<TSelf> : ITypedValue<TSelf>
    where TSelf : struct, IArrayBufferView<TSelf>
{
    //TODO: ArrayBufferLike
    ArrayBuffer buffer { get; set; }
    @number byteLength { get; set; }
    @number byteOffset { get; set; }
}

public partial struct ArrayBufferView : IArrayBufferView<ArrayBufferView>
{
}

public partial interface IDataView<TSelf> : ITypedValue<TSelf>
    where TSelf : struct, IDataView<TSelf>
{
    ArrayBuffer buffer { get; }
    @number byteLength { get; }
    @number byteOffset { get; }
    @number getFloat32(@number byteOffset, @boolean? littleEndian = null);
    @number getFloat64(@number byteOffset, @boolean? littleEndian = null);
    @number getInt8(@number byteOffset);
    @number getInt16(@number byteOffset, @boolean? littleEndian = null);
    @number getInt32(@number byteOffset, @boolean? littleEndian = null);
    @number getUint8(@number byteOffset);
    @number getUint16(@number byteOffset, @boolean? littleEndian = null);
    @number getUint32(@number byteOffset, @boolean? littleEndian = null);
    @void setFloat32(@number byteOffset, @number value, @boolean? littleEndian = null);
    @void setFloat64(@number byteOffset, @number value, @boolean? littleEndian = null);
    @void setInt8(@number byteOffset, @number value);
    @void setInt16(@number byteOffset, @number value, @boolean? littleEndian = null);
    @void setInt32(@number byteOffset, @number value, @boolean? littleEndian = null);
    @void setUint8(@number byteOffset, @number value);
    @void setUint16(@number byteOffset, @number value, @boolean? littleEndian = null);
    @void setUint32(@number byteOffset, @number value, @boolean? littleEndian = null);
}

public partial struct DataView : IDataView<DataView>
{
}

public partial interface IDataViewConstructor<TSelf> : ITypedValue<TSelf>
    where TSelf : struct, IDataViewConstructor<TSelf>
{
    DataView prototype { get; }
    DataView New(ArrayBuffer buffer, @number? byteOffset = null, @number? byteLength = null);
}

public partial struct DataViewConstructor : IDataViewConstructor<DataViewConstructor>
{
}

public partial interface IGlobal
{
    DataViewConstructor DataView { get; set; }
}

public partial interface ITypedArray<TSelf> : ITypedValue<TSelf>
    where TSelf : struct, ITypedArray<TSelf>
{
    @number BYTES_PER_ELEMENT { get; }
    ArrayBuffer buffer { get; }
    @number byteLength { get; }
    @number byteOffset { get; }
    TSelf copyWithin(@number target, @number start, @number? end = null);
    @boolean every(Function<@number /*value*/, @number /*index*/, TSelf /*array*/, @unknown> predicate, @any? thisArg = null);
    TSelf fill(@number value, @number? start = null, @number? end = null);
    TSelf filter(Function<@number /*value*/, @number /*index*/, TSelf /*array*/, @any> predicate, @any? thisArg = null);
    @number? find(Function<@number /*value*/, @number /*index*/, TSelf /*array*/, @boolean> predicate, @any? thisArg = null);
    @number findIndex(Function<@number /*value*/, @number /*index*/, TSelf /*array*/, @boolean> predicate, @any? thisArg = null);
    @void forEach(Function<@number /*value*/, @number /*index*/, TSelf /*array*/, @void> callbackfn, @any? thisArg = null);
    @number indexOf(@number searchElement, @number? fromIndex = null);
    @string join(@string? separator = null);
    @number lastIndexOf(@number searchElement, @number? fromIndex = null);
    @number length { get; }
    TSelf map(Function<@number /*value*/, @number /*index*/, TSelf /*array*/, @number> callbackfn, @any? thisArg = null);
    @number reduce(Function<@number /*previousValue*/, @number /*currentValue*/, @number/*currentIndex*/, TSelf /*array*/, @number> callbackfn);
    @number reduce(Function<@number /*previousValue*/, @number /*currentValue*/, @number/*currentIndex*/, TSelf /*array*/, @number> callbackfn, @number initialValue);
    U reduce<U>(Function<U /*previousValue*/, @number /*currentValue*/, @number /*currentIndex*/, TSelf /*array*/, U> callbackfn, U initialValue)
        where U : struct, ITypedValue<U>;
    @number reduceRight(Function<@number /*previousValue*/, @number /*currentValue*/, @number/*currentIndex*/, TSelf /*array*/, @number> callbackfn);
    @number reduceRight(Function<@number /*previousValue*/, @number /*currentValue*/, @number/*currentIndex*/, TSelf /*array*/, @number> callbackfn, @number initialValue);
    U reduceRight<U>(Function<@number /*previousValue*/, @number /*currentValue*/, @number/*currentIndex*/, TSelf /*array*/, U> callbackfn, U initialValue)
        where U : struct, ITypedValue<U>;
    TSelf reverse();
    @void set(ArrayLike<@number> array, @number? offset = null);
    TSelf slice(@number? start = null, @number? end = null);
    @boolean some(Function<@number /*value*/, @number /*index*/, TSelf /*array*/, @unknown> predicate, @any? thisArg = null);
    TSelf sort(Function<@number /*a*/, @number /*b*/, @number>? compareFn = null);
    TSelf subarray(@number? begin = null, @number? end = null);
    @string toLocaleString();
    @string toString();
    TSelf valueOf();
    @number this[@number index] { get; set; }
}

public partial interface ITypedArrayConstructor<TSelf, TTypedArray> : ITypedValue<TSelf>
    where TSelf : struct, ITypedArrayConstructor<TSelf, TTypedArray>
    where TTypedArray : struct, ITypedArray<TTypedArray>
{
    TTypedArray prototype { get; }
    TTypedArray New(@number length);
    TTypedArray New(OneOf<ArrayLike<@number>, ArrayBuffer> array);
    TTypedArray New(ArrayBuffer buffer, @number? byteOffset = null, @number? length = null);
    @number BYTES_PER_ELEMENT { get; }
    TTypedArray of(params @number[] items);
    TTypedArray from(ArrayLike<@number> arrayLike);
    TTypedArray from<T>(ArrayLike<T> arrayLike, Function<T /*v*/, @number /*k*/, @number> mapfn, @any? thisArg = null)
        where T : struct, ITypedValue<T>;
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

public partial struct Function : IFunction<Function>
{
}

public partial struct Function<TResult> : IFunction<Function<TResult>>
    where TResult : struct, ITypedValue<TResult>
{
}

public partial struct Function<TArg0, TResult>
    : IFunction<Function<TArg0, TResult>>
    where TArg0 : struct, ITypedValue<TArg0>
    where TResult : struct, ITypedValue<TResult>
{
}


public partial struct Function<TArg0, TArg1, TResult>
    : IFunction<Function<TArg0, TArg1, TResult>>
    where TArg0 : struct, ITypedValue<TArg0>
    where TArg1 : struct, ITypedValue<TArg1>
    where TResult : struct, ITypedValue<TResult>
{
}

public partial struct Function<TArg0, TArg1, TArg2, TResult>
    : IFunction<Function<TArg0, TArg1, TArg2, TResult>>
    where TArg0 : struct, ITypedValue<TArg0>
    where TArg1 : struct, ITypedValue<TArg1>
    where TArg2 : struct, ITypedValue<TArg2>
    where TResult : struct, ITypedValue<TResult>
{
}

public partial struct Function<TArg0, TArg1, TArg2, TArg3, TResult>
    : IFunction<Function<TArg0, TArg1, TArg2, TArg3, TResult>>
    where TArg0 : struct, ITypedValue<TArg0>
    where TArg1 : struct, ITypedValue<TArg1>
    where TArg2 : struct, ITypedValue<TArg2>
    where TArg3 : struct, ITypedValue<TArg3>
    where TResult : struct, ITypedValue<TResult>
{
}

/**
 * Marker for contextual 'this' type
 */
// interface ThisType<T> { }
public partial struct ThisType<T> : ITypedValue<ThisType<T>>
{
}

public interface IFunction<TSelf> : ITypedValue<TSelf>
    where TSelf : struct, IFunction<TSelf>
{
}

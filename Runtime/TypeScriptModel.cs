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
    where TSelf : IGlobal<TSelf>
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

public interface IJSValueHolder<TSelf> where TSelf : IJSValueHolder<TSelf>
{
    public static abstract explicit operator TSelf(JSValue value);
    public static abstract implicit operator JSValue(TSelf value);
}

public interface IFunction<TSelf> : IJSValueHolder<TSelf>
    where TSelf : IFunction<TSelf>
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
    where TSelf : IString<TSelf>
{
    /** Returns a string representation of a string. */
    // toString(): string;
    String toString();

    /**
     * Returns the character at the specified index.
     * @param pos The zero-based index of the desired character.
     */
    // charAt(pos: number): string;
    String charAt(Number index);

    /**
     * Returns the Unicode value of the character at the specified location.
     * @param index The zero-based index of the desired character. If there is no character at the specified index, NaN is returned.
     */
    // charCodeAt(index: number): number;
    String charCodeAt(Number index);

    /**
     * Returns a string that contains the concatenation of two or more strings.
     * @param strings The strings to append to the end of the string.
     */
    String concat(params String[] strings);

    /**
     * Returns the position of the first occurrence of a substring.
     * @param searchString The substring to search for in the string
     * @param position The index at which to begin searching the String object. If omitted, search starts at the beginning of the string.
     */
    // indexOf(searchString: string, position?: number): number;
    Number indexOf(String searchString, Number? position);

    /**
     * Returns the last occurrence of a substring in the string.
     * @param searchString The substring to search for.
     * @param position The index at which to begin searching. If omitted, the search begins at the end of the string.
     */
    // lastIndexOf(searchString: string, position?: number): number;
    Number lastIndexOf(String searchString, Number? position);

    /**
     * Determines whether two strings are equivalent in the current locale.
     * @param that String to compare to target string
     */
    // localeCompare(that: string): number;
    Number localeCompare(String value);

    /**
     * Matches a string with a regular expression, and returns an array containing the results of that search.
     * @param regexp A variable name or string literal containing the regular expression pattern and flags.
     */
    // match(regexp: string | RegExp): RegExpMatchArray | null;
    //TODO:Nullable<RegExpMatchArray> match(Or<String, RegExp> regexp);

    /**
     * Replaces text in a string, using a regular expression or search string.
     * @param searchValue A string or regular expression to search for.
     * @param replaceValue A string containing the text to replace. When the {@linkcode searchValue} is a `RegExp`, all matches are replaced if the `g` flag is set (or only those matches at the beginning, if the `y` flag is also present). Otherwise, only the first match of {@linkcode searchValue} is replaced.
     */
    // replace(searchValue: string | RegExp, replaceValue: string): string;
    //TODO:

    /**
     * Replaces text in a string, using a regular expression or search string.
     * @param searchValue A string to search for.
     * @param replacer A function that returns the replacement text.
     */
    // replace(searchValue: string | RegExp, replacer: (substring: string, ...args: any[]) => string): string;
    //TODO:

    /**
     * Finds the first substring match in a regular expression search.
     * @param regexp The regular expression pattern and applicable flags.
     */
    // search(regexp: string | RegExp): number;
    //TODO:

    /**
     * Returns a section of a string.
     * @param start The index to the beginning of the specified portion of stringObj.
     * @param end The index to the end of the specified portion of stringObj. The substring includes the characters up to, but not including, the character indicated by end.
     * If this value is not specified, the substring continues to the end of stringObj.
     */
    // slice(start?: number, end?: number): string;
    String slice(Number? start = null, Number? end = null);

    /**
     * Split a string into substrings using the specified separator and return them as an array.
     * @param separator A string that identifies character or characters to use in separating the string. If omitted, a single-element array containing the entire string is returned.
     * @param limit A value used to limit the number of elements returned in the array.
     */
    // split(separator: string | RegExp, limit?: number): string[];
    // TODO:

    /**
     * Returns the substring at the specified location within a String object.
     * @param start The zero-based index number indicating the beginning of the substring.
     * @param end Zero-based index number indicating the end of the substring. The substring includes the characters up to, but not including, the character indicated by end.
     * If end is omitted, the characters from start through the end of the original string are returned.
     */
    // substring(start: number, end?: number): string;
    String substring(Number start, Number? end = null);

    /** Converts all the alphabetic characters in a string to lowercase. */
    // toLowerCase(): string;
    String toLowerCase();

    /** Converts all alphabetic characters to lowercase, taking into account the host environment's current locale. */
    // toLocaleLowerCase(locales?: string | string[]): string;
    String toLocaleLowerCase(Union<String, Array<String>>? locales = null);

    /** Converts all the alphabetic characters in a string to uppercase. */
    // toUpperCase(): string;
    String toUpperCase();

    /** Returns a string where all alphabetic characters have been converted to uppercase, taking into account the host environment's current locale. */
    // toLocaleUpperCase(locales?: string | string[]): string;
    String toLocaleUpperCase(Union<String, Array<String>>? locales = null);

    /** Removes the leading and trailing white space and line terminator characters from a string. */
    // trim(): string;
    String trim();

    /** Returns the length of a String object. */
    // readonly length: number;
    Number length { get; }

    /** Returns the primitive value of the specified object. */
    // valueOf(): string;
    String valueOf();

    // readonly [index: number]: string;
    String this[Number index] { get; }
}

public partial struct String : IJSValueHolder<String>
{
    /** Returns a string representation of a string. */
    // toString(): string;
    public String toString()
         => (String)_value.Call(NameTable.toString);

    /**
     * Returns the character at the specified index.
     * @param pos The zero-based index of the desired character.
     */
    // charAt(pos: number): string;
    public String charAt(Number index)
         => (String)_value.Call(NameTable.charAt, index);

    /**
     * Returns the Unicode value of the character at the specified location.
     * @param index The zero-based index of the desired character. If there is no character at the specified index, NaN is returned.
     */
    // charCodeAt(index: number): number;
    public String charCodeAt(Number index)
         => (String)_value.Call(NameTable.charCodeAt, index);

    //    /**
    //     * Returns a string that contains the concatenation of two or more strings.
    //     * @param strings The strings to append to the end of the string.
    //     */
    // concat(...strings: string[]): string;
    //    public String concat(params String[] strings);

    // /**
    //  * Returns the position of the first occurrence of a substring.
    //  * @param searchString The substring to search for in the string
    //  * @param position The index at which to begin searching the String object. If omitted, search starts at the beginning of the string.
    //  */
    // indexOf(searchString: string, position?: number): number;

    // /**
    //  * Returns the last occurrence of a substring in the string.
    //  * @param searchString The substring to search for.
    //  * @param position The index at which to begin searching. If omitted, the search begins at the end of the string.
    //  */
    // lastIndexOf(searchString: string, position?: number): number;

    // /**
    //  * Determines whether two strings are equivalent in the current locale.
    //  * @param that String to compare to target string
    //  */
    // localeCompare(that: string): number;

    // /**
    //  * Matches a string with a regular expression, and returns an array containing the results of that search.
    //  * @param regexp A variable name or string literal containing the regular expression pattern and flags.
    //  */
    // match(regexp: string | RegExp): RegExpMatchArray | null;

    // /**
    //  * Replaces text in a string, using a regular expression or search string.
    //  * @param searchValue A string or regular expression to search for.
    //  * @param replaceValue A string containing the text to replace. When the {@linkcode searchValue} is a `RegExp`, all matches are replaced if the `g` flag is set (or only those matches at the beginning, if the `y` flag is also present). Otherwise, only the first match of {@linkcode searchValue} is replaced.
    //  */
    // replace(searchValue: string | RegExp, replaceValue: string): string;

    // /**
    //  * Replaces text in a string, using a regular expression or search string.
    //  * @param searchValue A string to search for.
    //  * @param replacer A function that returns the replacement text.
    //  */
    // replace(searchValue: string | RegExp, replacer: (substring: string, ...args: any[]) => string): string;

    // /**
    //  * Finds the first substring match in a regular expression search.
    //  * @param regexp The regular expression pattern and applicable flags.
    //  */
    // search(regexp: string | RegExp): number;

    // /**
    //  * Returns a section of a string.
    //  * @param start The index to the beginning of the specified portion of stringObj.
    //  * @param end The index to the end of the specified portion of stringObj. The substring includes the characters up to, but not including, the character indicated by end.
    //  * If this value is not specified, the substring continues to the end of stringObj.
    //  */
    // slice(start?: number, end?: number): string;

    // /**
    //  * Split a string into substrings using the specified separator and return them as an array.
    //  * @param separator A string that identifies character or characters to use in separating the string. If omitted, a single-element array containing the entire string is returned.
    //  * @param limit A value used to limit the number of elements returned in the array.
    //  */
    // split(separator: string | RegExp, limit?: number): string[];

    // /**
    //  * Returns the substring at the specified location within a String object.
    //  * @param start The zero-based index number indicating the beginning of the substring.
    //  * @param end Zero-based index number indicating the end of the substring. The substring includes the characters up to, but not including, the character indicated by end.
    //  * If end is omitted, the characters from start through the end of the original string are returned.
    //  */
    // substring(start: number, end?: number): string;

    // /** Converts all the alphabetic characters in a string to lowercase. */
    // toLowerCase(): string;

    // /** Converts all alphabetic characters to lowercase, taking into account the host environment's current locale. */
    // toLocaleLowerCase(locales?: string | string[]): string;

    // /** Converts all the alphabetic characters in a string to uppercase. */
    // toUpperCase(): string;

    // /** Returns a string where all alphabetic characters have been converted to uppercase, taking into account the host environment's current locale. */
    // toLocaleUpperCase(locales?: string | string[]): string;

    // /** Removes the leading and trailing white space and line terminator characters from a string. */
    // trim(): string;

    // /** Returns the length of a String object. */
    // readonly length: number;

    // // IE extensions
    // /**
    //  * Gets a substring beginning at the specified location and having the specified length.
    //  * @deprecated A legacy feature for browser compatibility
    //  * @param from The starting position of the desired substring. The index of the first character in the string is zero.
    //  * @param length The number of characters to include in the returned substring.
    //  */
    // substr(from: number, length?: number): string;

    // /** Returns the primitive value of the specified object. */
    // valueOf(): string;

    // readonly [index: number]: string;
}

//interface StringConstructor {
//    new(value?: any): String;
//    (value?: any): string;
//    readonly prototype: String;
//    fromCharCode(...codes: number[]): string;
//}

///**
// * Allows manipulation and formatting of text strings and determination and location of substrings within strings.
// */
//declare var String: StringConstructor;

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
    public static JSValue slice => GetStringName(nameof(slice));
    public static JSValue charAt => GetStringName(nameof(charAt));
    public static JSValue charCodeAt => GetStringName(nameof(charCodeAt));
    public static JSValue concat => GetStringName(nameof(concat));
    public static JSValue indexOf => GetStringName(nameof(indexOf));
    public static JSValue lastIndexOf => GetStringName(nameof(lastIndexOf));
    public static JSValue localeCompare => GetStringName(nameof(localeCompare));
    public static JSValue match => GetStringName(nameof(match));
    public static JSValue replace => GetStringName(nameof(replace));
    public static JSValue search => GetStringName(nameof(search));
    public static JSValue split => GetStringName(nameof(split));
    public static JSValue substring => GetStringName(nameof(substring));
    public static JSValue toLowerCase => GetStringName(nameof(toLowerCase));
    public static JSValue toLocaleLowerCase => GetStringName(nameof(toLocaleLowerCase));
    public static JSValue toUpperCase => GetStringName(nameof(toUpperCase));
    public static JSValue toLocaleUpperCase => GetStringName(nameof(toLocaleUpperCase));
    public static JSValue trim => GetStringName(nameof(trim));
    public static JSValue length => GetStringName(nameof(length));
    public static JSValue substr => GetStringName(nameof(substr));
    public static JSValue fromCharCode => GetStringName(nameof(fromCharCode));


    // TODO: Implement
    public static JSValue GetStringName(string value) => JSValue.Null;
}

public partial struct Union<T1, T2> : IJSValueHolder<Union<T1, T2>>
{
}

public partial struct Union<T1, T2, T3> : IJSValueHolder<Union<T1, T2, T3>>
{
}

public partial struct Boolean : IJSValueHolder<Boolean>
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

// Common definitions for the JavaScript typed model based on TypeScript

using System;
using System.Reflection;

namespace NodeApi.TypedModel;

public interface ITypedValue<TSelf> where TSelf : struct, ITypedValue<TSelf>
{
    public static abstract explicit operator TSelf(JSValue value);
    public static abstract implicit operator JSValue(TSelf value);
    public static abstract implicit operator JSValue(TSelf? value);
}

public interface ITypedConstructor<TSelf, TObject> : ITypedValue<TSelf>
    where TSelf : struct, ITypedConstructor<TSelf, TObject>
    where TObject : struct, ITypedValue<TObject>
{
    static abstract TSelf Instance { get; }
}

[TypedValue(JSValueType.Undefined)]
public partial struct @undefined : ITypedValue<@undefined>
{
    public static @undefined Value => (@undefined)JSValue.Undefined;
}

[TypedValue(JSValueType.Null)]
public partial struct @null : ITypedValue<@null>
{
    public static @null Value => (@null)JSValue.Null;
}

[TypedValue(JSValueType.Boolean)]
public partial struct @boolean : ITypedValue<@boolean>
{
    public static @boolean True => (@boolean)true;
    public static @boolean False => (@boolean)false;

    public static implicit operator @boolean(bool value) => (@boolean)JSValue.GetBoolean(value);
    public static explicit operator bool(@boolean value) => value._value.GetValueBool();
}

[TypedValue(JSValueType.Number)]
public partial struct @number : ITypedValue<@number>
{
    public static implicit operator @number(sbyte value) => (@number)JSValue.CreateNumber(value);
    public static implicit operator @number(byte value) => (@number)JSValue.CreateNumber(value);
    public static implicit operator @number(short value) => (@number)JSValue.CreateNumber(value);
    public static implicit operator @number(ushort value) => (@number)JSValue.CreateNumber(value);
    public static implicit operator @number(int value) => (@number)JSValue.CreateNumber(value);
    public static implicit operator @number(uint value) => (@number)JSValue.CreateNumber(value);
    public static implicit operator @number(long value) => (@number)JSValue.CreateNumber(value);
    public static implicit operator @number(ulong value) => (@number)JSValue.CreateNumber(value);
    public static implicit operator @number(float value) => (@number)JSValue.CreateNumber(value);
    public static implicit operator @number(double value) => (@number)JSValue.CreateNumber(value);

    public static explicit operator sbyte(@number value) => (sbyte)value._value.GetValueInt32();
    public static explicit operator byte(@number value) => (byte)value._value.GetValueUInt32();
    public static explicit operator short(@number value) => (short)value._value.GetValueInt32();
    public static explicit operator ushort(@number value) => (ushort)value._value.GetValueUInt32();
    public static explicit operator int(@number value) => value._value.GetValueInt32();
    public static explicit operator uint(@number value) => value._value.GetValueUInt32();
    public static explicit operator long(@number value) => value._value.GetValueInt64();
    public static explicit operator ulong(@number value) => (ulong)value._value.GetValueInt64();
    public static explicit operator float(@number value) => (float)value._value.GetValueDouble();
    public static explicit operator double(@number value) => value._value.GetValueDouble();
}

[TypedValue(JSValueType.String)]
public partial struct @string : ITypedValue<@string>
{
    public static implicit operator @string(string value) => (@string)JSValue.CreateStringUtf16(value);
    public static implicit operator @string(char[] value) => (@string)JSValue.CreateStringUtf16(value);
    public static implicit operator @string(Span<char> value) => (@string)JSValue.CreateStringUtf16(value);
    public static implicit operator @string(ReadOnlySpan<char> value) => (@string)JSValue.CreateStringUtf16(value);
    public static implicit operator @string(byte[] value) => (@string)JSValue.CreateStringUtf8(value);
    public static implicit operator @string(Span<byte> value) => (@string)JSValue.CreateStringUtf8(value);
    public static implicit operator @string(ReadOnlySpan<byte> value) => (@string)JSValue.CreateStringUtf8(value);

    public static explicit operator string(@string value) => value._value.GetValueStringUtf16();
    public static explicit operator char[](@string value) => value._value.GetValueStringUtf16AsCharArray();
    public static explicit operator byte[](@string value) => value._value.GetValueStringUtf8();
}

[TypedValue(JSValueType.Symbol)]
public partial struct @symbol : ITypedValue<@symbol>
{
    public static implicit operator @symbol(@string value) => (@symbol)JSValue.CreateSymbol(value);
    public static implicit operator @symbol(string value) => (@string)JSValue.CreateSymbol(value);
    public static implicit operator @symbol(ReadOnlySpan<char> value) => (@string)JSValue.CreateSymbol(value);
    public static implicit operator @symbol(ReadOnlySpan<byte> value) => (@string)JSValue.CreateSymbol(value);
}

[TypedValue(JSValueType.Object)]
public partial struct @object : ITypedValue<@object>
{
    public @object() => _value = JSValue.CreateObject();
}

[TypedValue(JSValueType.BigInt)]
public partial struct @bigint : ITypedValue<@bigint>
{
    public @bigint(int signBit, ReadOnlySpan<ulong> words) => _value = JSValue.CreateBigInt(signBit, words);

    public (long Value, bool isLossless) GetInt64Value() => (_value.GetValueBigIntInt64(out bool isLossless), isLossless);
    public (ulong Value, bool isLossless) GetUInt64Value() => (_value.GetValueBigIntUInt64(out bool isLossless), isLossless);
    public (ulong[] Words, int SignBit) GetWords() => (_value.GetValueBigIntWords(out int signBit), signBit);

    public static implicit operator @bigint(long value) => (@bigint)JSValue.CreateBigInt(value);
    public static implicit operator @bigint(ulong value) => (@bigint)JSValue.CreateBigInt(value);

    public static explicit operator (long Value, bool isLossless)(@bigint value) => value.GetInt64Value();
    public static explicit operator (ulong Value, bool isLossless)(@bigint value) => value.GetUInt64Value();
    public static explicit operator (ulong[] Words, int SignBit)(@bigint value) => value.GetWords();
}

public partial struct @any : ITypedValue<@any> { }

public partial struct @unknown : ITypedValue<@unknown> { }

[TypedValue(JSValueType.Undefined)]
public partial struct @void : ITypedValue<@void> { }

[TypedValue(JSValueType.Undefined)]
public partial struct @never : ITypedValue<@never> { }

public partial struct Nullable<T> : ITypedValue<Nullable<T>> { }

public partial struct Readonly<T> : ITypedValue<Readonly<T>> { }

public partial struct OneOf<T1, T2> : ITypedValue<OneOf<T1, T2>> { }

public partial struct OneOf<T1, T2, T3> : ITypedValue<OneOf<T1, T2, T3>> { }

public partial struct OneOf<T1, T2, T3, T4> : ITypedValue<OneOf<T1, T2, T3, T4>> { }

public partial struct Intersect<T1, T2> : ITypedValue<Intersect<T1, T2>> { }

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


///**
// * Make all properties in T optional
// */
//type Partial<T> = {
//    [P in keyof T]?: T[P];
//};

///**
// * Make all properties in T required
// */
//type Required<T> = {
//    [P in keyof T] -?: T[P];
//};

///**
// * Make all properties in T readonly
// */
//type Readonly<T> = {
//    readonly [P in keyof T]: T[P];
//};

///**
// * From T, pick a set of properties whose keys are in the union K
// */
//type Pick<T, K extends keyof T> = {
//    [P in K]: T[P];
//};

///**
// * Construct a type with a set of properties K of type T
// */
//type Record<K extends keyof any, T> = {
//    [P in K]: T;
//};

///**
// * Exclude from T those types that are assignable to U
// */
//type Exclude<T, U> = T extends U? never : T;

///**
// * Extract from T those types that are assignable to U
// */
//type Extract<T, U> = T extends U? T : never;

///**
// * Construct a type with the properties of T except for those in type K.
// */
//type Omit<T, K extends keyof any> = Pick<T, Exclude<keyof T, K>>;

///**
// * Exclude null and undefined from T
// */
//type NonNullable<T> = T & { };

///**
// * Obtain the parameters of a function type in a tuple
// */
//type Parameters<T extends (...args: any) => any > = T extends(...args: infer P) => any ? P : never;

///**
// * Obtain the parameters of a constructor function type in a tuple
// */
//type ConstructorParameters<T extends abstract new (...args: any) => any> = T extends abstract new (...args: infer P) => any? P : never;

///**
// * Obtain the return type of a function type
// */
//type ReturnType<T extends (...args: any) => any > = T extends(...args: any) => infer R? R : any;

///**
// * Obtain the return type of a constructor function type
// */
//type InstanceType<T extends abstract new (...args: any) => any> = T extends abstract new (...args: any) => infer R ? R : any;

///**
// * Convert string literal type to uppercase
// */
//type Uppercase<S extends string> = intrinsic;

///**
// * Convert string literal type to lowercase
// */
//type Lowercase<S extends string> = intrinsic;

///**
// * Convert first character of string literal type to uppercase
// */
//type Capitalize<S extends string> = intrinsic;

///**
// * Convert first character of string literal type to lowercase
// */
//type Uncapitalize<S extends string> = intrinsic;

///**
// * Marker for contextual 'this' type
// */
//interface ThisType<T> { }

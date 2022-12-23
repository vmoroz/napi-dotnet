using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NodeApi.EcmaScript;

// ECMAScript APIs as they defined in
// https://github.com/microsoft/TypeScript/blob/main/src/lib/es5.d.ts

public partial struct Global
{
    private JSValue _value;

    public static explicit operator Global(JSValue value) => new Global { _value = value };
    public static implicit operator JSValue(Global value) => value._value;

    // es5.d.ts: declare var NaN: number;
    public Number NaN
    {
        get => (Number)_value.GetProperty(NameTable.NaN);
        set => _value.SetProperty(NameTable.NaN, value);
    }

    // es5.d.ts: declare var Infinity: number;
    public Number Infinity
    {
        get => (Number)_value.GetProperty(NameTable.Infinity);
        set => _value.SetProperty(NameTable.Infinity, value);
    }

    // es5.d.ts: declare function eval(x: string): any;
    public Any Eval(String value)
        => (Any)CallMethod(NameTable.eval, value);

    // es5.d.ts: declare function parseInt(string: string, radix?: number): number;
    public Number ParseInt(String value, Number? radix = null)
        => (Number)CallMethod(NameTable.parseInt, value, radix);


    // es5.d.ts: declare function parseFloat(string: string): number;
    public Number ParseFloat(String value)
        => (Number)CallMethod(NameTable.parseFloat, value);

    // es5.d.ts: declare function isNaN(number: number): boolean;
    public Boolean IsNaN(Number value)
        => (Boolean)CallMethod(NameTable.isNaN, value);

    // es5.d.ts: declare function isFinite(number: number): boolean;
    public Boolean IsFinite(Number value)
        => (Boolean)CallMethod(NameTable.isFinite, value);

    // es5.d.ts: declare function decodeURI(encodedURI: string): string;
    public String DecodeURI(String value)
        => (String)CallMethod(NameTable.decodeURI, value);

    // es5.d.ts: declare function decodeURIComponent(encodedURIComponent: string): string;
    public String DecodeURIComponent(String value)
        => (String)CallMethod(NameTable.decodeURIComponent, value);

    // es5.d.ts: declare function encodeURI(uri: string): string;
    public String EncodeURI(String value)
        => (String)CallMethod(NameTable.encodeURI, value);

    // es5.d.ts: declare function encodeURIComponent(uriComponent: string | number | boolean): string;
    public String EncodeURIComponent(Union<String, Number, Boolean> value)
        => (String)CallMethod(NameTable.encodeURIComponent, value);


    private JSValue CallMethod(JSValue name, params JSValue[] args)
        => _value.GetProperty(name).Call(_value, args);
}

public class NameTable
{
    public static JSValue NaN => GetStringName(nameof(NaN));
    public static JSValue Infinity => GetStringName(nameof(Infinity));
    public static JSValue eval => GetStringName(nameof(eval));
    public static JSValue parseInt => GetStringName(nameof(parseInt));
    public static JSValue parseFloat => GetStringName(nameof(parseFloat));
    public static JSValue isNaN => GetStringName(nameof(isNaN));
    public static JSValue isFinite => GetStringName(nameof(isFinite));
    public static JSValue decodeURI => GetStringName(nameof(decodeURI));
    public static JSValue decodeURIComponent => GetStringName(nameof(decodeURIComponent));
    public static JSValue encodeURI => GetStringName(nameof(encodeURI));
    public static JSValue encodeURIComponent => GetStringName(nameof(encodeURIComponent));


    // TODO: Implement
    public static JSValue GetStringName(string value) => JSValue.Null;
}

public struct Union<T1, T2, T3> {
    private JSValue _value;

    public static explicit operator Union<T1, T2, T3>(JSValue value) => new Union<T1, T2, T3> { _value = value };
    public static implicit operator JSValue(Union<T1, T2, T3> value) => value._value;
}

public struct Boolean
{
    private JSValue _value;

    public static explicit operator Boolean(JSValue value) => new Boolean { _value = value };
    public static implicit operator JSValue(Boolean value) => value._value;

    public static implicit operator JSValue(Boolean? value)
        => value is Boolean boolValue ? boolValue._value : JSValue.Undefined;
}

public struct Number
{
    private JSValue _value;

    public static explicit operator Number(JSValue value) => new Number { _value = value };
    public static implicit operator JSValue(Number value) => value._value;

    public static implicit operator JSValue(Number? value)
        => value is Number numberValue ? numberValue._value : JSValue.Undefined;
}

public struct Any
{
    private JSValue _value;

    public static explicit operator Any(JSValue value) => new Any { _value = value };
    public static implicit operator JSValue(Any value) => value._value;
}

public struct String
{
    private JSValue _value;

    public static explicit operator String(JSValue value) => new String { _value = value };
    public static implicit operator JSValue(String value) => value._value;
}

struct TsUnknown
{
}

struct TsAny
{
}

struct TsUndefined { }
struct TsNull { }
struct TsBoolean { }
struct TsNumber { }
struct TsString { }
struct TsObject { }
struct TsObject<T> { }
struct TsFunction { }
struct TsFunction<TResult> { }
struct TsFunction<T1, TResult> { }
struct TsFunction<T1, T2, TResult> { } // Etc.

struct TsArray<T> { }

struct TsBigInt { }
struct TsSymbol { }

struct TsUnion<T1, T2> { }
struct TsUnion<T1, T2, T3> { }
struct TsUnion<T1, T2, T3, T4> { }

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

struct TsOptional<T> { }
struct TsNullable<T> { }
struct TsNullish<T> { }

interface ITsObject { }
struct TsRecord<TKey, TValue> where TKey : ITsObject { }

[AttributeUsageAttribute(AttributeTargets.GenericParameter)]
class MySimpleAttribute : Attribute
{
    public string Name { get; set; }
    public MySimpleAttribute(string name) { this.Name = name; }
}

interface IFoo<T>
{
}

struct Foo<T> : IFoo<T>
{
}

struct Bar
{
    public int BarValue { get; set; }
}

class Baz
{
    public string BazValue { get; set; }

    public void Print() { }

    public string CustomValue => "World";

    public string CustomValue2 { get => "World"; set { } }
}

struct BarTag
{
}

struct BazTag
{
}

struct TS
{
    public static Bar As(IFoo<BarTag> _)
    {
        return new Bar();
    }

    public static Baz As(IFoo<BazTag> _)
    {
        return new Baz();
    }
}

struct SimpleTest
{
    public void Func((int a, int b) x) { }
    public void Test(TsObject<(TsString foo, TsOptional<TsString> bar)> objType)
    {
    }

    public void Test2(Foo<BarTag> objType)
    {
        var x = TS.As(objType);
        x.BarValue = 2;
    }

    public void Test3(Foo<BazTag> objType)
    {
        var x = TS.As(objType);
        x.BazValue = "Hello";

        TS.As(objType).Print();
        var w = TS.As(objType).CustomValue;
        TS.As(objType).CustomValue2 = "aaa";

    }
}

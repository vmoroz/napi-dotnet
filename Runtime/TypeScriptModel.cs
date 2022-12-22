using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeApi;

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
struct TsRecord<TKey, TValue> where TKey: ITsObject { }

[AttributeUsageAttribute(AttributeTargets.GenericParameter)]
class MySimpleAttribute : Attribute {
    public string Name { get; set; }
    public MySimpleAttribute(string name) { this.Name = name;}
}

struct SimpleTest
{
    void Func((int a, int b) x) { }
    void Test(TsObject<(TsString foo, TsOptional<TsString> bar)> objType)
    {

    }
}

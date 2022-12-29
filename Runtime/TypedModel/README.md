In Typed Model we define a type system for Node-API that mimics the TypeScript type system.

The C# type system may look similar to TypeScript. It allows to defined interfaces and other types.
But it also has a number of significant differences which cannot be mapped directly.
For example, a C# interface is an abstract type definition that must be implemented by a class or a struct.
In TypeScript an interface is a "typed view" to access already existing object properties.
Thus, to implement TypeScript-like type system we must define a set of idioms that can express it using C#.
 We also want it to be efficient and do not take extra runtime resources. For that we prefer use of structs vs classes.

== Interfaces.==

TypeScript interface is a typed view to a JavaScript object's properties and methods.
In Node-API Typed Model we represent an interface as a struct that wraps JSValue and provides properties
and methods to call the JSValue members.

public partial interface IMyInterfaceSpec<TSelf>
{
    @string name { get; }     // readonly property
    @number age { get; set; } // writable property
    @number doAction();       // a method
}

[TypedInterface]
public partial struct MyInterface : IMyInterfaceSpec<MyInterface> { }

In this definition we have a typed interface type MyInterface.
The MyInterface has the TypedInterface attribute and inherits from the IMyInterfaceSpec.
Both the struct and the spec interface are partial because code generator will generate
missing definitions:
 - the interface will be inherited from IJSValueHolder<TSelf> and TSelf will have type constraint:
    `where TSelf: struct, IMyInterfaceSpec<TSelf>`
 - the struct will implement all static abstract members from IJSValueHolder and the members defined in IMyInterfaceSpec.

Generator will skip generation of any members or definitions which are already defined in the code.


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

//TODO: Lower case types:
//TODO: any, bigint, boolean, never, null, number, object, string, symbol, undefined, unknown, void

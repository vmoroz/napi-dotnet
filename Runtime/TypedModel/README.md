# Node-API Typed Model

The C# Typed Model defines a type system for Node-API that mimics the TypeScript type system.

The C# type system may look similar to TypeScript. It has interfaces and other types.
But it also has a number of significant differences which cannot be mapped directly.
For example, a C# interface is an abstract type definition that must be implemented by a class or a struct.
In TypeScript an interface is a "typed view" to access already existing object properties.
Our goal is to implement a TypeScript-like type system we define a set of idioms that mimic the
TypeScript behavior.

What does the TypeScript give to developers?
- TypeScript is a set of definitions that work on top of untyped JavaScript values.
- The type definitions help Intellisense in code editors. E.g. they specify a set of object properties.
- TypeScript allows to define types that are just a union of specific strings or
  strings that match specific patterns.
- The type definitions define rules for conversion between types. It is a compilation error
  if we try to assign a value to a variable of incompatible type.

We define the C# Node-API typed model to address the same concerns and use the following approach:
- All types are structs that have a single `JSValue` field. They all are wrappers on top of
  untyped JS values. C# structs help to avoid unnecessary memory allocations. They do not
  support inheritance, but relationship between TypeScript types is much more complex and
  the inheritance would not help much anyway. All such structs must be inherited from
  the `IJSTypedValue` interface, or interfaces that are inherited from it.
- Each struct defines properties and methods that access properties and methods of the
  enclosed `JSValue`. They help to enable Intelisense in the code. We use code generator
  to simplify implementation of such definitions. The generator reads definitions from the 
  struct's base interfaces and generates them in the partial struct. All typed model
  structs must be partial to support it. The generator skips generation of properties
  and methods that are already defined in the code.
- Complex type definitions such as a string unions, or when a set of properties is derived from
  another type (e.g. `Readonly<T>`) are specified by special custom attributes.
  These attributes are used by code generator to generate definitions or produce compilation errors.
- All the type conversion rules are enforced by the code generator that acts in this case as code analyzer.
  These rules can be quite complex and different from the typical C# inheritance model.
  E.g. value of one type can be assign to a variable of another type if it have the same set of properties.

## Base types

JavaScript has the following types:
- `undefined`
- `null`
- `boolean`
- `number`
- `string`
- `symbol`
- `object`
- `function`
- `bigint`

In addition to them TypeScript defines the following types:

- `any`
- `never`
- `unknown`
- `void`

In the Typed Model we define structs with the same name and the following behavior.

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

# Node-API Typed Model

## NOTE

It is an early draft that speaks about work in progress.
Not everything what is described in this document is implemented yet.
Everything described in this document can be changed while we work on the implementation details.

## Overview

The C# Typed Model defines a type system for Node-API that mimics the TypeScript type system.

The C# type system may look similar to TypeScript. It has interfaces and other types.
But it also has a number of significant differences which cannot be mapped directly.
For example, a C# interface is an abstract type definition that must be implemented by a class or a struct.
In TypeScript an interface is a "typed view" to access already existing object properties.
Our goal is to implement a TypeScript-like type system where define a set of idioms that mimic the
TypeScript behavior.

What does the TypeScript give to developers?
- TypeScript is a set of definitions that work on top of untyped JavaScript values.
- The type definitions help Intellisense in code editors. E.g. they specify a set of object properties.
- TypeScript supports type definitions that are just a union of specific strings or
  strings that match specific patterns.
- The type definitions define rules for conversion between types. It is a compilation error
  if we try to assign a value to a variable of incompatible type.
- Typescript does not add any additional runtime checks on its own. Instead, it does the most work
  at the compile time by adding semantic to normal JavaScript code. (The only exception are the enums
  that are discouraged from being used in new code.)

We define the C# Node-API typed model to address the same concerns and use the following approach:
- All types are structs that have a single `JSValue` field. They all are wrappers on top of
  untyped JS values. C# structs help to avoid unnecessary memory allocations. They do not
  support inheritance, but relationships between TypeScript types are much more complex and
  the inheritance would not help much anyway. All such structs must be inherited from
  the `ITypedValue` interface, or interfaces that are inherited from it.
- Each struct defines properties and methods that access properties and methods of the
  enclosed `JSValue`. They help to enable Intelisense in the code. We use code generator
  to simplify implementation of such definitions. The generator reads definitions from the 
  struct's base interfaces and generates them in the partial struct. All typed model
  structs must be partial to support it. The generator skips generation of properties
  and methods that are already defined in the code.
- Complex type definitions such as a string unions, or when a set of properties is derived from
  another type (e.g. `Readonly<T>`) are specified by special custom attributes.
  These attributes modify code generator behavior while generating properties and methods.
  Some type definitions do not affect generated code, but used to analyze the code as we do not want to
  add additional runtime overhead.
- All the type conversion rules are enforced by the code analyzer.
  These rules can be quite complex and different from the typical C# inheritance model.
  E.g. a value of one type can be assign to a variable of another type if it has the same set of properties.

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

In addition to them, TypeScript defines the following types:

- `any`
- `never`
- `unknown`
- `void`

In the Typed Model we define structs with the same names.
Since these names start with a lower case letter, they must be prefixed with `@` symbol in C#:

### `@undefined`

The `@undefined` type represents the JavaScript `undefined` value.
Use the `@undefined.Value` static property to get the value of the `@undefined` type.
In the C# code we threat `@undefined.Value` the same way as the C# `null` type.
TypeScript types like `string | undefined` can be written in C# as `@string?`.
We use the `@undefined.Value` equivalence to `null` as a way to define optional object
properties and optional function parameters.

### `@null`

The `@null` type represents the JavaScript `null` value.
Use the `@null.Value` static property to get the value of the `@null` type.
Note that `@null` is not the same as C# `null`. We map C# `null` to `@undefined`.

### `@boolean`

The `@boolean` type represents the JavaScript `boolean` primitive type.
It has implicit conversion from C# `bool` type, and explicit conversion back to it.
Note the difference with the typed model `Boolean` type which is an object type
corresponding to JavaScript `Boolean` object.

### `@number`

The `@number` type represents the JavaScript `number` primitive type.
It has implicit conversions from C# numeric types `sbyte`, `byte`, `short`, `ushort`,
`int`, `uint`, `long`, `ulong`, `float`, and `double`, and then explicit conversion
back to them.
Note the difference with the typed model `Number` type which is an object type
corresponding to JavaScript `Number` object.

### `@string`

The `@string` type represents the JavaScript `string` primitive type.
It has implicit conversions from C# UTF-16 string types `string`, `char[]`,
`Span<char>`, `ReadOnlySpan<char>`, and from UTF-8 string types `byte[]`,
`Span<byte>`, `ReadOnlySpan<byte>`. Then, it has explicit conversion to `string`,
`char[]`, and `byte[]`.
Note the difference with the typed model `String` type which is an object type
corresponding to JavaScript `String` object.

### `@symbol`

The `@symbol` type represents the JavaScript `symbol` primitive type.
It has implicit conversions from `@string` and from the same C# types as `@string`.
There are no explicit conversions back to these types.
These implicit conversions use `Symbol.for(str)` JavaScript calls.
Use `Symbol.Invoke()`to call JavaScript `Symbol(str)` that produces unique symbols for each call.
Note that unlike other primitive types, the corresponding `Symbol` built-in object
does not have a constructor call to create `Symbol` object. Instead, it is used
as a factory for `@symbol` primitives.

### `@object`
[To be completed]

### TypeScript interfaces

TypeScript interface is a typed view to a JavaScript object's properties and methods.
In Node-API Typed Model we represent an interface as a struct that wraps `JSValue` and
provides properties and methods to access the `JSValue` members.

``` C#
public partial interface IMyInterface : ITypedValue<MyInterface>
{
    @string name { get; }     // readonly property
    @number age { get; set; } // writable property
    @number doAction();       // a method
}

public partial struct MyInterface : IMyInterfaceSpec<MyInterface> { }
```

In this definition we have a typed interface `MyInterface`.
The `MyInterface` inherits from the C# interface `IMyInterface`.
The `IMyInterface` is a specification used by code generator to provide implementation for
the `MyInterface` properties and methods that access wrapped up `JSValue` members.
Note that a C# interface is considered as a spec only when it eventually inherits from
the `ITypedValue` interface.

The C# interface `IMyInterface` is partial because we allow it to be augmented in other source files.
The struct `MyInterface` must be partial for generator to create partial implementation of its members.
Generator will skip generation of any members or definitions which are already defined in the code.

### TypeScript type aliases

Each type alias is a C# partial struct with special custom attribute.

### TypeScript type assertions

The type assertion help the code analyzer decide on correct use of types.
They are going to be implemented as a set of helper functions and custom attributes of user functions.
Code analyzer will enforce these rules.

### TypeScript literal types

Literal types can be represented as C# constants or static readonly field.
It is much more complex to represent unions of literal type in C#.
The current proposal is to use custom attributes to describe literal unions,
and then enforce them by code analyzer.

It is not clear yet how to represent object literals efficiently and if we want to do it at all.

### TypeScript enums

In modern TypeScript code developers are discourage from using them because they add significant
code to the runtime. Should we support them?

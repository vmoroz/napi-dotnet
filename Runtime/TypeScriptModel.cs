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

    // es5.d.ts: deprecated: declare function escape(string: string): string;
    // es5.d.ts: deprecated: declare function unescape(string: string): string;

    private JSValue CallMethod(JSValue name, params JSValue[] args)
        => _value.GetProperty(name).Call(_value, args);
}

public static class Ext
{
    public static JSValue CallMethod(this JSValue thisValue, JSValue name, params JSValue[] args)
        => thisValue.GetProperty(name).Call(thisValue, args);
}

// es5.d.ts: interface Symbol
public partial struct Symbol
{
    private JSValue _value;

    public static explicit operator Symbol(JSValue value) => new Symbol { _value = value };
    public static implicit operator JSValue(Symbol value) => value._value;

    // toString(): string;
    public new String ToString()
        => (String)_value.CallMethod(NameTable.toString);

    // valueOf(): symbol;
    public Symbol ValueOf()
        => (Symbol)_value.CallMethod(NameTable.valueOf);
}

// declare type PropertyKey = string | number | symbol;
public struct PropertyKey
{
    private JSValue _value;

    public static explicit operator PropertyKey(JSValue value) => new PropertyKey { _value = value };
    public static implicit operator JSValue(PropertyKey value) => value._value;

    public static implicit operator PropertyKey(String value) => new PropertyKey { _value = (JSValue)value };
    public static implicit operator PropertyKey(Number value) => new PropertyKey { _value = (JSValue)value };
    public static implicit operator PropertyKey(Symbol value) => new PropertyKey { _value = (JSValue)value };

    public static explicit operator String(PropertyKey value) => (String)value._value;
    public static explicit operator Number(PropertyKey value) => (Number)value._value;
    public static explicit operator Symbol(PropertyKey value) => (Symbol)value._value;
}

// interface PropertyDescriptor
public struct PropertyDescriptor
{
    private JSValue _value;

    public static explicit operator PropertyDescriptor(JSValue value) => new PropertyDescriptor { _value = value };
    public static implicit operator JSValue(PropertyDescriptor value) => value._value;

    public static explicit operator PropertyDescriptor?(JSValue value)
        => value.TypeOf() == JSValueType.Object ? (PropertyDescriptor)value : null;


    //configurable?: boolean;
    public Boolean? Configurable
    {
        get => (Boolean?)_value.GetProperty(NameTable.configurable);
        set => _value.SetProperty(NameTable.configurable, value);
    }

    //enumerable?: boolean;
    public Boolean? Enumerable
    {
        get => (Boolean?)_value.GetProperty(NameTable.enumerable);
        set => _value.SetProperty(NameTable.enumerable, value);
    }

    //value?: any;
    public Any? Value
    {
        get => (Any?)_value.GetProperty(NameTable.enumerable);
        set => _value.SetProperty(NameTable.enumerable, value);
    }

    //writable?: boolean;
    public Boolean? Writable
    {
        get => (Boolean?)_value.GetProperty(NameTable.writable);
        set => _value.SetProperty(NameTable.writable, value);
    }

    //get? (): any;
    public Function<Any>? Get
    {
        get => (Function<Any>?)_value.GetProperty(NameTable.get);
        set => _value.SetProperty(NameTable.get, value);
    }

    //set? (v: any): void;
    public Function<Any, Void>? Set
    {
        get => (Function<Any, Void>?)_value.GetProperty(NameTable.set);
        set => _value.SetProperty(NameTable.set, value);
    }
}

// interface PropertyDescriptorMap
public struct PropertyDescriptorMap
{
    private JSValue _value;

    public static explicit operator PropertyDescriptorMap(JSValue value) => new PropertyDescriptorMap { _value = value };
    public static implicit operator JSValue(PropertyDescriptorMap value) => value._value;

    public PropertyDescriptor this[PropertyKey key]
    {
        get => (PropertyDescriptor)_value.GetProperty(key);
        set => _value.SetProperty(key, value);
    }
}

public struct Nullable<T>
{
    private JSValue _value;

    public static explicit operator Nullable<T>(JSValue value) => new Nullable<T> { _value = value };
    public static implicit operator JSValue(Nullable<T> value) => value._value;
}

// interface Object
public struct Object
{
    private JSValue _value;

    public static explicit operator Object(JSValue value) => new Object { _value = value };
    public static implicit operator JSValue(Object value) => value._value;

    /** The initial value of Object.prototype.constructor is the standard built-in Object constructor. */
    //constructor: Function;
    public Function Constructor
    {
        get => (Function)_value.GetProperty(NameTable.constructor);
        set => _value.SetProperty(NameTable.constructor, value);
    }

    /** Returns a string representation of an object. */
    // toString(): string;
    public new String ToString()
        => (String)_value.CallMethod(NameTable.toString);

    /** Returns a date converted to a string using the current locale. */
    // toLocaleString(): string;
    public String ToLocaleString()
        => (String)_value.CallMethod(NameTable.toLocaleString);

    /** Returns the primitive value of the specified object. */
    // valueOf(): Object;
    public Object ValueOf()
        => (Object)_value.CallMethod(NameTable.valueOf);

    /**
     * Determines whether an object has a property with the specified name.
     * @param v A property name.
     */
    // hasOwnProperty(v: PropertyKey): boolean;
    public Boolean HasOwnProperty()
        => (Boolean)_value.CallMethod(NameTable.hasOwnProperty);

    /**
     * Determines whether an object exists in another object's prototype chain.
     * @param v Another object whose prototype chain is to be checked.
     */
    // isPrototypeOf(v: Object): boolean;
    public Boolean IsPrototypeOf(Object value)
        => (Boolean)_value.CallMethod(NameTable.isPrototypeOf, value);

    /**
     * Determines whether a specified property is enumerable.
     * @param v A property name.
     */
    // propertyIsEnumerable(v: PropertyKey): boolean;
    public Boolean PropertyIsEnumerable(PropertyKey key)
        => (Boolean)_value.CallMethod(NameTable.propertyIsEnumerable, key);
}

public partial struct Global
{
    public static Global Instance => (Global)JSValue.Global;
}

/**
 * Marker for contextual 'this' type
 */
// interface ThisType<T> { }
public struct ThisType<T>
{
    private JSValue _value;

    public static explicit operator ThisType<T>(JSValue value) => new ThisType<T> { _value = value };
    public static implicit operator JSValue(ThisType<T> value) => value._value;
}

public struct Intersect<T1, T2>
{
    private JSValue _value;

    public static explicit operator Intersect<T1, T2>(JSValue value) => new Intersect<T1, T2> { _value = value };
    public static implicit operator JSValue(Intersect<T1, T2> value) => value._value;
}

public struct Readonly<T> : IUnknown<Readonly<T>>
{
    private JSValue _value;

    public static explicit operator Readonly<T>(JSValue value) => new Readonly<T> { _value = value };
    public static implicit operator JSValue(Readonly<T> value) => value._value;
}

public interface IUnknown<TSelf> where TSelf : IUnknown<TSelf>
{
    public static abstract explicit operator TSelf(JSValue value);
    public static abstract implicit operator JSValue(TSelf value);
}

public interface IFunction<TSelf> : IUnknown<TSelf>
    where TSelf : IFunction<TSelf>
{
}

public struct ObjectConstructor
{
    private JSValue _value;

    public static explicit operator ObjectConstructor(JSValue value) => new ObjectConstructor { _value = value };
    public static implicit operator JSValue(ObjectConstructor value) => value._value;

    // new(value?: any): Object;
    public Object New(Any? value) =>
        (Object)Global.Instance.Object._value.CallAsConstructor(value);

    // (): any;
    public Any Call() =>
        (Any)Global.Instance.Object._value.Call();

    // (value: any): any;
    public Any Call(Any value) =>
        (Any)Global.Instance.Object._value.Call(value);

    /** A reference to the prototype for a class of objects. */
    // readonly prototype: Object;
    public Object Prototype => (Object)_value.GetProperty(NameTable.prototype);

    /**
     * Returns the prototype of an object.
     * @param o The object that references the prototype.
     */
    // getPrototypeOf(o: any): any;
    public Any PropertyIsEnumerable(Any value)
        => (Any)_value.CallMethod(NameTable.getPrototypeOf, value);

    /**
     * Gets the own property descriptor of the specified object.
     * An own property descriptor is one that is defined directly on the object and is not inherited from the object's prototype.
     * @param o Object that contains the property.
     * @param p Name of the property.
     */
    // getOwnPropertyDescriptor(o: any, p: PropertyKey): PropertyDescriptor | undefined;
    public PropertyDescriptor? GetOwnPropertyDescriptor(Any value, PropertyKey key)
        => (PropertyDescriptor?)_value.CallMethod(NameTable.getOwnPropertyDescriptor, value, key);

    /**
     * Returns the names of the own properties of an object. The own properties of an object are those that are defined directly
     * on that object, and are not inherited from the object's prototype. The properties of an object include both fields (objects) and functions.
     * @param o Object that contains the own properties.
     */
    // getOwnPropertyNames(o: any): string[];
    public Array<String> GetOwnPropertyNames(Any value)
        => (Array<String>)_value.CallMethod(NameTable.getOwnPropertyNames, value);

    /**
     * Creates an object that has the specified prototype or that has null prototype.
     * @param o Object to use as a prototype. May be null.
     */
    // create(o: object | null): any;
    public Any Create(Nullable<Object> value)
        => (Any)_value.CallMethod(NameTable.create, value);
    /**
     * Creates an object that has the specified prototype, and that optionally contains specified properties.
     * @param o Object to use as a prototype. May be null
     * @param properties JavaScript object that contains one or more property descriptors.
     */
    // create(o: object | null, properties: PropertyDescriptorMap & ThisType<any>): any;
    public Any Create(Nullable<Object> value, Intersect<PropertyDescriptorMap, ThisType<Any>> properties)
        => (Any)_value.CallMethod(NameTable.create, value, properties);

    /**
     * Adds a property to an object, or modifies attributes of an existing property.
     * @param o Object on which to add or modify the property. This can be a native JavaScript object (that is, a user-defined object or a built in object) or a DOM object.
     * @param p The property name.
     * @param attributes Descriptor for the property. It can be for a data property or an accessor property.
     */
    // defineProperty<T>(o: T, p: PropertyKey, attributes: PropertyDescriptor & ThisType<any>): T;
    public T DefineProperty<T>(T value, PropertyKey key, Intersect<PropertyDescriptor, ThisType<Any>> attributes)
        where T : struct, IUnknown<T>
        => (T)_value.CallMethod(NameTable.defineProperty, value, key, attributes);
    /**
     * Adds one or more properties to an object, and/or modifies attributes of existing properties.
     * @param o Object on which to add or modify the properties. This can be a native JavaScript object or a DOM object.
     * @param properties JavaScript object that contains one or more descriptor objects. Each descriptor object describes a data property or an accessor property.
     */
    // defineProperties<T>(o: T, properties: PropertyDescriptorMap & ThisType<any>): T;
    public T DefineProperties<T>(T value, Intersect<PropertyDescriptorMap, ThisType<Any>> properties)
        where T : struct, IUnknown<T>
        => (T)_value.CallMethod(NameTable.defineProperties, value, properties);

    /**
     * Prevents the modification of attributes of existing properties, and prevents the addition of new properties.
     * @param o Object on which to lock the attributes.
     */
    // seal<T>(o: T): T;
    public T Seal<T>(T value)
        where T : struct, IUnknown<T>
        => (T)_value.CallMethod(NameTable.seal, value);

    /**
     * Prevents the modification of existing property attributes and values, and prevents the addition of new properties.
     * @param f Object on which to lock the attributes.
     */
    // freeze<T extends Function>(f: T): T;
    //public T Freeze<T>(T value)
    //    where T : struct, IFunction<T>
    //    => (T)_value.CallMethod(NameTable.freeze, value);

    /**
     * Prevents the modification of existing property attributes and values, and prevents the addition of new properties.
     * @param o Object on which to lock the attributes.
     */
    // freeze<T extends {[idx: string]: U | null | undefined | object}, U extends string | bigint | number | boolean | symbol>(o: T): Readonly<T>;

    /**
     * Prevents the modification of existing property attributes and values, and prevents the addition of new properties.
     * @param o Object on which to lock the attributes.
     */
    // freeze<T>(o: T): Readonly<T>;
    public Readonly<T> Freeze<T>(T value)
        where T : struct, IUnknown<T>
        => (Readonly<T>)_value.CallMethod(NameTable.freeze, value);

    /**
     * Prevents the addition of new properties to an object.
     * @param o Object to make non-extensible.
     */
    // preventExtensions<T>(o: T): T;
    public T PreventExtensions<T>(T value)
        where T : struct, IUnknown<T>
        => (T)_value.CallMethod(NameTable.preventExtensions, value);

    /**
     * Returns true if existing property attributes cannot be modified in an object and new properties cannot be added to the object.
     * @param o Object to test.
     */
    // isSealed(o: any): boolean;
    public Boolean IsSealed(Any value)
        => (Boolean)_value.CallMethod(NameTable.isSealed, value);

    /**
     * Returns true if existing property attributes and values cannot be modified in an object, and new properties cannot be added to the object.
     * @param o Object to test.
     */
    // isFrozen(o: any): boolean;
    public Boolean IsFrozen(Any value)
        => (Boolean)_value.CallMethod(NameTable.isFrozen, value);
    /**
     * Returns a value that indicates whether new properties can be added to an object.
     * @param o Object to test.
     */
    // isExtensible(o: any): boolean;
    public Boolean IsExtensible(Any value)
        => (Boolean)_value.CallMethod(NameTable.isExtensible, value);
    /**
     * Returns the names of the enumerable string properties and methods of an object.
     * @param o Object that contains the properties and methods. This can be an object that you created or an existing Document Object Model (DOM) object.
     */
    // keys(o: object): string[];
    public Array<String> Keys(Object value)
        => (Array<String>)_value.CallMethod(NameTable.keys, value);
}

public partial struct Global
{
    /**
     * Provides functionality common to all JavaScript objects.
     */
    // declare var Object: ObjectConstructor;
    public ObjectConstructor Object
    {
        get => (ObjectConstructor)_value.GetProperty(NameTable.Object);
        set => _value.SetProperty(NameTable.Object, value);
    }
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
    public static JSValue toString => GetStringName(nameof(toString));
    public static JSValue valueOf => GetStringName(nameof(valueOf));
    public static JSValue configurable => GetStringName(nameof(configurable));
    public static JSValue enumerable => GetStringName(nameof(enumerable));
    public static JSValue value => GetStringName(nameof(value));
    public static JSValue writable => GetStringName(nameof(writable));
    public static JSValue get => GetStringName(nameof(get));
    public static JSValue set => GetStringName(nameof(set));
    public static JSValue constructor => GetStringName(nameof(constructor));
    public static JSValue toLocaleString => GetStringName(nameof(toLocaleString));
    public static JSValue hasOwnProperty => GetStringName(nameof(hasOwnProperty));
    public static JSValue isPrototypeOf => GetStringName(nameof(isPrototypeOf));
    public static JSValue propertyIsEnumerable => GetStringName(nameof(propertyIsEnumerable));
    public static JSValue prototype => GetStringName(nameof(prototype));
    public static JSValue getPrototypeOf => GetStringName(nameof(getPrototypeOf));
    public static JSValue getOwnPropertyDescriptor => GetStringName(nameof(getOwnPropertyDescriptor));
    public static JSValue getOwnPropertyNames => GetStringName(nameof(getOwnPropertyNames));
    public static JSValue create => GetStringName(nameof(create));
    public static JSValue defineProperty => GetStringName(nameof(defineProperty));
    public static JSValue defineProperties => GetStringName(nameof(defineProperties));
    public static JSValue seal => GetStringName(nameof(seal));
    public static JSValue freeze => GetStringName(nameof(freeze));
    public static JSValue preventExtensions => GetStringName(nameof(preventExtensions));
    public static JSValue isSealed => GetStringName(nameof(isSealed));
    public static JSValue isFrozen => GetStringName(nameof(isFrozen));
    public static JSValue isExtensible => GetStringName(nameof(isExtensible));
    public static JSValue keys => GetStringName(nameof(keys));
    public static JSValue Object => GetStringName(nameof(Object));


    // TODO: Implement
    public static JSValue GetStringName(string value) => JSValue.Null;
}

public struct Union<T1, T2, T3>
{
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

    public static explicit operator Number?(JSValue value)
        => value.TypeOf() == JSValueType.Number ? (Number)value : null;
    public static implicit operator JSValue(Number? value)
        => value is Number numberValue ? numberValue._value : JSValue.Undefined;
}

public struct Function : IFunction<Function>
{
    private JSValue _value;

    public static explicit operator Function(JSValue value) => new Function { _value = value };
    public static implicit operator JSValue(Function value) => value._value;
}

public struct Function<TResult> : IFunction<Function<TResult>>
{
    private JSValue _value;

    public static explicit operator Function<TResult>(JSValue value) => new Function<TResult> { _value = value };
    public static implicit operator JSValue(Function<TResult> value) => value._value;

    public static explicit operator Function<TResult>?(JSValue value)
        => value.TypeOf() == JSValueType.Number ? (Function<TResult>)value : null;
    public static implicit operator JSValue(Function<TResult>? value)
        => value is Function<TResult> functionValue ? functionValue._value : JSValue.Undefined;
}

public struct Function<TArg1, TResult> : IFunction<Function<TArg1, TResult>>
{
    private JSValue _value;

    public static explicit operator Function<TArg1, TResult>(JSValue value) => new Function<TArg1, TResult> { _value = value };
    public static implicit operator JSValue(Function<TArg1, TResult> value) => value._value;

    public static explicit operator Function<TArg1, TResult>?(JSValue value)
        => value.TypeOf() == JSValueType.Number ? (Function<TArg1, TResult>)value : null;
    public static implicit operator JSValue(Function<TArg1, TResult>? value)
        => value is Function<TArg1, TResult> functionValue ? functionValue._value : JSValue.Undefined;
}

public struct Void
{
    private JSValue _value;

    public static explicit operator Void(JSValue value) => new Void { _value = value };
    public static implicit operator JSValue(Void value) => value._value;
}

public struct Any
{
    private JSValue _value;

    public static explicit operator Any(JSValue value) => new Any { _value = value };
    public static implicit operator JSValue(Any value) => value._value;

    public static explicit operator Any?(JSValue value)
        => value.TypeOf() != JSValueType.Undefined ? (Any)value : null;
    public static implicit operator JSValue(Any? value)
        => value is Any anyValue ? anyValue._value : JSValue.Undefined;
}

public struct Array<T>
{
    private JSValue _value;

    public static explicit operator Array<T>(JSValue value) => new Array<T> { _value = value };
    public static implicit operator JSValue(Array<T> value) => value._value;

    public static explicit operator Array<T>?(JSValue value)
        => value.TypeOf() != JSValueType.Undefined ? (Array<T>)value : null;
    public static implicit operator JSValue(Array<T>? value)
        => value is Array<T> anyValue ? anyValue._value : JSValue.Undefined;
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

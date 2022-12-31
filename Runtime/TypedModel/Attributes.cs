using System;
using System.Diagnostics.CodeAnalysis;

namespace NodeApi.TypedModel;

[AttributeUsage(AttributeTargets.Struct, Inherited = false)]
sealed class TypedValueAttribute : Attribute
{
    public JSValueType ValueType { get; init; }

    [SetsRequiredMembers]
    public TypedValueAttribute(JSValueType valueType)
    {
        ValueType = valueType;
    }
}

[AttributeUsage(AttributeTargets.Struct, Inherited = false)]
sealed class TypedConstructorAttribute<T> : Attribute
{
    public Type ConstructorType { get; init; }

    [SetsRequiredMembers]
    public TypedConstructorAttribute()
    {
        ConstructorType = typeof(T);
    }
}

[AttributeUsage(AttributeTargets.Struct, Inherited = false)]
sealed class GenerateInstanceInGlobalCacheAttribute : Attribute
{
    public string GlobalPropertyName { get; }

    [SetsRequiredMembers]
    public GenerateInstanceInGlobalCacheAttribute(string globalPropertyName)
    {
        GlobalPropertyName = globalPropertyName;
    }
}

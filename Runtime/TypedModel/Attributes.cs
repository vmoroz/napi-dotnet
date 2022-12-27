using System;
using System.Diagnostics.CodeAnalysis;

namespace NodeApi.TypedModel;

[AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = true)]
sealed class TypedInterfaceAttribute : Attribute
{
    public string? Name { get; init; }

    [SetsRequiredMembers]
    public TypedInterfaceAttribute(string? name = null)
    {
        Name = name;
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

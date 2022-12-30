// Common definitions for the JavaScript typed model based on TypeScript

namespace NodeApi.TypedModel;

public partial struct @symbol : ITypedValue<@symbol> { }
public partial struct @object : ITypedValue<@object> { }
public partial struct @string : ITypedValue<@string> { }
public partial struct @boolean : ITypedValue<@boolean> { }
public partial struct @number : ITypedValue<@number> { }
public partial struct @void : ITypedValue<@void> { }
public partial struct @any : ITypedValue<@any> { }
public partial struct @unknown : ITypedValue<@unknown> { }

public partial struct OneOf<T1, T2> : ITypedValue<OneOf<T1, T2>>
{
}

public partial struct OneOf<T1, T2, T3> : ITypedValue<OneOf<T1, T2, T3>>
{
}

public partial struct Intersect<T1, T2> : ITypedValue<Intersect<T1, T2>>
{
}

public partial struct Readonly<T> : ITypedValue<Readonly<T>>
{
}

public interface ITypedValue<TSelf> where TSelf : struct, ITypedValue<TSelf>
{
    public static abstract explicit operator TSelf(JSValue value);
    public static abstract implicit operator JSValue(TSelf value);

    public static abstract explicit operator TSelf?(JSValue value);
    public static abstract implicit operator JSValue(TSelf? value);
}

public interface ITypedConstructor<TSelf, TObject> : ITypedValue<TSelf>
    where TSelf : struct, ITypedConstructor<TSelf, TObject>
    where TObject : struct, ITypedValue<TObject>
{
    static abstract TSelf Instance { get; }
}

public partial struct Nullable<T> : ITypedValue<Nullable<T>>
{
}

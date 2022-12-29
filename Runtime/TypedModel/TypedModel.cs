// Common definitions for the JavaScript typed model based on TypeScript

namespace NodeApi.TypedModel;

public partial struct @symbol : IJSValueHolder<@symbol> { }
public partial struct @object : IJSValueHolder<@object> { }
public partial struct @string : IJSValueHolder<@string> { }
public partial struct @boolean : IJSValueHolder<@boolean> { }
public partial struct @number : IJSValueHolder<@number> { }
public partial struct @void : IJSValueHolder<@void> { }
public partial struct @any : IJSValueHolder<@any> { }
public partial struct @unknown : IJSValueHolder<@unknown> { }

public partial struct OneOf<T1, T2> : IJSValueHolder<OneOf<T1, T2>>
{
}

public partial struct OneOf<T1, T2, T3> : IJSValueHolder<OneOf<T1, T2, T3>>
{
}

public partial struct Intersect<T1, T2> : IJSValueHolder<Intersect<T1, T2>>
{
}

public partial struct Readonly<T> : IJSValueHolder<Readonly<T>>
{
}

public interface IJSValueHolder<TSelf> where TSelf : struct, IJSValueHolder<TSelf>
{
    public static abstract explicit operator TSelf(JSValue value);
    public static abstract implicit operator JSValue(TSelf value);

    public static abstract explicit operator TSelf?(JSValue value);
    public static abstract implicit operator JSValue(TSelf? value);
}

public interface ITypedConstructor<TSelf, TObject> : IJSValueHolder<TSelf>
    where TSelf : struct, ITypedConstructor<TSelf, TObject>
    where TObject : struct, IJSValueHolder<TObject>
{
    static abstract TSelf Instance { get; }
}

public partial struct Nullable<T> : IJSValueHolder<Nullable<T>>
{
}

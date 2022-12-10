namespace NodeApi.Examples;

[JSModule]
public class Addon2
{
    public JSValue Add(JSCallbackArgs args)
    {
        return (double)(JSNumber)args[0] + (double)args[1];
    }
}
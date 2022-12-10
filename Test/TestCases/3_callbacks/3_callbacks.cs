namespace NodeApi.Examples;

[JSModule]
public class Addon3
{
    public void RunCallback(JSCallbackArgs args)
    {
        args[0].Call(JSValue.Global, "hello world");
    }

    public void RunCallbackWithRecv(JSCallbackArgs args)
    {
        args[0].Call(args[1]);
    }
}
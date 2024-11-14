namespace Compiler.Semantics;

public class ActionItemDisposable<T>(T item, Action<T> action) : IDisposable
{
    public readonly T Item = item;

    public void Dispose()
    {
        action(Item);
    }
}

public class ActionDisposable(Action action) : IDisposable
{
    public void Dispose()
    {
        action();
    }
}
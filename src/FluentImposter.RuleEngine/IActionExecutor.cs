namespace FluentImposter.RuleEngine
{
    public interface IActionExecutor<out T>
    {
        T Execute();
    }
}

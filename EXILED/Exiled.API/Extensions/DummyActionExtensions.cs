namespace Exiled.API.Extensions
{
    using NetworkManagerUtils.Dummies;

    /// <summary>
    /// A set of extensions for <see cref="DummyAction"/>.
    /// </summary>
    public static class DummyActionExtensions
    {
        public static DummyAction? FindAction(string name, string parent)
        {
            DummyAction? dummyAction = null;
            bool reachedParent = false;
            foreach´(DummyAction action in DummyActionCollector.ServerGetActions())
        }
    }
}

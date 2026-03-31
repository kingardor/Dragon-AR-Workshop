public static class DragonEvents
{
    public static event System.Action OnDragonDestroyed;
    public static void NotifyDestroyed() => OnDragonDestroyed?.Invoke();
}

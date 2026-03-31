public static class DragonEvents
{
    // Fired when any dragon is destroyed (user deletion or new spawn replacing it).
    // IMPORTANT: Subscribers MUST unsubscribe in OnDisable/OnDestroy to prevent
    // accumulation across Unity Editor Play sessions (static state persists).
    public static event System.Action OnDragonDestroyed;
    public static void NotifyDestroyed() => OnDragonDestroyed?.Invoke();
}

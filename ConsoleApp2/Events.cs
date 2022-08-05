using ConsoleApp2;

public static class Events
{
    public static event Action update;
    public static void Update() { update?.Invoke(); }
    public static event Action<string> addMessage;
    public static void AddMessage(string message) { addMessage?.Invoke(message); }
    public static event Action<Pokemon> streakUpdated;
    public static void StreakUpdated(Pokemon p) { streakUpdated?.Invoke(p); }
    public static event Action<Pokemon> addStreaker;
    public static void AddStreaker(Pokemon p) { addStreaker?.Invoke(p); }
}

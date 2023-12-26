namespace Pulse;

public class Context
{
    public string HostName { get; set; }
    public string From { get; set; }
    public List<string> Recipients { get; set; } = new List<string>();
}
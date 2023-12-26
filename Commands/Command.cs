using System.Text.RegularExpressions;

namespace Pulse.Commands;

public abstract class Command
{
    public abstract Regex Pattern { get; }
    public abstract Task Execute(GroupCollection groups);
}
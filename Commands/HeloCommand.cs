using System.Text.RegularExpressions;

namespace Pulse.Commands;

public class HeloCommand : Command
{
    public override Regex Pattern => new Regex(@"^HELO\s+(.*)$", RegexOptions.IgnoreCase);
    private readonly Context _context;
    private readonly StreamWriter _writer;

    public HeloCommand(
        Context context,
        StreamWriter writer
    )
    {
        _context = context;
        _writer = writer;
    }

    public override async Task Execute(GroupCollection groups)
    {
        _context.HostName = groups[1].ToString();

        await _writer.WriteLineAsync($"250 Hello {_context.HostName}");
    }
}
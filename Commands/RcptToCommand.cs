using System.Text.RegularExpressions;

namespace Pulse.Commands;

public class RcptToCommand : Command
{
    public override Regex Pattern => new Regex(@"^RCPT\s+TO:\s*<(.*)>$", RegexOptions.IgnoreCase);
    private readonly Context _context;
    private readonly StreamWriter _writer;

    public RcptToCommand(
        Context context,
        StreamWriter writer
    )
    {
        _context = context;
        _writer = writer;
    }

    public override async Task Execute(GroupCollection groups)
    {
        var recipient = groups[1].ToString();

        if (!_context.Recipients.Any(x => x.ToLower() == recipient))
            _context.Recipients.Add(recipient);

        await _writer.WriteLineAsync($"250 Ok");
    }
}
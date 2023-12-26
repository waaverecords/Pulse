using System.Text.RegularExpressions;

namespace Pulse.Commands;

public class MailFromCommand : Command
{
    public override Regex Pattern => new Regex(@"^MAIL\s+FROM:\s*<(.*)>$", RegexOptions.IgnoreCase);
    private readonly Context _context;
    private readonly StreamWriter _writer;

    public MailFromCommand(
        Context context,
        StreamWriter writer
    )
    {
        _context = context;
        _writer = writer;
    }

    public override async Task Execute(GroupCollection groups)
    {
        // TODO: verify the domain with mx records
        _context.From = groups[1].ToString();

        await _writer.WriteLineAsync($"250 Ok");
    }
}
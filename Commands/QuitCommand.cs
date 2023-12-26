using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Pulse.Commands;

public class QuitCommand : Command
{
    public override Regex Pattern => new Regex("^QUIT", RegexOptions.IgnoreCase);
    private readonly StreamWriter _writer;
    private readonly TcpClient _client;

    public QuitCommand(
        StreamWriter writer,
        TcpClient client
    )
    {
        _writer = writer;
        _client = client;
    }

    public override async Task Execute(GroupCollection groups)
    {
        await _writer.WriteLineAsync("221 Closing connection");
        _client.Close();
    }
}
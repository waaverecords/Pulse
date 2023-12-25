using System.Net.Sockets;
using System.Reflection;
using Pulse.Commands;

namespace Pulse;

public class InboundClient
{
    private readonly TcpClient _client;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;
    private readonly IServiceProvider _serviceProvider;

    public InboundClient(
        TcpClient client,
        StreamReader reader,
        StreamWriter writer,
        IServiceProvider serviceProvider
    )
    {
        _client = client;
        _reader = reader;
        _writer = writer;
        _serviceProvider = serviceProvider;
    }

    public async Task Run()
    {
        await _writer.WriteLineAsync($"220 {Settings.Domain} ESMTP Pulse");

        while (_client.Connected)
        {
            var message = await _reader.ReadLineAsync();
            Console.WriteLine(message);

            if (string.IsNullOrEmpty(message))
                continue;

            var baseType = typeof(Command);
            var commandNamespace = baseType.Namespace;
            var types = Assembly.GetExecutingAssembly().GetTypes();
            var derivedTypes = types
                .Where(t => t.Namespace == commandNamespace && baseType.IsAssignableFrom(t) && t != baseType)
                .ToList();

            foreach (var derivedType in derivedTypes)
            {
                // TODO: move all Pattern properties to a function which returns the proper command type
                // instead of instanciating the entire command

                var command = _serviceProvider.GetService(derivedType) as Command;

                if (command.Pattern.IsMatch(message))
                {
                    await command.Execute();
                    break;
                }
            }

            // switch (message)
            // {
            //     case string s when EHLO_reg.Match(message) is { Success: true, Groups: { Count: >= 2 } groups}:
            //         var response = new List<string>
            //         {
            //             $"250-{Settings.Domain} Hello {groups[1]}",
            //             $"250-SIZE {Settings.Size}"
            //         };
            //         await writer.WriteLineAsync(string.Join("\n\r", response));
            //     break;

            //     default:
            //         await writer.WriteLineAsync("500 Syntax error, command unrecognized");
            //     break;
            // }
        }
    }
}
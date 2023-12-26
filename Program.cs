using Microsoft.Extensions.DependencyInjection;
using Pulse;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

var services = new ServiceCollection()

    .AddScoped<TcpClientAccessor>()
    .AddScoped<TcpClient>(x => x.GetRequiredService<TcpClientAccessor>().Client)

    .AddScoped<StreamReaderAccessor>()
    .AddScoped<StreamReader>(x => x.GetRequiredService<StreamReaderAccessor>().Reader)

    .AddScoped<StreamWriterAccessor>()
    .AddScoped<StreamWriter>(x => x.GetRequiredService<StreamWriterAccessor>().Writer)

    .AddScoped<ContextAccessor>()
    .AddScoped<Context>(x => x.GetRequiredService<ContextAccessor>().Context)

    .AddScoped<InboundClient>();

var commandNamespace = typeof(Pulse.Commands.Command).Namespace;
var commandTypes = Assembly.GetExecutingAssembly().GetTypes()
    .Where(t => t.Namespace == commandNamespace && t.IsClass && !t.IsAbstract);
foreach (var commandType in commandTypes)
    services.AddScoped(commandType);
    
var serviceProvider = services.BuildServiceProvider();

var listener = new TcpListener(IPAddress.Any, Settings.Port);
listener.Start();

while (true)
{
    var client = listener.AcceptTcpClient();
    var thread = new Thread(async () =>
    {
        using var stream = client.GetStream();
        using var reader = new StreamReader(stream, Encoding.ASCII);
        using var writer = new StreamWriter(stream, Encoding.ASCII) { AutoFlush = true };

        using var scope = serviceProvider.CreateScope();

        var clientAccessor = scope.ServiceProvider.GetRequiredService<TcpClientAccessor>();
        clientAccessor.Client = client;

        var readerAccessor = scope.ServiceProvider.GetRequiredService<StreamReaderAccessor>();
        readerAccessor.Reader = reader;

        var writerAccessor = scope.ServiceProvider.GetRequiredService<StreamWriterAccessor>();
        writerAccessor.Writer = writer;

        var contextAccessor = scope.ServiceProvider.GetRequiredService<ContextAccessor>();
        contextAccessor.Context = new Context();

        var inboundClient = scope.ServiceProvider.GetRequiredService<InboundClient>();
        await inboundClient.Run();
    });
    thread.Start();
}
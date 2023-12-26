// using System.Net.Sockets;
// using System.Text.RegularExpressions;

// namespace Pulse.Commands;

// public class EhloCommand : Command
// {
//     public override Regex Pattern => new Regex(@"^EHLO\s(.*)$", RegexOptions.IgnoreCase);
//     private readonly Context _context;
//     private readonly StreamWriter _writer;

//     public EhloCommand(
//         Context context,
//         StreamWriter writer
//     )
//     {
//         _context = context;
//         _writer = writer;
//     }

//     public override async Task Execute(GroupCollection groups)
//     {
//         _context.HostName = groups[1].ToString();

//         var response = new List<string>
//         {
//             $"250-{Settings.Domain} Hello {_context.HostName}",
//             $"250-SIZE {Settings.Size}"
//         };
//         await _writer.WriteLineAsync(string.Join("\r\n", response));
//     }
// }
using MailKit.Net.Smtp;
using MimeKit;
using System.ComponentModel;

namespace smtp
{
	public class Program
	{
		private static Args A = new([]);
		public class Args// : Dictionary<string, string>
		{
			private readonly string[] args;
			private readonly Dictionary<string, string> _args = [];
			public static readonly Dictionary<string, string> help = new()
			{
				["user"] = "* username",
				["password"] = "password",
				["server"] = "* server (smtp.*)",
				["from"] = "source email; by def: user",
				["fromfield"] = "'from' field; by def: from",
				["to"] = "* dest email",
				["tofield"] = "'to' field; by def: to",
				["port"] = "* port",
				["subject"] = "subj",
				["body"] = "body",
				["dry"] = "don't send, do test login",
				["skipconn"] = "skip connection; 'do nothing' switch"
			};
			public string this[string key]
			{
				get
				{
					//Console.WriteLine($"this key: {key}");
					_args.TryGetValue(key, out var ret);
					//Console.WriteLine($"ret: {ret}_{ret == string.Empty}");
					return ret;
				}
			}
			public Args(string[] a)
			{
				args = a;
				_args = ParseArgs(args);
			}
			public bool Defined(string key)
			{
				return _args.ContainsKey(key);
			}
			public static Dictionary<string, string> ParseArgs(string[] args)
			{
				Dictionary<string, string> ret = [];
				for (int i = 0; i < args.Length; i++)
				{
					// get param and val
					string p = args[i];
					string v = "";
					// p is not param
					if (!p.StartsWith("--")) continue;
					p = p.Substring(2).ToLower();
					if (i < args.Length - 1)
					{
						v = args[i + 1];
						// is param empty (dry etc)
						v = v.StartsWith("--") ? "" : v;
					}
					ret[p] = v;
				}
				return ret;
			}
		}
		public static void Main(string[] arg)
		{
			A = new Args(arg);
			Main1();
		}
		public static void Main1()
		{
			if (A.Defined("help") || !(A.Defined("user") & A.Defined("to") & A.Defined("server") & A.Defined("port")))
			{
				string help = "Simple MailKit-based mailer. Usage: 'smtp --param value'; * = required.\n";
				foreach (var item in Args.help) help += string.Format("{0, -11} {1, -0}",  $"{item.Key}:", $"{item.Value}") + "\n";
				Console.Write(help);
				Environment.Exit(1);
			}
			var message = new MimeMessage();
			message.From.Add(new MailboxAddress(A["fromfield"] ?? A["from"] ?? A["user"] ?? "", A["from"] ?? A["user"] ?? ""));
			message.To.Add(new MailboxAddress(A["tofield"] ?? A["to"] ?? "", A["to"] ?? ""));
			message.Subject = A["subject"] ?? "no subject defined";

			message.Body = new TextPart("plain") { Text = A["body"] ?? "sorry, mario, our body is in another castle" };

			using (var client = new SmtpClient())
			{
				if (!A.Defined("skipconn"))
				{
					client.Connect(A["server"], int.Parse(A["port"]), true);
					client.Authenticate(A["user"], A["password"]);
					if (!A.Defined("dry"))
					{
						client.Send(message);
					}
					else
					{
						Console.WriteLine("dry");
					}
					client.Disconnect(true);
				}
				else
				{
					Console.WriteLine("skipconn");
				}
			}
		}
	}
}

using ManyConsole;
using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TfsGitAdmin
{
    class Program
    {
        static int Main(string[] args)
        {
            bool displayHeader = true;
            var preset = new OptionSet()
                .Add("no|nologo", n => displayHeader = false);
            preset.Parse(args);

            if (displayHeader)
            {
                Console.WriteLine(GetHeader());
            }

            try
            {
                // locate any commands in the assembly (or use an IoC container, or whatever source)
                var commands = GetCommands();
                // then run them.
                int rc = ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);
                if (rc == 0)
                {
                    Console.WriteLine("Succeeded.");
                }//if
                return rc;
            }
            catch (Exception e)
            {
                e.Dump(Console.Out);
                return 99;
            }//try
        }

        public static IEnumerable<ConsoleCommand> GetCommands()
        {
            return ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(Program));
        }

        static private T GetCustomAttribute<T>()
            where T : Attribute
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(T), false).FirstOrDefault() as T;
        }

        static internal string GetHeader()
        {
            var title = GetCustomAttribute<System.Reflection.AssemblyTitleAttribute>();
            var descr = GetCustomAttribute<System.Reflection.AssemblyDescriptionAttribute>();
            var copy = GetCustomAttribute<System.Reflection.AssemblyCopyrightAttribute>();
            var fileVersion = GetCustomAttribute<System.Reflection.AssemblyFileVersionAttribute>();
            var infoVersion = GetCustomAttribute<System.Reflection.AssemblyInformationalVersionAttribute>();

            var sb = new StringBuilder();
            sb.AppendFormat("{0} {1}", title.Title, infoVersion.InformationalVersion);
            sb.AppendLine();
            sb.AppendLine(descr.Description);
            //sb.AppendFormat("Build: {0}", fileVersion.Version);
            sb.AppendLine();
            sb.AppendLine(copy.Copyright);

            return sb.ToString();
        }
    }
}

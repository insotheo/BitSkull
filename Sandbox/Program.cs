using BitSkull;

namespace Sandbox
{
    internal class Program
    {
        static void Main()
        {
            Log.Pattern = "[%T]: %e";

            string[] skeleton = "\r\n               _.---._\r\n             .'       `.\r\n             :)       (:\r\n             \\ (@) (@) /\r\n              \\   A   /\r\n               )     (\r\n               \\\"\"\"\"\"/\r\n                `._.'\r\n                 .=.\r\n         .---._.-.=.-._.---.\r\n        / ':-(_.-: :-._)-:` \\\r\n       / /' (__.-: :-.__) `\\ \\\r\n      / /  (___.-` '-.___)  \\ \\\r\n     / /   (___.-'^`-.___)   \\ \\\r\n    / /    (___.-'=`-.___)    \\ \\\r\n   / /     (____.'=`.____)     \\ \\\r\n  / /       (___.'=`.___)       \\ \\\r\n (_.;       `---'.=.`---'       ;._)\r\n ;||        __  _.=._  __        ||;\r\n ;||       (  `.-.=.-.'  )       ||;\r\n ;||       \\    `.=.'    /       ||;\r\n ;||        \\    .=.    /        ||;\r\n ;||       .-`.`-._.-'.'-.       ||;\r\n.:::\\      ( ,): O O :(, )      /:::.\r\n|||| `     / /'`--'--'`\\ \\     ' ||||\r\n''''      / /           \\ \\      ''''\r\n         / /             \\ \\\r\n        / /               \\ \\\r\n       / /                 \\ \\\r\n      / /                   \\ \\\r\n     / /                     \\ \\\r\n    /.'                       `.\\\r\n   (_)'                       `(_)\r\n    \\\\.                       .//\r\n     \\\\.                     .//\r\n      \\\\.                   .//\r\n       \\\\.                 .//\r\n        \\\\.               .//\r\n         \\\\.             .//\r\n          \\\\.           .//\r\n          ///)         (\\\\\\\r\n        ,///'           `\\\\\\,\r\n       ///'               `\\\\\\\r\n      \"\"'                   '\"\"\r\n       Skeleton\r\n\r\n".Split("\n");

            for(int i = 0; i < skeleton.Length; i++)
            {
                if (i % 4 == 0) Log.Error(skeleton[i]);
                else if (i % 3 == 0) Log.Warn(skeleton[i]);
                else if (i % 2 == 0) Log.Info(skeleton[i]);
                else Log.Trace(skeleton[i]);
            }
        }
    }
}
    
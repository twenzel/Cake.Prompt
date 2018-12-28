using System;
using System.Threading;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Annotations;

namespace Cake.Common.IO
{
    /// <summary>
    /// Contains functionality related to interactive console prompts.
    /// </summary>
    [CakeAliasCategory("Prompt")]
    public static class PromptAliases
    {
        /// <summary>
        /// Prompts the user for input.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="message">The message which is shown to the user.</param>
        /// <returns>The user input.</returns>
        [CakeMethodAlias]
        public static string Prompt(this ICakeContext context, string message, string defaultResult = "",
            TimeSpan timeout = default)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            timeout = timeout == default ? TimeSpan.FromSeconds(30) : timeout;

            if (timeout <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(timeout), "timeout must be greater than zero");

            var cts = new CancellationTokenSource(timeout);

            try
            {
                return Task.Run(() =>
                {
                    Console.Write(string.IsNullOrEmpty(defaultResult) ? message : $"{message} [{defaultResult}]");
                    var readLine = Console.ReadLine();
                    return string.IsNullOrEmpty(readLine) ? defaultResult : readLine;
                }, cts.Token).GetAwaiter().GetResult();
            }
            catch (OperationCanceledException ex)
            {
                throw new TimeoutException($"Prompt timed out after {timeout:g}.");
            }
        }
    }
}
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
        private const string InteractiveOption = "interactive";

        /// <summary>
        /// Prompts the user for input.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="message">The message which is shown to the user.</param>
        /// <returns>The user input.</returns>
        [CakeMethodAlias]
        public static string Prompt(this ICakeContext context, string message, TimeSpan timeout = default)
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
                    Console.Write("{0}", message);
                    return Console.ReadLine();
                }, cts.Token).GetAwaiter().GetResult();
            }
            catch (OperationCanceledException ex)
            {
                throw new TimeoutException($"Prompt timed out after {timeout:g}.");
            }
        }
    }
}
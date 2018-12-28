﻿using System;
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
        public static string Prompt(this ICakeContext context, string message)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var interactive = context.Arguments.HasArgument(InteractiveOption) 
                              && bool.TryParse(context.Arguments.GetArgument(InteractiveOption), out var b) 
                              && b;

            if (!interactive)
                throw new InvalidOperationException($"Cannot Prompt when cake doesn't have '{InteractiveOption}=true' argument.");

            Console.Write("{0}", message);
            return Console.ReadLine();
        }
    }
}
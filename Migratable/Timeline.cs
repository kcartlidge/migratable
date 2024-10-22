using Migratable.Interfaces;
using Migratable.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Migratable
{
    /// <summary>Handles the timeline of migrations and database.</summary>
    public class Timeline : ITimeline
    {
        private readonly IProvider _provider;
        private readonly SortedList<int, Migration> _migrations;
        private readonly int _indent;
        private readonly string _title;
        private readonly string _here;

        /// <summary>Create a new timeline using the given migrations and provider.</summary>
        /// <param name="provider">An IProvider implementation (eg Postgres).</param>
        /// <param name="migrations">The loaded migration definitions.</param>
        /// <param name="indent">Optional text indent for generated text.</param>
        /// <param name="here">Optional replacement text for the current database position.</param>
        public Timeline(
            IProvider provider,
            SortedList<int, Migration> migrations,
            int indent = 0,
            string title = "STATUS",
            string here = "YOU ARE HERE")
        {
            _provider = provider;
            _migrations = migrations;
            _indent = indent;
            _title = title;
            _here = here;
        }

        /// <inheritdoc/>
        public void Show()
        {
            // Fetch.
            var lines = Get();

            // Display.
            Console.WriteLine();
            foreach (var line in lines) Console.WriteLine(line);
            Console.WriteLine();
        }

        /// <inheritdoc/>
        public List<string> Get()
        {
            var result = new List<string>();

            // Get the basics.
            var currentVersion = _provider.GetVersion();
            var maxVersion = _migrations.Any()
                ? _migrations.Max(x => x.Value.Version)
                : 0;

            // Calculate the left column width (the migration number).
            var width = Math.Max(3, $"{maxVersion}".Length + 1);

            // Pre-derive some standard text.
            var shown = false;
            var pad = "".PadLeft(_indent, ' ');
            var hereMessage = pad + "<".PadRight(width, '-') + $" {_here}";

            // Add any title.
            if (_title.Any()) result.Add($"{pad}{_title}");

            // Show each migration in turn.
            foreach (var m in _migrations)
            {
                // If we're at this version, say so.
                if (m.Key > currentVersion && !shown)
                {
                    shown = true;
                    result.Add(hereMessage);
                }

                // Present a formatted indented description.
                var num = m.Key.ToString().PadLeft(width, '0');
                result.Add($"{pad}{num} {m.Value.Name}");
            }

            // If never shown the database position we must be at the end.
            if (!shown) result.Add(hereMessage);

            return result;
        }
    }
}

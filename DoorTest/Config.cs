using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Exiled.API.Interfaces;

namespace DoorTest
{
    /// <summary>
    /// Configuration for the DoorTest plugin.
    /// Add values here to control test behaviour from server config.
    /// </summary>
    public class Config : IConfig
    {
        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;

        /// <inheritdoc/>
        public bool Debug { get; set; } = false;

        /// <summary>
        /// If true the plugin will run a simple auto-test when enabled (will only run server-side).
        /// </summary>
        public bool EnableAutoTest { get; set; } = false;

        public bool Legacy { get; set; } = false;

        /// <summary>
        /// Optional door name to target for automatic tests. If empty, a random door is used.
        /// </summary>
        public string TestDoorName { get; set; } = string.Empty;

        /// <summary>
        /// Controls whether event callbacks log to server log.
        /// </summary>
        public bool LogOnEvents { get; set; } = true;

        /// <summary>
        /// Example numbers used when simulating access denied/granted screens.
        /// </summary>
        public int AccessPermA { get; set; } = 0;
        public int AccessPermB { get; set; } = 0;
        public int AccessPermC { get; set; } = 0;
    }
}
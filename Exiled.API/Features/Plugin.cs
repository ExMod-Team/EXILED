// -----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System.Linq;

#pragma warning disable SA1402
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using CommandSystem;
    using Enums;
    using Extensions;
    using Interfaces;
    using RemoteAdmin;

    /// <summary>
    /// Expose how a plugin has to be made.
    /// </summary>
    /// <typeparam name="TConfig">The config type.</typeparam>
    public abstract class Plugin<TConfig> : IPlugin<TConfig>
        where TConfig : IConfig, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Plugin{TConfig}"/> class.
        /// </summary>
        public Plugin()
        {
            Assembly = Assembly.GetCallingAssembly();
            Name = Assembly.GetName().Name;
            Prefix = Name.ToSnakeCase();
            Author = Assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;
            Version = Assembly.GetName().Version;
        }

        /// <inheritdoc/>
        public Assembly Assembly { get; protected set; }

        /// <inheritdoc/>
        public virtual string Name { get; }

        /// <inheritdoc/>
        public virtual string Prefix { get; }

        /// <inheritdoc/>
        public virtual string Author { get; }

        /// <inheritdoc/>
        public virtual PluginPriority Priority { get; }

        /// <inheritdoc/>
        public virtual Version Version { get; }

        /// <inheritdoc/>
        public virtual Version RequiredExiledVersion { get; } = typeof(IPlugin<>).Assembly.GetName().Version;

        /// <inheritdoc/>
        public virtual bool IgnoreRequiredVersionCheck { get; } = false;

        /// <inheritdoc/>
        public Dictionary<Type, Dictionary<Type, ICommand>> Commands { get; } = new()
        {
            { typeof(RemoteAdminCommandHandler), new Dictionary<Type, ICommand>() },
            { typeof(GameConsoleCommandHandler), new Dictionary<Type, ICommand>() },
            { typeof(ClientCommandHandler), new Dictionary<Type, ICommand>() },
        };

        /// <inheritdoc/>
        public TConfig Config { get; } = new();

        /// <inheritdoc/>
        public ITranslation InternalTranslation { get; protected set; }

        /// <inheritdoc/>
        public string ConfigPath => Paths.GetConfigPath(Prefix);

        /// <inheritdoc/>
        public string TranslationPath => Paths.GetTranslationPath(Prefix);

        /// <inheritdoc/>
        public virtual void OnEnabled()
        {
            AssemblyInformationalVersionAttribute attribute = Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            Log.Info($"{Name} v{(Version is not null ? $"{Version.Major}.{Version.Minor}.{Version.Build}" : attribute is not null ? attribute.InformationalVersion : string.Empty)} by {Author} has been enabled!");
        }

        /// <inheritdoc/>
        public virtual void OnDisabled() => Log.Info($"{Name} has been disabled!");

        /// <inheritdoc/>
        public virtual void OnReloaded() => Log.Info($"{Name} has been reloaded!");

        /// <inheritdoc/>
        public virtual void OnRegisteringCommands()
        {
            Dictionary<Type, List<ICommand>> toRegister = new();

            foreach (Type type in Assembly.GetTypes())
            {
                if (type.GetInterface("ICommand") != typeof(ICommand))
                    continue;

                if (!Attribute.IsDefined(type, typeof(CommandHandlerAttribute)))
                    continue;

                foreach (CustomAttributeData customAttributeData in type.GetCustomAttributesData())
                {
                    try
                    {
                        if (customAttributeData.AttributeType != typeof(CommandHandlerAttribute))
                            continue;

                        Type commandHandlerType = (Type)customAttributeData.ConstructorArguments[0].Value;

                        ICommand command = GetCommand(type) ?? (ICommand)Activator.CreateInstance(type);

                        if (typeof(ParentCommand).IsAssignableFrom(commandHandlerType))
                        {
                            ParentCommand parentCommand = GetCommand(commandHandlerType) as ParentCommand;

                            if (parentCommand == null)
                            {
                                if (!toRegister.TryGetValue(commandHandlerType, out List<ICommand> list))
                                    toRegister.Add(commandHandlerType, new() { command });
                                else
                                    list.Add(command);

                                continue;
                            }

                            parentCommand.RegisterCommand(command);
                            continue;
                        }

                        try
                        {
                            if (commandHandlerType == typeof(RemoteAdminCommandHandler))
                                CommandProcessor.RemoteAdminCommandHandler.RegisterCommand(command);
                            else if (commandHandlerType == typeof(GameConsoleCommandHandler))
                                GameCore.Console.ConsoleCommandHandler.RegisterCommand(command);
                            else if (commandHandlerType == typeof(ClientCommandHandler))
                                QueryProcessor.DotCommandHandler.RegisterCommand(command);
                        }
                        catch (ArgumentException e)
                        {
                            if (e.Message.StartsWith("An"))
                            {
                                Log.Error($"Command with same name has already registered! Command: {command.Command}");
                            }
                            else
                            {
                                Log.Error($"An error has occurred while registering a command: {e}");
                            }
                        }

                        Commands[commandHandlerType][type] = command;
                    }
                    catch (Exception exception)
                    {
                        Log.Error($"An error has occurred while registering a command: {exception}");
                    }
                }
            }

            foreach (KeyValuePair<Type, List<ICommand>> kvp in toRegister)
            {
                ParentCommand parentCommand = GetCommand(kvp.Key) as ParentCommand;

                foreach (ICommand command in kvp.Value)
                    parentCommand.RegisterCommand(command);
            }
        }

        /// <summary>
        /// Gets a command by it's type.
        /// </summary>
        /// <param name="type"><see cref="ICommand"/>'s type.</param>
        /// <param name="commandHandler"><see cref="CommandHandler"/>'s type. Defines in which command handler command is registered.</param>
        /// <returns>A <see cref="ICommand"/>. May be <see langword="null"/> if command is not registered.</returns>
        public ICommand GetCommand(Type type, Type commandHandler = null)
        {
            if (type.GetInterface("ICommand") != typeof(ICommand))
                return null;

            if (commandHandler != null)
            {
                if (!Commands.TryGetValue(commandHandler, out Dictionary<Type, ICommand> commands))
                    return null;

                if (!commands.TryGetValue(type, out ICommand command))
                    return null;

                return command;
            }

            return Commands.Keys.Select(commandHandlerType => GetCommand(type, commandHandlerType)).FirstOrDefault(command => command != null);
        }

        /// <inheritdoc/>
        public virtual void OnUnregisteringCommands()
        {
            foreach (KeyValuePair<Type, Dictionary<Type, ICommand>> types in Commands)
            {
                foreach (ICommand command in types.Value.Values)
                {
                    if (types.Key == typeof(RemoteAdminCommandHandler))
                        CommandProcessor.RemoteAdminCommandHandler.UnregisterCommand(command);
                    else if (types.Key == typeof(GameConsoleCommandHandler))
                        GameCore.Console.ConsoleCommandHandler.UnregisterCommand(command);
                    else if (types.Key == typeof(ClientCommandHandler))
                        QueryProcessor.DotCommandHandler.UnregisterCommand(command);
                }
            }
        }

        /// <inheritdoc/>
        public int CompareTo(IPlugin<IConfig> other) => -Priority.CompareTo(other.Priority);
    }

    /// <summary>
    /// Expose how a plugin has to be made.
    /// </summary>
    /// <typeparam name="TConfig">The config type.</typeparam>
    /// <typeparam name="TTranslation">The translation type.</typeparam>
    public abstract class Plugin<TConfig, TTranslation> : Plugin<TConfig>
        where TConfig : IConfig, new()
        where TTranslation : ITranslation, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Plugin{TConfig, TTranslation}"/> class.
        /// </summary>
        public Plugin()
        {
            Assembly = Assembly.GetCallingAssembly();
            InternalTranslation = new TTranslation();
        }

        /// <summary>
        /// Gets the plugin translations.
        /// </summary>
        public TTranslation Translation => (TTranslation)InternalTranslation;
    }
}
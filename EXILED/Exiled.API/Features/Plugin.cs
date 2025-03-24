// -----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
#pragma warning disable SA1402 // File may only contain a single type
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using CommandSystem;
    using Enums;
    using Exiled.API.Features.Pools;
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
            Dictionary<Type, List<ICommand>> toRegister = DictionaryPool<Type, List<ICommand>>.Pool.Get();
            Dictionary<Type, ParentCommand> parentCommands = DictionaryPool<Type, ParentCommand>.Pool.Get();

            foreach (Type type in Assembly.GetTypes())
            {
                if (type.GetInterface("ICommand") != typeof(ICommand))
                    continue;

                ICommand command = (ICommand)Activator.CreateInstance(type);

                foreach (CustomAttributeData attributeData in type.GetCustomAttributesData())
                {
                    if (attributeData.AttributeType == typeof(CommandHandlerAttribute))
                        continue;

                    Type attribute = (Type)attributeData.ConstructorArguments[0].Value;

                    try
                    {
                        if (typeof(ParentCommand).IsAssignableFrom(attribute))
                        {
                            if (parentCommands.TryGetValue(attribute, out ParentCommand parentCommand))
                            {
                                parentCommand.RegisterCommand(command);
                            }
                            else
                            {
                                if (toRegister.TryGetValue(attribute, out List<ICommand> list))
                                    list.Add(command);
                                else
                                    toRegister.Add(attribute, new() { command });
                            }
                        }
                        else
                        {
                            if (attribute == typeof(RemoteAdminCommandHandler))
                                CommandProcessor.RemoteAdminCommandHandler.RegisterCommand(command);
                            else if (attribute == typeof(GameConsoleCommandHandler))
                                GameCore.Console.singleton.ConsoleCommandHandler.RegisterCommand(command);
                            else if (attribute == typeof(ClientCommandHandler))
                                QueryProcessor.DotCommandHandler.RegisterCommand(command);
                        }
                    }
                    catch (ArgumentException argumentException)
                    {
                        if (argumentException.Message.StartsWith("An"))
                        {
                            Log.Error($"Command with same name has already registered! Command: {command.Command}");
                        }
                        else
                        {
                            Log.Error($"An error has occurred while registering a command: {argumentException}");
                        }
                    }
                    catch (Exception exception)
                    {
                        Log.Error($"An error has occurred while registering a command: {exception}");
                    }

                    if (command is ParentCommand)
                    {
                        ParentCommand parentCommand = (ParentCommand)command;

                        if (toRegister.TryGetValue(type, out List<ICommand> list))
                        {
                            foreach (ICommand com in list)
                                parentCommand.RegisterCommand(com);
                        }
                    }
                }
            }

            DictionaryPool<Type, ParentCommand>.Pool.Return(parentCommands);
            DictionaryPool<Type, List<ICommand>>.Pool.Return(toRegister);
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
                        GameCore.Console.singleton.ConsoleCommandHandler.UnregisterCommand(command);
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
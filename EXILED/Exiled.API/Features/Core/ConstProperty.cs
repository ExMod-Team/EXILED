// -----------------------------------------------------------------------
// <copyright file="ConstProperty.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using HarmonyLib;
    using Mono.Cecil;

    /// <summary>
    /// A class to easily manipulate const values.
    /// </summary>
    /// <typeparam name="T">Type of constant.</typeparam>
    public class ConstProperty<T>
        where T : IEquatable<T>
    {
        /// <summary>
        /// Gets the list of all <see cref="ConstProperty{T}"/>.
        /// </summary>
        internal static readonly List<ConstProperty<T>> List = new();

        private static readonly Harmony Harmony = new($"Exiled.API-{typeof(T).Name}");

        private readonly IEnumerable<MethodInfo> skipMethods;
        private readonly Type type;
        private IEnumerable<MethodInfo> methodsToPatch;
        private IEnumerable<MethodInfo> patchedMethods;

        private T value;
        private bool patched;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstProperty{T}"/> class.
        /// </summary>
        /// <param name="defaultValue">Constant value.</param>
        /// <param name="type">Type to patch.</param>
        /// <param name="skipMethods">Methods that won't be patched.</param>
        internal ConstProperty(T defaultValue, Type type, params string[] skipMethods)
        {
            DefaultValue = defaultValue;
            value = defaultValue;

            this.type = type;
            this.skipMethods = skipMethods.Select(x => type.GetMethod(x, AccessTools.all));

            List.Add(this);
        }

        /// <summary>
        /// Gets the default constant value.
        /// </summary>
        public T DefaultValue { get; }

        /// <summary>
        /// Gets or sets the new value.
        /// </summary>
        public T Value
        {
            get => value;
            set
            {
                if (this.value.Equals(value))
                    return;

                this.value = value;

                if (!patched)
                    patchedMethods = Patch();
                else
                    Repatch();
            }
        }

        /// <summary>
        /// Gets the methods that will be or already patched to replace constant value.
        /// </summary>
        public IEnumerable<MethodInfo> MethodsToPatch => methodsToPatch ??= GetMethodsToPatch();

        /// <summary>
        /// Gets the patched methods.
        /// </summary>
        /// <remarks>Can be <c>null</c> if no patches were performed.</remarks>
        public IEnumerable<MethodInfo> PatchedMethods => !patched ? null : patchedMethods;

        /// <summary>
        /// Patches methods to replace values.
        /// </summary>
        /// <returns>A collection of methods that replacing actual methods.</returns>
        public IEnumerable<MethodInfo> Patch()
        {
            foreach (MethodInfo method in MethodsToPatch)
            {
                MethodInfo returnInfo;

                try
                {
                     returnInfo = Harmony.Patch(method, transpiler: new(typeof(ConstProperty<T>), nameof(Transpiler)));
                }
                catch (HarmonyException exception)
                {
                    Log.Error(exception);
                    continue;
                }

                yield return returnInfo;
            }

            patched = true;
        }

        /// <summary>
        /// Unpatches methods and then patches them again.
        /// </summary>
        public void Repatch()
        {
            foreach (MethodInfo method in MethodsToPatch)
                Harmony.Unpatch(method, PatchedMethods.First(x => x.Name.Contains(method.Name)));

            patchedMethods = Patch();
        }

        /// <summary>
        /// Gets the methods that should be patched to replace constant value.
        /// </summary>
        /// <returns><see cref="IEnumerable{MethodInfo}"/> of <see cref="MethodInfo"/>.</returns>
        public IEnumerable<MethodInfo> GetMethodsToPatch()
        {
            AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(Path.Combine(Paths.ManagedAssemblies, "Assembly-CSharp.dll"));

            foreach (MethodInfo method in type.GetProperties().Where(x => x.DeclaringType == type && x.GetMethod != null && !skipMethods.Contains(x.GetMethod)).Select(x => x.GetMethod))
            {
                if (assembly.MainModule.ImportReference(method).Resolve().Body.Instructions.Any(x => x.Operand is T obj && obj.Equals(DefaultValue)))
                    yield return method;
            }

            foreach (MethodInfo method in type.GetProperties().Where(x => x.DeclaringType == type && x.SetMethod != null && !skipMethods.Contains(x.SetMethod)).Select(x => x.SetMethod))
            {
                if (assembly.MainModule.ImportReference(method).Resolve().Body.Instructions.Any(x => x.Operand is T obj && obj.Equals(DefaultValue)))
                    yield return method;
            }

            foreach (MethodInfo method in type.GetMethods().Where(x => x.DeclaringType == type && !skipMethods.Contains(x)))
            {
                if (assembly.MainModule.ImportReference(method).Resolve().Body.Instructions.Any(x => x.Operand is T obj && obj.Equals(DefaultValue)))
                    yield return method;
            }
        }

        /// <summary>
        /// Gets the <see cref="ConstProperty{T}"/> for the specified method.
        /// </summary>
        /// <param name="method">Method to patch.</param>
        /// <param name="defaultValue">Default value to replace with.</param>
        /// <returns><see cref="ConstProperty{T}"/> or <c>null</c>.</returns>
        internal static ConstProperty<T> Get(MethodInfo method, T defaultValue)
        {
            List<ConstProperty<T>> properties = List.FindAll(x => x.MethodsToPatch.Contains(method));

            return properties.Count switch
            {
                0 => null,
                1 => properties[0],
                _ => properties.Find(x => x.DefaultValue.Equals(defaultValue))
            };
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.operand is not T obj)
                {
                    yield return instruction;
                    continue;
                }

                ConstProperty<T> property = Get((MethodInfo)original, obj);

                if (property == null || !property.DefaultValue.Equals(obj))
                {
                    yield return instruction;
                    continue;
                }

                if (typeof(T) == typeof(float))
                    yield return new(OpCodes.Ldc_R4, obj);
                else
                    yield return new(instruction.opcode, obj);
            }
        }
    }
}
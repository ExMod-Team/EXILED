// -----------------------------------------------------------------------
// <copyright file="AttachmentIdentifierConverter.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader.Features.Configs.CustomConverters
{
    using System;
    using System.Linq;

    using API.Structs;

    using Exiled.API.Enums;
    using Exiled.API.Features.Items;

    using InventorySystem.Items.Firearms.Attachments;

    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Converter <see cref="FirearmType"/> and <see cref="AttachmentName"/> to <see cref="AttachmentIdentifier"/>.
    /// </summary>
    public sealed class AttachmentIdentifierConverter : IYamlTypeConverter
    {
        /// <inheritdoc cref="IYamlTypeConverter" />
        public bool Accepts(Type type) => type == typeof(AttachmentIdentifier);

        /// <inheritdoc cref="IYamlTypeConverter" />
        public object ReadYaml(IParser parser, Type type)
        {
            if (!parser.TryConsume(out Scalar scalar))
                throw new InvalidOperationException($"Expected a scalar for {nameof(AttachmentIdentifier)}.");

            string[] parts = scalar?.Value?.Split(':');
            if (parts.Length != 3)
                throw new InvalidOperationException($"Invalid AttachmentIdentifier format: '{scalar?.Value}'. Expected 'FirearmType:AttachmentName'.");

            if (!Enum.TryParse(parts[0], true, out AttachmentName attachmentName))
                throw new InvalidOperationException($"Invalid AttachmentName: '{parts[0]}'.");

            if (!Enum.TryParse(parts[1], true, out AttachmentSlot attachmentSlot))
                throw new InvalidOperationException($"Invalid AttachmentName: '{parts[1]}'.");

            if (!uint.TryParse(parts[2], out uint code))
                throw new InvalidOperationException($"Invalid AttachmentName: '{parts[2]}'.");

            return Firearm.AvailableAttachments
                .SelectMany(kvp => kvp.Value)
                .FirstOrDefault(att => att.Name == attachmentName && att.Slot == attachmentSlot && att.Code == code);
        }

        /// <inheritdoc cref="IYamlTypeConverter" />
        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            AttachmentIdentifier attId = default;

            if (value is AttachmentIdentifier castAttId)
                attId = castAttId;

            // If anyone ever looks at this code and doesn't understand why it's implemented the way it is, here's an explanation. The problem is that the NW code doesn't provide a way to properly serialize attachments into a string so that this string can then be deserialized back into an object while maintaining integrity. Therefore, literally the only way to obtain an object is to store it as three properties. Storing only AttachmentName will cause problems. Storing it as "FirearmType:AttachmentName" will also cause problems.
            // https://discord.com/channels/656673194693885975/1002713309876854924/1488655471697989682
            emitter.Emit(new Scalar($"{attId.Name}:{attId.Slot}:{attId.Code}"));
        }
    }
}
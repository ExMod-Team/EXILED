// -----------------------------------------------------------------------
// <copyright file="MetalKeycard.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items.Keycards
{
    using System.Text;

    using Exiled.API.Features.Pools;
    using Exiled.API.Interfaces.Keycards;

    using InventorySystem.Items;
    using InventorySystem.Items.Keycards;

    using UnityEngine;

    /// <summary>
    /// Represents the Metal Custom Keycard.
    /// </summary>
    public class MetalKeycard : CustomKeycard, INameTagKeycard, ILabelKeycard, IWearKeycard, ISerialLabelKeycard
    {
        private CustomSerialNumberDetail serialNumberDetail;
        private bool serialNumberSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetalKeycard"/> class.
        /// </summary>
        /// <param name="itemBase">The <see cref="ItemBase"/> to encapsulate.</param>
        internal MetalKeycard(KeycardItem itemBase)
            : base(itemBase)
        {
        }

        /// <inheritdoc cref="INameTagKeycard.NameTag"/>
        public string NameTag
        {
            get => Gfx.NameFields[0].text;

            set
            {
                Gfx.NameFields[0].text = value;
                Resync();
            }
        }

        /// <inheritdoc cref="ILabelKeycard.Label"/>
        public string Label
        {
            get => Gfx.KeycardLabels[0].text;

            set
            {
                Gfx.KeycardLabels[0].text = value;
                Resync();
            }
        }

        /// <inheritdoc cref="ILabelKeycard.LabelColor"/>
        public Color LabelColor
        {
            get => Gfx.KeycardLabels[0].color;

            set
            {
                Gfx.KeycardLabels[0].color = value;
                Resync();
            }
        }

        /// <inheritdoc cref="IWearKeycard.Wear"/>
        /// <remarks>Capped from 0-4 for Site-02 keycards, returns 255 if no wear level is found.</remarks>
        public byte Wear
        {
            get
            {
                for (byte i = 0; i < Gfx.ElementVariants.Length; i++)
                {
                    GameObject obj = Gfx.ElementVariants[i];

                    if (obj.activeSelf)
                        return i;
                }

                return byte.MaxValue;
            }

            set
            {
                for (byte i = 0; i < Gfx.ElementVariants.Length; i++)
                {
                    Gfx.ElementVariants[i].SetActive(i == value);
                }

                Resync();
            }
        }

        /// <inheritdoc cref="ISerialLabelKeycard.SerialLabel"/>
        public string SerialLabel
        {
            get
            {
                if (!serialNumberSet)
                {
                    serialNumberDetail = GetDetail<CustomSerialNumberDetail>();
                    serialNumberSet = true;
                }

                StringBuilder builder = StringBuilderPool.Pool.Get(12);

                foreach (Renderer digit in Gfx.SerialNumberDigits)
                {
                    int index = SerialNumberDetail.DigitMats[serialNumberDetail._sourceMaterial].IndexOf(digit.sharedMaterial);

                    if (index is 10)
                    {
                        builder.Append('-');
                        continue;
                    }

                    builder.Append(index);
                }

                return StringBuilderPool.Pool.ToStringReturn(builder);
            }

            set
            {
                if (!serialNumberSet)
                {
                    serialNumberDetail = GetDetail<CustomSerialNumberDetail>();
                    serialNumberSet = true;
                }

                if (value.Length > 12)
                    value = value.Substring(0, 12);

                foreach (Renderer digit in Gfx.SerialNumberDigits)
                {
                    digit.sharedMaterial = serialNumberDetail.GetDigitMaterial(value[0] - 48);
                }

                Resync();
            }
        }
    }
}
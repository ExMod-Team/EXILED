// -----------------------------------------------------------------------
// <copyright file="TaskForceKeycard.cs" company="ExMod Team">
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
    /// Represents the Task Force Custom Keycard.
    /// </summary>
    public class TaskForceKeycard : CustomKeycard, INameTagKeycard, ISerialLabelKeycard, IRankKeycard
    {
        private CustomSerialNumberDetail serialNumberDetail;
        private bool serialNumberSet;

        private CustomRankDetail rankDetail;
        private bool rankDetailSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskForceKeycard"/> class.
        /// </summary>
        /// <param name="itemBase">The <see cref="ItemBase"/> to encapsulate.</param>
        internal TaskForceKeycard(KeycardItem itemBase)
            : base(itemBase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskForceKeycard"/> class.
        /// </summary>
        /// <param name="itemType">The <see cref="ItemType"/> of the item to create.</param>
        internal TaskForceKeycard(ItemType itemType)
            : base(itemType)
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

        /// <inheritdoc cref="IRankKeycard.Rank"/>
        public int Rank
        {
            get
            {
                if (!rankDetailSet)
                {
                    rankDetail = GetDetail<CustomRankDetail>();
                    rankDetailSet = true;
                }

                return rankDetail._options.IndexOf(Gfx.RankFilter.sharedMesh);
            }

            set
            {
                if (!rankDetailSet)
                {
                    rankDetail = GetDetail<CustomRankDetail>();
                    rankDetailSet = true;
                }

                Gfx.RankFilter.sharedMesh = rankDetail._options[Mathf.Clamp(value, 0, 3)];
                Resync();
            }
        }
    }
}
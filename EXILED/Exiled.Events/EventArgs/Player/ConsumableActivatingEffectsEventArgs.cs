namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Interfaces;

    using BaseConsumable = InventorySystem.Items.Usables.Consumable;

    /// <summary>
    /// Interesting.
    /// </summary>
    public class ConsumableActivatingEffectsEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumableActivatingEffectsEventArgs"/> class.
        /// </summary>
        /// <param name="referenceHub"><inheritdoc cref="ReferenceHub"/></param>
        /// <param name="consumable"><inheritdoc cref="BaseConsumable" /></param>
        /// <param name="isAllowed"><inheritdoc cref="bool" /></param>
        public ConsumableActivatingEffectsEventArgs(ReferenceHub referenceHub, BaseConsumable consumable, bool isAllowed = true)
        {
            Player = API.Features.Player.Get(referenceHub);
            IsAllowed = isAllowed;
            Consumable = (Consumable)Item.Get(consumable);
        }

        /// <summary>
        /// Gets the player that consumed the <see cref="Consumable"/>
        /// </summary>
        public API.Features.Player Player { get; }

        /// <inheritdoc />
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the <see cref="API.Features.Items.Consumable"/> that was consumed.
        /// </summary>
        public Consumable Consumable { get; }
    }
}
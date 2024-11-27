namespace Exiled.API.Features.Items
{
    using Exiled.API.Interfaces;
    using InventorySystem.Items.Usables.Scp1344;

    /// <summary>
    /// A wrapper class for SCP-1344.
    /// </summary>
    public class Scp1344 : Usable, IWrapper<Scp1344Item>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp1344"/> class.
        /// </summary>
        /// <param name="itemBase">The item base to wrap.</param>
        public Scp1344(Scp1344Item itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp1344"/> class.
        /// </summary>
        internal Scp1344()
            : this((Scp1344Item)Server.Host.Inventory.CreateItemInstance(new(ItemType.SCP1344, 0), false))
        {
        }

        /// <summary>
        /// Gets the <see cref="Scp1344"/> base.
        /// </summary>
        public new Scp1344Item Base { get; }
    }
}
// -----------------------------------------------------------------------
// <copyright file="Event.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Features
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;
    using MEC;

    /// <summary>
    /// The custom <see cref="EventHandler"/> delegate, with empty parameters.
    /// </summary>
    public delegate void CustomEventHandler();

    /// <summary>
    /// THe custom <see cref="EventHandler"/> delegate, with empty parameters. Holds async events with <see cref="MEC"/>.
    /// </summary>
    /// <returns><see cref="IEnumerator{T}"/> of <see cref="float"/>.</returns>
    public delegate IEnumerator<float> CustomAsyncEventHandler();

    /// <summary>
    /// An implementation of <see cref="IExiledEvent"/> that encapsulates a no-argument event.
    /// </summary>
    public class Event : IExiledEvent
    {
        private record Registration(CustomEventHandler handler, int priority);

        private record AsyncRegistration(CustomAsyncEventHandler handler, int priority);

        private static readonly List<Event> EventsValue = new();

        private static readonly IComparer<Registration> RegisterComparable = Comparer<Registration>.Create((x, y) => y.priority - x.priority);

        private static readonly IComparer<AsyncRegistration> AsyncRegisterComparable = Comparer<AsyncRegistration>.Create((x, y) => y.priority - x.priority);

        private readonly List<Registration> innerEvent = new();

        private readonly List<AsyncRegistration> innerAsyncEvent = new();

        private bool patched;

        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/> class.
        /// </summary>
        public Event()
        {
            EventsValue.Add(this);
        }

        /// <summary>
        /// Gets a <see cref="IReadOnlyList{T}"/> of <see cref="Event{T}"/> which contains all the <see cref="Event{T}"/> instances.
        /// </summary>
        public static IReadOnlyList<Event> List => EventsValue;

        /// <summary>
        /// Subscribes a <see cref="CustomEventHandler"/> to the inner event, and checks patches if dynamic patching is enabled.
        /// </summary>
        /// <param name="event">The <see cref="Event"/> to subscribe the <see cref="CustomEventHandler"/> to.</param>
        /// <param name="handler">The <see cref="CustomEventHandler"/> to subscribe to the <see cref="Event"/>.</param>
        /// <returns>The <see cref="Event"/> with the handler added to it.</returns>
        public static Event operator +(Event @event, CustomEventHandler handler)
        {
            @event.Subscribe(handler);
            return @event;
        }

        /// <summary>
        /// Subscribes a <see cref="CustomAsyncEventHandler"/> to the inner event, and checks patches if dynamic patching is enabled.
        /// </summary>
        /// <param name="event">The <see cref="Event"/> to subscribe the <see cref="CustomAsyncEventHandler"/> to.</param>
        /// <param name="asyncEventHandler">The <see cref="CustomAsyncEventHandler"/> to subscribe to the <see cref="Event"/>.</param>
        /// <returns>The <see cref="Event"/> with the handler added to it.</returns>
        public static Event operator +(Event @event, CustomAsyncEventHandler asyncEventHandler)
        {
            @event.Subscribe(asyncEventHandler);
            return @event;
        }

        /// <summary>
        /// Unsubscribes a target <see cref="CustomEventHandler"/> from the inner event, and checks if unpatching is possible, if dynamic patching is enabled.
        /// </summary>
        /// <param name="event">The <see cref="Event"/> the <see cref="CustomEventHandler"/> will be unsubscribed from.</param>
        /// <param name="handler">The <see cref="CustomEventHandler"/> that will be unsubscribed from the <see cref="Event"/>.</param>
        /// <returns>The <see cref="Event"/> with the handler unsubscribed from it.</returns>
        public static Event operator -(Event @event, CustomEventHandler handler)
        {
            @event.Unsubscribe(handler);
            return @event;
        }

        /// <summary>
        /// Unsubscribes a target <see cref="CustomAsyncEventHandler"/> from the inner event, and checks if unpatching is possible, if dynamic patching is enabled.
        /// </summary>
        /// <param name="event">The <see cref="Event"/> the <see cref="CustomAsyncEventHandler"/> will be unsubscribed from.</param>
        /// <param name="asyncEventHandler">The <see cref="CustomAsyncEventHandler"/> that will be unsubscribed from the <see cref="Event"/>.</param>
        /// <returns>The <see cref="Event"/> with the handler unsubscribed from it.</returns>
        public static Event operator -(Event @event, CustomAsyncEventHandler asyncEventHandler)
        {
            @event.Unsubscribe(asyncEventHandler);
            return @event;
        }

        /// <summary>
        /// Subscribes a target <see cref="CustomEventHandler"/> to the inner event if the conditional is true.
        /// </summary>
        /// <param name="handler">The handler to add.</param>
        public void Subscribe(CustomEventHandler handler)
            => Subscribe(handler, 0);

        /// <summary>
        /// Subscribes a target <see cref="CustomEventHandler"/> to the inner event if the conditional is true.
        /// </summary>
        /// <param name="handler">The handler to add.</param>
        /// <param name="priority">The highest priority is the first called, the lowest the last.</param>
        public void Subscribe(CustomEventHandler handler, int priority)
        {
            Log.Assert(Events.Instance is not null, $"{nameof(Events.Instance)} is null, please ensure you have exiled_events enabled!");

            if (Events.Instance.Config.UseDynamicPatching && !patched)
            {
                Events.Instance.Patcher.Patch(this);
                patched = true;
            }

            if (handler == null)
                return;

            Registration registration = new Registration(handler, priority);
            int index = innerEvent.BinarySearch(registration, RegisterComparable);
            if (index < 0)
            {
                innerEvent.Insert(~index, registration);
            }
            else
            {
                while (index < innerEvent.Count && innerEvent[index].priority == priority)
                    index++;
                innerEvent.Insert(index, registration);
            }
        }

        /// <summary>
        /// Subscribes a target <see cref="CustomAsyncEventHandler"/> to the inner event if the conditional is true.
        /// </summary>
        /// <param name="handler">The handler to add.</param>
        public void Subscribe(CustomAsyncEventHandler handler)
            => Subscribe(handler, 0);

        /// <summary>
        /// Subscribes a target <see cref="CustomAsyncEventHandler"/> to the inner event if the conditional is true.
        /// </summary>
        /// <param name="handler">The handler to add.</param>
        /// <param name="priority">The highest priority is the first called, the lowest the last.</param>
        public void Subscribe(CustomAsyncEventHandler handler, int priority)
        {
            Log.Assert(Events.Instance is not null, $"{nameof(Events.Instance)} is null, please ensure you have exiled_events enabled!");

            if (Events.Instance.Config.UseDynamicPatching && !patched)
            {
                Events.Instance.Patcher.Patch(this);
                patched = true;
            }

            if (handler == null)
                return;

            AsyncRegistration registration = new AsyncRegistration(handler, 0);
            int index = innerAsyncEvent.BinarySearch(registration, AsyncRegisterComparable);
            if (index < 0)
            {
                innerAsyncEvent.Insert(~index, registration);
            }
            else
            {
                while (index < innerAsyncEvent.Count && innerAsyncEvent[index].priority == priority)
                    index++;
                innerAsyncEvent.Insert(index, registration);
            }
        }

        /// <summary>
        /// Unsubscribes a target <see cref="CustomEventHandler"/> from the inner event if the conditional is true.
        /// </summary>
        /// <param name="handler">The handler to add.</param>
        public void Unsubscribe(CustomEventHandler handler)
        {
            int index = innerEvent.FindIndex(p => p.handler == handler);
            if (index != -1)
                innerEvent.RemoveAt(index);
        }

        /// <summary>
        /// Unsubscribes a target <see cref="CustomAsyncEventHandler"/> from the inner event if the conditional is true.
        /// </summary>
        /// <param name="handler">The handler to add.</param>
        public void Unsubscribe(CustomAsyncEventHandler handler)
        {
            int index = innerAsyncEvent.FindIndex(p => p.handler == handler);
            if (index != -1)
                innerAsyncEvent.RemoveAt(index);
        }

        /// <summary>
        /// Executes all <see cref="CustomEventHandler"/> listeners safely.
        /// </summary>
        public void InvokeSafely()
        {
            BlendedInvoke();
        }

        /// <inheritdoc cref="InvokeSafely"/>
        internal void BlendedInvoke()
        {
            Registration[] innerEvent = this.innerEvent.ToArray();
            AsyncRegistration[] innerAsyncEvent = this.innerAsyncEvent.ToArray();
            int count = innerEvent.Length + innerAsyncEvent.Length;
            int eventIndex = 0, asyncEventIndex = 0;

            for (int i = 0; i < count; i++)
            {
                if (eventIndex < innerEvent.Length && (asyncEventIndex >= innerAsyncEvent.Length || innerEvent[eventIndex].priority >= innerAsyncEvent[asyncEventIndex].priority))
                {
                    try
                    {
                        innerEvent[eventIndex].handler();
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Method \"{innerEvent[eventIndex].handler.Method.Name}\" of the class \"{innerEvent[eventIndex].handler.Method.ReflectedType.FullName}\" caused an exception when handling the event \"{GetType().FullName}\"\n{ex}");
                    }

                    eventIndex++;
                }
                else
                {
                    try
                    {
                        Timing.RunCoroutine(innerAsyncEvent[asyncEventIndex].handler());
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Method \"{innerAsyncEvent[asyncEventIndex].handler.Method.Name}\" of the class \"{innerAsyncEvent[asyncEventIndex].handler.Method.ReflectedType.FullName}\" caused an exception when handling the event \"{GetType().FullName}\"\n{ex}");
                    }

                    asyncEventIndex++;
                }
            }
        }

        /// <inheritdoc cref="InvokeSafely"/>
        internal void InvokeNormal()
        {
            Registration[] innerEvent = this.innerEvent.ToArray();
            foreach (Registration registration in innerEvent)
            {
                try
                {
                    registration.handler();
                }
                catch (Exception ex)
                {
                    Log.Error($"Method \"{registration.handler.Method.Name}\" of the class \"{registration.handler.Method.ReflectedType.FullName}\" caused an exception when handling the event \"{GetType().FullName}\"\n{ex}");
                }
            }
        }

        /// <inheritdoc cref="InvokeSafely"/>
        internal void InvokeAsync()
        {
            AsyncRegistration[] innerAsyncEvent = this.innerAsyncEvent.ToArray();
            foreach (AsyncRegistration registration in innerAsyncEvent)
            {
                try
                {
                    Timing.RunCoroutine(registration.handler());
                }
                catch (Exception ex)
                {
                    Log.Error($"Method \"{registration.handler.Method.Name}\" of the class \"{registration.handler.Method.ReflectedType.FullName}\" caused an exception when handling the event \"{GetType().FullName}\"\n{ex}");
                }
            }
        }
    }
}

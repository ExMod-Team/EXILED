// -----------------------------------------------------------------------
// <copyright file="Event{T}.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;
    using MEC;

    /// <summary>
    /// The custom <see cref="EventHandler"/> delegate.
    /// </summary>
    /// <typeparam name="TEventArgs">The <see cref="EventHandler{TEventArgs}"/> type.</typeparam>
    /// <param name="ev">The <see cref="EventHandler{TEventArgs}"/> instance.</param>
    public delegate void CustomEventHandler<TEventArgs>(TEventArgs ev);

    /// <summary>
    /// The custom <see cref="EventHandler"/> delegate.
    /// </summary>
    /// <typeparam name="TEventArgs">The <see cref="EventHandler{TEventArgs}"/> type.</typeparam>
    /// <param name="ev">The <see cref="EventHandler{TEventArgs}"/> instance.</param>
    /// <returns><see cref="IEnumerator{T}"/> of <see cref="float"/>.</returns>
    public delegate IEnumerator<float> CustomAsyncEventHandler<TEventArgs>(TEventArgs ev);

    /// <summary>
    /// An implementation of the <see cref="IExiledEvent"/> interface that encapsulates an event with arguments.
    /// </summary>
    /// <typeparam name="T">The specified <see cref="EventArgs"/> that the event will use.</typeparam>
    public class Event<T> : IExiledEvent
    {
        private record Registration(CustomEventHandler<T> handler, int priority);

        private record AsyncRegistration(CustomAsyncEventHandler<T> handler, int priority);

        private static readonly Dictionary<Type, Event<T>> TypeToEvent = new();

        private static readonly IComparer<Registration> RegisterComparable = Comparer<Registration>.Create((x, y) => y.priority - x.priority);

        private static readonly IComparer<AsyncRegistration> AsyncRegisterComparable = Comparer<AsyncRegistration>.Create((x, y) => y.priority - x.priority);

        private readonly List<Registration> innerEvent = new();

        private readonly List<AsyncRegistration> innerAsyncEvent = new();

        private bool patched;

        /// <summary>
        /// Initializes a new instance of the <see cref="Event{T}"/> class.
        /// </summary>
        public Event()
        {
            TypeToEvent.Add(typeof(T), this);
        }

        /// <summary>
        /// Gets a <see cref="IReadOnlyCollection{T}"/> of <see cref="Event{T}"/> which contains all the <see cref="Event{T}"/> instances.
        /// </summary>
        public static IReadOnlyDictionary<Type, Event<T>> Dictionary => TypeToEvent;

        /// <summary>
        /// Subscribes a target <see cref="CustomEventHandler{TEventArgs}"/> to the inner event and checks if patching is possible, if dynamic patching is enabled.
        /// </summary>
        /// <param name="event">The <see cref="Event{T}"/> the <see cref="CustomEventHandler{T}"/> will be subscribed to.</param>
        /// <param name="handler">The <see cref="CustomEventHandler{T}"/> that will be subscribed to the <see cref="Event{T}"/>.</param>
        /// <returns>The <see cref="Event{T}"/> with the handler subscribed to it.</returns>
        public static Event<T> operator +(Event<T> @event, CustomEventHandler<T> handler)
        {
            @event.Subscribe(handler);
            return @event;
        }

        /// <summary>
        /// Subscribes a <see cref="CustomAsyncEventHandler"/> to the inner event, and checks patches if dynamic patching is enabled.
        /// </summary>
        /// <param name="event">The <see cref="Event{T}"/> to subscribe the <see cref="CustomAsyncEventHandler{T}"/> to.</param>
        /// <param name="asyncEventHandler">The <see cref="CustomAsyncEventHandler{T}"/> to subscribe to the <see cref="Event{T}"/>.</param>
        /// <returns>The <see cref="Event{T}"/> with the handler added to it.</returns>
        public static Event<T> operator +(Event<T> @event, CustomAsyncEventHandler<T> asyncEventHandler)
        {
            @event.Subscribe(asyncEventHandler);
            return @event;
        }

        /// <summary>
        /// Unsubscribes a target <see cref="CustomEventHandler{TEventArgs}"/> from the inner event and checks if unpatching is possible, if dynamic patching is enabled.
        /// </summary>
        /// <param name="event">The <see cref="Event{T}"/> the <see cref="CustomEventHandler{T}"/> will be unsubscribed from.</param>
        /// <param name="handler">The <see cref="CustomEventHandler{T}"/> that will be unsubscribed from the <see cref="Event{T}"/>.</param>
        /// <returns>The <see cref="Event{T}"/> with the handler unsubscribed from it.</returns>
        public static Event<T> operator -(Event<T> @event, CustomEventHandler<T> handler)
        {
            @event.Unsubscribe(handler);
            return @event;
        }

        /// <summary>
        /// Unsubscribes a target <see cref="CustomAsyncEventHandler{TEventArgs}"/> from the inner event, and checks if unpatching is possible, if dynamic patching is enabled.
        /// </summary>
        /// <param name="event">The <see cref="Event"/> the <see cref="CustomAsyncEventHandler{T}"/> will be unsubscribed from.</param>
        /// <param name="asyncEventHandler">The <see cref="CustomAsyncEventHandler{T}"/> that will be unsubscribed from the <see cref="Event{T}"/>.</param>
        /// <returns>The <see cref="Event{T}"/> with the handler unsubscribed from it.</returns>
        public static Event<T> operator -(Event<T> @event, CustomAsyncEventHandler<T> asyncEventHandler)
        {
            @event.Unsubscribe(asyncEventHandler);
            return @event;
        }

        /// <summary>
        /// Subscribes a target <see cref="CustomEventHandler{T}"/> to the inner event if the conditional is true.
        /// </summary>
        /// <param name="handler">The handler to add.</param>
        public void Subscribe(CustomEventHandler<T> handler)
            => Subscribe(handler, 0);

        /// <summary>
        /// Subscribes a target <see cref="CustomEventHandler{T}"/> to the inner event if the conditional is true.
        /// </summary>
        /// <param name="handler">The handler to add.</param>
        /// <param name="priority">The highest priority is the first called, the lowest the last.</param>
        public void Subscribe(CustomEventHandler<T> handler, int priority)
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
        /// Subscribes a target <see cref="CustomAsyncEventHandler{T}"/> to the inner event if the conditional is true.
        /// </summary>
        /// <param name="handler">The handler to add.</param>
        public void Subscribe(CustomAsyncEventHandler<T> handler)
            => Subscribe(handler, 0);

        /// <summary>
        /// Subscribes a target <see cref="CustomAsyncEventHandler{T}"/> to the inner event if the conditional is true.
        /// </summary>
        /// <param name="handler">The handler to add.</param>
        /// <param name="priority">The highest priority is the first called, the lowest the last.</param>
        public void Subscribe(CustomAsyncEventHandler<T> handler, int priority)
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
        /// Unsubscribes a target <see cref="CustomEventHandler{T}"/> from the inner event if the conditional is true.
        /// </summary>
        /// <param name="handler">The handler to add.</param>
        public void Unsubscribe(CustomEventHandler<T> handler)
        {
            int index = innerEvent.FindIndex(p => p.handler == handler);
            if (index != -1)
                innerEvent.RemoveAt(index);
        }

        /// <summary>
        /// Unsubscribes a target <see cref="CustomEventHandler{T}"/> from the inner event if the conditional is true.
        /// </summary>
        /// <param name="handler">The handler to add.</param>
        public void Unsubscribe(CustomAsyncEventHandler<T> handler)
        {
            int index = innerAsyncEvent.FindIndex(p => p.handler == handler);
            if (index != -1)
                innerAsyncEvent.RemoveAt(index);
        }

        /// <summary>
        /// Executes all <see cref="CustomEventHandler{TEventArgs}"/> listeners safely.
        /// </summary>
        /// <param name="arg">The event argument.</param>
        /// <exception cref="ArgumentNullException">Event or its arg is <see langword="null"/>.</exception>
        public void InvokeSafely(T arg)
        {
            BlendedInvoke(arg);
        }

        /// <inheritdoc cref="InvokeSafely"/>
        internal void BlendedInvoke(T arg)
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
                        innerEvent[eventIndex].handler(arg);
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
                        Timing.RunCoroutine(innerAsyncEvent[asyncEventIndex].handler(arg));
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
        internal void InvokeNormal(T arg)
        {
            Registration[] innerEvent = this.innerEvent.ToArray();
            foreach (Registration registration in innerEvent)
            {
                try
                {
                    registration.handler(arg);
                }
                catch (Exception ex)
                {
                    Log.Error($"Method \"{registration.handler.Method.Name}\" of the class \"{registration.handler.Method.ReflectedType.FullName}\" caused an exception when handling the event \"{GetType().FullName}\"\n{ex}");
                }
            }
        }

        /// <inheritdoc cref="InvokeSafely"/>
        internal void InvokeAsync(T arg)
        {
            AsyncRegistration[] innerAsyncEvent = this.innerAsyncEvent.ToArray();
            foreach (AsyncRegistration registration in innerAsyncEvent)
            {
                try
                {
                    Timing.RunCoroutine(registration.handler(arg));
                }
                catch (Exception ex)
                {
                    Log.Error($"Method \"{registration.handler.Method.Name}\" of the class \"{registration.handler.Method.ReflectedType.FullName}\" caused an exception when handling the event \"{GetType().FullName}\"\n{ex}");
                }
            }
        }
    }
}

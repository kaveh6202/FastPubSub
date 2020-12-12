﻿using System;

namespace PubSub
{
    class Subscriber
    {
        public Delegate Action { get; set; }
        public Type Type { get; set; }
        public WeakReference Sender { get; set; }
        public Delegate Filter { get; set; }
    }
}

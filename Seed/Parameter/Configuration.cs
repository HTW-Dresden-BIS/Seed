﻿using System;
using System.Collections.Generic;

namespace Seed.Parameter
{
    public class Configuration : Dictionary<Type, object>
    {
        public static Configuration Create(object[] args)
        {
            var s = new Configuration();
            foreach (var item in args)
            {
                s.AddOption(o: item);
            }

            return s;
        }

        public bool AddOption(object o)
        {
            return this.TryAdd(key: o.GetType(), value: o);
        }

        public T Get<T>()
        {
            this.TryGetValue(key: typeof(T), value: out object value);
            return (T) value;
        }

        public bool ReplaceOption(object o)
        {
            this.Remove(o.GetType());
            return AddOption(o);
        }
    }
}

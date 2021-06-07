using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Events
{
    public delegate void ValueChangedEventHandler<TValue>(object sender, ValueChangedEventArgs<TValue> e);

    public class ValueChangedEventArgs<TValue> : EventArgs
    {
        public TValue OldValue { get; protected set; }

        public TValue NewValue { get; protected set; }

        public ValueChangedEventArgs(TValue oldValue, TValue newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}

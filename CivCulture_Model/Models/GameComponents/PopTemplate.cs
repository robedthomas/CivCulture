using CivCulture_Model.Events;
using CivCulture_Model.Models.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public class PopTemplate : GameComponent
    {
        #region Fields
        private NeedCollection needs;
        #endregion

        #region Events
        public ValueChangedEventHandler<NeedCollection> NeedsChanged;
        #endregion

        #region Properties
        public NeedCollection Needs
        {
            get => needs;
            protected set
            {
                if (needs != value)
                {
                    NeedCollection oldNeeds = needs;
                    needs = value;
                    NeedsChanged?.Invoke(this, new ValueChangedEventArgs<NeedCollection>(oldNeeds, needs));
                }
            }
        }
        #endregion

        #region Constructors
        #endregion

        #region Methods
        #endregion
    }
}

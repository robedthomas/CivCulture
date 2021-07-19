﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public class Consumeable : GameComponent
    {
        #region Fields
        #endregion

        #region Events
        #endregion

        #region Properties
        public decimal BaseValue { get; protected set; }
        #endregion

        #region Constructors
        public Consumeable(decimal baseValue)
        {
            BaseValue = baseValue;
        }
        #endregion

        #region Methods
        #endregion
    }
}

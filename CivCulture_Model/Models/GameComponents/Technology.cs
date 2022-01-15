﻿using CivCulture_Model.Models.Collections;
using CivCulture_Model.Models.Modifiers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Models
{
    public class Technology : GameComponent
    {
        #region Events
        #endregion

        #region Fields
        #endregion

        #region Properties
        public string Name { get; protected set; }

        public ConsumeablesCollection Costs { get; protected set; }

        public HashSet<Technology> Parents { get; protected set; }

        public HashSet<Technology> Children { get; protected set; }

        public TechModifierCollection Modifiers { get; protected set; }

        public ObservableCollection<Culture> CulturesResearchedBy { get; protected set; }
        #endregion

        #region Constructors
        public Technology(string name, ConsumeablesCollection costs)
        {
            Name = name;
            Costs = new ConsumeablesCollection(costs);
            Parents = new HashSet<Technology>();
            Children = new HashSet<Technology>();
            Modifiers = new TechModifierCollection();
            CulturesResearchedBy = new ObservableCollection<Culture>();
        }
        #endregion

        #region Methods
        #endregion
    }
}
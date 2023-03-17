using CivCulture_Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_ViewModel.ViewModels
{
    public class TechTreeViewModel : BaseViewModel
    {
        #region Fields
        private Culture sourceCulture;
        #endregion

        #region Properties
        public Culture SourceCulture
        {
            get => sourceCulture;
            set
            {
                if (sourceCulture != value)
                {
                    sourceCulture = value;
                    OnPropertyChanged();
                }
            }
        }

        public TechnologyBubbleViewModel PermanentSettlementIViewModel { get; }
        public TechnologyBubbleViewModel AgricultureIViewModel { get; }
        public TechnologyBubbleViewModel OrganizedLaborIViewModel { get; }
        public TechnologyBubbleViewModel ElderReverenceIViewModel { get; }
        public TechnologyBubbleViewModel ElderReverenceIIViewModel { get; }
        #endregion

        #region Constructors
        public TechTreeViewModel(Culture sourceCulture)
        {
            SourceCulture = sourceCulture;
            PermanentSettlementIViewModel = new TechnologyBubbleViewModel(SourceCulture.AllPossibleTechs[SourceCulture.AllPossibleTechs.Keys.First(template => template.Name == "Permanent Settlement (I)")]);
            AgricultureIViewModel = new TechnologyBubbleViewModel(SourceCulture.AllPossibleTechs[SourceCulture.AllPossibleTechs.Keys.First(template => template.Name == "Agriculture (I)")]);
            OrganizedLaborIViewModel = new TechnologyBubbleViewModel(SourceCulture.AllPossibleTechs[SourceCulture.AllPossibleTechs.Keys.First(template => template.Name == "Organized Labor (I)")]);
            ElderReverenceIViewModel = new TechnologyBubbleViewModel(SourceCulture.AllPossibleTechs[SourceCulture.AllPossibleTechs.Keys.First(template => template.Name == "Elder Reverence (I)")]);
            ElderReverenceIIViewModel = new TechnologyBubbleViewModel(SourceCulture.AllPossibleTechs[SourceCulture.AllPossibleTechs.Keys.First(template => template.Name == "Elder Reverence (II)")]);
        }
        #endregion

        #region Methods
        #endregion
    }
}

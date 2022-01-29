﻿using CivCulture_Model.Events;
using CivCulture_Model.Models;
using CivCulture_ViewModel.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CivCulture_ViewModel.ViewModels
{
    public class MapSpaceViewModel : BaseViewModel
    {
        #region Fields
        private MapSpace sourceSpace;
        private CultureViewModel dominantCultureVM;
        #endregion

        #region Events
        #endregion

        #region Properties
        public MapSpace SourceSpace
        {
            get => sourceSpace;
            set
            {
                if (sourceSpace != value)
                {
                    if (sourceSpace != null)
                    {
                        sourceSpace.Pops.CollectionChanged -= Pops_CollectionChanged;
                        sourceSpace.Jobs.CollectionChanged -= Jobs_CollectionChanged;
                        sourceSpace.TerrainResources.CollectionChanged -= TerrainResources_CollectionChanged;
                        sourceSpace.TerrainChanged -= Terrain_Changed;
                        sourceSpace.DominantCultureChanged -= SourceSpace_DominantCultureChanged;
                        DominantCultureVM = null;
                    }
                    sourceSpace = value;
                    if (sourceSpace != null)
                    {
                        sourceSpace.Pops.CollectionChanged += Pops_CollectionChanged;
                        sourceSpace.Jobs.CollectionChanged += Jobs_CollectionChanged;
                        sourceSpace.TerrainResources.CollectionChanged += TerrainResources_CollectionChanged;
                        sourceSpace.TerrainChanged += Terrain_Changed;
                        sourceSpace.DominantCultureChanged += SourceSpace_DominantCultureChanged;
                        if (sourceSpace.DominantCulture != null)
                        {
                            DominantCultureVM = Parent.Parent.CultureVMs.First(cvm => cvm.SourceCulture == sourceSpace.DominantCulture);
                        }
                    }
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Row));
                    OnPropertyChanged(nameof(Column));
                    OnPropertyChanged(nameof(PopCount));
                    OnPropertyChanged(nameof(BackgroundBrush));
                }
            }
        }

        public GameMapViewModel Parent { get; protected set; }

        public CultureViewModel DominantCultureVM
        {
            get => dominantCultureVM;
            set
            {
                if (dominantCultureVM != value)
                {
                    dominantCultureVM = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(DarkCultureColor));
                    OnPropertyChanged(nameof(LightCultureColor));
                }
            }
        }

        public int Row
        {
            get => SourceSpace.Row;
        }

        public int Column
        {
            get => SourceSpace.Column;
        }

        public Brush BackgroundBrush
        {
            get
            {
                if (SourceSpace != null && SourceSpace.Terrain != null)
                {
                    return Brushes.Green; // @TODO
                }
                return Brushes.Black;
            }
        }

        public int PopCount
        {
            get { return SourceSpace.Pops.Count; }
        }

        public MapMode CurrentMapMode
        {
            get => Parent.SelectedMapMode;
        }

        public bool IsTerrainOverlayVisible
        {
            get => true;
        }

        public bool IsCultureOverlayVisible
        {
            get => CurrentMapMode == MapMode.Culture && SourceSpace.DominantCulture != null;
        }

        public Color DarkCultureColor
        {
            get => DominantCultureVM is null ? Colors.White : DominantCultureVM.CultureColor;
        }

        public Color LightCultureColor
        {
            get => GetLightCultureColorFromDarkCultureColor(DarkCultureColor);
        }
        #endregion

        #region Constructors
        public MapSpaceViewModel(MapSpace sourceSpace, GameMapViewModel parent)
        {
            Parent = parent;
            Parent.SelectedMapModeChanged += SelectedMapModeChanged;
            SourceSpace = sourceSpace;
        }
        #endregion

        #region Methods
        public static Color GetLightCultureColorFromDarkCultureColor(Color darkCultureColor)
        {
            return Color.FromArgb((byte)(darkCultureColor.A * 0.75), darkCultureColor.R, darkCultureColor.G, darkCultureColor.B);
        }

        private void SelectedMapModeChanged(object sender, ValueChangedEventArgs<MapMode> e)
        {
            OnPropertyChanged(nameof(CurrentMapMode));
            OnPropertyChanged(nameof(IsTerrainOverlayVisible));
            OnPropertyChanged(nameof(IsCultureOverlayVisible));
        }

        private void Pops_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null || e.OldItems != null)
            {
                OnPropertyChanged(nameof(PopCount));
            }
        }

        private void Jobs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // For now, do nothing. Might not ever do anything?
        }

        private void TerrainResources_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException(); // @TODO
        }

        private void Terrain_Changed(object sender, ValueChangedEventArgs<Terrain> e)
        {
            OnPropertyChanged(nameof(BackgroundBrush));
        }

        private void SourceSpace_DominantCultureChanged(object sender, ValueChangedEventArgs<Culture> e)
        {
            if (e.NewValue is null)
            {
                DominantCultureVM = null;
            }
            else
            {
                DominantCultureVM = Parent.Parent.CultureVMs.First(cvm => cvm.SourceCulture == e.NewValue);
            }
            OnPropertyChanged(nameof(IsCultureOverlayVisible));
        }
        #endregion
    }
}

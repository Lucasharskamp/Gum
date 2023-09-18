﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace WpfDataUi.DataTypes
{
    public class MemberCategory : INotifyPropertyChanged
    {
        #region Properties

        public string Name { get; set; }

        public string Label { get; set; }

        public Visibility Visibility
        {
            get
            {
                if (Members.Count == 0)
                {
                    return System.Windows.Visibility.Collapsed;
                }
                else
                {
                    return System.Windows.Visibility.Visible;

                }
            }
        }

        public bool HideHeader
        {
            get;
            set;
        }

        public int FontSize
        {
            get;
            set;
        }

        double categoryBorderThickness = 1;
        public double CategoryBorderThickness
        {
            get => categoryBorderThickness;
            set
            {
                if (categoryBorderThickness != value)
                {
                    categoryBorderThickness = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CategoryBorderThickness)));

                }
            }
        } 

        public ObservableCollection<InstanceMember> Members
        {
            get;
            private set;
        }

        double? width;
        public double? Width
        {
            get => width;
            set
            {
                if(width != value)
                {
                    width = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Width)));
                }
            }
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        public event Action<InstanceMember> MemberValueChangedByUi;

        #endregion

        #region Methods

        public MemberCategory() 
        {
            InstantiateAll();
        }

        public MemberCategory(string name, string label) 
        {
            InstantiateAll();
            Name = name; 
            Label = label;
        }

        void InstantiateAll()
        {
            HideHeader = false;

            FontSize = 12;

            Members = new ObservableCollection<InstanceMember>();

            Members.CollectionChanged += HandleMembersChanged;
        }

        void HandleMembersChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("Visibility");

            switch(e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach(InstanceMember newItem in e.NewItems)
                    {
                        newItem.Category = this;
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    foreach(InstanceMember newItem in e.NewItems)
                    {
                        newItem.Category = this;
                    }
                    break;
            }
        }

        void NotifyPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        internal void HandleValueSetByUi(InstanceMember instanceMember)
        {
            MemberValueChangedByUi?.Invoke(instanceMember);
        }

        public void SetAlternatingColors(Brush evenBrush, Brush oddBrush)
        {
            for (int i = 0; i < this.Members.Count; i++)
            {
                if (i % 2 == 0)
                {
                    Members[i].BackgroundColor = evenBrush;
                }
                else
                {
                    Members[i].BackgroundColor = oddBrush;
                }

            }
        }

        public override string ToString()
        {
            return Name + " (" + Members.Count + ")";
        }

        #endregion
    }

}

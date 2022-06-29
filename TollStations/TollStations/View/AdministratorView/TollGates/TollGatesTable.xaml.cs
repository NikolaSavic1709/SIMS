﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TollStations.Core.TollGates.Service;
using TollStations.Core.TollStations.Model;
using TollStations.ViewModels.AdministratorViewModels;

namespace TollStations.View.AdministratorView
{
    /// <summary>
    /// Interaction logic for TollGatesTable.xaml
    /// </summary>
    public partial class TollGatesTable : Window
    {
        TollStation _tollStation;
        ITollGateService _gateService;
        public TollGatesTable(ITollGateService tollGateService)
        {
            InitializeComponent();
            _gateService = tollGateService;
        }

        public void SetTollStation(TollStation tollStation)
        {
            _tollStation = tollStation;
            DataContext = new TollGatesTableViewModel(_tollStation, _gateService);
        }
    }
}

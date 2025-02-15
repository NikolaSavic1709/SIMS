﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TollStations.Commands;
using TollStations.Commands.CashierCommands;
using TollStations.Core.SystemUsers.Cashiers.Model;
using TollStations.Core.TollCards;

namespace TollStations.ViewModels.CashierViewModels
{
    public class CashierInitialWindowViewModel : ViewModelBase
    {
        ITollCardService _tollCardService;

        public CashierInitialWindowViewModel(ITollCardService tollCardService)
        {
            _tollCardService = tollCardService;
        }
        public void SetLoggedCashier(Cashier cashier)
        {
            VehicleEntryCommand = new VehicleEntryCommand(cashier, _tollCardService);
            VehicleExitCommand = new VehicleExitCommand(cashier);
            ReportFaultCommand = new ReportFaultCommand(cashier);
        }
        public void SetWindow(Window window)
        {
            LogoutCommand = new LogoutCommand(window);
        }
        public ICommand LogoutCommand { get; set; }
        public ICommand VehicleEntryCommand { get; set; }
        public ICommand VehicleExitCommand { get; set; }
        public ICommand ReportFaultCommand { get; set; }
    }
}

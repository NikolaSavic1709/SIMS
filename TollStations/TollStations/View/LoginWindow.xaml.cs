﻿using HealthInstitution.Core.DIContainer;
using System;
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
using TollStations.Core.Devices.Repository;
using TollStations.Core.Locations.Repository;
using TollStations.Core.Locations.Service;
using TollStations.Core.Prices.Repositor;
using TollStations.Core.RoadSections.Repository;
using TollStations.Core.SystemUsers.Cashiers.Repository;
using TollStations.Core.SystemUsers.Cashiers.Service;
using TollStations.Core.SystemUsers.Chiefs.Repository;
using TollStations.Core.SystemUsers.Chiefs.Service;
using TollStations.Core.SystemUsers.Users.Repository;
using TollStations.Core.SystemUsers.Users.Service;
using TollStations.Core.TollCards.Repository;
using TollStations.Core.TollGates.Repository;
using TollStations.Core.TollGates.Service;
using TollStations.Core.TollPayments.Repository;
using TollStations.Core.TollStations.Repository;
using TollStations.Core.TollStations.Service;
using TollStations.View.AdministratorView;
using TollStations.View.CashierView;

namespace TollStations.View
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            var tt = DIContainer.GetService<IRoadSectionRepository>();
            lbl.Content = tt.GetAll()[0].ExitStation.Chief.FirstName;
            var cashier = DIContainer.GetService<ICashierRepository>().GetById(1);
            new AdministratorWindow().ShowDialog();

            //CashierInitialWindow cashierWindow = new CashierInitialWindow(cashier);
            //cashierWindow.ShowDialog();
        }

        [STAThread]
        public static void Main()
        {
            var services = new DIServiceCollection();

            services.RegisterSingleton<IDeviceRepository, DeviceRepository>();
            services.RegisterSingleton<ILocationRepository, LocationRepository>();
            services.RegisterSingleton<IPriceRepository, PriceRepository>();
            services.RegisterSingleton<IRoadSectionRepository, RoadSectionRepository>();
            services.RegisterSingleton<IChiefRepository, ChiefRepository>();
            services.RegisterSingleton<ICashierRepository, CashierRepository>();
            services.RegisterSingleton<IUserRepository, UserRepository>();
            services.RegisterSingleton<ITollCardRepository, TollCardRepository>();
            services.RegisterSingleton<ITollGateRepository, TollGateRepository>();
            services.RegisterSingleton<ITollPaymentRepository, TollPaymentRepository>();
            services.RegisterSingleton<ITollStationRepository, TollStationRepository>();
            services.RegisterSingleton<IAccountRepository, AccountRepository>();


            services.RegisterSingleton<IUserService, UserService>();
            services.RegisterSingleton<IChiefService, ChiefService>();
            services.RegisterSingleton<ICashierService, CashierService>();
            services.RegisterSingleton<IAccountService, AccountService>();
            services.RegisterSingleton<ITollStationService, TollStationService>();
            services.RegisterSingleton<ITollGateService, TollGateService>();
            services.RegisterSingleton<ILocationService, LocationService>();

            services.RegisterTransient<UsersTable>();
            services.RegisterTransient<AccountsTable>();
            services.RegisterTransient<TollStationsTable>();
            services.RegisterTransient<TollGatesTable>();
            services.RegisterTransient<AddTollStationDialog>();
            services.RegisterTransient<EditTollStationDialog>();
            services.RegisterTransient<AddTollGateDialog>();
            services.RegisterTransient<EditTollGateDialog>();
            services.BuildContainer();

            DIContainer.GetService<IChiefRepository>();
            Window l = new LoginWindow();
            l.ShowDialog();
        }
    }
}

﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TollStations.Core.Devices;
using TollStations.Core.TollPayments.Model;
using TollStations.Core.TollStations.Model;
using TollStations.Core.TollStations.Repository;

namespace TollStations.Core.TollGates.Repository
{
    public class TollGateRepository : ITollGateRepository
    {
        private String _fileName = @"..\..\..\Data\tollStations.json";
        IDeviceRepository _deviceRepository;
        ICashierRepository _cashierRepository;
        ITollPaymentRepository _tollPaymentRepository;
        ITollStationRepository _tollStationRepository;
        public List<TollGate> TollGates { get; set; }
        public Dictionary<int, TollGate> TollGatesById { get; set; }
        private JsonSerializerOptions _options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() },
            PropertyNameCaseInsensitive = true
        };
        public TollGateRepository(IDeviceRepository deviceRepository, ICashierRepository cashierRepository,
            ITollPaymentRepository tollPaymentRepository, ITollStationRepository tollStationRepository)
        {
            _deviceRepository = deviceRepository;
            _cashierRepository = cashierRepository;
            _tollPaymentRepository = tollPaymentRepository;
            _tollStationRepository = tollStationRepository;
            TollGates = new List<TollGate>();
            TollGatesById = new Dictionary<int, TollGate>();
            this.LoadFromFile();
        }

        private List<Device> JToken2Devices(JToken tokens)
        {
            Dictionary<int, Device> devicesById = _deviceRepository.GetAllById();
            List<Device> items = new List<Device>();
            foreach (int token in tokens)
                items.Add(devicesById[token]);
            return items;
        }
        private List<TollPayment> JToken2TollPayments(JToken tokens)
        {
            Dictionary<int, TollPayment> tollPaymentsById = _tollPaymentRepository.GetAllById();
            List<TollPayment> items = new List<TollPayment>();
            foreach (int token in tokens)
                items.Add(tollPaymentsById[token]);
            return items;
        }

        private TollGate Parse(JToken? tollGate)
        {
            Dictionary<int, TollStation> tollStationsById = _tollStationRepository.GetAllById();
            PaymentType paymentType;
            Enum.TryParse(tollGate["paymentType"].ToString(), out paymentType);
            TollGateType tollGateType;
            Enum.TryParse(tollGate["tollGateType"].ToString(), out tollGateType);
            TollStation tollStation = tollStationsById[(int)tollGate["tollStation"]];
            TollGate preparedTollGate = new TollGate((int)tollGate["id"],
                                    (int)tollGate["number"], paymentType, tollGateType,
                                    JToken2Devices(tollGate["devices"]), null,
                                    JToken2TollPayments(tollGate["tollPayments"]), tollStation);
            _tollStationRepository.AddTollGate((int)tollGate["tollStation"], preparedTollGate);
            return preparedTollGate;
        }

        public void LoadFromFile()
        {
            var tollGates = JArray.Parse(File.ReadAllText(_fileName));
            foreach (var tollGate in tollGates)
            {
                TollGate loadedTollGate = Parse(tollGate);
                this.TollGates.Add(loadedTollGate);
                this.TollGatesById[loadedTollGate.Id] = loadedTollGate;
            }
        }

        private List<dynamic> PrepareForSerialization()
        {
            List<dynamic> reducedTollGates = new List<dynamic>();
            foreach (var tollGate in this.TollGates)
            {
                List<int> devicesId = new List<int>();
                List<int> tollPaymentsId = new List<int>();
                foreach (var i in tollGate.Devices)
                    devicesId.Add(i.Id);
                foreach (var i in tollGate.TollPayments)
                    tollPaymentsId.Add(i.Id);
                reducedTollGates.Add(new
                {
                    id = tollGate.Id,
                    number = tollGate.Number,
                    paymentType = tollGate.PaymentType,
                    tollGateType = tollGate.Type,
                    devices = tollGate.Devices,
                    currentCashier = tollGate.CurrentCashier,
                    tollPayments = tollGate.TollPayments,
                    tollStation = tollGate.TollStation
                });
            }
            return reducedTollGates;
        }

        public void Save()
        {
            var allMedicalRecords = JsonSerializer.Serialize(PrepareForSerialization(), _options);
            File.WriteAllText(this._fileName, allMedicalRecords);
        }

        public List<TollGate> GetAll()
        {
            return this.TollGates;
        }

        public Dictionary<int, TollGate> GetAllByUsername()
        {
            return this.TollGatesById;
        }

        public TollGate GetById(int id)
        {
            if (TollGatesById.ContainsKey(id))
                return TollGatesById[id];
            return null;
        }

        public void Add(TollGate tollGate)
        {
            this.TollGates.Add(tollGate);
            this.TollGatesById[tollGate.Id] = tollGate;
            Save();
        }

        public void Update(TollGate byTollGate)
        {
            TollGate tollGate = GetById(byTollGate.Id);
            tollGate.TollStation = byTollGate.TollStation;
            tollGate.CurrentCashier = byTollGate.CurrentCashier;
            tollGate.Type = byTollGate.Type;
            tollGate.PaymentType = byTollGate.PaymentType;
            tollGate.Devices = byTollGate.Devices;
            Save();
        }

        public void AddTollPayment(int id, TollPayment tollPayment)
        {
            TollGate tollGate = GetById(id);
            tollGate.TollPayments.Add(tollPayment);
            Save();
        }

        public void AddDevice(int id, Device device)
        {
            TollGate tollGate = GetById(id);
            tollGate.Devices.Add(device);
            Save();
        }

        public void Delete(TollGate tollGate)
        {
            this.TollGates.Remove(tollGate);
            this.TollGatesById.Remove(tollGate.Id);
            Save();
        }

        public void DeleteTollPayment(int id, TollPayment tollPayment)
        {
            TollGate tollGate = GetById(id);
            tollGate.TollPayments.Remove(tollPayment);
            Save();
        }

        public void DeleteDevice(int id, Device device)
        {
            TollGate tollGate = GetById(id);
            tollGate.Devices.Remove(device);
            Save();
        }
    }
}
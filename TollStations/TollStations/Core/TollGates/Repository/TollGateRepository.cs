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
using TollStations.Core.Devices.Repository;
using TollStations.Core.SystemUsers.Cashiers.Repository;
using TollStations.Core.TollPayments.Model;
using TollStations.Core.TollPayments.Repository;
using TollStations.Core.TollStations.Model;
using TollStations.Core.TollStations.Repository;

namespace TollStations.Core.TollGates.Repository
{
    public class TollGateRepository : ITollGateRepository
    {
        private String _fileName = @"..\..\..\Data\tollGates.json";
        private int _maxId;
        IDeviceRepository _deviceRepository;
        ITollStationRepository _tollStationRepository;
        public List<TollGate> TollGates { get; set; }
        public Dictionary<int, TollGate> TollGatesById { get; set; }
        private JsonSerializerOptions _options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() },
            PropertyNameCaseInsensitive = true
        };
        public TollGateRepository(IDeviceRepository deviceRepository, ITollStationRepository tollStationRepository)
        {
            _deviceRepository = deviceRepository;
            _tollStationRepository = tollStationRepository;
            TollGates = new List<TollGate>();
            TollGatesById = new Dictionary<int, TollGate>();
            _maxId = 0;
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
        private TollGate Parse(JToken? tollGate)
        {
            Dictionary<int, TollStation> tollStationsById = _tollStationRepository.GetAllById();
            PaymentType paymentType;
            Enum.TryParse(tollGate["paymentType"].ToString(), out paymentType);
            TollGateType tollGateType;
            Enum.TryParse(tollGate["tollGateType"].ToString(), out tollGateType);
            TollStation tollStation = tollStationsById[(int)tollGate["tollStation"]];
            TollGate loadedTollGate = new TollGate((int)tollGate["id"],
                                    (int)tollGate["number"], paymentType, tollGateType,
                                    JToken2Devices(tollGate["devices"]), null,
                                    new List<TollPayment>(), tollStation);
            tollStation.Gates.Add(loadedTollGate);
            return loadedTollGate;
        }

        public void LoadFromFile()
        {
            var tollGates = JArray.Parse(File.ReadAllText(_fileName));
            foreach (var tollGate in tollGates)
            {
                TollGate loadedTollGate = Parse(tollGate);
                if (loadedTollGate.Id > _maxId)
                {
                    _maxId = loadedTollGate.Id;
                }
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
                foreach (var i in tollGate.Devices)
                    devicesId.Add(i.Id);
                reducedTollGates.Add(new
                {
                    id = tollGate.Id,
                    number = tollGate.Number,
                    paymentType = tollGate.PaymentType,
                    tollGateType = tollGate.Type,
                    devices = devicesId,
                    currentCashier = tollGate.CurrentCashier.Id,
                    tollStation = tollGate.TollStation.Id
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

        public List<TollGate> GetByStation(int id)
        {
            return TollGates.FindAll(item => item.TollStation.Id == id);
        }

        public void Add(TollGate tollGate)
        {
            tollGate.Id = ++_maxId;
            this.TollGates.Add(tollGate);
            this.TollGatesById[tollGate.Id] = tollGate;
            Save();
        }

        public void Update(int id, TollGate byTollGate)
        {
            TollGate tollGate = GetById(id);
            tollGate.Number = byTollGate.Number;
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

        public void Delete(int id)
        {
            var tollGate = GetById(id);
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

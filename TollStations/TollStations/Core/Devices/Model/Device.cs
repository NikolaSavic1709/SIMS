﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TollStations.Core.Devices.Model;

namespace TollStations.Core.Devices
{
    public class Device
    {
        public int Id { get; set; }
        public DeviceType Type { get; set; }
        public bool IsValid { get; set; }

        
        [JsonConstructor]
        public Device(int id, DeviceType type, bool isValid)
        {
            Id = id;
            Type = type;
            IsValid = isValid;
        }

        public Device(DeviceDTO deviceDTO)
        {
            Type = deviceDTO.Type;
            IsValid = deviceDTO.IsValid;
        }
    }



    public enum DeviceType
    {
        RAMP,
        TAG_READER,
        PLATE_READER,
        CAMERA,
        MONITOR,
        TRAFFIC_LIGHT
    }
}

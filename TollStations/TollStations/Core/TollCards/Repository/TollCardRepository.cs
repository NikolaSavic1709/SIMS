﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TollStations.Core.TollCards.Model;
using TollStations.Core.TollStations.Repository;

namespace TollStations.Core.TollCards.Repository
{
    class TollCardRepository : ITollCardRepository
    {
        private String _fileName = @"..\..\..\Data\tollCards.json";

        private int _maxId;

        private ITollStationRepository tollStationRepository;
        public List<TollCard> TollCards { get; set; }
        public Dictionary<int, TollCard> TollCardsById { get; set; }

        private JsonSerializerOptions _options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() },
            PropertyNameCaseInsensitive = true
        };

        public TollCardRepository(ITollStationRepository tollStationRepository)
        {
            this.TollCards = new List<TollCard>();
            this.TollCardsById = new Dictionary<int, TollCard>();
            this._maxId = 0;
            this.tollStationRepository = tollStationRepository;
            this.LoadFromFile();
        }

        private TollCard Parse(JToken? tollCard)
        {
            return new TollCard((int)tollCard["id"], (DateTime)tollCard["time"], (string)tollCard["plate"], tollStationRepository.GetById((int)tollCard["entryStation"]), (bool)tollCard["valid"]);
        }

        public void LoadFromFile()
        {
            var tollCards = JArray.Parse(File.ReadAllText(_fileName));
            foreach (var card in tollCards)
            {
                TollCard loadedCard = Parse(card);
                if (loadedCard.Id > _maxId)
                {
                    _maxId = loadedCard.Id;
                }
                this.TollCards.Add(loadedCard);
                this.TollCardsById[loadedCard.Id] = loadedCard;
            }
        }
        private List<dynamic> PrepareForSerialization()
        {
            List<dynamic> reducedTollCards = new List<dynamic>();
            foreach (var tollCard in this.TollCards)
            {
                reducedTollCards.Add(new
                {
                    id = tollCard.Id,
                    time = tollCard.Time,
                    plate = tollCard.Plate,
                    entryStation = tollCard.EntryStation.Id,
                    valid= tollCard.Valid
                });
            }
            return reducedTollCards;
        }

        public void Save()
        {
            var allLocations = JsonSerializer.Serialize(PrepareForSerialization(), _options);
            File.WriteAllText(this._fileName, allLocations);
        }

        public List<TollCard> GetAll()
        {
            return this.TollCards;
        }

        public Dictionary<int, TollCard> GetAllById()
        {
            return this.TollCardsById;
        }

        public TollCard GetById(int id)
        {
            if (TollCardsById.ContainsKey(id))
                return TollCardsById[id];
            return null;
        }

        public TollCard Add(TollCard card)
        {
            this._maxId++;
            int id = this._maxId;
            card.Id = id;

            this.TollCards.Add(card);
            this.TollCardsById.Add(card.Id, card);
            Save();
            return card;
        }
    }
}

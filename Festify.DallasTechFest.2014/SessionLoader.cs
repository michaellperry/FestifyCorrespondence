using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using UpdateControls.Correspondence.FileStream;
using Festify.Logic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Festify.Model;
using Festify.Logic;
using UpdateControls.Correspondence.BinaryHTTPClient;
using UpdateControls.Correspondence.Memory;

namespace Festify.DallasTechFest._2014
{
    public class SessionLoader
    {
        private readonly Device _device;
        private readonly HttpClient _httpClient;

        public SessionLoader()
        {
            FileStreamStorageStrategy fileStream = new FileStreamStorageStrategy(
                Path.Combine(
                    Environment.CurrentDirectory,
                    "Correspondence"));
            _device = new Device(fileStream);
            var http = new HTTPConfigurationProvider();
            var communication = new BinaryHTTPAsynchronousCommunicationStrategy(http);
            _device.Community.AddAsynchronousCommunicationStrategy(communication);
            _device.Subscribe();

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://dallastechfest.com/api/", UriKind.Absolute);
        }

        public void Load()
        {
            LoadAsync().Wait();
        }

        private async Task LoadAsync()
        {
            if (!await SynchronizeAsync())
                return;

            await _device.CreateIndividualAsync();

            HttpResponseMessage sessionsResponse = await _httpClient.GetAsync("sessions");
            if (!sessionsResponse.IsSuccessStatusCode)
            {
                Output(String.Format("Failed: {0} {1}",
                    sessionsResponse.ReasonPhrase,
                    sessionsResponse.RequestMessage));
                return;
            }

            HttpContent content = sessionsResponse.Content;
            var sessionsJson = await content.ReadAsStringAsync();

            var sessions = JsonConvert.DeserializeObject<DTFSessionDTO[]>(sessionsJson);

            _device.Conference.Name = "Dallas TechFest 2014";

            foreach (var session in sessions)
            {
                await ImportSession(session);
            }

            if (!await SynchronizeAsync())
                return;

            Output("Success!");
        }

        private async Task ImportSession(DTFSessionDTO sessionDto)
        {
            var community = _device.Community;
            var conference = _device.Conference;

            var conferenceDate = new DateTime(2014, 10, 10, 0, 0, 0, DateTimeKind.Utc);
            var day = await community.AddFactAsync(new Day(conference, conferenceDate));
            var start = (conferenceDate + DateTime.Parse(sessionDto.timeSlot).TimeOfDay).AddHours(5);
            var time = await community.AddFactAsync(new Time(day, start));
            Output(start.ToLocalTime().ToString());

            var room = await FindRoom(sessionDto.room);

            var place = await community.AddFactAsync(new Place(time, room));

            var speaker = await FindSpeaker(sessionDto.speakerName);
            speaker.ImageUrl = sessionDto.speakerPhotoUrl;
            await speaker.SetBio(sessionDto.speakerBio);

            var session = await FindSession(speaker, sessionDto.id);
            session.Name = sessionDto.title;
            await session.SetDescription(sessionDto.@abstract);

            var sessionPlaces = await session.CurrentSessionPlaces.EnsureAsync();
            if (sessionPlaces.Count() == 1)
            {
                Place oldPlace = await sessionPlaces.Single().Place.EnsureAsync();
                Place newPlace = place;
                if (oldPlace != newPlace)
                {
                    Output(String.Format("New session place {0} {1}.", sessionDto.timeSlot, sessionDto.title));
                    await community.AddFactAsync(new SessionPlace(session, place, sessionPlaces));
                }
            }
            else
            {
                Output(String.Format("New session place {0} {1}.", sessionDto.timeSlot, sessionDto.title));
                await community.AddFactAsync(new SessionPlace(session, place, sessionPlaces));
            }
        }

        private async Task<Room> FindRoom(string roomNumber)
        {
            var community = _device.Community;
            var conference = _device.Conference;

            var rooms = await _device.Conference.Rooms.EnsureAsync();
            foreach (var room in rooms)
            {
                string rn = await room.RoomNumber.EnsureAsync();
                if (rn == roomNumber)
                    return room;
            }

            var newRoom = await community.AddFactAsync(new Room(conference));
            newRoom.RoomNumber = roomNumber;

            Output(String.Format("New room {0}", roomNumber));
            return newRoom;
        }

        private async Task<Speaker> FindSpeaker(string speakerName)
        {
            var community = _device.Community;
            var conference = _device.Conference;

            var speakers = await _device.Conference.Speakers.EnsureAsync();
            foreach (var speaker in speakers)
            {
                string sn = await speaker.Name.EnsureAsync();
                if (sn == speakerName)
                    return speaker;
            }

            var newSpeaker = await community.AddFactAsync(new Speaker(conference));
            newSpeaker.Name = speakerName;

            Output(String.Format("New speaker {0}", speakerName));
            return newSpeaker;
        }

        private async Task<Session> FindSession(Speaker speaker, string sessionId)
        {
            var community = _device.Community;
            var conference = _device.Conference;

            var sessions = await speaker.Sessions.EnsureAsync();
            foreach (var session in sessions)
            {
                string id = await session.DtfSessionId.EnsureAsync();
                if (id == sessionId)
                    return session;
            }

            var newSession = await community.AddFactAsync(new Session(conference, speaker, null));
            newSession.DtfSessionId = sessionId;

            Output(String.Format("New session {0}", sessionId));
            return newSession;
        }

        private async Task<bool> SynchronizeAsync()
        {
            _device.Community.BeginReceiving();

            while (_device.Community.Synchronizing)
                await Task.Delay(500);

            _device.Community.BeginSending();

            while (_device.Community.Synchronizing)
                await Task.Delay(500);

            bool ok = _device.Community.LastException == null;
            if (!ok)
                Output(_device.Community.LastException.Message);

            return ok;
        }

        private void Output(string line)
        {
            Console.WriteLine(line);
        }
    }
}

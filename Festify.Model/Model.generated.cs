﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;
using System;
using System.IO;

/**
/ For use with http://graphviz.org/
digraph "Festify.Model"
{
    rankdir=BT
    EnableToastNotification -> Individual
    DisableToastNotification -> EnableToastNotification [label="  *"]
    Conference__name -> Conference [color="red"]
    Conference__name -> Conference__name [label="  *"]
    Conference__mapUrl -> Conference [color="red"]
    Conference__mapUrl -> Conference__mapUrl [label="  *"]
    Attendee -> Conference
    IndividualAttendee -> Individual [color="red"]
    IndividualAttendee -> Attendee
    Day -> Conference [color="red"]
    Time -> Day
    TimeDelete -> Time
    TimeUndelete -> TimeDelete
    Room -> Conference [color="red"]
    Room__roomNumber -> Room
    Room__roomNumber -> Room__roomNumber [label="  *"]
    Track -> Conference [color="red"]
    Speaker -> Conference [color="red"]
    Speaker__name -> Speaker
    Speaker__name -> Speaker__name [label="  *"]
    Speaker__imageUrl -> Speaker
    Speaker__imageUrl -> Speaker__imageUrl [label="  *"]
    Speaker__contact -> Speaker
    Speaker__contact -> Speaker__contact [label="  *"]
    Speaker__bio -> Speaker
    Speaker__bio -> Speaker__bio [label="  *"]
    Speaker__bio -> DocumentSegment [label="  *"]
    ConferenceNotice -> Conference [color="red"]
    Place -> Time
    Place -> Room
    Session -> Conference [color="red"]
    Session -> Speaker
    Session -> Track [label="  ?"]
    Session__name -> Session
    Session__name -> Session__name [label="  *"]
    Session__description -> Session
    Session__description -> Session__description [label="  *"]
    Session__description -> DocumentSegment [label="  *"]
    Session__level -> Session
    Session__level -> Session__level [label="  *"]
    Session__level -> Level
    Session__dtfSessionId -> Session
    Session__dtfSessionId -> Session__dtfSessionId [label="  *"]
    SessionDelete -> Session
    SessionUndelete -> SessionDelete
    SessionNotice -> Session [color="red"]
    SessionPlace -> Session
    SessionPlace -> Place
    SessionPlace -> SessionPlace [label="  *"]
    LikeSession -> Attendee [color="red"]
    LikeSession -> Session [color="red"]
    UnlikeSession -> LikeSession
}
**/

namespace Festify.Model
{
    public partial class Individual : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				Individual newFact = new Individual(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._unique = (Guid)_fieldSerializerByType[typeof(Guid)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Individual fact = (Individual)obj;
				_fieldSerializerByType[typeof(Guid)].WriteData(output, fact._unique);
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return Individual.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return Individual.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.Individual", 2);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static Individual GetUnloadedInstance()
        {
            return new Individual((FactMemento)null) { IsLoaded = false };
        }

        public static Individual GetNullInstance()
        {
            return new Individual((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<Individual> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (Individual)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles

        // Queries
        private static Query _cacheQueryAttendees;

        public static Query GetQueryAttendees()
		{
            if (_cacheQueryAttendees == null)
            {
			    _cacheQueryAttendees = new Query()
		    		.JoinSuccessors(IndividualAttendee.GetRoleIndividual())
		    		.JoinPredecessors(IndividualAttendee.GetRoleAttendee())
                ;
            }
            return _cacheQueryAttendees;
		}
        private static Query _cacheQueryIsToastNotificationEnabled;

        public static Query GetQueryIsToastNotificationEnabled()
		{
            if (_cacheQueryIsToastNotificationEnabled == null)
            {
			    _cacheQueryIsToastNotificationEnabled = new Query()
    				.JoinSuccessors(EnableToastNotification.GetRoleIndividual(), Condition.WhereIsEmpty(EnableToastNotification.GetQueryIsDisabled())
				)
                ;
            }
            return _cacheQueryIsToastNotificationEnabled;
		}
        private static Query _cacheQueryLikedSessions;

        public static Query GetQueryLikedSessions()
		{
            if (_cacheQueryLikedSessions == null)
            {
			    _cacheQueryLikedSessions = new Query()
		    		.JoinSuccessors(IndividualAttendee.GetRoleIndividual())
		    		.JoinPredecessors(IndividualAttendee.GetRoleAttendee())
    				.JoinSuccessors(LikeSession.GetRoleAttendee(), Condition.WhereIsEmpty(LikeSession.GetQueryIsDeleted())
				)
                ;
            }
            return _cacheQueryLikedSessions;
		}

        // Predicates

        // Predecessors

        // Unique
        private Guid _unique;

        // Fields

        // Results
        private Result<Attendee> _attendees;
        private Result<EnableToastNotification> _isToastNotificationEnabled;
        private Result<LikeSession> _likedSessions;

        // Business constructor
        public Individual(
            )
        {
            _unique = Guid.NewGuid();
            InitializeResults();
        }

        // Hydration constructor
        private Individual(FactMemento memento)
        {
            InitializeResults();
        }

        // Result initializer
        private void InitializeResults()
        {
            _attendees = new Result<Attendee>(this, GetQueryAttendees(), Attendee.GetUnloadedInstance, Attendee.GetNullInstance);
            _isToastNotificationEnabled = new Result<EnableToastNotification>(this, GetQueryIsToastNotificationEnabled(), EnableToastNotification.GetUnloadedInstance, EnableToastNotification.GetNullInstance);
            _likedSessions = new Result<LikeSession>(this, GetQueryLikedSessions(), LikeSession.GetUnloadedInstance, LikeSession.GetNullInstance);
        }

        // Predecessor access

        // Field access
		public Guid Unique { get { return _unique; } }


        // Query result access
        public Result<Attendee> Attendees
        {
            get { return _attendees; }
        }
        public Result<EnableToastNotification> IsToastNotificationEnabled
        {
            get { return _isToastNotificationEnabled; }
        }
        public Result<LikeSession> LikedSessions
        {
            get { return _likedSessions; }
        }

        // Mutable property access

    }
    
    public partial class EnableToastNotification : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				EnableToastNotification newFact = new EnableToastNotification(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._unique = (Guid)_fieldSerializerByType[typeof(Guid)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				EnableToastNotification fact = (EnableToastNotification)obj;
				_fieldSerializerByType[typeof(Guid)].WriteData(output, fact._unique);
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return EnableToastNotification.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return EnableToastNotification.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.EnableToastNotification", -1243635142);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static EnableToastNotification GetUnloadedInstance()
        {
            return new EnableToastNotification((FactMemento)null) { IsLoaded = false };
        }

        public static EnableToastNotification GetNullInstance()
        {
            return new EnableToastNotification((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<EnableToastNotification> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (EnableToastNotification)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleIndividual;
        public static Role GetRoleIndividual()
        {
            if (_cacheRoleIndividual == null)
            {
                _cacheRoleIndividual = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "individual",
			        Individual._correspondenceFactType,
			        false));
            }
            return _cacheRoleIndividual;
        }

        // Queries
        private static Query _cacheQueryIsDisabled;

        public static Query GetQueryIsDisabled()
		{
            if (_cacheQueryIsDisabled == null)
            {
			    _cacheQueryIsDisabled = new Query()
		    		.JoinSuccessors(DisableToastNotification.GetRoleEnable())
                ;
            }
            return _cacheQueryIsDisabled;
		}

        // Predicates
        public static Condition IsDisabled = Condition.WhereIsNotEmpty(GetQueryIsDisabled());

        // Predecessors
        private PredecessorObj<Individual> _individual;

        // Unique
        private Guid _unique;

        // Fields

        // Results

        // Business constructor
        public EnableToastNotification(
            Individual individual
            )
        {
            _unique = Guid.NewGuid();
            InitializeResults();
            _individual = new PredecessorObj<Individual>(this, GetRoleIndividual(), individual);
        }

        // Hydration constructor
        private EnableToastNotification(FactMemento memento)
        {
            InitializeResults();
            _individual = new PredecessorObj<Individual>(this, GetRoleIndividual(), memento, Individual.GetUnloadedInstance, Individual.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Individual Individual
        {
            get { return IsNull ? Individual.GetNullInstance() : _individual.Fact; }
        }

        // Field access
		public Guid Unique { get { return _unique; } }


        // Query result access

        // Mutable property access

    }
    
    public partial class DisableToastNotification : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				DisableToastNotification newFact = new DisableToastNotification(memento);


				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				DisableToastNotification fact = (DisableToastNotification)obj;
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return DisableToastNotification.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return DisableToastNotification.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.DisableToastNotification", -686635848);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static DisableToastNotification GetUnloadedInstance()
        {
            return new DisableToastNotification((FactMemento)null) { IsLoaded = false };
        }

        public static DisableToastNotification GetNullInstance()
        {
            return new DisableToastNotification((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<DisableToastNotification> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (DisableToastNotification)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleEnable;
        public static Role GetRoleEnable()
        {
            if (_cacheRoleEnable == null)
            {
                _cacheRoleEnable = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "enable",
			        EnableToastNotification._correspondenceFactType,
			        false));
            }
            return _cacheRoleEnable;
        }

        // Queries

        // Predicates

        // Predecessors
        private PredecessorList<EnableToastNotification> _enable;

        // Fields

        // Results

        // Business constructor
        public DisableToastNotification(
            IEnumerable<EnableToastNotification> enable
            )
        {
            InitializeResults();
            _enable = new PredecessorList<EnableToastNotification>(this, GetRoleEnable(), enable);
        }

        // Hydration constructor
        private DisableToastNotification(FactMemento memento)
        {
            InitializeResults();
            _enable = new PredecessorList<EnableToastNotification>(this, GetRoleEnable(), memento, EnableToastNotification.GetUnloadedInstance, EnableToastNotification.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public PredecessorList<EnableToastNotification> Enable
        {
            get { return _enable; }
        }

        // Field access

        // Query result access

        // Mutable property access

    }
    
    public partial class Conference : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				Conference newFact = new Conference(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._id = (string)_fieldSerializerByType[typeof(string)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Conference fact = (Conference)obj;
				_fieldSerializerByType[typeof(string)].WriteData(output, fact._id);
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return Conference.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return Conference.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.Conference", 8);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static Conference GetUnloadedInstance()
        {
            return new Conference((FactMemento)null) { IsLoaded = false };
        }

        public static Conference GetNullInstance()
        {
            return new Conference((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<Conference> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (Conference)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles

        // Queries
        private static Query _cacheQueryName;

        public static Query GetQueryName()
		{
            if (_cacheQueryName == null)
            {
			    _cacheQueryName = new Query()
    				.JoinSuccessors(Conference__name.GetRoleConference(), Condition.WhereIsEmpty(Conference__name.GetQueryIsCurrent())
				)
                ;
            }
            return _cacheQueryName;
		}
        private static Query _cacheQueryMapUrl;

        public static Query GetQueryMapUrl()
		{
            if (_cacheQueryMapUrl == null)
            {
			    _cacheQueryMapUrl = new Query()
    				.JoinSuccessors(Conference__mapUrl.GetRoleConference(), Condition.WhereIsEmpty(Conference__mapUrl.GetQueryIsCurrent())
				)
                ;
            }
            return _cacheQueryMapUrl;
		}
        private static Query _cacheQueryDays;

        public static Query GetQueryDays()
		{
            if (_cacheQueryDays == null)
            {
			    _cacheQueryDays = new Query()
    				.JoinSuccessors(Day.GetRoleConference(), Condition.WhereIsNotEmpty(Day.GetQueryHasTimes())
				)
                ;
            }
            return _cacheQueryDays;
		}
        private static Query _cacheQueryAllTracks;

        public static Query GetQueryAllTracks()
		{
            if (_cacheQueryAllTracks == null)
            {
			    _cacheQueryAllTracks = new Query()
		    		.JoinSuccessors(Track.GetRoleConference())
                ;
            }
            return _cacheQueryAllTracks;
		}
        private static Query _cacheQueryTracks;

        public static Query GetQueryTracks()
		{
            if (_cacheQueryTracks == null)
            {
			    _cacheQueryTracks = new Query()
    				.JoinSuccessors(Track.GetRoleConference(), Condition.WhereIsNotEmpty(Track.GetQueryHasSessions())
				)
                ;
            }
            return _cacheQueryTracks;
		}
        private static Query _cacheQuerySessions;

        public static Query GetQuerySessions()
		{
            if (_cacheQuerySessions == null)
            {
			    _cacheQuerySessions = new Query()
    				.JoinSuccessors(Session.GetRoleConference(), Condition.WhereIsEmpty(Session.GetQueryIsDeleted())
				)
                ;
            }
            return _cacheQuerySessions;
		}
        private static Query _cacheQueryUnscheduledSessions;

        public static Query GetQueryUnscheduledSessions()
		{
            if (_cacheQueryUnscheduledSessions == null)
            {
			    _cacheQueryUnscheduledSessions = new Query()
    				.JoinSuccessors(Session.GetRoleConference(), Condition.WhereIsEmpty(Session.GetQueryIsDeleted())
	    				.And().IsEmpty(Session.GetQueryIsScheduled())
				)
                ;
            }
            return _cacheQueryUnscheduledSessions;
		}
        private static Query _cacheQuerySpeakers;

        public static Query GetQuerySpeakers()
		{
            if (_cacheQuerySpeakers == null)
            {
			    _cacheQuerySpeakers = new Query()
		    		.JoinSuccessors(Speaker.GetRoleConference())
                ;
            }
            return _cacheQuerySpeakers;
		}
        private static Query _cacheQueryNotices;

        public static Query GetQueryNotices()
		{
            if (_cacheQueryNotices == null)
            {
			    _cacheQueryNotices = new Query()
		    		.JoinSuccessors(ConferenceNotice.GetRoleConference())
                ;
            }
            return _cacheQueryNotices;
		}
        private static Query _cacheQueryRooms;

        public static Query GetQueryRooms()
		{
            if (_cacheQueryRooms == null)
            {
			    _cacheQueryRooms = new Query()
		    		.JoinSuccessors(Room.GetRoleConference())
                ;
            }
            return _cacheQueryRooms;
		}

        // Predicates

        // Predecessors

        // Fields
        private string _id;

        // Results
        private Result<Conference__name> _name;
        private Result<Conference__mapUrl> _mapUrl;
        private Result<Day> _days;
        private Result<Track> _allTracks;
        private Result<Track> _tracks;
        private Result<Session> _sessions;
        private Result<Session> _unscheduledSessions;
        private Result<Speaker> _speakers;
        private Result<ConferenceNotice> _notices;
        private Result<Room> _rooms;

        // Business constructor
        public Conference(
            string id
            )
        {
            InitializeResults();
            _id = id;
        }

        // Hydration constructor
        private Conference(FactMemento memento)
        {
            InitializeResults();
        }

        // Result initializer
        private void InitializeResults()
        {
            _name = new Result<Conference__name>(this, GetQueryName(), Conference__name.GetUnloadedInstance, Conference__name.GetNullInstance);
            _mapUrl = new Result<Conference__mapUrl>(this, GetQueryMapUrl(), Conference__mapUrl.GetUnloadedInstance, Conference__mapUrl.GetNullInstance);
            _days = new Result<Day>(this, GetQueryDays(), Day.GetUnloadedInstance, Day.GetNullInstance);
            _allTracks = new Result<Track>(this, GetQueryAllTracks(), Track.GetUnloadedInstance, Track.GetNullInstance);
            _tracks = new Result<Track>(this, GetQueryTracks(), Track.GetUnloadedInstance, Track.GetNullInstance);
            _sessions = new Result<Session>(this, GetQuerySessions(), Session.GetUnloadedInstance, Session.GetNullInstance);
            _unscheduledSessions = new Result<Session>(this, GetQueryUnscheduledSessions(), Session.GetUnloadedInstance, Session.GetNullInstance);
            _speakers = new Result<Speaker>(this, GetQuerySpeakers(), Speaker.GetUnloadedInstance, Speaker.GetNullInstance);
            _notices = new Result<ConferenceNotice>(this, GetQueryNotices(), ConferenceNotice.GetUnloadedInstance, ConferenceNotice.GetNullInstance);
            _rooms = new Result<Room>(this, GetQueryRooms(), Room.GetUnloadedInstance, Room.GetNullInstance);
        }

        // Predecessor access

        // Field access
        public string Id
        {
            get { return _id; }
        }

        // Query result access
        public Result<Day> Days
        {
            get { return _days; }
        }
        public Result<Track> AllTracks
        {
            get { return _allTracks; }
        }
        public Result<Track> Tracks
        {
            get { return _tracks; }
        }
        public Result<Session> Sessions
        {
            get { return _sessions; }
        }
        public Result<Session> UnscheduledSessions
        {
            get { return _unscheduledSessions; }
        }
        public Result<Speaker> Speakers
        {
            get { return _speakers; }
        }
        public Result<ConferenceNotice> Notices
        {
            get { return _notices; }
        }
        public Result<Room> Rooms
        {
            get { return _rooms; }
        }

        // Mutable property access
        public TransientDisputable<Conference__name, string> Name
        {
            get { return _name.AsTransientDisputable(fact => fact.Value); }
			set
			{
                Community.Perform(async delegate()
                {
                    var current = (await _name.EnsureAsync()).ToList();
                    if (current.Count != 1 || !object.Equals(current[0].Value, value.Value))
                    {
                        await Community.AddFactAsync(new Conference__name(this, _name, value.Value));
                    }
                });
			}
        }
        public TransientDisputable<Conference__mapUrl, string> MapUrl
        {
            get { return _mapUrl.AsTransientDisputable(fact => fact.Value); }
			set
			{
                Community.Perform(async delegate()
                {
                    var current = (await _mapUrl.EnsureAsync()).ToList();
                    if (current.Count != 1 || !object.Equals(current[0].Value, value.Value))
                    {
                        await Community.AddFactAsync(new Conference__mapUrl(this, _mapUrl, value.Value));
                    }
                });
			}
        }

    }
    
    public partial class Conference__name : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				Conference__name newFact = new Conference__name(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._value = (string)_fieldSerializerByType[typeof(string)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Conference__name fact = (Conference__name)obj;
				_fieldSerializerByType[typeof(string)].WriteData(output, fact._value);
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return Conference__name.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return Conference__name.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.Conference__name", 803419988);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static Conference__name GetUnloadedInstance()
        {
            return new Conference__name((FactMemento)null) { IsLoaded = false };
        }

        public static Conference__name GetNullInstance()
        {
            return new Conference__name((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<Conference__name> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (Conference__name)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleConference;
        public static Role GetRoleConference()
        {
            if (_cacheRoleConference == null)
            {
                _cacheRoleConference = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "conference",
			        Conference._correspondenceFactType,
			        true));
            }
            return _cacheRoleConference;
        }
        private static Role _cacheRolePrior;
        public static Role GetRolePrior()
        {
            if (_cacheRolePrior == null)
            {
                _cacheRolePrior = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "prior",
			        Conference__name._correspondenceFactType,
			        false));
            }
            return _cacheRolePrior;
        }

        // Queries
        private static Query _cacheQueryIsCurrent;

        public static Query GetQueryIsCurrent()
		{
            if (_cacheQueryIsCurrent == null)
            {
			    _cacheQueryIsCurrent = new Query()
		    		.JoinSuccessors(Conference__name.GetRolePrior())
                ;
            }
            return _cacheQueryIsCurrent;
		}

        // Predicates
        public static Condition IsCurrent = Condition.WhereIsEmpty(GetQueryIsCurrent());

        // Predecessors
        private PredecessorObj<Conference> _conference;
        private PredecessorList<Conference__name> _prior;

        // Fields
        private string _value;

        // Results

        // Business constructor
        public Conference__name(
            Conference conference
            ,IEnumerable<Conference__name> prior
            ,string value
            )
        {
            InitializeResults();
            _conference = new PredecessorObj<Conference>(this, GetRoleConference(), conference);
            _prior = new PredecessorList<Conference__name>(this, GetRolePrior(), prior);
            _value = value;
        }

        // Hydration constructor
        private Conference__name(FactMemento memento)
        {
            InitializeResults();
            _conference = new PredecessorObj<Conference>(this, GetRoleConference(), memento, Conference.GetUnloadedInstance, Conference.GetNullInstance);
            _prior = new PredecessorList<Conference__name>(this, GetRolePrior(), memento, Conference__name.GetUnloadedInstance, Conference__name.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Conference Conference
        {
            get { return IsNull ? Conference.GetNullInstance() : _conference.Fact; }
        }
        public PredecessorList<Conference__name> Prior
        {
            get { return _prior; }
        }

        // Field access
        public string Value
        {
            get { return _value; }
        }

        // Query result access

        // Mutable property access

    }
    
    public partial class Conference__mapUrl : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				Conference__mapUrl newFact = new Conference__mapUrl(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._value = (string)_fieldSerializerByType[typeof(string)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Conference__mapUrl fact = (Conference__mapUrl)obj;
				_fieldSerializerByType[typeof(string)].WriteData(output, fact._value);
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return Conference__mapUrl.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return Conference__mapUrl.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.Conference__mapUrl", 803419988);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static Conference__mapUrl GetUnloadedInstance()
        {
            return new Conference__mapUrl((FactMemento)null) { IsLoaded = false };
        }

        public static Conference__mapUrl GetNullInstance()
        {
            return new Conference__mapUrl((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<Conference__mapUrl> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (Conference__mapUrl)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleConference;
        public static Role GetRoleConference()
        {
            if (_cacheRoleConference == null)
            {
                _cacheRoleConference = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "conference",
			        Conference._correspondenceFactType,
			        true));
            }
            return _cacheRoleConference;
        }
        private static Role _cacheRolePrior;
        public static Role GetRolePrior()
        {
            if (_cacheRolePrior == null)
            {
                _cacheRolePrior = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "prior",
			        Conference__mapUrl._correspondenceFactType,
			        false));
            }
            return _cacheRolePrior;
        }

        // Queries
        private static Query _cacheQueryIsCurrent;

        public static Query GetQueryIsCurrent()
		{
            if (_cacheQueryIsCurrent == null)
            {
			    _cacheQueryIsCurrent = new Query()
		    		.JoinSuccessors(Conference__mapUrl.GetRolePrior())
                ;
            }
            return _cacheQueryIsCurrent;
		}

        // Predicates
        public static Condition IsCurrent = Condition.WhereIsEmpty(GetQueryIsCurrent());

        // Predecessors
        private PredecessorObj<Conference> _conference;
        private PredecessorList<Conference__mapUrl> _prior;

        // Fields
        private string _value;

        // Results

        // Business constructor
        public Conference__mapUrl(
            Conference conference
            ,IEnumerable<Conference__mapUrl> prior
            ,string value
            )
        {
            InitializeResults();
            _conference = new PredecessorObj<Conference>(this, GetRoleConference(), conference);
            _prior = new PredecessorList<Conference__mapUrl>(this, GetRolePrior(), prior);
            _value = value;
        }

        // Hydration constructor
        private Conference__mapUrl(FactMemento memento)
        {
            InitializeResults();
            _conference = new PredecessorObj<Conference>(this, GetRoleConference(), memento, Conference.GetUnloadedInstance, Conference.GetNullInstance);
            _prior = new PredecessorList<Conference__mapUrl>(this, GetRolePrior(), memento, Conference__mapUrl.GetUnloadedInstance, Conference__mapUrl.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Conference Conference
        {
            get { return IsNull ? Conference.GetNullInstance() : _conference.Fact; }
        }
        public PredecessorList<Conference__mapUrl> Prior
        {
            get { return _prior; }
        }

        // Field access
        public string Value
        {
            get { return _value; }
        }

        // Query result access

        // Mutable property access

    }
    
    public partial class Attendee : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				Attendee newFact = new Attendee(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._identifier = (string)_fieldSerializerByType[typeof(string)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Attendee fact = (Attendee)obj;
				_fieldSerializerByType[typeof(string)].WriteData(output, fact._identifier);
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return Attendee.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return Attendee.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.Attendee", 1099669528);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static Attendee GetUnloadedInstance()
        {
            return new Attendee((FactMemento)null) { IsLoaded = false };
        }

        public static Attendee GetNullInstance()
        {
            return new Attendee((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<Attendee> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (Attendee)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleConference;
        public static Role GetRoleConference()
        {
            if (_cacheRoleConference == null)
            {
                _cacheRoleConference = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "conference",
			        Conference._correspondenceFactType,
			        false));
            }
            return _cacheRoleConference;
        }

        // Queries

        // Predicates

        // Predecessors
        private PredecessorObj<Conference> _conference;

        // Fields
        private string _identifier;

        // Results

        // Business constructor
        public Attendee(
            Conference conference
            ,string identifier
            )
        {
            InitializeResults();
            _conference = new PredecessorObj<Conference>(this, GetRoleConference(), conference);
            _identifier = identifier;
        }

        // Hydration constructor
        private Attendee(FactMemento memento)
        {
            InitializeResults();
            _conference = new PredecessorObj<Conference>(this, GetRoleConference(), memento, Conference.GetUnloadedInstance, Conference.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Conference Conference
        {
            get { return IsNull ? Conference.GetNullInstance() : _conference.Fact; }
        }

        // Field access
        public string Identifier
        {
            get { return _identifier; }
        }

        // Query result access

        // Mutable property access

    }
    
    public partial class IndividualAttendee : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				IndividualAttendee newFact = new IndividualAttendee(memento);


				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				IndividualAttendee fact = (IndividualAttendee)obj;
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return IndividualAttendee.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return IndividualAttendee.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.IndividualAttendee", -387550108);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static IndividualAttendee GetUnloadedInstance()
        {
            return new IndividualAttendee((FactMemento)null) { IsLoaded = false };
        }

        public static IndividualAttendee GetNullInstance()
        {
            return new IndividualAttendee((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<IndividualAttendee> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (IndividualAttendee)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleIndividual;
        public static Role GetRoleIndividual()
        {
            if (_cacheRoleIndividual == null)
            {
                _cacheRoleIndividual = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "individual",
			        Individual._correspondenceFactType,
			        true));
            }
            return _cacheRoleIndividual;
        }
        private static Role _cacheRoleAttendee;
        public static Role GetRoleAttendee()
        {
            if (_cacheRoleAttendee == null)
            {
                _cacheRoleAttendee = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "attendee",
			        Attendee._correspondenceFactType,
			        false));
            }
            return _cacheRoleAttendee;
        }

        // Queries

        // Predicates

        // Predecessors
        private PredecessorObj<Individual> _individual;
        private PredecessorObj<Attendee> _attendee;

        // Fields

        // Results

        // Business constructor
        public IndividualAttendee(
            Individual individual
            ,Attendee attendee
            )
        {
            InitializeResults();
            _individual = new PredecessorObj<Individual>(this, GetRoleIndividual(), individual);
            _attendee = new PredecessorObj<Attendee>(this, GetRoleAttendee(), attendee);
        }

        // Hydration constructor
        private IndividualAttendee(FactMemento memento)
        {
            InitializeResults();
            _individual = new PredecessorObj<Individual>(this, GetRoleIndividual(), memento, Individual.GetUnloadedInstance, Individual.GetNullInstance);
            _attendee = new PredecessorObj<Attendee>(this, GetRoleAttendee(), memento, Attendee.GetUnloadedInstance, Attendee.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Individual Individual
        {
            get { return IsNull ? Individual.GetNullInstance() : _individual.Fact; }
        }
        public Attendee Attendee
        {
            get { return IsNull ? Attendee.GetNullInstance() : _attendee.Fact; }
        }

        // Field access

        // Query result access

        // Mutable property access

    }
    
    public partial class Day : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				Day newFact = new Day(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._conferenceDate = (DateTime)_fieldSerializerByType[typeof(DateTime)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Day fact = (Day)obj;
				_fieldSerializerByType[typeof(DateTime)].WriteData(output, fact._conferenceDate);
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return Day.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return Day.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.Day", 1099669724);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static Day GetUnloadedInstance()
        {
            return new Day((FactMemento)null) { IsLoaded = false };
        }

        public static Day GetNullInstance()
        {
            return new Day((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<Day> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (Day)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleConference;
        public static Role GetRoleConference()
        {
            if (_cacheRoleConference == null)
            {
                _cacheRoleConference = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "conference",
			        Conference._correspondenceFactType,
			        true));
            }
            return _cacheRoleConference;
        }

        // Queries
        private static Query _cacheQueryTimes;

        public static Query GetQueryTimes()
		{
            if (_cacheQueryTimes == null)
            {
			    _cacheQueryTimes = new Query()
    				.JoinSuccessors(Time.GetRoleDay(), Condition.WhereIsEmpty(Time.GetQueryIsDeleted())
				)
                ;
            }
            return _cacheQueryTimes;
		}
        private static Query _cacheQueryHasTimes;

        public static Query GetQueryHasTimes()
		{
            if (_cacheQueryHasTimes == null)
            {
			    _cacheQueryHasTimes = new Query()
    				.JoinSuccessors(Time.GetRoleDay(), Condition.WhereIsEmpty(Time.GetQueryIsDeleted())
				)
                ;
            }
            return _cacheQueryHasTimes;
		}

        // Predicates
        public static Condition HasTimes = Condition.WhereIsNotEmpty(GetQueryHasTimes());

        // Predecessors
        private PredecessorObj<Conference> _conference;

        // Fields
        private DateTime _conferenceDate;

        // Results
        private Result<Time> _times;

        // Business constructor
        public Day(
            Conference conference
            ,DateTime conferenceDate
            )
        {
            InitializeResults();
            _conference = new PredecessorObj<Conference>(this, GetRoleConference(), conference);
            _conferenceDate = conferenceDate;
        }

        // Hydration constructor
        private Day(FactMemento memento)
        {
            InitializeResults();
            _conference = new PredecessorObj<Conference>(this, GetRoleConference(), memento, Conference.GetUnloadedInstance, Conference.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
            _times = new Result<Time>(this, GetQueryTimes(), Time.GetUnloadedInstance, Time.GetNullInstance);
        }

        // Predecessor access
        public Conference Conference
        {
            get { return IsNull ? Conference.GetNullInstance() : _conference.Fact; }
        }

        // Field access
        public DateTime ConferenceDate
        {
            get { return _conferenceDate; }
        }

        // Query result access
        public Result<Time> Times
        {
            get { return _times; }
        }

        // Mutable property access

    }
    
    public partial class Time : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				Time newFact = new Time(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._start = (DateTime)_fieldSerializerByType[typeof(DateTime)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Time fact = (Time)obj;
				_fieldSerializerByType[typeof(DateTime)].WriteData(output, fact._start);
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return Time.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return Time.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.Time", -2087703020);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static Time GetUnloadedInstance()
        {
            return new Time((FactMemento)null) { IsLoaded = false };
        }

        public static Time GetNullInstance()
        {
            return new Time((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<Time> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (Time)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleDay;
        public static Role GetRoleDay()
        {
            if (_cacheRoleDay == null)
            {
                _cacheRoleDay = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "day",
			        Day._correspondenceFactType,
			        false));
            }
            return _cacheRoleDay;
        }

        // Queries
        private static Query _cacheQueryAvailableSessions;

        public static Query GetQueryAvailableSessions()
		{
            if (_cacheQueryAvailableSessions == null)
            {
			    _cacheQueryAvailableSessions = new Query()
		    		.JoinSuccessors(Place.GetRolePlaceTime())
    				.JoinSuccessors(SessionPlace.GetRolePlace(), Condition.WhereIsEmpty(SessionPlace.GetQueryIsCurrent())
	    				.And().IsEmpty(SessionPlace.GetQueryIsDeleted())
				)
                ;
            }
            return _cacheQueryAvailableSessions;
		}
        private static Query _cacheQueryDeletes;

        public static Query GetQueryDeletes()
		{
            if (_cacheQueryDeletes == null)
            {
			    _cacheQueryDeletes = new Query()
    				.JoinSuccessors(TimeDelete.GetRoleDeleted(), Condition.WhereIsEmpty(TimeDelete.GetQueryIsUndeleted())
				)
                ;
            }
            return _cacheQueryDeletes;
		}
        private static Query _cacheQueryIsDeleted;

        public static Query GetQueryIsDeleted()
		{
            if (_cacheQueryIsDeleted == null)
            {
			    _cacheQueryIsDeleted = new Query()
    				.JoinSuccessors(TimeDelete.GetRoleDeleted(), Condition.WhereIsEmpty(TimeDelete.GetQueryIsUndeleted())
				)
                ;
            }
            return _cacheQueryIsDeleted;
		}

        // Predicates
        public static Condition IsDeleted = Condition.WhereIsNotEmpty(GetQueryIsDeleted());

        // Predecessors
        private PredecessorObj<Day> _day;

        // Fields
        private DateTime _start;

        // Results
        private Result<SessionPlace> _availableSessions;
        private Result<TimeDelete> _deletes;

        // Business constructor
        public Time(
            Day day
            ,DateTime start
            )
        {
            InitializeResults();
            _day = new PredecessorObj<Day>(this, GetRoleDay(), day);
            _start = start;
        }

        // Hydration constructor
        private Time(FactMemento memento)
        {
            InitializeResults();
            _day = new PredecessorObj<Day>(this, GetRoleDay(), memento, Day.GetUnloadedInstance, Day.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
            _availableSessions = new Result<SessionPlace>(this, GetQueryAvailableSessions(), SessionPlace.GetUnloadedInstance, SessionPlace.GetNullInstance);
            _deletes = new Result<TimeDelete>(this, GetQueryDeletes(), TimeDelete.GetUnloadedInstance, TimeDelete.GetNullInstance);
        }

        // Predecessor access
        public Day Day
        {
            get { return IsNull ? Day.GetNullInstance() : _day.Fact; }
        }

        // Field access
        public DateTime Start
        {
            get { return _start; }
        }

        // Query result access
        public Result<SessionPlace> AvailableSessions
        {
            get { return _availableSessions; }
        }
        public Result<TimeDelete> Deletes
        {
            get { return _deletes; }
        }

        // Mutable property access

    }
    
    public partial class TimeDelete : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				TimeDelete newFact = new TimeDelete(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._unique = (Guid)_fieldSerializerByType[typeof(Guid)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				TimeDelete fact = (TimeDelete)obj;
				_fieldSerializerByType[typeof(Guid)].WriteData(output, fact._unique);
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return TimeDelete.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return TimeDelete.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.TimeDelete", 227497178);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static TimeDelete GetUnloadedInstance()
        {
            return new TimeDelete((FactMemento)null) { IsLoaded = false };
        }

        public static TimeDelete GetNullInstance()
        {
            return new TimeDelete((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<TimeDelete> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (TimeDelete)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleDeleted;
        public static Role GetRoleDeleted()
        {
            if (_cacheRoleDeleted == null)
            {
                _cacheRoleDeleted = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "deleted",
			        Time._correspondenceFactType,
			        false));
            }
            return _cacheRoleDeleted;
        }

        // Queries
        private static Query _cacheQueryIsUndeleted;

        public static Query GetQueryIsUndeleted()
		{
            if (_cacheQueryIsUndeleted == null)
            {
			    _cacheQueryIsUndeleted = new Query()
		    		.JoinSuccessors(TimeUndelete.GetRoleUndeleted())
                ;
            }
            return _cacheQueryIsUndeleted;
		}

        // Predicates
        public static Condition IsUndeleted = Condition.WhereIsNotEmpty(GetQueryIsUndeleted());

        // Predecessors
        private PredecessorObj<Time> _deleted;

        // Unique
        private Guid _unique;

        // Fields

        // Results

        // Business constructor
        public TimeDelete(
            Time deleted
            )
        {
            _unique = Guid.NewGuid();
            InitializeResults();
            _deleted = new PredecessorObj<Time>(this, GetRoleDeleted(), deleted);
        }

        // Hydration constructor
        private TimeDelete(FactMemento memento)
        {
            InitializeResults();
            _deleted = new PredecessorObj<Time>(this, GetRoleDeleted(), memento, Time.GetUnloadedInstance, Time.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Time Deleted
        {
            get { return IsNull ? Time.GetNullInstance() : _deleted.Fact; }
        }

        // Field access
		public Guid Unique { get { return _unique; } }


        // Query result access

        // Mutable property access

    }
    
    public partial class TimeUndelete : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				TimeUndelete newFact = new TimeUndelete(memento);


				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				TimeUndelete fact = (TimeUndelete)obj;
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return TimeUndelete.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return TimeUndelete.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.TimeUndelete", 1763225232);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static TimeUndelete GetUnloadedInstance()
        {
            return new TimeUndelete((FactMemento)null) { IsLoaded = false };
        }

        public static TimeUndelete GetNullInstance()
        {
            return new TimeUndelete((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<TimeUndelete> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (TimeUndelete)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleUndeleted;
        public static Role GetRoleUndeleted()
        {
            if (_cacheRoleUndeleted == null)
            {
                _cacheRoleUndeleted = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "undeleted",
			        TimeDelete._correspondenceFactType,
			        false));
            }
            return _cacheRoleUndeleted;
        }

        // Queries

        // Predicates

        // Predecessors
        private PredecessorObj<TimeDelete> _undeleted;

        // Fields

        // Results

        // Business constructor
        public TimeUndelete(
            TimeDelete undeleted
            )
        {
            InitializeResults();
            _undeleted = new PredecessorObj<TimeDelete>(this, GetRoleUndeleted(), undeleted);
        }

        // Hydration constructor
        private TimeUndelete(FactMemento memento)
        {
            InitializeResults();
            _undeleted = new PredecessorObj<TimeDelete>(this, GetRoleUndeleted(), memento, TimeDelete.GetUnloadedInstance, TimeDelete.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public TimeDelete Undeleted
        {
            get { return IsNull ? TimeDelete.GetNullInstance() : _undeleted.Fact; }
        }

        // Field access

        // Query result access

        // Mutable property access

    }
    
    public partial class Room : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				Room newFact = new Room(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._unique = (Guid)_fieldSerializerByType[typeof(Guid)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Room fact = (Room)obj;
				_fieldSerializerByType[typeof(Guid)].WriteData(output, fact._unique);
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return Room.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return Room.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.Room", -1711482154);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static Room GetUnloadedInstance()
        {
            return new Room((FactMemento)null) { IsLoaded = false };
        }

        public static Room GetNullInstance()
        {
            return new Room((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<Room> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (Room)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleConference;
        public static Role GetRoleConference()
        {
            if (_cacheRoleConference == null)
            {
                _cacheRoleConference = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "conference",
			        Conference._correspondenceFactType,
			        true));
            }
            return _cacheRoleConference;
        }

        // Queries
        private static Query _cacheQueryRoomNumber;

        public static Query GetQueryRoomNumber()
		{
            if (_cacheQueryRoomNumber == null)
            {
			    _cacheQueryRoomNumber = new Query()
    				.JoinSuccessors(Room__roomNumber.GetRoleRoom(), Condition.WhereIsEmpty(Room__roomNumber.GetQueryIsCurrent())
				)
                ;
            }
            return _cacheQueryRoomNumber;
		}

        // Predicates

        // Predecessors
        private PredecessorObj<Conference> _conference;

        // Unique
        private Guid _unique;

        // Fields

        // Results
        private Result<Room__roomNumber> _roomNumber;

        // Business constructor
        public Room(
            Conference conference
            )
        {
            _unique = Guid.NewGuid();
            InitializeResults();
            _conference = new PredecessorObj<Conference>(this, GetRoleConference(), conference);
        }

        // Hydration constructor
        private Room(FactMemento memento)
        {
            InitializeResults();
            _conference = new PredecessorObj<Conference>(this, GetRoleConference(), memento, Conference.GetUnloadedInstance, Conference.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
            _roomNumber = new Result<Room__roomNumber>(this, GetQueryRoomNumber(), Room__roomNumber.GetUnloadedInstance, Room__roomNumber.GetNullInstance);
        }

        // Predecessor access
        public Conference Conference
        {
            get { return IsNull ? Conference.GetNullInstance() : _conference.Fact; }
        }

        // Field access
		public Guid Unique { get { return _unique; } }


        // Query result access

        // Mutable property access
        public TransientDisputable<Room__roomNumber, string> RoomNumber
        {
            get { return _roomNumber.AsTransientDisputable(fact => fact.Value); }
			set
			{
                Community.Perform(async delegate()
                {
                    var current = (await _roomNumber.EnsureAsync()).ToList();
                    if (current.Count != 1 || !object.Equals(current[0].Value, value.Value))
                    {
                        await Community.AddFactAsync(new Room__roomNumber(this, _roomNumber, value.Value));
                    }
                });
			}
        }

    }
    
    public partial class Room__roomNumber : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				Room__roomNumber newFact = new Room__roomNumber(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._value = (string)_fieldSerializerByType[typeof(string)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Room__roomNumber fact = (Room__roomNumber)obj;
				_fieldSerializerByType[typeof(string)].WriteData(output, fact._value);
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return Room__roomNumber.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return Room__roomNumber.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.Room__roomNumber", -968433032);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static Room__roomNumber GetUnloadedInstance()
        {
            return new Room__roomNumber((FactMemento)null) { IsLoaded = false };
        }

        public static Room__roomNumber GetNullInstance()
        {
            return new Room__roomNumber((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<Room__roomNumber> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (Room__roomNumber)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleRoom;
        public static Role GetRoleRoom()
        {
            if (_cacheRoleRoom == null)
            {
                _cacheRoleRoom = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "room",
			        Room._correspondenceFactType,
			        false));
            }
            return _cacheRoleRoom;
        }
        private static Role _cacheRolePrior;
        public static Role GetRolePrior()
        {
            if (_cacheRolePrior == null)
            {
                _cacheRolePrior = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "prior",
			        Room__roomNumber._correspondenceFactType,
			        false));
            }
            return _cacheRolePrior;
        }

        // Queries
        private static Query _cacheQueryIsCurrent;

        public static Query GetQueryIsCurrent()
		{
            if (_cacheQueryIsCurrent == null)
            {
			    _cacheQueryIsCurrent = new Query()
		    		.JoinSuccessors(Room__roomNumber.GetRolePrior())
                ;
            }
            return _cacheQueryIsCurrent;
		}

        // Predicates
        public static Condition IsCurrent = Condition.WhereIsEmpty(GetQueryIsCurrent());

        // Predecessors
        private PredecessorObj<Room> _room;
        private PredecessorList<Room__roomNumber> _prior;

        // Fields
        private string _value;

        // Results

        // Business constructor
        public Room__roomNumber(
            Room room
            ,IEnumerable<Room__roomNumber> prior
            ,string value
            )
        {
            InitializeResults();
            _room = new PredecessorObj<Room>(this, GetRoleRoom(), room);
            _prior = new PredecessorList<Room__roomNumber>(this, GetRolePrior(), prior);
            _value = value;
        }

        // Hydration constructor
        private Room__roomNumber(FactMemento memento)
        {
            InitializeResults();
            _room = new PredecessorObj<Room>(this, GetRoleRoom(), memento, Room.GetUnloadedInstance, Room.GetNullInstance);
            _prior = new PredecessorList<Room__roomNumber>(this, GetRolePrior(), memento, Room__roomNumber.GetUnloadedInstance, Room__roomNumber.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Room Room
        {
            get { return IsNull ? Room.GetNullInstance() : _room.Fact; }
        }
        public PredecessorList<Room__roomNumber> Prior
        {
            get { return _prior; }
        }

        // Field access
        public string Value
        {
            get { return _value; }
        }

        // Query result access

        // Mutable property access

    }
    
    public partial class Track : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				Track newFact = new Track(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._name = (string)_fieldSerializerByType[typeof(string)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Track fact = (Track)obj;
				_fieldSerializerByType[typeof(string)].WriteData(output, fact._name);
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return Track.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return Track.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.Track", 1099669676);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static Track GetUnloadedInstance()
        {
            return new Track((FactMemento)null) { IsLoaded = false };
        }

        public static Track GetNullInstance()
        {
            return new Track((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<Track> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (Track)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleConference;
        public static Role GetRoleConference()
        {
            if (_cacheRoleConference == null)
            {
                _cacheRoleConference = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "conference",
			        Conference._correspondenceFactType,
			        true));
            }
            return _cacheRoleConference;
        }

        // Queries
        private static Query _cacheQueryCurrentSessionPlaces;

        public static Query GetQueryCurrentSessionPlaces()
		{
            if (_cacheQueryCurrentSessionPlaces == null)
            {
			    _cacheQueryCurrentSessionPlaces = new Query()
    				.JoinSuccessors(Session.GetRoleTrack(), Condition.WhereIsEmpty(Session.GetQueryIsDeleted())
				)
    				.JoinSuccessors(SessionPlace.GetRoleSession(), Condition.WhereIsEmpty(SessionPlace.GetQueryIsCurrent())
	    				.And().IsEmpty(SessionPlace.GetQueryTimeIsDeleted())
				)
                ;
            }
            return _cacheQueryCurrentSessionPlaces;
		}
        private static Query _cacheQueryHasSessions;

        public static Query GetQueryHasSessions()
		{
            if (_cacheQueryHasSessions == null)
            {
			    _cacheQueryHasSessions = new Query()
    				.JoinSuccessors(Session.GetRoleTrack(), Condition.WhereIsEmpty(Session.GetQueryIsDeleted())
				)
                ;
            }
            return _cacheQueryHasSessions;
		}

        // Predicates
        public static Condition HasSessions = Condition.WhereIsNotEmpty(GetQueryHasSessions());

        // Predecessors
        private PredecessorObj<Conference> _conference;

        // Fields
        private string _name;

        // Results
        private Result<SessionPlace> _currentSessionPlaces;

        // Business constructor
        public Track(
            Conference conference
            ,string name
            )
        {
            InitializeResults();
            _conference = new PredecessorObj<Conference>(this, GetRoleConference(), conference);
            _name = name;
        }

        // Hydration constructor
        private Track(FactMemento memento)
        {
            InitializeResults();
            _conference = new PredecessorObj<Conference>(this, GetRoleConference(), memento, Conference.GetUnloadedInstance, Conference.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
            _currentSessionPlaces = new Result<SessionPlace>(this, GetQueryCurrentSessionPlaces(), SessionPlace.GetUnloadedInstance, SessionPlace.GetNullInstance);
        }

        // Predecessor access
        public Conference Conference
        {
            get { return IsNull ? Conference.GetNullInstance() : _conference.Fact; }
        }

        // Field access
        public string Name
        {
            get { return _name; }
        }

        // Query result access
        public Result<SessionPlace> CurrentSessionPlaces
        {
            get { return _currentSessionPlaces; }
        }

        // Mutable property access

    }
    
    public partial class Speaker : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				Speaker newFact = new Speaker(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._unique = (Guid)_fieldSerializerByType[typeof(Guid)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Speaker fact = (Speaker)obj;
				_fieldSerializerByType[typeof(Guid)].WriteData(output, fact._unique);
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return Speaker.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return Speaker.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.Speaker", -1711482154);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static Speaker GetUnloadedInstance()
        {
            return new Speaker((FactMemento)null) { IsLoaded = false };
        }

        public static Speaker GetNullInstance()
        {
            return new Speaker((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<Speaker> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (Speaker)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleConference;
        public static Role GetRoleConference()
        {
            if (_cacheRoleConference == null)
            {
                _cacheRoleConference = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "conference",
			        Conference._correspondenceFactType,
			        true));
            }
            return _cacheRoleConference;
        }

        // Queries
        private static Query _cacheQueryName;

        public static Query GetQueryName()
		{
            if (_cacheQueryName == null)
            {
			    _cacheQueryName = new Query()
    				.JoinSuccessors(Speaker__name.GetRoleSpeaker(), Condition.WhereIsEmpty(Speaker__name.GetQueryIsCurrent())
				)
                ;
            }
            return _cacheQueryName;
		}
        private static Query _cacheQueryImageUrl;

        public static Query GetQueryImageUrl()
		{
            if (_cacheQueryImageUrl == null)
            {
			    _cacheQueryImageUrl = new Query()
    				.JoinSuccessors(Speaker__imageUrl.GetRoleSpeaker(), Condition.WhereIsEmpty(Speaker__imageUrl.GetQueryIsCurrent())
				)
                ;
            }
            return _cacheQueryImageUrl;
		}
        private static Query _cacheQueryContact;

        public static Query GetQueryContact()
		{
            if (_cacheQueryContact == null)
            {
			    _cacheQueryContact = new Query()
    				.JoinSuccessors(Speaker__contact.GetRoleSpeaker(), Condition.WhereIsEmpty(Speaker__contact.GetQueryIsCurrent())
				)
                ;
            }
            return _cacheQueryContact;
		}
        private static Query _cacheQueryBio;

        public static Query GetQueryBio()
		{
            if (_cacheQueryBio == null)
            {
			    _cacheQueryBio = new Query()
    				.JoinSuccessors(Speaker__bio.GetRoleSpeaker(), Condition.WhereIsEmpty(Speaker__bio.GetQueryIsCurrent())
				)
                ;
            }
            return _cacheQueryBio;
		}
        private static Query _cacheQuerySessions;

        public static Query GetQuerySessions()
		{
            if (_cacheQuerySessions == null)
            {
			    _cacheQuerySessions = new Query()
    				.JoinSuccessors(Session.GetRoleSpeaker(), Condition.WhereIsEmpty(Session.GetQueryIsDeleted())
				)
                ;
            }
            return _cacheQuerySessions;
		}
        private static Query _cacheQueryAvailableSessions;

        public static Query GetQueryAvailableSessions()
		{
            if (_cacheQueryAvailableSessions == null)
            {
			    _cacheQueryAvailableSessions = new Query()
    				.JoinSuccessors(Session.GetRoleSpeaker(), Condition.WhereIsEmpty(Session.GetQueryIsDeleted())
				)
    				.JoinSuccessors(SessionPlace.GetRoleSession(), Condition.WhereIsEmpty(SessionPlace.GetQueryIsCurrent())
	    				.And().IsEmpty(SessionPlace.GetQueryTimeIsDeleted())
				)
                ;
            }
            return _cacheQueryAvailableSessions;
		}

        // Predicates

        // Predecessors
        private PredecessorObj<Conference> _conference;

        // Unique
        private Guid _unique;

        // Fields

        // Results
        private Result<Speaker__name> _name;
        private Result<Speaker__imageUrl> _imageUrl;
        private Result<Speaker__contact> _contact;
        private Result<Speaker__bio> _bio;
        private Result<Session> _sessions;
        private Result<SessionPlace> _availableSessions;

        // Business constructor
        public Speaker(
            Conference conference
            )
        {
            _unique = Guid.NewGuid();
            InitializeResults();
            _conference = new PredecessorObj<Conference>(this, GetRoleConference(), conference);
        }

        // Hydration constructor
        private Speaker(FactMemento memento)
        {
            InitializeResults();
            _conference = new PredecessorObj<Conference>(this, GetRoleConference(), memento, Conference.GetUnloadedInstance, Conference.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
            _name = new Result<Speaker__name>(this, GetQueryName(), Speaker__name.GetUnloadedInstance, Speaker__name.GetNullInstance);
            _imageUrl = new Result<Speaker__imageUrl>(this, GetQueryImageUrl(), Speaker__imageUrl.GetUnloadedInstance, Speaker__imageUrl.GetNullInstance);
            _contact = new Result<Speaker__contact>(this, GetQueryContact(), Speaker__contact.GetUnloadedInstance, Speaker__contact.GetNullInstance);
            _bio = new Result<Speaker__bio>(this, GetQueryBio(), Speaker__bio.GetUnloadedInstance, Speaker__bio.GetNullInstance);
            _sessions = new Result<Session>(this, GetQuerySessions(), Session.GetUnloadedInstance, Session.GetNullInstance);
            _availableSessions = new Result<SessionPlace>(this, GetQueryAvailableSessions(), SessionPlace.GetUnloadedInstance, SessionPlace.GetNullInstance);
        }

        // Predecessor access
        public Conference Conference
        {
            get { return IsNull ? Conference.GetNullInstance() : _conference.Fact; }
        }

        // Field access
		public Guid Unique { get { return _unique; } }


        // Query result access
        public Result<Session> Sessions
        {
            get { return _sessions; }
        }
        public Result<SessionPlace> AvailableSessions
        {
            get { return _availableSessions; }
        }

        // Mutable property access
        public TransientDisputable<Speaker__name, string> Name
        {
            get { return _name.AsTransientDisputable(fact => fact.Value); }
			set
			{
                Community.Perform(async delegate()
                {
                    var current = (await _name.EnsureAsync()).ToList();
                    if (current.Count != 1 || !object.Equals(current[0].Value, value.Value))
                    {
                        await Community.AddFactAsync(new Speaker__name(this, _name, value.Value));
                    }
                });
			}
        }
        public TransientDisputable<Speaker__imageUrl, string> ImageUrl
        {
            get { return _imageUrl.AsTransientDisputable(fact => fact.Value); }
			set
			{
                Community.Perform(async delegate()
                {
                    var current = (await _imageUrl.EnsureAsync()).ToList();
                    if (current.Count != 1 || !object.Equals(current[0].Value, value.Value))
                    {
                        await Community.AddFactAsync(new Speaker__imageUrl(this, _imageUrl, value.Value));
                    }
                });
			}
        }
        public TransientDisputable<Speaker__contact, string> Contact
        {
            get { return _contact.AsTransientDisputable(fact => fact.Value); }
			set
			{
                Community.Perform(async delegate()
                {
                    var current = (await _contact.EnsureAsync()).ToList();
                    if (current.Count != 1 || !object.Equals(current[0].Value, value.Value))
                    {
                        await Community.AddFactAsync(new Speaker__contact(this, _contact, value.Value));
                    }
                });
			}
        }

        public TransientDisputable<Speaker__bio, IEnumerable<DocumentSegment>> Bio
        {
            get { return _bio.AsTransientDisputable(fact => (IEnumerable<DocumentSegment>)fact.Value); }
			set
			{
				Community.Perform(async delegate()
				{
					var current = (await _bio.EnsureAsync()).ToList();
					if (current.Count != 1 || !object.Equals(current[0].Value, value.Value))
					{
						await Community.AddFactAsync(new Speaker__bio(this, _bio, value.Value));
					}
				});
			}
        }
    }
    
    public partial class Speaker__name : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				Speaker__name newFact = new Speaker__name(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._value = (string)_fieldSerializerByType[typeof(string)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Speaker__name fact = (Speaker__name)obj;
				_fieldSerializerByType[typeof(string)].WriteData(output, fact._value);
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return Speaker__name.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return Speaker__name.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.Speaker__name", 443104904);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static Speaker__name GetUnloadedInstance()
        {
            return new Speaker__name((FactMemento)null) { IsLoaded = false };
        }

        public static Speaker__name GetNullInstance()
        {
            return new Speaker__name((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<Speaker__name> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (Speaker__name)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleSpeaker;
        public static Role GetRoleSpeaker()
        {
            if (_cacheRoleSpeaker == null)
            {
                _cacheRoleSpeaker = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "speaker",
			        Speaker._correspondenceFactType,
			        false));
            }
            return _cacheRoleSpeaker;
        }
        private static Role _cacheRolePrior;
        public static Role GetRolePrior()
        {
            if (_cacheRolePrior == null)
            {
                _cacheRolePrior = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "prior",
			        Speaker__name._correspondenceFactType,
			        false));
            }
            return _cacheRolePrior;
        }

        // Queries
        private static Query _cacheQueryIsCurrent;

        public static Query GetQueryIsCurrent()
		{
            if (_cacheQueryIsCurrent == null)
            {
			    _cacheQueryIsCurrent = new Query()
		    		.JoinSuccessors(Speaker__name.GetRolePrior())
                ;
            }
            return _cacheQueryIsCurrent;
		}

        // Predicates
        public static Condition IsCurrent = Condition.WhereIsEmpty(GetQueryIsCurrent());

        // Predecessors
        private PredecessorObj<Speaker> _speaker;
        private PredecessorList<Speaker__name> _prior;

        // Fields
        private string _value;

        // Results

        // Business constructor
        public Speaker__name(
            Speaker speaker
            ,IEnumerable<Speaker__name> prior
            ,string value
            )
        {
            InitializeResults();
            _speaker = new PredecessorObj<Speaker>(this, GetRoleSpeaker(), speaker);
            _prior = new PredecessorList<Speaker__name>(this, GetRolePrior(), prior);
            _value = value;
        }

        // Hydration constructor
        private Speaker__name(FactMemento memento)
        {
            InitializeResults();
            _speaker = new PredecessorObj<Speaker>(this, GetRoleSpeaker(), memento, Speaker.GetUnloadedInstance, Speaker.GetNullInstance);
            _prior = new PredecessorList<Speaker__name>(this, GetRolePrior(), memento, Speaker__name.GetUnloadedInstance, Speaker__name.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Speaker Speaker
        {
            get { return IsNull ? Speaker.GetNullInstance() : _speaker.Fact; }
        }
        public PredecessorList<Speaker__name> Prior
        {
            get { return _prior; }
        }

        // Field access
        public string Value
        {
            get { return _value; }
        }

        // Query result access

        // Mutable property access

    }
    
    public partial class Speaker__imageUrl : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				Speaker__imageUrl newFact = new Speaker__imageUrl(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._value = (string)_fieldSerializerByType[typeof(string)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Speaker__imageUrl fact = (Speaker__imageUrl)obj;
				_fieldSerializerByType[typeof(string)].WriteData(output, fact._value);
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return Speaker__imageUrl.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return Speaker__imageUrl.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.Speaker__imageUrl", 443104904);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static Speaker__imageUrl GetUnloadedInstance()
        {
            return new Speaker__imageUrl((FactMemento)null) { IsLoaded = false };
        }

        public static Speaker__imageUrl GetNullInstance()
        {
            return new Speaker__imageUrl((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<Speaker__imageUrl> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (Speaker__imageUrl)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleSpeaker;
        public static Role GetRoleSpeaker()
        {
            if (_cacheRoleSpeaker == null)
            {
                _cacheRoleSpeaker = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "speaker",
			        Speaker._correspondenceFactType,
			        false));
            }
            return _cacheRoleSpeaker;
        }
        private static Role _cacheRolePrior;
        public static Role GetRolePrior()
        {
            if (_cacheRolePrior == null)
            {
                _cacheRolePrior = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "prior",
			        Speaker__imageUrl._correspondenceFactType,
			        false));
            }
            return _cacheRolePrior;
        }

        // Queries
        private static Query _cacheQueryIsCurrent;

        public static Query GetQueryIsCurrent()
		{
            if (_cacheQueryIsCurrent == null)
            {
			    _cacheQueryIsCurrent = new Query()
		    		.JoinSuccessors(Speaker__imageUrl.GetRolePrior())
                ;
            }
            return _cacheQueryIsCurrent;
		}

        // Predicates
        public static Condition IsCurrent = Condition.WhereIsEmpty(GetQueryIsCurrent());

        // Predecessors
        private PredecessorObj<Speaker> _speaker;
        private PredecessorList<Speaker__imageUrl> _prior;

        // Fields
        private string _value;

        // Results

        // Business constructor
        public Speaker__imageUrl(
            Speaker speaker
            ,IEnumerable<Speaker__imageUrl> prior
            ,string value
            )
        {
            InitializeResults();
            _speaker = new PredecessorObj<Speaker>(this, GetRoleSpeaker(), speaker);
            _prior = new PredecessorList<Speaker__imageUrl>(this, GetRolePrior(), prior);
            _value = value;
        }

        // Hydration constructor
        private Speaker__imageUrl(FactMemento memento)
        {
            InitializeResults();
            _speaker = new PredecessorObj<Speaker>(this, GetRoleSpeaker(), memento, Speaker.GetUnloadedInstance, Speaker.GetNullInstance);
            _prior = new PredecessorList<Speaker__imageUrl>(this, GetRolePrior(), memento, Speaker__imageUrl.GetUnloadedInstance, Speaker__imageUrl.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Speaker Speaker
        {
            get { return IsNull ? Speaker.GetNullInstance() : _speaker.Fact; }
        }
        public PredecessorList<Speaker__imageUrl> Prior
        {
            get { return _prior; }
        }

        // Field access
        public string Value
        {
            get { return _value; }
        }

        // Query result access

        // Mutable property access

    }
    
    public partial class Speaker__contact : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				Speaker__contact newFact = new Speaker__contact(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._value = (string)_fieldSerializerByType[typeof(string)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Speaker__contact fact = (Speaker__contact)obj;
				_fieldSerializerByType[typeof(string)].WriteData(output, fact._value);
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return Speaker__contact.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return Speaker__contact.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.Speaker__contact", 443104904);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static Speaker__contact GetUnloadedInstance()
        {
            return new Speaker__contact((FactMemento)null) { IsLoaded = false };
        }

        public static Speaker__contact GetNullInstance()
        {
            return new Speaker__contact((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<Speaker__contact> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (Speaker__contact)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleSpeaker;
        public static Role GetRoleSpeaker()
        {
            if (_cacheRoleSpeaker == null)
            {
                _cacheRoleSpeaker = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "speaker",
			        Speaker._correspondenceFactType,
			        false));
            }
            return _cacheRoleSpeaker;
        }
        private static Role _cacheRolePrior;
        public static Role GetRolePrior()
        {
            if (_cacheRolePrior == null)
            {
                _cacheRolePrior = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "prior",
			        Speaker__contact._correspondenceFactType,
			        false));
            }
            return _cacheRolePrior;
        }

        // Queries
        private static Query _cacheQueryIsCurrent;

        public static Query GetQueryIsCurrent()
		{
            if (_cacheQueryIsCurrent == null)
            {
			    _cacheQueryIsCurrent = new Query()
		    		.JoinSuccessors(Speaker__contact.GetRolePrior())
                ;
            }
            return _cacheQueryIsCurrent;
		}

        // Predicates
        public static Condition IsCurrent = Condition.WhereIsEmpty(GetQueryIsCurrent());

        // Predecessors
        private PredecessorObj<Speaker> _speaker;
        private PredecessorList<Speaker__contact> _prior;

        // Fields
        private string _value;

        // Results

        // Business constructor
        public Speaker__contact(
            Speaker speaker
            ,IEnumerable<Speaker__contact> prior
            ,string value
            )
        {
            InitializeResults();
            _speaker = new PredecessorObj<Speaker>(this, GetRoleSpeaker(), speaker);
            _prior = new PredecessorList<Speaker__contact>(this, GetRolePrior(), prior);
            _value = value;
        }

        // Hydration constructor
        private Speaker__contact(FactMemento memento)
        {
            InitializeResults();
            _speaker = new PredecessorObj<Speaker>(this, GetRoleSpeaker(), memento, Speaker.GetUnloadedInstance, Speaker.GetNullInstance);
            _prior = new PredecessorList<Speaker__contact>(this, GetRolePrior(), memento, Speaker__contact.GetUnloadedInstance, Speaker__contact.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Speaker Speaker
        {
            get { return IsNull ? Speaker.GetNullInstance() : _speaker.Fact; }
        }
        public PredecessorList<Speaker__contact> Prior
        {
            get { return _prior; }
        }

        // Field access
        public string Value
        {
            get { return _value; }
        }

        // Query result access

        // Mutable property access

    }
    
    public partial class Speaker__bio : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				Speaker__bio newFact = new Speaker__bio(memento);


				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Speaker__bio fact = (Speaker__bio)obj;
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return Speaker__bio.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return Speaker__bio.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.Speaker__bio", -577141912);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static Speaker__bio GetUnloadedInstance()
        {
            return new Speaker__bio((FactMemento)null) { IsLoaded = false };
        }

        public static Speaker__bio GetNullInstance()
        {
            return new Speaker__bio((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<Speaker__bio> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (Speaker__bio)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleSpeaker;
        public static Role GetRoleSpeaker()
        {
            if (_cacheRoleSpeaker == null)
            {
                _cacheRoleSpeaker = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "speaker",
			        Speaker._correspondenceFactType,
			        false));
            }
            return _cacheRoleSpeaker;
        }
        private static Role _cacheRolePrior;
        public static Role GetRolePrior()
        {
            if (_cacheRolePrior == null)
            {
                _cacheRolePrior = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "prior",
			        Speaker__bio._correspondenceFactType,
			        false));
            }
            return _cacheRolePrior;
        }
        private static Role _cacheRoleValue;
        public static Role GetRoleValue()
        {
            if (_cacheRoleValue == null)
            {
                _cacheRoleValue = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "value",
			        DocumentSegment._correspondenceFactType,
			        false));
            }
            return _cacheRoleValue;
        }

        // Queries
        private static Query _cacheQueryIsCurrent;

        public static Query GetQueryIsCurrent()
		{
            if (_cacheQueryIsCurrent == null)
            {
			    _cacheQueryIsCurrent = new Query()
		    		.JoinSuccessors(Speaker__bio.GetRolePrior())
                ;
            }
            return _cacheQueryIsCurrent;
		}

        // Predicates
        public static Condition IsCurrent = Condition.WhereIsEmpty(GetQueryIsCurrent());

        // Predecessors
        private PredecessorObj<Speaker> _speaker;
        private PredecessorList<Speaker__bio> _prior;
        private PredecessorList<DocumentSegment> _value;

        // Fields

        // Results

        // Business constructor
        public Speaker__bio(
            Speaker speaker
            ,IEnumerable<Speaker__bio> prior
            ,IEnumerable<DocumentSegment> value
            )
        {
            InitializeResults();
            _speaker = new PredecessorObj<Speaker>(this, GetRoleSpeaker(), speaker);
            _prior = new PredecessorList<Speaker__bio>(this, GetRolePrior(), prior);
            _value = new PredecessorList<DocumentSegment>(this, GetRoleValue(), value);
        }

        // Hydration constructor
        private Speaker__bio(FactMemento memento)
        {
            InitializeResults();
            _speaker = new PredecessorObj<Speaker>(this, GetRoleSpeaker(), memento, Speaker.GetUnloadedInstance, Speaker.GetNullInstance);
            _prior = new PredecessorList<Speaker__bio>(this, GetRolePrior(), memento, Speaker__bio.GetUnloadedInstance, Speaker__bio.GetNullInstance);
            _value = new PredecessorList<DocumentSegment>(this, GetRoleValue(), memento, DocumentSegment.GetUnloadedInstance, DocumentSegment.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Speaker Speaker
        {
            get { return IsNull ? Speaker.GetNullInstance() : _speaker.Fact; }
        }
        public PredecessorList<Speaker__bio> Prior
        {
            get { return _prior; }
        }
        public PredecessorList<DocumentSegment> Value
        {
            get { return _value; }
        }

        // Field access

        // Query result access

        // Mutable property access

    }
    
    public partial class ConferenceNotice : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				ConferenceNotice newFact = new ConferenceNotice(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._timeSent = (DateTime)_fieldSerializerByType[typeof(DateTime)].ReadData(output);
						newFact._text = (string)_fieldSerializerByType[typeof(string)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				ConferenceNotice fact = (ConferenceNotice)obj;
				_fieldSerializerByType[typeof(DateTime)].WriteData(output, fact._timeSent);
				_fieldSerializerByType[typeof(string)].WriteData(output, fact._text);
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return ConferenceNotice.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return ConferenceNotice.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.ConferenceNotice", 2033074576);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static ConferenceNotice GetUnloadedInstance()
        {
            return new ConferenceNotice((FactMemento)null) { IsLoaded = false };
        }

        public static ConferenceNotice GetNullInstance()
        {
            return new ConferenceNotice((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<ConferenceNotice> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (ConferenceNotice)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleConference;
        public static Role GetRoleConference()
        {
            if (_cacheRoleConference == null)
            {
                _cacheRoleConference = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "conference",
			        Conference._correspondenceFactType,
			        true));
            }
            return _cacheRoleConference;
        }

        // Queries

        // Predicates

        // Predecessors
        private PredecessorObj<Conference> _conference;

        // Fields
        private DateTime _timeSent;
        private string _text;

        // Results

        // Business constructor
        public ConferenceNotice(
            Conference conference
            ,DateTime timeSent
            ,string text
            )
        {
            InitializeResults();
            _conference = new PredecessorObj<Conference>(this, GetRoleConference(), conference);
            _timeSent = timeSent;
            _text = text;
        }

        // Hydration constructor
        private ConferenceNotice(FactMemento memento)
        {
            InitializeResults();
            _conference = new PredecessorObj<Conference>(this, GetRoleConference(), memento, Conference.GetUnloadedInstance, Conference.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Conference Conference
        {
            get { return IsNull ? Conference.GetNullInstance() : _conference.Fact; }
        }

        // Field access
        public DateTime TimeSent
        {
            get { return _timeSent; }
        }
        public string Text
        {
            get { return _text; }
        }

        // Query result access

        // Mutable property access

    }
    
    public partial class Place : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				Place newFact = new Place(memento);


				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Place fact = (Place)obj;
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return Place.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return Place.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.Place", -1871516312);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static Place GetUnloadedInstance()
        {
            return new Place((FactMemento)null) { IsLoaded = false };
        }

        public static Place GetNullInstance()
        {
            return new Place((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<Place> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (Place)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRolePlaceTime;
        public static Role GetRolePlaceTime()
        {
            if (_cacheRolePlaceTime == null)
            {
                _cacheRolePlaceTime = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "placeTime",
			        Time._correspondenceFactType,
			        false));
            }
            return _cacheRolePlaceTime;
        }
        private static Role _cacheRoleRoom;
        public static Role GetRoleRoom()
        {
            if (_cacheRoleRoom == null)
            {
                _cacheRoleRoom = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "room",
			        Room._correspondenceFactType,
			        false));
            }
            return _cacheRoleRoom;
        }

        // Queries
        private static Query _cacheQueryCurrentSessionPlaces;

        public static Query GetQueryCurrentSessionPlaces()
		{
            if (_cacheQueryCurrentSessionPlaces == null)
            {
			    _cacheQueryCurrentSessionPlaces = new Query()
    				.JoinSuccessors(SessionPlace.GetRolePlace(), Condition.WhereIsEmpty(SessionPlace.GetQueryIsCurrent())
	    				.And().IsEmpty(SessionPlace.GetQueryIsDeleted())
				)
                ;
            }
            return _cacheQueryCurrentSessionPlaces;
		}

        // Predicates

        // Predecessors
        private PredecessorObj<Time> _placeTime;
        private PredecessorObj<Room> _room;

        // Fields

        // Results
        private Result<SessionPlace> _currentSessionPlaces;

        // Business constructor
        public Place(
            Time placeTime
            ,Room room
            )
        {
            InitializeResults();
            _placeTime = new PredecessorObj<Time>(this, GetRolePlaceTime(), placeTime);
            _room = new PredecessorObj<Room>(this, GetRoleRoom(), room);
        }

        // Hydration constructor
        private Place(FactMemento memento)
        {
            InitializeResults();
            _placeTime = new PredecessorObj<Time>(this, GetRolePlaceTime(), memento, Time.GetUnloadedInstance, Time.GetNullInstance);
            _room = new PredecessorObj<Room>(this, GetRoleRoom(), memento, Room.GetUnloadedInstance, Room.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
            _currentSessionPlaces = new Result<SessionPlace>(this, GetQueryCurrentSessionPlaces(), SessionPlace.GetUnloadedInstance, SessionPlace.GetNullInstance);
        }

        // Predecessor access
        public Time PlaceTime
        {
            get { return IsNull ? Time.GetNullInstance() : _placeTime.Fact; }
        }
        public Room Room
        {
            get { return IsNull ? Room.GetNullInstance() : _room.Fact; }
        }

        // Field access

        // Query result access
        public Result<SessionPlace> CurrentSessionPlaces
        {
            get { return _currentSessionPlaces; }
        }

        // Mutable property access

    }
    
    public partial class Level : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				Level newFact = new Level(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._name = (string)_fieldSerializerByType[typeof(string)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Level fact = (Level)obj;
				_fieldSerializerByType[typeof(string)].WriteData(output, fact._name);
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return Level.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return Level.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.Level", 8);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static Level GetUnloadedInstance()
        {
            return new Level((FactMemento)null) { IsLoaded = false };
        }

        public static Level GetNullInstance()
        {
            return new Level((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<Level> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (Level)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles

        // Queries

        // Predicates

        // Predecessors

        // Fields
        private string _name;

        // Results

        // Business constructor
        public Level(
            string name
            )
        {
            InitializeResults();
            _name = name;
        }

        // Hydration constructor
        private Level(FactMemento memento)
        {
            InitializeResults();
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access

        // Field access
        public string Name
        {
            get { return _name; }
        }

        // Query result access

        // Mutable property access

    }
    
    public partial class Session : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				Session newFact = new Session(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._unique = (Guid)_fieldSerializerByType[typeof(Guid)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Session fact = (Session)obj;
				_fieldSerializerByType[typeof(Guid)].WriteData(output, fact._unique);
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return Session.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return Session.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.Session", 383681038);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static Session GetUnloadedInstance()
        {
            return new Session((FactMemento)null) { IsLoaded = false };
        }

        public static Session GetNullInstance()
        {
            return new Session((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<Session> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (Session)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleConference;
        public static Role GetRoleConference()
        {
            if (_cacheRoleConference == null)
            {
                _cacheRoleConference = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "conference",
			        Conference._correspondenceFactType,
			        true));
            }
            return _cacheRoleConference;
        }
        private static Role _cacheRoleSpeaker;
        public static Role GetRoleSpeaker()
        {
            if (_cacheRoleSpeaker == null)
            {
                _cacheRoleSpeaker = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "speaker",
			        Speaker._correspondenceFactType,
			        false));
            }
            return _cacheRoleSpeaker;
        }
        private static Role _cacheRoleTrack;
        public static Role GetRoleTrack()
        {
            if (_cacheRoleTrack == null)
            {
                _cacheRoleTrack = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "track",
			        Track._correspondenceFactType,
			        false));
            }
            return _cacheRoleTrack;
        }

        // Queries
        private static Query _cacheQueryName;

        public static Query GetQueryName()
		{
            if (_cacheQueryName == null)
            {
			    _cacheQueryName = new Query()
    				.JoinSuccessors(Session__name.GetRoleSession(), Condition.WhereIsEmpty(Session__name.GetQueryIsCurrent())
				)
                ;
            }
            return _cacheQueryName;
		}
        private static Query _cacheQueryDescription;

        public static Query GetQueryDescription()
		{
            if (_cacheQueryDescription == null)
            {
			    _cacheQueryDescription = new Query()
    				.JoinSuccessors(Session__description.GetRoleSession(), Condition.WhereIsEmpty(Session__description.GetQueryIsCurrent())
				)
                ;
            }
            return _cacheQueryDescription;
		}
        private static Query _cacheQueryLevel;

        public static Query GetQueryLevel()
		{
            if (_cacheQueryLevel == null)
            {
			    _cacheQueryLevel = new Query()
    				.JoinSuccessors(Session__level.GetRoleSession(), Condition.WhereIsEmpty(Session__level.GetQueryIsCurrent())
				)
                ;
            }
            return _cacheQueryLevel;
		}
        private static Query _cacheQueryDtfSessionId;

        public static Query GetQueryDtfSessionId()
		{
            if (_cacheQueryDtfSessionId == null)
            {
			    _cacheQueryDtfSessionId = new Query()
    				.JoinSuccessors(Session__dtfSessionId.GetRoleSession(), Condition.WhereIsEmpty(Session__dtfSessionId.GetQueryIsCurrent())
				)
                ;
            }
            return _cacheQueryDtfSessionId;
		}
        private static Query _cacheQueryCurrentSessionPlaces;

        public static Query GetQueryCurrentSessionPlaces()
		{
            if (_cacheQueryCurrentSessionPlaces == null)
            {
			    _cacheQueryCurrentSessionPlaces = new Query()
    				.JoinSuccessors(SessionPlace.GetRoleSession(), Condition.WhereIsEmpty(SessionPlace.GetQueryIsCurrent())
				)
                ;
            }
            return _cacheQueryCurrentSessionPlaces;
		}
        private static Query _cacheQueryNotices;

        public static Query GetQueryNotices()
		{
            if (_cacheQueryNotices == null)
            {
			    _cacheQueryNotices = new Query()
		    		.JoinSuccessors(SessionNotice.GetRoleSession())
                ;
            }
            return _cacheQueryNotices;
		}
        private static Query _cacheQueryIsDeleted;

        public static Query GetQueryIsDeleted()
		{
            if (_cacheQueryIsDeleted == null)
            {
			    _cacheQueryIsDeleted = new Query()
    				.JoinSuccessors(SessionDelete.GetRoleDeleted(), Condition.WhereIsEmpty(SessionDelete.GetQueryIsUndeleted())
				)
                ;
            }
            return _cacheQueryIsDeleted;
		}
        private static Query _cacheQuerySessionDeletes;

        public static Query GetQuerySessionDeletes()
		{
            if (_cacheQuerySessionDeletes == null)
            {
			    _cacheQuerySessionDeletes = new Query()
    				.JoinSuccessors(SessionDelete.GetRoleDeleted(), Condition.WhereIsEmpty(SessionDelete.GetQueryIsUndeleted())
				)
                ;
            }
            return _cacheQuerySessionDeletes;
		}
        private static Query _cacheQueryIsScheduled;

        public static Query GetQueryIsScheduled()
		{
            if (_cacheQueryIsScheduled == null)
            {
			    _cacheQueryIsScheduled = new Query()
		    		.JoinSuccessors(SessionPlace.GetRoleSession())
                ;
            }
            return _cacheQueryIsScheduled;
		}

        // Predicates
        public static Condition IsDeleted = Condition.WhereIsNotEmpty(GetQueryIsDeleted());
        public static Condition IsScheduled = Condition.WhereIsNotEmpty(GetQueryIsScheduled());

        // Predecessors
        private PredecessorObj<Conference> _conference;
        private PredecessorObj<Speaker> _speaker;
        private PredecessorOpt<Track> _track;

        // Unique
        private Guid _unique;

        // Fields

        // Results
        private Result<Session__name> _name;
        private Result<Session__description> _description;
        private Result<Session__level> _level;
        private Result<Session__dtfSessionId> _dtfSessionId;
        private Result<SessionPlace> _currentSessionPlaces;
        private Result<SessionNotice> _notices;
        private Result<SessionDelete> _sessionDeletes;

        // Business constructor
        public Session(
            Conference conference
            ,Speaker speaker
            ,Track track
            )
        {
            _unique = Guid.NewGuid();
            InitializeResults();
            _conference = new PredecessorObj<Conference>(this, GetRoleConference(), conference);
            _speaker = new PredecessorObj<Speaker>(this, GetRoleSpeaker(), speaker);
            _track = new PredecessorOpt<Track>(this, GetRoleTrack(), track
                ?? Track.GetNullInstance());
        }

        // Hydration constructor
        private Session(FactMemento memento)
        {
            InitializeResults();
            _conference = new PredecessorObj<Conference>(this, GetRoleConference(), memento, Conference.GetUnloadedInstance, Conference.GetNullInstance);
            _speaker = new PredecessorObj<Speaker>(this, GetRoleSpeaker(), memento, Speaker.GetUnloadedInstance, Speaker.GetNullInstance);
            _track = new PredecessorOpt<Track>(this, GetRoleTrack(), memento, Track.GetUnloadedInstance, Track.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
            _name = new Result<Session__name>(this, GetQueryName(), Session__name.GetUnloadedInstance, Session__name.GetNullInstance);
            _description = new Result<Session__description>(this, GetQueryDescription(), Session__description.GetUnloadedInstance, Session__description.GetNullInstance);
            _level = new Result<Session__level>(this, GetQueryLevel(), Session__level.GetUnloadedInstance, Session__level.GetNullInstance);
            _dtfSessionId = new Result<Session__dtfSessionId>(this, GetQueryDtfSessionId(), Session__dtfSessionId.GetUnloadedInstance, Session__dtfSessionId.GetNullInstance);
            _currentSessionPlaces = new Result<SessionPlace>(this, GetQueryCurrentSessionPlaces(), SessionPlace.GetUnloadedInstance, SessionPlace.GetNullInstance);
            _notices = new Result<SessionNotice>(this, GetQueryNotices(), SessionNotice.GetUnloadedInstance, SessionNotice.GetNullInstance);
            _sessionDeletes = new Result<SessionDelete>(this, GetQuerySessionDeletes(), SessionDelete.GetUnloadedInstance, SessionDelete.GetNullInstance);
        }

        // Predecessor access
        public Conference Conference
        {
            get { return IsNull ? Conference.GetNullInstance() : _conference.Fact; }
        }
        public Speaker Speaker
        {
            get { return IsNull ? Speaker.GetNullInstance() : _speaker.Fact; }
        }
        public Track Track
        {
            get { return IsNull ? Track.GetNullInstance() : _track.Fact; }
        }

        // Field access
		public Guid Unique { get { return _unique; } }


        // Query result access
        public Result<SessionPlace> CurrentSessionPlaces
        {
            get { return _currentSessionPlaces; }
        }
        public Result<SessionNotice> Notices
        {
            get { return _notices; }
        }
        public Result<SessionDelete> SessionDeletes
        {
            get { return _sessionDeletes; }
        }

        // Mutable property access
        public TransientDisputable<Session__name, string> Name
        {
            get { return _name.AsTransientDisputable(fact => fact.Value); }
			set
			{
                Community.Perform(async delegate()
                {
                    var current = (await _name.EnsureAsync()).ToList();
                    if (current.Count != 1 || !object.Equals(current[0].Value, value.Value))
                    {
                        await Community.AddFactAsync(new Session__name(this, _name, value.Value));
                    }
                });
			}
        }
        public TransientDisputable<Session__dtfSessionId, string> DtfSessionId
        {
            get { return _dtfSessionId.AsTransientDisputable(fact => fact.Value); }
			set
			{
                Community.Perform(async delegate()
                {
                    var current = (await _dtfSessionId.EnsureAsync()).ToList();
                    if (current.Count != 1 || !object.Equals(current[0].Value, value.Value))
                    {
                        await Community.AddFactAsync(new Session__dtfSessionId(this, _dtfSessionId, value.Value));
                    }
                });
			}
        }

        public TransientDisputable<Session__description, IEnumerable<DocumentSegment>> Description
        {
            get { return _description.AsTransientDisputable(fact => (IEnumerable<DocumentSegment>)fact.Value); }
			set
			{
				Community.Perform(async delegate()
				{
					var current = (await _description.EnsureAsync()).ToList();
					if (current.Count != 1 || !object.Equals(current[0].Value, value.Value))
					{
						await Community.AddFactAsync(new Session__description(this, _description, value.Value));
					}
				});
			}
        }
        public TransientDisputable<Session__level, Level> Level
        {
            get { return _level.AsTransientDisputable(fact => (Level)fact.Value); }
			set
			{
				Community.Perform(async delegate()
				{
					var current = (await _level.EnsureAsync()).ToList();
					if (current.Count != 1 || !object.Equals(current[0].Value, value.Value))
					{
						await Community.AddFactAsync(new Session__level(this, _level, value.Value));
					}
				});
			}
        }
    }
    
    public partial class Session__name : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				Session__name newFact = new Session__name(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._value = (string)_fieldSerializerByType[typeof(string)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Session__name fact = (Session__name)obj;
				_fieldSerializerByType[typeof(string)].WriteData(output, fact._value);
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return Session__name.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return Session__name.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.Session__name", -1186408944);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static Session__name GetUnloadedInstance()
        {
            return new Session__name((FactMemento)null) { IsLoaded = false };
        }

        public static Session__name GetNullInstance()
        {
            return new Session__name((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<Session__name> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (Session__name)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleSession;
        public static Role GetRoleSession()
        {
            if (_cacheRoleSession == null)
            {
                _cacheRoleSession = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "session",
			        Session._correspondenceFactType,
			        false));
            }
            return _cacheRoleSession;
        }
        private static Role _cacheRolePrior;
        public static Role GetRolePrior()
        {
            if (_cacheRolePrior == null)
            {
                _cacheRolePrior = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "prior",
			        Session__name._correspondenceFactType,
			        false));
            }
            return _cacheRolePrior;
        }

        // Queries
        private static Query _cacheQueryIsCurrent;

        public static Query GetQueryIsCurrent()
		{
            if (_cacheQueryIsCurrent == null)
            {
			    _cacheQueryIsCurrent = new Query()
		    		.JoinSuccessors(Session__name.GetRolePrior())
                ;
            }
            return _cacheQueryIsCurrent;
		}

        // Predicates
        public static Condition IsCurrent = Condition.WhereIsEmpty(GetQueryIsCurrent());

        // Predecessors
        private PredecessorObj<Session> _session;
        private PredecessorList<Session__name> _prior;

        // Fields
        private string _value;

        // Results

        // Business constructor
        public Session__name(
            Session session
            ,IEnumerable<Session__name> prior
            ,string value
            )
        {
            InitializeResults();
            _session = new PredecessorObj<Session>(this, GetRoleSession(), session);
            _prior = new PredecessorList<Session__name>(this, GetRolePrior(), prior);
            _value = value;
        }

        // Hydration constructor
        private Session__name(FactMemento memento)
        {
            InitializeResults();
            _session = new PredecessorObj<Session>(this, GetRoleSession(), memento, Session.GetUnloadedInstance, Session.GetNullInstance);
            _prior = new PredecessorList<Session__name>(this, GetRolePrior(), memento, Session__name.GetUnloadedInstance, Session__name.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Session Session
        {
            get { return IsNull ? Session.GetNullInstance() : _session.Fact; }
        }
        public PredecessorList<Session__name> Prior
        {
            get { return _prior; }
        }

        // Field access
        public string Value
        {
            get { return _value; }
        }

        // Query result access

        // Mutable property access

    }
    
    public partial class Session__description : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				Session__description newFact = new Session__description(memento);


				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Session__description fact = (Session__description)obj;
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return Session__description.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return Session__description.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.Session__description", 2088311536);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static Session__description GetUnloadedInstance()
        {
            return new Session__description((FactMemento)null) { IsLoaded = false };
        }

        public static Session__description GetNullInstance()
        {
            return new Session__description((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<Session__description> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (Session__description)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleSession;
        public static Role GetRoleSession()
        {
            if (_cacheRoleSession == null)
            {
                _cacheRoleSession = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "session",
			        Session._correspondenceFactType,
			        false));
            }
            return _cacheRoleSession;
        }
        private static Role _cacheRolePrior;
        public static Role GetRolePrior()
        {
            if (_cacheRolePrior == null)
            {
                _cacheRolePrior = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "prior",
			        Session__description._correspondenceFactType,
			        false));
            }
            return _cacheRolePrior;
        }
        private static Role _cacheRoleValue;
        public static Role GetRoleValue()
        {
            if (_cacheRoleValue == null)
            {
                _cacheRoleValue = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "value",
			        DocumentSegment._correspondenceFactType,
			        false));
            }
            return _cacheRoleValue;
        }

        // Queries
        private static Query _cacheQueryIsCurrent;

        public static Query GetQueryIsCurrent()
		{
            if (_cacheQueryIsCurrent == null)
            {
			    _cacheQueryIsCurrent = new Query()
		    		.JoinSuccessors(Session__description.GetRolePrior())
                ;
            }
            return _cacheQueryIsCurrent;
		}

        // Predicates
        public static Condition IsCurrent = Condition.WhereIsEmpty(GetQueryIsCurrent());

        // Predecessors
        private PredecessorObj<Session> _session;
        private PredecessorList<Session__description> _prior;
        private PredecessorList<DocumentSegment> _value;

        // Fields

        // Results

        // Business constructor
        public Session__description(
            Session session
            ,IEnumerable<Session__description> prior
            ,IEnumerable<DocumentSegment> value
            )
        {
            InitializeResults();
            _session = new PredecessorObj<Session>(this, GetRoleSession(), session);
            _prior = new PredecessorList<Session__description>(this, GetRolePrior(), prior);
            _value = new PredecessorList<DocumentSegment>(this, GetRoleValue(), value);
        }

        // Hydration constructor
        private Session__description(FactMemento memento)
        {
            InitializeResults();
            _session = new PredecessorObj<Session>(this, GetRoleSession(), memento, Session.GetUnloadedInstance, Session.GetNullInstance);
            _prior = new PredecessorList<Session__description>(this, GetRolePrior(), memento, Session__description.GetUnloadedInstance, Session__description.GetNullInstance);
            _value = new PredecessorList<DocumentSegment>(this, GetRoleValue(), memento, DocumentSegment.GetUnloadedInstance, DocumentSegment.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Session Session
        {
            get { return IsNull ? Session.GetNullInstance() : _session.Fact; }
        }
        public PredecessorList<Session__description> Prior
        {
            get { return _prior; }
        }
        public PredecessorList<DocumentSegment> Value
        {
            get { return _value; }
        }

        // Field access

        // Query result access

        // Mutable property access

    }
    
    public partial class Session__level : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				Session__level newFact = new Session__level(memento);


				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Session__level fact = (Session__level)obj;
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return Session__level.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return Session__level.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.Session__level", 2088311544);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static Session__level GetUnloadedInstance()
        {
            return new Session__level((FactMemento)null) { IsLoaded = false };
        }

        public static Session__level GetNullInstance()
        {
            return new Session__level((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<Session__level> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (Session__level)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleSession;
        public static Role GetRoleSession()
        {
            if (_cacheRoleSession == null)
            {
                _cacheRoleSession = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "session",
			        Session._correspondenceFactType,
			        false));
            }
            return _cacheRoleSession;
        }
        private static Role _cacheRolePrior;
        public static Role GetRolePrior()
        {
            if (_cacheRolePrior == null)
            {
                _cacheRolePrior = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "prior",
			        Session__level._correspondenceFactType,
			        false));
            }
            return _cacheRolePrior;
        }
        private static Role _cacheRoleValue;
        public static Role GetRoleValue()
        {
            if (_cacheRoleValue == null)
            {
                _cacheRoleValue = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "value",
			        Level._correspondenceFactType,
			        false));
            }
            return _cacheRoleValue;
        }

        // Queries
        private static Query _cacheQueryIsCurrent;

        public static Query GetQueryIsCurrent()
		{
            if (_cacheQueryIsCurrent == null)
            {
			    _cacheQueryIsCurrent = new Query()
		    		.JoinSuccessors(Session__level.GetRolePrior())
                ;
            }
            return _cacheQueryIsCurrent;
		}

        // Predicates
        public static Condition IsCurrent = Condition.WhereIsEmpty(GetQueryIsCurrent());

        // Predecessors
        private PredecessorObj<Session> _session;
        private PredecessorList<Session__level> _prior;
        private PredecessorObj<Level> _value;

        // Fields

        // Results

        // Business constructor
        public Session__level(
            Session session
            ,IEnumerable<Session__level> prior
            ,Level value
            )
        {
            InitializeResults();
            _session = new PredecessorObj<Session>(this, GetRoleSession(), session);
            _prior = new PredecessorList<Session__level>(this, GetRolePrior(), prior);
            _value = new PredecessorObj<Level>(this, GetRoleValue(), value);
        }

        // Hydration constructor
        private Session__level(FactMemento memento)
        {
            InitializeResults();
            _session = new PredecessorObj<Session>(this, GetRoleSession(), memento, Session.GetUnloadedInstance, Session.GetNullInstance);
            _prior = new PredecessorList<Session__level>(this, GetRolePrior(), memento, Session__level.GetUnloadedInstance, Session__level.GetNullInstance);
            _value = new PredecessorObj<Level>(this, GetRoleValue(), memento, Level.GetUnloadedInstance, Level.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Session Session
        {
            get { return IsNull ? Session.GetNullInstance() : _session.Fact; }
        }
        public PredecessorList<Session__level> Prior
        {
            get { return _prior; }
        }
        public Level Value
        {
            get { return IsNull ? Level.GetNullInstance() : _value.Fact; }
        }

        // Field access

        // Query result access

        // Mutable property access

    }
    
    public partial class Session__dtfSessionId : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				Session__dtfSessionId newFact = new Session__dtfSessionId(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._value = (string)_fieldSerializerByType[typeof(string)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Session__dtfSessionId fact = (Session__dtfSessionId)obj;
				_fieldSerializerByType[typeof(string)].WriteData(output, fact._value);
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return Session__dtfSessionId.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return Session__dtfSessionId.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.Session__dtfSessionId", -1186408944);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static Session__dtfSessionId GetUnloadedInstance()
        {
            return new Session__dtfSessionId((FactMemento)null) { IsLoaded = false };
        }

        public static Session__dtfSessionId GetNullInstance()
        {
            return new Session__dtfSessionId((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<Session__dtfSessionId> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (Session__dtfSessionId)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleSession;
        public static Role GetRoleSession()
        {
            if (_cacheRoleSession == null)
            {
                _cacheRoleSession = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "session",
			        Session._correspondenceFactType,
			        false));
            }
            return _cacheRoleSession;
        }
        private static Role _cacheRolePrior;
        public static Role GetRolePrior()
        {
            if (_cacheRolePrior == null)
            {
                _cacheRolePrior = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "prior",
			        Session__dtfSessionId._correspondenceFactType,
			        false));
            }
            return _cacheRolePrior;
        }

        // Queries
        private static Query _cacheQueryIsCurrent;

        public static Query GetQueryIsCurrent()
		{
            if (_cacheQueryIsCurrent == null)
            {
			    _cacheQueryIsCurrent = new Query()
		    		.JoinSuccessors(Session__dtfSessionId.GetRolePrior())
                ;
            }
            return _cacheQueryIsCurrent;
		}

        // Predicates
        public static Condition IsCurrent = Condition.WhereIsEmpty(GetQueryIsCurrent());

        // Predecessors
        private PredecessorObj<Session> _session;
        private PredecessorList<Session__dtfSessionId> _prior;

        // Fields
        private string _value;

        // Results

        // Business constructor
        public Session__dtfSessionId(
            Session session
            ,IEnumerable<Session__dtfSessionId> prior
            ,string value
            )
        {
            InitializeResults();
            _session = new PredecessorObj<Session>(this, GetRoleSession(), session);
            _prior = new PredecessorList<Session__dtfSessionId>(this, GetRolePrior(), prior);
            _value = value;
        }

        // Hydration constructor
        private Session__dtfSessionId(FactMemento memento)
        {
            InitializeResults();
            _session = new PredecessorObj<Session>(this, GetRoleSession(), memento, Session.GetUnloadedInstance, Session.GetNullInstance);
            _prior = new PredecessorList<Session__dtfSessionId>(this, GetRolePrior(), memento, Session__dtfSessionId.GetUnloadedInstance, Session__dtfSessionId.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Session Session
        {
            get { return IsNull ? Session.GetNullInstance() : _session.Fact; }
        }
        public PredecessorList<Session__dtfSessionId> Prior
        {
            get { return _prior; }
        }

        // Field access
        public string Value
        {
            get { return _value; }
        }

        // Query result access

        // Mutable property access

    }
    
    public partial class SessionDelete : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				SessionDelete newFact = new SessionDelete(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._unique = (Guid)_fieldSerializerByType[typeof(Guid)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				SessionDelete fact = (SessionDelete)obj;
				_fieldSerializerByType[typeof(Guid)].WriteData(output, fact._unique);
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return SessionDelete.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return SessionDelete.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.SessionDelete", 227497178);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static SessionDelete GetUnloadedInstance()
        {
            return new SessionDelete((FactMemento)null) { IsLoaded = false };
        }

        public static SessionDelete GetNullInstance()
        {
            return new SessionDelete((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<SessionDelete> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (SessionDelete)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleDeleted;
        public static Role GetRoleDeleted()
        {
            if (_cacheRoleDeleted == null)
            {
                _cacheRoleDeleted = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "deleted",
			        Session._correspondenceFactType,
			        false));
            }
            return _cacheRoleDeleted;
        }

        // Queries
        private static Query _cacheQueryIsUndeleted;

        public static Query GetQueryIsUndeleted()
		{
            if (_cacheQueryIsUndeleted == null)
            {
			    _cacheQueryIsUndeleted = new Query()
		    		.JoinSuccessors(SessionUndelete.GetRoleUndeleted())
                ;
            }
            return _cacheQueryIsUndeleted;
		}

        // Predicates
        public static Condition IsUndeleted = Condition.WhereIsNotEmpty(GetQueryIsUndeleted());

        // Predecessors
        private PredecessorObj<Session> _deleted;

        // Unique
        private Guid _unique;

        // Fields

        // Results

        // Business constructor
        public SessionDelete(
            Session deleted
            )
        {
            _unique = Guid.NewGuid();
            InitializeResults();
            _deleted = new PredecessorObj<Session>(this, GetRoleDeleted(), deleted);
        }

        // Hydration constructor
        private SessionDelete(FactMemento memento)
        {
            InitializeResults();
            _deleted = new PredecessorObj<Session>(this, GetRoleDeleted(), memento, Session.GetUnloadedInstance, Session.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Session Deleted
        {
            get { return IsNull ? Session.GetNullInstance() : _deleted.Fact; }
        }

        // Field access
		public Guid Unique { get { return _unique; } }


        // Query result access

        // Mutable property access

    }
    
    public partial class SessionUndelete : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				SessionUndelete newFact = new SessionUndelete(memento);


				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				SessionUndelete fact = (SessionUndelete)obj;
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return SessionUndelete.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return SessionUndelete.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.SessionUndelete", 1763225232);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static SessionUndelete GetUnloadedInstance()
        {
            return new SessionUndelete((FactMemento)null) { IsLoaded = false };
        }

        public static SessionUndelete GetNullInstance()
        {
            return new SessionUndelete((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<SessionUndelete> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (SessionUndelete)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleUndeleted;
        public static Role GetRoleUndeleted()
        {
            if (_cacheRoleUndeleted == null)
            {
                _cacheRoleUndeleted = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "undeleted",
			        SessionDelete._correspondenceFactType,
			        false));
            }
            return _cacheRoleUndeleted;
        }

        // Queries

        // Predicates

        // Predecessors
        private PredecessorObj<SessionDelete> _undeleted;

        // Fields

        // Results

        // Business constructor
        public SessionUndelete(
            SessionDelete undeleted
            )
        {
            InitializeResults();
            _undeleted = new PredecessorObj<SessionDelete>(this, GetRoleUndeleted(), undeleted);
        }

        // Hydration constructor
        private SessionUndelete(FactMemento memento)
        {
            InitializeResults();
            _undeleted = new PredecessorObj<SessionDelete>(this, GetRoleUndeleted(), memento, SessionDelete.GetUnloadedInstance, SessionDelete.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public SessionDelete Undeleted
        {
            get { return IsNull ? SessionDelete.GetNullInstance() : _undeleted.Fact; }
        }

        // Field access

        // Query result access

        // Mutable property access

    }
    
    public partial class SessionNotice : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				SessionNotice newFact = new SessionNotice(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._timeSent = (DateTime)_fieldSerializerByType[typeof(DateTime)].ReadData(output);
						newFact._text = (string)_fieldSerializerByType[typeof(string)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				SessionNotice fact = (SessionNotice)obj;
				_fieldSerializerByType[typeof(DateTime)].WriteData(output, fact._timeSent);
				_fieldSerializerByType[typeof(string)].WriteData(output, fact._text);
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return SessionNotice.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return SessionNotice.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.SessionNotice", 43251120);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static SessionNotice GetUnloadedInstance()
        {
            return new SessionNotice((FactMemento)null) { IsLoaded = false };
        }

        public static SessionNotice GetNullInstance()
        {
            return new SessionNotice((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<SessionNotice> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (SessionNotice)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleSession;
        public static Role GetRoleSession()
        {
            if (_cacheRoleSession == null)
            {
                _cacheRoleSession = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "session",
			        Session._correspondenceFactType,
			        true));
            }
            return _cacheRoleSession;
        }

        // Queries

        // Predicates

        // Predecessors
        private PredecessorObj<Session> _session;

        // Fields
        private DateTime _timeSent;
        private string _text;

        // Results

        // Business constructor
        public SessionNotice(
            Session session
            ,DateTime timeSent
            ,string text
            )
        {
            InitializeResults();
            _session = new PredecessorObj<Session>(this, GetRoleSession(), session);
            _timeSent = timeSent;
            _text = text;
        }

        // Hydration constructor
        private SessionNotice(FactMemento memento)
        {
            InitializeResults();
            _session = new PredecessorObj<Session>(this, GetRoleSession(), memento, Session.GetUnloadedInstance, Session.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Session Session
        {
            get { return IsNull ? Session.GetNullInstance() : _session.Fact; }
        }

        // Field access
        public DateTime TimeSent
        {
            get { return _timeSent; }
        }
        public string Text
        {
            get { return _text; }
        }

        // Query result access

        // Mutable property access

    }
    
    public partial class SessionPlace : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				SessionPlace newFact = new SessionPlace(memento);


				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				SessionPlace fact = (SessionPlace)obj;
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return SessionPlace.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return SessionPlace.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.SessionPlace", 1353992592);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static SessionPlace GetUnloadedInstance()
        {
            return new SessionPlace((FactMemento)null) { IsLoaded = false };
        }

        public static SessionPlace GetNullInstance()
        {
            return new SessionPlace((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<SessionPlace> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (SessionPlace)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleSession;
        public static Role GetRoleSession()
        {
            if (_cacheRoleSession == null)
            {
                _cacheRoleSession = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "session",
			        Session._correspondenceFactType,
			        false));
            }
            return _cacheRoleSession;
        }
        private static Role _cacheRolePlace;
        public static Role GetRolePlace()
        {
            if (_cacheRolePlace == null)
            {
                _cacheRolePlace = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "place",
			        Place._correspondenceFactType,
			        false));
            }
            return _cacheRolePlace;
        }
        private static Role _cacheRolePrior;
        public static Role GetRolePrior()
        {
            if (_cacheRolePrior == null)
            {
                _cacheRolePrior = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "prior",
			        SessionPlace._correspondenceFactType,
			        false));
            }
            return _cacheRolePrior;
        }

        // Queries
        private static Query _cacheQueryIsCurrent;

        public static Query GetQueryIsCurrent()
		{
            if (_cacheQueryIsCurrent == null)
            {
			    _cacheQueryIsCurrent = new Query()
		    		.JoinSuccessors(SessionPlace.GetRolePrior())
                ;
            }
            return _cacheQueryIsCurrent;
		}
        private static Query _cacheQueryIsDeleted;

        public static Query GetQueryIsDeleted()
		{
            if (_cacheQueryIsDeleted == null)
            {
			    _cacheQueryIsDeleted = new Query()
		    		.JoinPredecessors(SessionPlace.GetRoleSession())
    				.JoinSuccessors(SessionDelete.GetRoleDeleted(), Condition.WhereIsEmpty(SessionDelete.GetQueryIsUndeleted())
				)
                ;
            }
            return _cacheQueryIsDeleted;
		}
        private static Query _cacheQueryTimeIsDeleted;

        public static Query GetQueryTimeIsDeleted()
		{
            if (_cacheQueryTimeIsDeleted == null)
            {
			    _cacheQueryTimeIsDeleted = new Query()
		    		.JoinPredecessors(SessionPlace.GetRolePlace())
		    		.JoinPredecessors(Place.GetRolePlaceTime())
		    		.JoinSuccessors(TimeDelete.GetRoleDeleted())
                ;
            }
            return _cacheQueryTimeIsDeleted;
		}

        // Predicates
        public static Condition IsCurrent = Condition.WhereIsEmpty(GetQueryIsCurrent());
        public static Condition IsDeleted = Condition.WhereIsNotEmpty(GetQueryIsDeleted());
        public static Condition TimeIsDeleted = Condition.WhereIsNotEmpty(GetQueryTimeIsDeleted());

        // Predecessors
        private PredecessorObj<Session> _session;
        private PredecessorObj<Place> _place;
        private PredecessorList<SessionPlace> _prior;

        // Fields

        // Results

        // Business constructor
        public SessionPlace(
            Session session
            ,Place place
            ,IEnumerable<SessionPlace> prior
            )
        {
            InitializeResults();
            _session = new PredecessorObj<Session>(this, GetRoleSession(), session);
            _place = new PredecessorObj<Place>(this, GetRolePlace(), place);
            _prior = new PredecessorList<SessionPlace>(this, GetRolePrior(), prior);
        }

        // Hydration constructor
        private SessionPlace(FactMemento memento)
        {
            InitializeResults();
            _session = new PredecessorObj<Session>(this, GetRoleSession(), memento, Session.GetUnloadedInstance, Session.GetNullInstance);
            _place = new PredecessorObj<Place>(this, GetRolePlace(), memento, Place.GetUnloadedInstance, Place.GetNullInstance);
            _prior = new PredecessorList<SessionPlace>(this, GetRolePrior(), memento, SessionPlace.GetUnloadedInstance, SessionPlace.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Session Session
        {
            get { return IsNull ? Session.GetNullInstance() : _session.Fact; }
        }
        public Place Place
        {
            get { return IsNull ? Place.GetNullInstance() : _place.Fact; }
        }
        public PredecessorList<SessionPlace> Prior
        {
            get { return _prior; }
        }

        // Field access

        // Query result access

        // Mutable property access

    }
    
    public partial class DocumentSegment : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				DocumentSegment newFact = new DocumentSegment(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._text = (string)_fieldSerializerByType[typeof(string)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				DocumentSegment fact = (DocumentSegment)obj;
				_fieldSerializerByType[typeof(string)].WriteData(output, fact._text);
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return DocumentSegment.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return DocumentSegment.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.DocumentSegment", 8);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static DocumentSegment GetUnloadedInstance()
        {
            return new DocumentSegment((FactMemento)null) { IsLoaded = false };
        }

        public static DocumentSegment GetNullInstance()
        {
            return new DocumentSegment((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<DocumentSegment> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (DocumentSegment)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles

        // Queries

        // Predicates

        // Predecessors

        // Fields
        private string _text;

        // Results

        // Business constructor
        public DocumentSegment(
            string text
            )
        {
            InitializeResults();
            _text = text;
        }

        // Hydration constructor
        private DocumentSegment(FactMemento memento)
        {
            InitializeResults();
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access

        // Field access
        public string Text
        {
            get { return _text; }
        }

        // Query result access

        // Mutable property access

    }
    
    public partial class LikeSession : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				LikeSession newFact = new LikeSession(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._unique = (Guid)_fieldSerializerByType[typeof(Guid)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				LikeSession fact = (LikeSession)obj;
				_fieldSerializerByType[typeof(Guid)].WriteData(output, fact._unique);
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return LikeSession.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return LikeSession.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.LikeSession", -1764209630);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static LikeSession GetUnloadedInstance()
        {
            return new LikeSession((FactMemento)null) { IsLoaded = false };
        }

        public static LikeSession GetNullInstance()
        {
            return new LikeSession((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<LikeSession> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (LikeSession)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleAttendee;
        public static Role GetRoleAttendee()
        {
            if (_cacheRoleAttendee == null)
            {
                _cacheRoleAttendee = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "attendee",
			        Attendee._correspondenceFactType,
			        true));
            }
            return _cacheRoleAttendee;
        }
        private static Role _cacheRoleSession;
        public static Role GetRoleSession()
        {
            if (_cacheRoleSession == null)
            {
                _cacheRoleSession = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "session",
			        Session._correspondenceFactType,
			        true));
            }
            return _cacheRoleSession;
        }

        // Queries
        private static Query _cacheQueryIsDeleted;

        public static Query GetQueryIsDeleted()
		{
            if (_cacheQueryIsDeleted == null)
            {
			    _cacheQueryIsDeleted = new Query()
		    		.JoinSuccessors(UnlikeSession.GetRoleLikeSession())
                ;
            }
            return _cacheQueryIsDeleted;
		}

        // Predicates
        public static Condition IsDeleted = Condition.WhereIsNotEmpty(GetQueryIsDeleted());

        // Predecessors
        private PredecessorObj<Attendee> _attendee;
        private PredecessorObj<Session> _session;

        // Unique
        private Guid _unique;

        // Fields

        // Results

        // Business constructor
        public LikeSession(
            Attendee attendee
            ,Session session
            )
        {
            _unique = Guid.NewGuid();
            InitializeResults();
            _attendee = new PredecessorObj<Attendee>(this, GetRoleAttendee(), attendee);
            _session = new PredecessorObj<Session>(this, GetRoleSession(), session);
        }

        // Hydration constructor
        private LikeSession(FactMemento memento)
        {
            InitializeResults();
            _attendee = new PredecessorObj<Attendee>(this, GetRoleAttendee(), memento, Attendee.GetUnloadedInstance, Attendee.GetNullInstance);
            _session = new PredecessorObj<Session>(this, GetRoleSession(), memento, Session.GetUnloadedInstance, Session.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Attendee Attendee
        {
            get { return IsNull ? Attendee.GetNullInstance() : _attendee.Fact; }
        }
        public Session Session
        {
            get { return IsNull ? Session.GetNullInstance() : _session.Fact; }
        }

        // Field access
		public Guid Unique { get { return _unique; } }


        // Query result access

        // Mutable property access

    }
    
    public partial class UnlikeSession : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				UnlikeSession newFact = new UnlikeSession(memento);


				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				UnlikeSession fact = (UnlikeSession)obj;
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return UnlikeSession.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return UnlikeSession.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Festify.Model.UnlikeSession", 1103775080);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static UnlikeSession GetUnloadedInstance()
        {
            return new UnlikeSession((FactMemento)null) { IsLoaded = false };
        }

        public static UnlikeSession GetNullInstance()
        {
            return new UnlikeSession((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<UnlikeSession> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (UnlikeSession)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleLikeSession;
        public static Role GetRoleLikeSession()
        {
            if (_cacheRoleLikeSession == null)
            {
                _cacheRoleLikeSession = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "likeSession",
			        LikeSession._correspondenceFactType,
			        false));
            }
            return _cacheRoleLikeSession;
        }

        // Queries

        // Predicates

        // Predecessors
        private PredecessorObj<LikeSession> _likeSession;

        // Fields

        // Results

        // Business constructor
        public UnlikeSession(
            LikeSession likeSession
            )
        {
            InitializeResults();
            _likeSession = new PredecessorObj<LikeSession>(this, GetRoleLikeSession(), likeSession);
        }

        // Hydration constructor
        private UnlikeSession(FactMemento memento)
        {
            InitializeResults();
            _likeSession = new PredecessorObj<LikeSession>(this, GetRoleLikeSession(), memento, LikeSession.GetUnloadedInstance, LikeSession.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public LikeSession LikeSession
        {
            get { return IsNull ? LikeSession.GetNullInstance() : _likeSession.Fact; }
        }

        // Field access

        // Query result access

        // Mutable property access

    }
    

	public class CorrespondenceModel : ICorrespondenceModel
	{
		public void RegisterAllFactTypes(Community community, IDictionary<Type, IFieldSerializer> fieldSerializerByType)
		{
			community.AddType(
				Individual._correspondenceFactType,
				new Individual.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Individual._correspondenceFactType }));
			community.AddQuery(
				Individual._correspondenceFactType,
				Individual.GetQueryAttendees().QueryDefinition);
			community.AddQuery(
				Individual._correspondenceFactType,
				Individual.GetQueryIsToastNotificationEnabled().QueryDefinition);
			community.AddQuery(
				Individual._correspondenceFactType,
				Individual.GetQueryLikedSessions().QueryDefinition);
			community.AddType(
				EnableToastNotification._correspondenceFactType,
				new EnableToastNotification.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { EnableToastNotification._correspondenceFactType }));
			community.AddQuery(
				EnableToastNotification._correspondenceFactType,
				EnableToastNotification.GetQueryIsDisabled().QueryDefinition);
			community.AddType(
				DisableToastNotification._correspondenceFactType,
				new DisableToastNotification.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { DisableToastNotification._correspondenceFactType }));
			community.AddType(
				Conference._correspondenceFactType,
				new Conference.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Conference._correspondenceFactType }));
			community.AddQuery(
				Conference._correspondenceFactType,
				Conference.GetQueryName().QueryDefinition);
			community.AddQuery(
				Conference._correspondenceFactType,
				Conference.GetQueryMapUrl().QueryDefinition);
			community.AddQuery(
				Conference._correspondenceFactType,
				Conference.GetQueryDays().QueryDefinition);
			community.AddQuery(
				Conference._correspondenceFactType,
				Conference.GetQueryAllTracks().QueryDefinition);
			community.AddQuery(
				Conference._correspondenceFactType,
				Conference.GetQueryTracks().QueryDefinition);
			community.AddQuery(
				Conference._correspondenceFactType,
				Conference.GetQuerySessions().QueryDefinition);
			community.AddQuery(
				Conference._correspondenceFactType,
				Conference.GetQueryUnscheduledSessions().QueryDefinition);
			community.AddQuery(
				Conference._correspondenceFactType,
				Conference.GetQuerySpeakers().QueryDefinition);
			community.AddQuery(
				Conference._correspondenceFactType,
				Conference.GetQueryNotices().QueryDefinition);
			community.AddQuery(
				Conference._correspondenceFactType,
				Conference.GetQueryRooms().QueryDefinition);
			community.AddType(
				Conference__name._correspondenceFactType,
				new Conference__name.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Conference__name._correspondenceFactType }));
			community.AddQuery(
				Conference__name._correspondenceFactType,
				Conference__name.GetQueryIsCurrent().QueryDefinition);
			community.AddType(
				Conference__mapUrl._correspondenceFactType,
				new Conference__mapUrl.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Conference__mapUrl._correspondenceFactType }));
			community.AddQuery(
				Conference__mapUrl._correspondenceFactType,
				Conference__mapUrl.GetQueryIsCurrent().QueryDefinition);
			community.AddType(
				Attendee._correspondenceFactType,
				new Attendee.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Attendee._correspondenceFactType }));
			community.AddType(
				IndividualAttendee._correspondenceFactType,
				new IndividualAttendee.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { IndividualAttendee._correspondenceFactType }));
			community.AddType(
				Day._correspondenceFactType,
				new Day.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Day._correspondenceFactType }));
			community.AddQuery(
				Day._correspondenceFactType,
				Day.GetQueryTimes().QueryDefinition);
			community.AddQuery(
				Day._correspondenceFactType,
				Day.GetQueryHasTimes().QueryDefinition);
			community.AddType(
				Time._correspondenceFactType,
				new Time.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Time._correspondenceFactType }));
			community.AddQuery(
				Time._correspondenceFactType,
				Time.GetQueryAvailableSessions().QueryDefinition);
			community.AddQuery(
				Time._correspondenceFactType,
				Time.GetQueryDeletes().QueryDefinition);
			community.AddQuery(
				Time._correspondenceFactType,
				Time.GetQueryIsDeleted().QueryDefinition);
			community.AddType(
				TimeDelete._correspondenceFactType,
				new TimeDelete.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { TimeDelete._correspondenceFactType }));
			community.AddQuery(
				TimeDelete._correspondenceFactType,
				TimeDelete.GetQueryIsUndeleted().QueryDefinition);
			community.AddType(
				TimeUndelete._correspondenceFactType,
				new TimeUndelete.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { TimeUndelete._correspondenceFactType }));
			community.AddType(
				Room._correspondenceFactType,
				new Room.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Room._correspondenceFactType }));
			community.AddQuery(
				Room._correspondenceFactType,
				Room.GetQueryRoomNumber().QueryDefinition);
			community.AddType(
				Room__roomNumber._correspondenceFactType,
				new Room__roomNumber.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Room__roomNumber._correspondenceFactType }));
			community.AddQuery(
				Room__roomNumber._correspondenceFactType,
				Room__roomNumber.GetQueryIsCurrent().QueryDefinition);
			community.AddType(
				Track._correspondenceFactType,
				new Track.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Track._correspondenceFactType }));
			community.AddQuery(
				Track._correspondenceFactType,
				Track.GetQueryCurrentSessionPlaces().QueryDefinition);
			community.AddQuery(
				Track._correspondenceFactType,
				Track.GetQueryHasSessions().QueryDefinition);
			community.AddType(
				Speaker._correspondenceFactType,
				new Speaker.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Speaker._correspondenceFactType }));
			community.AddQuery(
				Speaker._correspondenceFactType,
				Speaker.GetQueryName().QueryDefinition);
			community.AddQuery(
				Speaker._correspondenceFactType,
				Speaker.GetQueryImageUrl().QueryDefinition);
			community.AddQuery(
				Speaker._correspondenceFactType,
				Speaker.GetQueryContact().QueryDefinition);
			community.AddQuery(
				Speaker._correspondenceFactType,
				Speaker.GetQueryBio().QueryDefinition);
			community.AddQuery(
				Speaker._correspondenceFactType,
				Speaker.GetQuerySessions().QueryDefinition);
			community.AddQuery(
				Speaker._correspondenceFactType,
				Speaker.GetQueryAvailableSessions().QueryDefinition);
			community.AddType(
				Speaker__name._correspondenceFactType,
				new Speaker__name.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Speaker__name._correspondenceFactType }));
			community.AddQuery(
				Speaker__name._correspondenceFactType,
				Speaker__name.GetQueryIsCurrent().QueryDefinition);
			community.AddType(
				Speaker__imageUrl._correspondenceFactType,
				new Speaker__imageUrl.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Speaker__imageUrl._correspondenceFactType }));
			community.AddQuery(
				Speaker__imageUrl._correspondenceFactType,
				Speaker__imageUrl.GetQueryIsCurrent().QueryDefinition);
			community.AddType(
				Speaker__contact._correspondenceFactType,
				new Speaker__contact.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Speaker__contact._correspondenceFactType }));
			community.AddQuery(
				Speaker__contact._correspondenceFactType,
				Speaker__contact.GetQueryIsCurrent().QueryDefinition);
			community.AddType(
				Speaker__bio._correspondenceFactType,
				new Speaker__bio.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Speaker__bio._correspondenceFactType }));
			community.AddQuery(
				Speaker__bio._correspondenceFactType,
				Speaker__bio.GetQueryIsCurrent().QueryDefinition);
			community.AddType(
				ConferenceNotice._correspondenceFactType,
				new ConferenceNotice.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { ConferenceNotice._correspondenceFactType }));
			community.AddType(
				Place._correspondenceFactType,
				new Place.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Place._correspondenceFactType }));
			community.AddQuery(
				Place._correspondenceFactType,
				Place.GetQueryCurrentSessionPlaces().QueryDefinition);
			community.AddType(
				Level._correspondenceFactType,
				new Level.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Level._correspondenceFactType }));
			community.AddType(
				Session._correspondenceFactType,
				new Session.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Session._correspondenceFactType }));
			community.AddQuery(
				Session._correspondenceFactType,
				Session.GetQueryName().QueryDefinition);
			community.AddQuery(
				Session._correspondenceFactType,
				Session.GetQueryDescription().QueryDefinition);
			community.AddQuery(
				Session._correspondenceFactType,
				Session.GetQueryLevel().QueryDefinition);
			community.AddQuery(
				Session._correspondenceFactType,
				Session.GetQueryDtfSessionId().QueryDefinition);
			community.AddQuery(
				Session._correspondenceFactType,
				Session.GetQueryCurrentSessionPlaces().QueryDefinition);
			community.AddQuery(
				Session._correspondenceFactType,
				Session.GetQueryNotices().QueryDefinition);
			community.AddQuery(
				Session._correspondenceFactType,
				Session.GetQueryIsDeleted().QueryDefinition);
			community.AddQuery(
				Session._correspondenceFactType,
				Session.GetQuerySessionDeletes().QueryDefinition);
			community.AddQuery(
				Session._correspondenceFactType,
				Session.GetQueryIsScheduled().QueryDefinition);
			community.AddType(
				Session__name._correspondenceFactType,
				new Session__name.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Session__name._correspondenceFactType }));
			community.AddQuery(
				Session__name._correspondenceFactType,
				Session__name.GetQueryIsCurrent().QueryDefinition);
			community.AddType(
				Session__description._correspondenceFactType,
				new Session__description.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Session__description._correspondenceFactType }));
			community.AddQuery(
				Session__description._correspondenceFactType,
				Session__description.GetQueryIsCurrent().QueryDefinition);
			community.AddType(
				Session__level._correspondenceFactType,
				new Session__level.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Session__level._correspondenceFactType }));
			community.AddQuery(
				Session__level._correspondenceFactType,
				Session__level.GetQueryIsCurrent().QueryDefinition);
			community.AddType(
				Session__dtfSessionId._correspondenceFactType,
				new Session__dtfSessionId.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Session__dtfSessionId._correspondenceFactType }));
			community.AddQuery(
				Session__dtfSessionId._correspondenceFactType,
				Session__dtfSessionId.GetQueryIsCurrent().QueryDefinition);
			community.AddType(
				SessionDelete._correspondenceFactType,
				new SessionDelete.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { SessionDelete._correspondenceFactType }));
			community.AddQuery(
				SessionDelete._correspondenceFactType,
				SessionDelete.GetQueryIsUndeleted().QueryDefinition);
			community.AddType(
				SessionUndelete._correspondenceFactType,
				new SessionUndelete.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { SessionUndelete._correspondenceFactType }));
			community.AddType(
				SessionNotice._correspondenceFactType,
				new SessionNotice.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { SessionNotice._correspondenceFactType }));
			community.AddType(
				SessionPlace._correspondenceFactType,
				new SessionPlace.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { SessionPlace._correspondenceFactType }));
			community.AddQuery(
				SessionPlace._correspondenceFactType,
				SessionPlace.GetQueryIsCurrent().QueryDefinition);
			community.AddQuery(
				SessionPlace._correspondenceFactType,
				SessionPlace.GetQueryIsDeleted().QueryDefinition);
			community.AddQuery(
				SessionPlace._correspondenceFactType,
				SessionPlace.GetQueryTimeIsDeleted().QueryDefinition);
			community.AddType(
				DocumentSegment._correspondenceFactType,
				new DocumentSegment.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { DocumentSegment._correspondenceFactType }));
			community.AddType(
				LikeSession._correspondenceFactType,
				new LikeSession.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { LikeSession._correspondenceFactType }));
			community.AddQuery(
				LikeSession._correspondenceFactType,
				LikeSession.GetQueryIsDeleted().QueryDefinition);
			community.AddType(
				UnlikeSession._correspondenceFactType,
				new UnlikeSession.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { UnlikeSession._correspondenceFactType }));
		}
	}
}

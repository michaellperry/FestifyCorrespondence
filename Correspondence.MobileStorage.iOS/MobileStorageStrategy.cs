﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpdateControls.Correspondence.Strategy;
using System.Threading.Tasks;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Queries;
using System.IO;
using System.Diagnostics;
using SQLite;
using UpdateControls.Correspondence;

namespace Correspondence.MobileStorage
{
    public class MobileStorageStrategy : IStorageStrategy
    {
        private readonly string _correspondencePath;

        public MobileStorageStrategy()
        {
            var myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _correspondencePath = Path.Combine(myDocuments, "Correspondence.db");

            //File.Delete(_correspondencePath);
            if (!File.Exists(_correspondencePath))
            {
                using (var db = new SQLiteConnection(_correspondencePath))
                {
                    db.CreateTable<FactTypeRecord>();
                    db.CreateTable<FactRecord>();
                    db.CreateTable<PredecessorRecord>();
                    db.CreateTable<RoleRecord>();
                }
            }
        }

        public bool IsSynchronous
        {
            get { return true; }
        }

        public Task<SaveResult> SaveAsync(FactMemento memento, int peerId)
        {
            FactID id;
            using (var db = new SQLiteConnection(_correspondencePath))
            {
                // First see if the fact is already in storage.
                if (FindExistingFact(memento, out id, db))
                    return Task.FromResult(new SaveResult { WasSaved = false, Id = id });

                // It isn't there, so store it.
                int typeId = SaveType(db, memento.FactType);

                var factRecord = new FactRecord
                {
                    FactTypeID = typeId,
                    Data = memento.Data,
                    HashCode = memento.GetHashCode()
                };
                db.Insert(factRecord);
                id.key = factRecord.ID;

                // Store the predecessors.
                foreach (PredecessorMemento predecessor in memento.Predecessors)
                {
                    int roleId = SaveRole(db, predecessor.Role);
                    var predecessorRecord = new PredecessorRecord
                    {
                        FactID = id.key,
                        RoleID = roleId,
                        PredecessorFactID = predecessor.ID.key,
                        IsPivot = predecessor.IsPivot
                    };
                    db.Insert(predecessorRecord);
                }

                db.Commit();
            }

            return Task.FromResult(new SaveResult { WasSaved = true, Id = id });
        }

        public Task<FactID?> FindExistingFactAsync(FactMemento memento)
        {
            throw new NotImplementedException();
            using (var db = new SQLiteConnection(_correspondencePath))
            {
                int factTypeId = 0;
                var matches = db.Table<FactRecord>().Where(r => r.FactTypeID == factTypeId);
            }
            return Task.FromResult<FactID?>(null);
        }

        public List<IdentifiedFactMemento> GetAllSuccessors(FactID factId)
        {
            throw new NotImplementedException();
        }

        public List<CorrespondenceFactType> GetAllTypes()
        {
            throw new NotImplementedException();
        }

        public Task<Guid> GetClientGuidAsync()
        {
            throw new NotImplementedException();
        }

        public Task<FactID> GetFactIDFromShareAsync(int peerId, FactID remoteFactId)
        {
            throw new NotImplementedException();
        }

        public Task<FactID?> GetIDAsync(string factName)
        {
            throw new NotImplementedException();
        }

        public List<IdentifiedFactMemento> GetPageOfFactsForType(CorrespondenceFactType type, int page)
        {
            throw new NotImplementedException();
        }

        public Task<FactID?> GetRemoteIdAsync(FactID localFactId, int peerId)
        {
            throw new NotImplementedException();
        }

        public Task<FactMemento> LoadAsync(FactID id)
        {
            throw new NotImplementedException();
        }

        public Task<TimestampID> LoadIncomingTimestampAsync(int peerId, FactID pivotId)
        {
            throw new NotImplementedException();
        }

        public Task<TimestampID> LoadOutgoingTimestampAsync(int peerId)
        {
            throw new NotImplementedException();
        }

        public Task<List<MessageMemento>> LoadRecentMessagesForServerAsync(int peerId, TimestampID timestamp)
        {
            throw new NotImplementedException();
        }

        public Task<List<IdentifiedFactMemento>> QueryForFactsAsync(QueryDefinition queryDefinition, FactID startingId, QueryOptions options)
        {
            throw new NotImplementedException();
        }

        public Task<List<FactID>> QueryForIdsAsync(QueryDefinition queryDefinition, FactID startingId)
        {
            throw new NotImplementedException();
        }

        public Task SaveIncomingTimestampAsync(int peerId, FactID pivotId, TimestampID timestamp)
        {
            throw new NotImplementedException();
        }

        public Task SaveOutgoingTimestampAsync(int peerId, TimestampID timestamp)
        {
            throw new NotImplementedException();
        }

        public Task<int> SavePeerAsync(string protocolName, string peerName)
        {
            Debug.WriteLine("SavePeerAsync");
            throw new NotImplementedException();
        }

        public Task SaveShareAsync(int peerId, FactID remoteFactId, FactID localFactId)
        {
            throw new NotImplementedException();
        }

        public Task SetIDAsync(string factName, FactID id)
        {
            throw new NotImplementedException();
        }

        private bool FindExistingFact(FactMemento memento, out FactID id, SQLiteConnection db)
        {
            int typeId = SaveType(db, memento.FactType);

            // Load all candidates that have the same hash code.
            int hashCode = memento.GetHashCode();
            var candidates = db.Table<FactRecord>()
                .Where(f => f.FactTypeID == typeId && f.HashCode == hashCode);
            foreach (var candidate in candidates)
            {
                var existingFact = LoadMemento(candidate, db);
                if (existingFact.Memento.Equals(memento))
                {
                    id = existingFact.Id;
                    return true;
                }
            }

            id = new FactID();
            return false;
        }

        private IdentifiedFactMemento LoadMemento(FactRecord factRecord, SQLiteConnection db)
        {
            var factType = LoadFactType(factRecord.FactTypeID, db);
            var factMemento = new FactMemento(factType)
            {
                Data = factRecord.Data ?? new byte[0]
            };

            var predecessorRecords = db.Table<PredecessorRecord>()
                .Where(p => p.FactID == factRecord.FactTypeID);
            foreach (var predecessorRecord in predecessorRecords)
            {
                factMemento.AddPredecessor(
                    LoadRole(predecessorRecord.RoleID, db),
                    new FactID { key = predecessorRecord.PredecessorFactID },
                    predecessorRecord.IsPivot);
            }

            return new IdentifiedFactMemento(new FactID { key = factRecord.ID }, factMemento);
        }

        private RoleMemento LoadRole(int roleId, SQLiteConnection db)
        {
            var roleRecords = db.Table<RoleRecord>()
                .Where(r => r.ID == roleId);

            foreach (var roleRecord in roleRecords)
            {
                return new RoleMemento(LoadFactType(roleRecord.DeclaringTypeID, db), roleRecord.Name, null, false);
            }

            throw new CorrespondenceException(String.Format("Failed to load role {0}.", roleId));
        }

        private CorrespondenceFactType LoadFactType(int factTypeId, SQLiteConnection db)
        {
            var factTypeRecords = db.Table<FactTypeRecord>()
                .Where(t => t.ID == factTypeId);

            foreach (var factTypeRecord in factTypeRecords)
            {
                return new CorrespondenceFactType(factTypeRecord.Name, factTypeRecord.Version);
            }

            throw new CorrespondenceException(String.Format("Failed to load fact type {0}.", factTypeId));
        }

        private int SaveType(SQLiteConnection db, CorrespondenceFactType factType)
        {
            // See if the type already exists.
            var matches = db.Table<FactTypeRecord>()
                .Where(t => t.Name == factType.TypeName && t.Version == factType.Version);

            foreach (var match in matches)
            {
                return match.ID;
            }

            // If not, create it.
            var record = new FactTypeRecord
            {
                Name = factType.TypeName,
                Version = factType.Version
            };
            db.Insert(record);
            return record.ID;
        }

        private int SaveRole(SQLiteConnection db, RoleMemento role)
        {
            int declaringTypeId = SaveType(db, role.DeclaringType);

            // See if the role already exists.
            var matches = db.Table<RoleRecord>()
                .Where(r => r.Name == role.RoleName && r.DeclaringTypeID == declaringTypeId);

            foreach (var match in matches)
            {
                return match.ID;
            }

            // If not, create it.
            var roleRecord = new RoleRecord
            {
                Name = role.RoleName,
                DeclaringTypeID = declaringTypeId
            };
            db.Insert(roleRecord);
            return roleRecord.ID;
        }
    }
}

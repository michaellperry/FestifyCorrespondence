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
                    db.CreateTable<NamedFactRecord>();
                    db.CreateTable<PeerRecord>();
                    db.CreateTable<ClientGuidRecord>();
                    db.CreateTable<TimestampRecord>();
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
            FactID id;
            using (var db = new SQLiteConnection(_correspondencePath))
            {
                // See if the fact is already in storage.
                if (FindExistingFact(memento, out id, db))
                    return Task.FromResult<FactID?>(id);

                // It isn't there.
                return Task.FromResult<FactID?>(null);
            }
        }

        public Task<FactMemento> LoadAsync(FactID id)
        {
            using (var db = new SQLiteConnection(_correspondencePath))
            {
                return Task.FromResult(LoadMementoById(id.key, db).Memento);
            }
        }

        public Task<FactID?> GetIDAsync(string factName)
        {
            using (var db = new SQLiteConnection(_correspondencePath))
            {
                var namedFactRecords = db.Table<NamedFactRecord>()
                    .Where(n => n.Name == factName);

                foreach (var namedFactRecord in namedFactRecords)
                {
                    return Task.FromResult<FactID?>(new FactID { key = namedFactRecord.FactID });
                }
            }

            return Task.FromResult<FactID?>(null);
        }

        public Task SetIDAsync(string factName, FactID id)
        {
            using (var db = new SQLiteConnection(_correspondencePath))
            {
                db.Insert(new NamedFactRecord
                {
                    Name = factName,
                    FactID = id.key
                });

                db.Commit();
            }

            return Task.FromResult(0);
        }

        public Task<int> SavePeerAsync(string protocolName, string peerName)
        {
            using (var db = new SQLiteConnection(_correspondencePath))
            {
                // See if the peer already exists.
                var matches = db.Table<PeerRecord>()
                    .Where(p => p.ProtocolName == protocolName && p.PeerName == peerName);

                foreach (var match in matches)
                {
                    return Task.FromResult(match.ID);
                }

                // If not, create it.
                var record = new PeerRecord
                {
                    ProtocolName = protocolName,
                    PeerName = peerName
                };
                db.Insert(record);

                db.Commit();

                return Task.FromResult(record.ID);
            }
        }

        public Task<FactID?> GetRemoteIdAsync(FactID localFactId, int peerId)
        {
            return Task.FromResult<FactID?>(null);
        }

        public Task<Guid> GetClientGuidAsync()
        {
            using (var db = new SQLiteConnection(_correspondencePath))
            {
                // See if the record already exists.
                var matches = db.Table<ClientGuidRecord>();

                foreach (var match in matches)
                {
                    return Task.FromResult(match.GUID);
                }

                // If not, create it.
                var record = new ClientGuidRecord
                {
                    GUID = Guid.NewGuid()
                };
                db.Insert(record);

                db.Commit();

                return Task.FromResult(record.GUID);
            }
        }

        public Task<TimestampID> LoadIncomingTimestampAsync(int peerId, FactID pivotId)
        {
            using (var db = new SQLiteConnection(_correspondencePath))
            {
                var matching = db.Table<TimestampRecord>()
                    .Where(t => t.PeerID == peerId && t.PivotID == pivotId.key);
                foreach (var match in matching)
                {
                    return Task.FromResult(new TimestampID(match.DatabaseID, match.FactID));
                }
            }

            return Task.FromResult(new TimestampID());
        }

        public Task SaveIncomingTimestampAsync(int peerId, FactID pivotId, TimestampID timestamp)
        {
            using (var db = new SQLiteConnection(_correspondencePath))
            {
                var matches = db.Table<TimestampRecord>()
                    .Where(t => t.PeerID == peerId && t.PivotID == pivotId.key);
                foreach (var match in matches)
                {
                    match.DatabaseID = timestamp.DatabaseId;
                    match.FactID = timestamp.Key;
                    db.Update(match);
                    db.Commit();
                    return Task.FromResult(0);
                }

                var record = new TimestampRecord
                {
                    PeerID = peerId,
                    PivotID = pivotId.key,
                    DatabaseID = timestamp.DatabaseId,
                    FactID = timestamp.Key
                };
                db.Insert(record);
                db.Commit();
                return Task.FromResult(0);
            }
        }

        public Task<List<IdentifiedFactMemento>> QueryForFactsAsync(QueryDefinition queryDefinition, FactID startingId, QueryOptions options)
        {
            using (var db = new SQLiteConnection(_correspondencePath))
            {
                return Task.FromResult(GetMatchingIds(queryDefinition, startingId.key, db)
                    .Select(id => LoadMementoById(id, db))
                    .ToList());
            }
        }

        public Task<List<FactID>> QueryForIdsAsync(QueryDefinition queryDefinition, FactID startingId)
        {
            using (var db = new SQLiteConnection(_correspondencePath))
            {
                return Task.FromResult(GetMatchingIds(queryDefinition, startingId.key, db)
                    .Select(id => new FactID { key = id })
                    .ToList());
            }
        }

        public List<IdentifiedFactMemento> GetAllSuccessors(FactID factId)
        {
            return new List<IdentifiedFactMemento>();
        }

        public List<CorrespondenceFactType> GetAllTypes()
        {
            return new List<CorrespondenceFactType>();
        }

        public Task<FactID> GetFactIDFromShareAsync(int peerId, FactID remoteFactId)
        {
            return Task.FromResult(new FactID());
        }

        public List<IdentifiedFactMemento> GetPageOfFactsForType(CorrespondenceFactType type, int page)
        {
            return new List<IdentifiedFactMemento>();
        }

        public Task<TimestampID> LoadOutgoingTimestampAsync(int peerId)
        {
            return Task.FromResult(new TimestampID());
        }

        public Task<List<MessageMemento>> LoadRecentMessagesForServerAsync(int peerId, TimestampID timestamp)
        {
            return Task.FromResult(new List<MessageMemento>());
        }

        public Task SaveOutgoingTimestampAsync(int peerId, TimestampID timestamp)
        {
            return Task.FromResult(0);
        }

        public Task SaveShareAsync(int peerId, FactID remoteFactId, FactID localFactId)
        {
            return Task.FromResult(0);
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

        private List<long> GetMatchingIds(QueryDefinition queryDefinition, long startingId, SQLiteConnection db)
        {
            List<long> matchingIds = new List<long>() { startingId };

            foreach (var join in queryDefinition.Joins)
            {
                int roleId = SaveRole(db, join.Role);
                if (join.Successor)
                {
                    matchingIds = db.Table<PredecessorRecord>()
                        .Where(p => matchingIds.Contains(p.PredecessorFactID) && p.RoleID == roleId)
                        .ToList()
                        .Select(p => p.FactID)
                        .Where(id => ConditionSatisfied(join.Condition, id, db))
                        .ToList();
                }
                else
                {
                    matchingIds = db.Table<PredecessorRecord>()
                        .Where(p => matchingIds.Contains(p.FactID) && p.RoleID == roleId)
                        .ToList()
                        .Select(p => p.PredecessorFactID)
                        .Where(id => ConditionSatisfied(join.Condition, id, db))
                        .ToList();
                }
            }
            return matchingIds;
        }

        private bool ConditionSatisfied(Condition condition, long factId, SQLiteConnection db)
        {
            if (condition != null)
            {
                foreach (var clause in condition.Clauses)
                {
                    var matchingIds = GetMatchingIds(clause.SubQuery, factId, db);
                    if (clause.IsEmpty && matchingIds.Any())
                        return false;
                    else if (!clause.IsEmpty && !matchingIds.Any())
                        return false;
                }
            }

            return true;
        }

        private IdentifiedFactMemento LoadMementoById(long factId, SQLiteConnection db)
        {
            var factRecords = db.Table<FactRecord>()
                                .Where(f => f.ID == factId);

            foreach (var factRecord in factRecords)
            {
                return LoadMemento(factRecord, db);
            }

            throw new CorrespondenceException(String.Format("Failed to load fact {0}.", factId));
        }

        private IdentifiedFactMemento LoadMemento(FactRecord factRecord, SQLiteConnection db)
        {
            var factType = LoadFactType(factRecord.FactTypeID, db);
            var factMemento = new FactMemento(factType)
            {
                Data = factRecord.Data ?? new byte[0]
            };

            var predecessorRecords = db.Table<PredecessorRecord>()
                .Where(p => p.FactID == factRecord.ID);
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

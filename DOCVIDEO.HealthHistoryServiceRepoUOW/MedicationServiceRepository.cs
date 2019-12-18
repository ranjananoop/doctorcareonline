using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.HealthHistoryServiceBoundedContext;
using DOCVIDEO.HealthHistoryServiceRepoUOW;
using DOCVIDEO.Domain;
using DOCVIDEO.ObjectState;


namespace DOCVIDEO.HealthHistoryServiceRepoUOW.Repositories.Disconnected
{
    public class MedicationRepository : IMedicationRepository
    {
        private readonly MedicationServiceContext _context;

        public MedicationRepository(MedicationUnitOfWork uow)
        {
            _context = uow.Context;
        }

        public IQueryable<MEDICATION> All
        {
            get { return _context.Medications; }
        }

        public List<MEDICATION> AllMedications
        {
            get { return _context.Medications.ToList(); }
        }

        public IQueryable<MEDICATION> AllIncluding(params Expression<Func<MEDICATION, object>>[] includeProperties)
        {
            IQueryable<MEDICATION> query = _context.Medications;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public MEDICATION Find(int id)
        {
            return _context.Medications.Find(id);
        }

        public void InsertOrUpdateGraph(MEDICATION medicationGraph)
        {
            if (medicationGraph.State == DOCVIDEO.ObjectState.State.Added)
            {
                _context.Medications.Add(medicationGraph);
            }
            else
            {
                _context.Medications.Add(medicationGraph);
                _context.ApplyStateChanges();
            }
        }

        public void InsertOrUpdate(MEDICATION medication)
        {
            if (medication.MEDICATIONID == default(int)) // New entity
            {
                _context.Entry(medication).State = EntityState.Added;
            }
            else        // Existing entity
            {
                _context.Entry(medication).State = EntityState.Modified;

            }
        }


        public void Delete(int id)
        {
            var medication = _context.Medications.Find(id);

            _context.Medications.Remove(medication);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public MEDICATION Find(string message)
        {
            throw new NotImplementedException();

        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

    }
}




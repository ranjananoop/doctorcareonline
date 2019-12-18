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
    public class HealthHistoryRepository : IHealthHistoryRepository
    {
        private readonly HealthHistoryServiceContext _context;

        public HealthHistoryRepository(UnitOfWork uow)
        {
            _context = uow.Context;
        }

        public IQueryable<PATIENTREPORT> All
        {
            get { return _context.PatientReports; }
        }

        public List<PATIENTREPORT> AllPatientreports
        {
            get { return _context.PatientReports.ToList(); }
        }

        public IQueryable<PATIENTREPORT> AllIncluding(params Expression<Func<PATIENTREPORT, object>>[] includeProperties)
        {
            IQueryable<PATIENTREPORT> query = _context.PatientReports;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public PATIENTREPORT Find(int id)
        {
            return _context.PatientReports.Find(id);
        }

        public void InsertOrUpdateGraph(PATIENTREPORT patientreportGraph)
        {
            if (patientreportGraph.State == DOCVIDEO.ObjectState.State.Added)
            {
                _context.PatientReports.Add(patientreportGraph);
            }
            else
            {
                _context.PatientReports.Add(patientreportGraph);
                _context.ApplyStateChanges();
            }
        }


        public void InsertOrUpdate(PATIENTREPORT patientreport)
        {
            if (patientreport.PATIENTREPORTID == default(int)) // New entity
            {
                _context.Entry(patientreport).State = EntityState.Added;
            }
            else        // Existing entity
            {
                _context.Entry(patientreport).State = EntityState.Modified;

            }

        }

        public void Delete(int id)
        {
            var patientreport = _context.PatientReports.Find(id);
            _context.PatientReports.Remove(patientreport);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public PATIENTREPORT Find(string message)
        {
            throw new NotImplementedException();


        }


        public void Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}
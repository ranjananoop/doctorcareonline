 using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.Domain;
using DOCVIDEO.UserServiceBoundedContext;
using DOCVIDEO.UserServiceRepoUOW;

namespace UserService.Repositories.Disconnected
{
    public class DoctorSpecialitiesServiceRepository :IDoctorSpecialityRepository
    {
        private readonly DoctorSpecialityServiceContext _context;

        public DoctorSpecialitiesServiceRepository(SpecialityUnitOfWork uow)
        {
            _context = uow.Context;
        }

        public IQueryable<DOCTORSPECIALITY> All
        {
            get { return _context.DoctorsSpecialities; }
        }

        public List<DOCTORSPECIALITY> Allworks
        {
            get { return _context.DoctorsSpecialities.ToList(); }
        }


        public IQueryable<DOCTORSPECIALITY> AllIncluding(params Expression<Func<DOCTORSPECIALITY, object>>[] includeProperties)
        {
            IQueryable<DOCTORSPECIALITY> query = _context.DoctorsSpecialities;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public DOCTORSPECIALITY Find(string id)
        {
            return _context.DoctorsSpecialities.Find(id);
        }


        public void InsertOrUpdateGraph(DOCTORSPECIALITY specGraph)
        {
            if (specGraph.State == DOCVIDEO.ObjectState.State.Added)
            {
                _context.DoctorsSpecialities.Add(specGraph);
            }
            else
            {
                _context.DoctorsSpecialities.Add(specGraph);
                _context.ApplyStateChanges();
            }
        }

        public void InsertOrUpdate(DOCTORSPECIALITY spec)
        {
            if (spec.SPECIALITYID  == default(int)) // New entity
            {
                _context.Entry(spec).State = EntityState.Added;
            }
            else        // Existing entity
            {
                _context.Entry(spec).State = EntityState.Modified;

            }
        }

        public void Delete(string id)
        {
            try
            {
                var doctorspecialities = _context.DoctorsSpecialities.Where(a => a.UserName == id);
                if (doctorspecialities != null)
                {
                    foreach (var speciality in doctorspecialities)
                    {
                        _context.DoctorsSpecialities.Remove(speciality);
                    }
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }


        public DOCTORSPECIALITY Find(int id)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
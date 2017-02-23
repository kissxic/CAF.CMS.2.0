
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Caching;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Domain.Cms.Members;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Exceptions;
using CAF.Infrastructure.Core.Pages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CAF.WebSite.Application.Services.Members
{

    public partial class MemberGradeService : IMemberGradeService
    {

        #region Constants


        #endregion

        #region Fields
        private readonly IRepository<MemberGrade> _memberGradeRepository;
        private readonly IRequestCache _requestCache;
        private readonly IEventPublisher _eventPublisher;
        private bool? _isSingleMemberGradeMode = null;
        #endregion

        #region Ctor


        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="requestCache"></param>
        /// <param name="memberGradeRepository"></param>
        /// <param name="eventPublisher"></param>
        public MemberGradeService(IRequestCache requestCache,
            IRepository<MemberGrade> memberGradeRepository,
            IEventPublisher eventPublisher)
        {
            this._requestCache = requestCache;
            this._memberGradeRepository = memberGradeRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        #region Utilities

        #endregion

        #region MemberGrade

        public IPagedList<MemberGrade> GetAllMemberGrades(int pageIndex, int pageSize)
        {
            var query = _memberGradeRepository.Table;
            query = query.OrderByDescending(c => c.Id);

            var MemberGrade = new PagedList<MemberGrade>(query, pageIndex, pageSize);
            return MemberGrade;
        }

        public void DeleteMemberGrade(MemberGrade MemberGrade)
        {
            if (MemberGrade == null)
                throw new ArgumentNullException("MemberGrade");

          
            _memberGradeRepository.Delete(MemberGrade);

            //event notification
            _eventPublisher.EntityDeleted(MemberGrade);
        }

        public MemberGrade GetMemberGradeById(int memberGradeId)
        {
            if (memberGradeId == 0)
                return null;

            var MemberGrade = _memberGradeRepository.GetById(memberGradeId);
            return MemberGrade;
        }

        public MemberGrade GetMemberGradeByName(string Name)
        {
            if (string.IsNullOrWhiteSpace(Name))
                return null;
            var query = from c in _memberGradeRepository.Table
                        where c.GradeName == Name
                        select c;
            var MemberGrade = query.FirstOrDefault();
            return MemberGrade;
        }
        

        public IList<MemberGrade> GetMemberGradeByIds(int[] memberGradeIds)
        {
            if (memberGradeIds == null || memberGradeIds.Length == 0)
                return new List<MemberGrade>();

            var query = from c in _memberGradeRepository.Table
                        where memberGradeIds.Contains(c.Id)
                        select c;
            var MemberGrade = query.ToList();
            //sort by passed identifiers
            var sortedMemberGrade = new List<MemberGrade>();
            foreach (int id in memberGradeIds)
            {
                var memberGrade = MemberGrade.Find(x => x.Id == id);
                if (memberGrade != null)
                    sortedMemberGrade.Add(memberGrade);
            }
            return sortedMemberGrade;
        }

        /// <summary>
        /// Gets all MemberGrade
        /// </summary>
        /// <returns>MemberGrade</returns>
        public IList<MemberGrade> GetAllMemberGrades()
        {
            var query = from s in _memberGradeRepository.Table
                        orderby s.Id
                        select s;
            var memberGrade = query.ToList();
            return memberGrade;
        }
 

        public void InsertMemberGrade(MemberGrade MemberGrade)
        {
            if (MemberGrade == null)
                throw new ArgumentNullException("MemberGrade");

            _memberGradeRepository.Insert(MemberGrade);

            //event notification
            _eventPublisher.EntityInserted(MemberGrade);
        }
      
        /// <summary>
        /// Updates the MemberGrade
        /// </summary>
        /// <param name="MemberGrade">MemberGrade</param>
        public virtual void UpdateMemberGrade(MemberGrade memberGrade)
        {
            if (memberGrade == null)
                throw new ArgumentNullException("memberGrade");

            _memberGradeRepository.Update(memberGrade);

            //event notification
            _eventPublisher.EntityUpdated(memberGrade);
        }
       
        #endregion
        #endregion

      

    }
}

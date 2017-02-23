using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Caching;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Domain.Cms.Members;
using CAF.Infrastructure.Core.Domain.Sites;
using CAF.WebSite.Application.Services.Members;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CAF.WebSite.Application.Services.Members
{
	/// <summary>
	/// MemberGrade mapping service
	/// </summary>
	public partial class MemberGradeMappingService : IMemberGradeMappingService
    {
		#region Constants

        private const string MEMBERGRADMAPPING_BY_ENTITYID_NAME_KEY = "CAF.WebSite.membergrademapping.entityid-name-{0}-{1}";
        private const string MEMBERGRADMAPPING_PATTERN_KEY = "CAF.WebSite.membergrademapping.";

		#endregion

		#region Fields

		private readonly IRepository<MemberGradeMapping> _memberGradeMappingRepository;
		private readonly IWorkContext _workContext;
		private readonly IMemberGradeService _memberGradeService;
		private readonly IRequestCache _requestCache;

		#endregion

		#region Ctor

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="requestCache">Cache manager</param>
		/// <param name="memberGradeContext">Site context</param>
		/// <param name="memberGradeMappingRepository">Site mapping repository</param>
		public MemberGradeMappingService(IRequestCache requestCache,
            IWorkContext workContext,
			IMemberGradeService memberGradeService,
			IRepository<MemberGradeMapping> memberGradeMappingRepository)
		{
			this._requestCache = requestCache;
			this._workContext = workContext;
			this._memberGradeService = memberGradeService;
			this._memberGradeMappingRepository = memberGradeMappingRepository;

			this.QuerySettings = DbQuerySettings.Default;
		}

		public DbQuerySettings QuerySettings { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Deletes a memberGrade mapping record
		/// </summary>
		/// <param name="memberGradeMapping">MemberGrade mapping record</param>
		public virtual void DeleteMemberGradeMapping(MemberGradeMapping memberGradeMapping)
		{
			if (memberGradeMapping == null)
				throw new ArgumentNullException("memberGradeMapping");

			_memberGradeMappingRepository.Delete(memberGradeMapping);

			//cache
			_requestCache.RemoveByPattern(MEMBERGRADMAPPING_PATTERN_KEY);
		}

		/// <summary>
		/// Gets a memberGrade mapping record
		/// </summary>
		/// <param name="memberGradeMappingId">MemberGrade mapping record identifier</param>
		/// <returns>MemberGrade mapping record</returns>
		public virtual MemberGradeMapping GetMemberGradeMappingById(int memberGradeMappingId)
		{
			if (memberGradeMappingId == 0)
				return null;

			var memberGradeMapping = _memberGradeMappingRepository.GetById(memberGradeMappingId);
			return memberGradeMapping;
		}

		/// <summary>
		/// Gets memberGrade mapping records
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="entity">Entity</param>
		/// <returns>MemberGrade mapping records</returns>
		public virtual IList<MemberGradeMapping> GetMemberGradeMappings<T>(T entity) where T : BaseEntity, IMemberGradeMappingSupported
		{
			if (entity == null)
				throw new ArgumentNullException("entity");

			int entityId = entity.Id;
			string entityName = typeof(T).Name;

			var query = from sm in _memberGradeMappingRepository.Table
						where sm.EntityId == entityId &&
						sm.EntityName == entityName
						select sm;
			var memberGradeMappings = query.ToList();
			return memberGradeMappings;
		}

		/// <summary>
		/// Gets memberGrade mapping records
		/// </summary>
		/// <param name="entityName">Could be null</param>
		/// <param name="entityId">Could be 0</param>
		/// <returns>MemberGrade mapping record query</returns>
		public virtual IQueryable<MemberGradeMapping> GetMemberGradeMappingsFor(string entityName, int entityId)
		{
			var query = _memberGradeMappingRepository.Table;

			if (entityName.HasValue())
				query = query.Where(x => x.EntityName == entityName);

			if (entityId != 0)
				query = query.Where(x => x.EntityId == entityId);

			return query;
		}

		/// <summary>
		/// Save the memberGrade napping for an entity
		/// </summary>
		/// <typeparam name="T">Entity type</typeparam>
		/// <param name="entity">The entity</param>
		/// <param name="selectedMemberGradeIds">Array of selected memberGrade ids</param>
		public virtual void SaveMemberGradeMappings<T>(T entity, int[] selectedMemberGradeIds) where T : BaseEntity, IMemberGradeMappingSupported
		{
			var existingMemberGradeMappings = GetMemberGradeMappings(entity);
			var allMemberGrades = _memberGradeService.GetAllMemberGrades();

			foreach (var memberGrade in allMemberGrades)
			{
				if (selectedMemberGradeIds != null && selectedMemberGradeIds.Contains(memberGrade.Id))
				{
					if (existingMemberGradeMappings.Where(sm => sm.MemberGradeId == memberGrade.Id).Count() == 0)
						InsertMemberGradeMapping(entity, memberGrade.Id);
				}
				else
				{
					var memberGradeMappingToDelete = existingMemberGradeMappings.Where(sm => sm.MemberGradeId == memberGrade.Id).FirstOrDefault();
					if (memberGradeMappingToDelete != null)
						DeleteMemberGradeMapping(memberGradeMappingToDelete);
				}
			}
		}

		/// <summary>
		/// Inserts a memberGrade mapping record
		/// </summary>
		/// <param name="memberGradeMapping">MemberGrade mapping</param>
		public virtual void InsertMemberGradeMapping(MemberGradeMapping memberGradeMapping)
		{
			if (memberGradeMapping == null)
				throw new ArgumentNullException("memberGradeMapping");

			_memberGradeMappingRepository.Insert(memberGradeMapping);

			//cache
			_requestCache.RemoveByPattern(MEMBERGRADMAPPING_PATTERN_KEY);
		}

		/// <summary>
		/// Inserts a memberGrade mapping record
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="memberGradeId">MemberGrade id</param>
		/// <param name="entity">Entity</param>
		public virtual void InsertMemberGradeMapping<T>(T entity, int memberGradeId) where T : BaseEntity, IMemberGradeMappingSupported
		{
			if (entity == null)
				throw new ArgumentNullException("entity");

			if (memberGradeId == 0)
				throw new ArgumentOutOfRangeException("memberGradeId");

			int entityId = entity.Id;
			string entityName = typeof(T).Name;

			var memberGradeMapping = new MemberGradeMapping()
			{
				EntityId = entityId,
				EntityName = entityName,
				MemberGradeId = memberGradeId
			};

			InsertMemberGradeMapping(memberGradeMapping);
		}

		/// <summary>
		/// Updates the memberGrade mapping record
		/// </summary>
		/// <param name="memberGradeMapping">MemberGrade mapping</param>
		public virtual void UpdateMemberGradeMapping(MemberGradeMapping memberGradeMapping)
		{
			if (memberGradeMapping == null)
				throw new ArgumentNullException("memberGradeMapping");

			_memberGradeMappingRepository.Update(memberGradeMapping);

			//cache
			_requestCache.RemoveByPattern(MEMBERGRADMAPPING_PATTERN_KEY);
		}

		/// <summary>
		/// Find memberGrade identifiers with granted access (mapped to the entity)
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="entity">Wntity</param>
		/// <returns>MemberGrade identifiers</returns>
		public virtual int[] GetMemberGradesIdsWithAccess<T>(T entity) where T : BaseEntity, IMemberGradeMappingSupported
		{
			if (entity == null)
				throw new ArgumentNullException("entity");

			int entityId = entity.Id;
			string entityName = typeof(T).Name;

			string key = string.Format(MEMBERGRADMAPPING_BY_ENTITYID_NAME_KEY, entityId, entityName);
			return _requestCache.Get(key, () =>
			{
				var query = from sm in _memberGradeMappingRepository.Table
							where sm.EntityId == entityId &&
							sm.EntityName == entityName
							select sm.MemberGradeId;
				var result = query.ToArray();
				//little hack here. nulls aren't cacheable so set it to ""
				if (result == null)
					result = new int[0];
				return result;
			});
		}

		/// <summary>
		/// Authorize whether entity could be accessed in the current memberGrade (mapped to this memberGrade)
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="entity">Wntity</param>
		/// <returns>true - authorized; otherwise, false</returns>
		public virtual bool Authorize<T>(T entity) where T : BaseEntity, IMemberGradeMappingSupported
		{
			return Authorize(entity, _workContext.CurrentMemberGrade.Id);
		}

		/// <summary>
		/// Authorize whether entity could be accessed in a memberGrade (mapped to this memberGrade)
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="entity">Entity</param>
		/// <param name="memberGrade">MemberGrade</param>
		/// <returns>true - authorized; otherwise, false</returns>
		public virtual bool Authorize<T>(T entity, int memberGradeId) where T : BaseEntity, IMemberGradeMappingSupported
		{
			if (entity == null)
				return false;

			if (memberGradeId == 0)
				//return true if no memberGrade specified/found
				return true;

			if (!entity.LimitedToMemberGrades)
				return true;

			foreach (var memberGradeIdWithAccess in GetMemberGradesIdsWithAccess(entity))
				if (memberGradeId == memberGradeIdWithAccess)
					//yes, we have such permission
					return true;

			//no permission found
			return false;
		}

		#endregion
	}
}
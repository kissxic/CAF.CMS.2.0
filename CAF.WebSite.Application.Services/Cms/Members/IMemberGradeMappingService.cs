using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Members;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CAF.WebSite.Application.Services.Members
{
	/// <summary>
	/// MemberGrade mapping service interface
	/// </summary>
    public partial interface IMemberGradeMappingService
    {
        /// <summary>
        /// Deletes a memberGrade mapping record
        /// </summary>
        /// <param name="memberGradeMapping">MemberGrade mapping record</param>
        void DeleteMemberGradeMapping(MemberGradeMapping memberGradeMapping);

        /// <summary>
        /// Gets a memberGrade mapping record
        /// </summary>
        /// <param name="memberGradeMappingId">MemberGrade mapping record identifier</param>
        /// <returns>MemberGrade mapping record</returns>
        MemberGradeMapping GetMemberGradeMappingById(int memberGradeMappingId);

        /// <summary>
        /// Gets memberGrade mapping records
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>MemberGrade mapping records</returns>
        IList<MemberGradeMapping> GetMemberGradeMappings<T>(T entity) where T : BaseEntity, IMemberGradeMappingSupported;

        /// <summary>
        /// Gets memberGrade mapping records
        /// </summary>
        /// <param name="entityName">Could be null</param>
        /// <param name="entityId">Could be 0</param>
        /// <returns>MemberGrade mapping record query</returns>
        IQueryable<MemberGradeMapping> GetMemberGradeMappingsFor(string entityName, int entityId);

        /// <summary>
        /// Save the memberGrade napping for an entity
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">The entity</param>
        /// <param name="selectedMemberGradeIds">Array of selected memberGrade ids</param>
        void SaveMemberGradeMappings<T>(T entity, int[] selectedMemberGradeIds) where T : BaseEntity, IMemberGradeMappingSupported;

        /// <summary>
        /// Inserts a memberGrade mapping record
        /// </summary>
        /// <param name="memberGradeMapping">MemberGrade mapping</param>
        void InsertMemberGradeMapping(MemberGradeMapping memberGradeMapping);

        /// <summary>
        /// Inserts a memberGrade mapping record
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="memberGradeId">MemberGrade id</param>
        /// <param name="entity">Entity</param>
        void InsertMemberGradeMapping<T>(T entity, int memberGradeId) where T : BaseEntity, IMemberGradeMappingSupported;

        /// <summary>
        /// Updates the memberGrade mapping record
        /// </summary>
        /// <param name="memberGradeMapping">MemberGrade mapping</param>
        void UpdateMemberGradeMapping(MemberGradeMapping memberGradeMapping);

        /// <summary>
        /// Find memberGrade identifiers with granted access (mapped to the entity)
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Wntity</param>
        /// <returns>MemberGrade identifiers</returns>
        int[] GetMemberGradesIdsWithAccess<T>(T entity) where T : BaseEntity, IMemberGradeMappingSupported;

        /// <summary>
        /// Authorize whether entity could be accessed in the current memberGrade (mapped to this memberGrade)
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Wntity</param>
        /// <returns>true - authorized; otherwise, false</returns>
        bool Authorize<T>(T entity) where T : BaseEntity, IMemberGradeMappingSupported;

        /// <summary>
        /// Authorize whether entity could be accessed in a memberGrade (mapped to this memberGrade)
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="memberGradeId">MemberGrade identifier</param>
        /// <returns>true - authorized; otherwise, false</returns>
        bool Authorize<T>(T entity, int memberGradeId) where T : BaseEntity, IMemberGradeMappingSupported;
	}
}
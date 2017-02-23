using CAF.Infrastructure.Core.Domain.Cms.Members;
using CAF.Infrastructure.Core.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.WebSite.Application.Services.Members
{

    public partial interface IMemberGradeService
    {

        #region MemberGrade

        /// <summary>
        /// Gets all MemberGrade 
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>MemberGrade</returns>
        IPagedList<MemberGrade> GetAllMemberGrades(int pageIndex, int pageSize);

        /// <summary>
        /// Delete a MemberGrade
        /// </summary>
        /// <param name="MemberGrade">MemberGrade</param>
        void DeleteMemberGrade(MemberGrade memberGrade);

        /// <summary>
        /// Gets a MemberGrade
        /// </summary>
        /// <param name="MemberGradeId">MemberGrade identifier</param>
        /// <returns>A MemberGrade</returns>
        MemberGrade GetMemberGradeById(int memberGradeId);

        /// <summary>
        /// Get MemberGrade by identifiers
        /// </summary>
        /// <param name="MemberGradeIds">MemberGrade identifiers</param>
        /// <returns>MemberGrade</returns>
        IList<MemberGrade> GetMemberGradeByIds(int[] memberGradeIds);

        /// <summary>
        /// Get MemberGrade by identifiers
        /// </summary>
        /// <returns>MemberGrade</returns>
        IList<MemberGrade> GetAllMemberGrades();

        /// <summary>
        /// Gets a MemberGrade
        /// </summary>
        /// <param name="Name">Name</param>
        /// <returns>MemberGrade</returns>
        MemberGrade GetMemberGradeByName(string name);

        /// <summary>
        /// Insert a MemberGrade
        /// </summary>
        /// <param name="MemberGrade">MemberGrade</param>
        void InsertMemberGrade(MemberGrade memberGrade);


        /// <summary>
        /// Updates the MemberGrade
        /// </summary>
        /// <param name="MemberGrade">MemberGrade</param>
        void UpdateMemberGrade(MemberGrade memberGrade);
        #endregion

     

    }
}

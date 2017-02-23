using System.Collections.Generic;
using System.Linq;
using CAF.Infrastructure.Core.Domain.Directory;

namespace CAF.WebSite.Application.Services.Directory
{
    /// <summary>
    /// District service interface
    /// </summary>
    public partial interface IDistrictService
    {
        /// <summary>
        /// Deletes a district
        /// </summary>
        /// <param name="district">The district</param>
        void DeleteDistrict(District district);

		/// <summary>
		/// Get all states/provinces
		/// </summary>
		/// <param name="showHidden">A value indicating whether to show hidden records</param>
		/// <returns></returns>
		IQueryable<District> GetAllDistricts();

		/// <summary>
		/// Gets a district
		/// </summary>
		/// <param name="districtId">The district identifier</param>
		/// <returns>State/province</returns>
		District GetDistrictById(int districtId);

        /// <summary>
        /// Gets a district 
        /// </summary>
        /// <param name="abbreviation">The district abbreviation</param>
        /// <returns>State/province</returns>
        District GetDistrictByDistrictCode(string districtCode);
        
        /// <summary>
        /// Gets a district collection by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>State/province collection</returns>
        IList<District> GetDistrictsByCityId(int cityId);

        /// <summary>
        /// Inserts a district
        /// </summary>
        /// <param name="district">State/province</param>
        void InsertDistrict(District district);

        /// <summary>
        /// Updates a district
        /// </summary>
        /// <param name="district">State/province</param>
        void UpdateDistrict(District district);
    }
}

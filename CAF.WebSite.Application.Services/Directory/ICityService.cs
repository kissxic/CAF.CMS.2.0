using System.Collections.Generic;
using System.Linq;
using CAF.Infrastructure.Core.Domain.Directory;

namespace CAF.WebSite.Application.Services.Directory
{
    /// <summary>
    /// State province service interface
    /// </summary>
    public partial interface ICityService
    {
        /// <summary>
        /// Deletes a state/province
        /// </summary>
        /// <param name="city">The state/province</param>
        void DeleteCity(City city);

		/// <summary>
		/// Get all states/provinces
		/// </summary>
		/// <param name="showHidden">A value indicating whether to show hidden records</param>
		/// <returns></returns>
		IQueryable<City> GetAllCitys();

		/// <summary>
		/// Gets a state/province
		/// </summary>
		/// <param name="cityId">The state/province identifier</param>
		/// <returns>State/province</returns>
		City GetCityById(int cityId);

        /// <summary>
        /// Gets a state/province 
        /// </summary>
        /// <param name="abbreviation">The state/province abbreviation</param>
        /// <returns>State/province</returns>
        City GetCityByCityCode(string cityCode);
        
        /// <summary>
        /// Gets a state/province collection by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>State/province collection</returns>
        IList<City> GetCitysByProvinceId(int provinceId);

        /// <summary>
        /// Inserts a state/province
        /// </summary>
        /// <param name="city">State/province</param>
        void InsertCity(City city);

        /// <summary>
        /// Updates a state/province
        /// </summary>
        /// <param name="city">State/province</param>
        void UpdateCity(City city);
    }
}

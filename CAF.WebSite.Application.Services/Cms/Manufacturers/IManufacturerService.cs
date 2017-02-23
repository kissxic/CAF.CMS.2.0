using CAF.Infrastructure.Core.Domain.Cms.Manufacturers;
using CAF.Infrastructure.Core.Pages;
using System.Collections.Generic;
using System.Linq;

namespace CAF.WebSite.Application.Services.Manufacturers
{
    /// <summary>
    /// Manufacturer service
    /// </summary>
    public partial interface IManufacturerService
    {
        /// <summary>
        /// Deletes a manufacturer
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        void DeleteManufacturer(Manufacturer manufacturer);

		/// <summary>
		/// Get manufacturer query
		/// </summary>
		/// <param name="showHidden">A value indicating whether to show hidden records</param>
		/// <param name="storeId">Store identifier</param>
		/// <returns>Manufacturer query</returns>
		IQueryable<Manufacturer> GetManufacturers(bool showHidden = false);

        /// <summary>
        /// Gets all manufacturers
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Manufacturer collection</returns>
        IList<Manufacturer> GetAllManufacturers(bool showHidden = false);

        /// <summary>
        /// Gets all manufacturers
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Manufacturer collection</returns>
        IList<Manufacturer> GetAllManufacturersById(int[] manufacturerIds);

        /// <summary>
        /// Gets all manufacturers
        /// </summary>
        /// <param name="manufacturerName">Manufacturer name</param>
        /// <param name="storeId">Whether to filter result by store identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Manufacturer collection</returns>
        IList<Manufacturer> GetAllManufacturers(string manufacturerName, bool showHidden = false);

		/// <summary>
		/// Gets all manufacturers
		/// </summary>
		/// <param name="manufacturerName">Manufacturer name</param>
		/// <param name="pageIndex">Page index</param>
		/// <param name="pageSize">Page size</param>
		/// <param name="storeId">Whether to filter result by store identifier</param>
		/// <param name="showHidden">A value indicating whether to show hidden records</param>
		/// <returns>Manufacturers</returns>
		IPagedList<Manufacturer> GetAllManufacturers(string manufacturerName,
            int pageIndex, int pageSize,bool showHidden = false);

        /// <summary>
        /// Gets a manufacturer
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <returns>Manufacturer</returns>
        Manufacturer GetManufacturerById(int manufacturerId);

        /// <summary>
        /// Inserts a manufacturer
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        void InsertManufacturer(Manufacturer manufacturer);

        /// <summary>
        /// Updates the manufacturer
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        void UpdateManufacturer(Manufacturer manufacturer);

       
    }
}


using CAF.Infrastructure.Core.Domain.Sellers;
using CAF.Infrastructure.Core.Pages;
using System.Collections.Generic;

namespace CAF.WebSite.Application.Services.Vendors
{
    /// <summary>
    /// Vendor service interface
    /// </summary>
    public partial interface IVendorService
    {
        /// <summary>
        /// Gets a vvendor by vvendor identifier
        /// </summary>
        /// <param name="vvendorId">Vendor identifier</param>
        /// <returns>Vendor</returns>
        Vendor GetVendorById(int vendorId);

        IList<Vendor> GetVendorById(int[] vendorIds);

        /// <summary>
        /// Delete a vvendor
        /// </summary>
        /// <param name="vvendor">Vendor</param>
        void DeleteVendor(Vendor vendor);

        /// <summary>
        /// Gets all vvendors
        /// </summary>
        /// <param name="name">Vendor name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Vendors</returns>
        IPagedList<Vendor> GetAllVendors(string name = "",
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Inserts a vvendor
        /// </summary>
        /// <param name="vvendor">Vendor</param>
        void InsertVendor(Vendor vendor);

        Vendor CreateEmptyVendor();

        /// <summary>
        /// Updates the vvendor
        /// </summary>
        /// <param name="vvendor">Vendor</param>
        void UpdateVendor(Vendor vendor);



        /// <summary>
        /// Gets a vvendor note note
        /// </summary>
        /// <param name="vvendorNoteId">The vvendor note identifier</param>
        /// <returns>Vendor note</returns>
        VendorNote GetVendorNoteById(int vendorNoteId);

        /// <summary>
        /// Deletes a vvendor note
        /// </summary>
        /// <param name="vvendorNote">The vvendor note</param>
        void DeleteVendorNote(VendorNote vendorNote);
    }
}

using System;
using System.Linq;
using CAF.Infrastructure.Core.Domain.Sellers;
using CAF.Infrastructure.Core.Pages;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using System.Collections.Generic;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Caching;

namespace CAF.WebSite.Application.Services.Vendors
{
    /// <summary>
    /// Vendor service
    /// </summary>
    public partial class VendorService : IVendorService
    {
        #region Constants

        private const string VENDOR_BY_ID_KEY = "CAF.WebSite.vendor.id-{0}";
        private const string VENDOR_PATTERN_KEY = "CAF.WebSite.vendor.";

        #endregion

        #region Fields
        private readonly IRepository<Vendor> _vendorRepository;
        private readonly IRepository<VendorNote> _vendorNoteRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRequestCache _requestCache;
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="vendorRepository">Vendor repository</param>
        /// <param name="vendorNoteRepository">Vendor note repository</param>
        /// <param name="eventPublisher">Event published</param>
        public VendorService(IRepository<Vendor> vendorRepository,
            IRepository<VendorNote> vendorNoteRepository,
            IEventPublisher eventPublisher,
            IRequestCache requestCache)
        {
            this._requestCache = requestCache;
            this._vendorRepository = vendorRepository;
            this._vendorNoteRepository = vendorNoteRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a vendor by vendor identifier
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <returns>Vendor</returns>
        public virtual Vendor GetVendorById(int vendorId)
        {
            if (vendorId == 0)
                return null;
            string key = string.Format(VENDOR_BY_ID_KEY, vendorId);
            return _requestCache.Get(key, () =>
            {
                return _vendorRepository.GetById(vendorId);
            });
           
        }

        public virtual IList<Vendor> GetVendorById(int[] vendorIds)
        {
            if (vendorIds == null || vendorIds.Length == 0)
                return new List<Vendor>();

            var query = from c in _vendorRepository.Table
                        where vendorIds.Contains(c.Id)
                        select c;
            var vendors = query.ToList();

            var sortedVendor = new List<Vendor>();
            foreach (int id in vendorIds)
            {
                var vendor = vendors.Find(x => x.Id == id);
                if (vendor != null)
                    sortedVendor.Add(vendor);
            }
            return sortedVendor;
        }


        /// <summary>
        /// Delete a vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        public virtual void DeleteVendor(Vendor vendor)
        {
            if (vendor == null)
                throw new ArgumentNullException("vendor");

            vendor.Deleted = true;
            UpdateVendor(vendor);

            //cache
            _requestCache.RemoveByPattern(string.Format(VENDOR_BY_ID_KEY, vendor.Id));
           
        }

        /// <summary>
        /// Gets all vendors
        /// </summary>
        /// <param name="name">Vendor name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Vendors</returns>
        public virtual IPagedList<Vendor> GetAllVendors(string name = "",
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = _vendorRepository.Table;
            if (!String.IsNullOrWhiteSpace(name))
                query = query.Where(v => v.Name.Contains(name));
            if (!showHidden)
                query = query.Where(v => v.Active);
            query = query.Where(v => !v.Deleted);
            query = query.OrderBy(v => v.DisplayOrder).ThenBy(v => v.Name);

            var vendors = new PagedList<Vendor>(query, pageIndex, pageSize);
            return vendors;
        }

        /// <summary>
        /// Inserts a vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        public virtual void InsertVendor(Vendor vendor)
        {
            if (vendor == null)
                throw new ArgumentNullException("vendor");

            _vendorRepository.Insert(vendor);

            //event notification
            _eventPublisher.EntityInserted(vendor);
        }
        /// <summary>
        /// 新建空供应商
        /// </summary>
        /// <returns></returns>
        public Vendor CreateEmptyVendor()
        {
            Vendor vendorInfo = null;
            using (var scope = new DbContextScope(ctx: _vendorRepository.Context, autoDetectChanges: false, proxyCreation: false, validateOnSave: false))
            {
                var autoCommit = _vendorRepository.AutoCommitEnabled;
                _vendorRepository.AutoCommitEnabled = false;

                vendorInfo = new Vendor()
                {
                    Name = "",
                    VendorGradeId = 1,
                    CompanyRegionId = 0,
                    CompanyEmployeeCount = CompanyEmployeeCount.LessThanFive,
                    CompanyRegisteredCapital = new decimal(0),
                    BankPictureId = 0,
                    BankRegionId = 0,
                    VendorStage = VendorStage.CompanyInfo,
                    CreateDate = DateTime.Now,

                };
                _vendorRepository.Insert(vendorInfo);
                _vendorRepository.Context.SaveChanges();

                _vendorRepository.AutoCommitEnabled = autoCommit;
            }

            return vendorInfo;
        }

        /// <summary>
        /// Updates the vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        public virtual void UpdateVendor(Vendor vendor)
        {
            if (vendor == null)
                throw new ArgumentNullException("vendor");

            _vendorRepository.Update(vendor);

            //cache
            _requestCache.RemoveByPattern(string.Format(VENDOR_BY_ID_KEY, vendor.Id));

            //event notification
            _eventPublisher.EntityUpdated(vendor);
        }

        /// <summary>
        /// Gets a vendor note note
        /// </summary>
        /// <param name="vendorNoteId">The vendor note identifier</param>
        /// <returns>Vendor note</returns>
        public virtual VendorNote GetVendorNoteById(int vendorNoteId)
        {
            if (vendorNoteId == 0)
                return null;

            return _vendorNoteRepository.GetById(vendorNoteId);
        }

        /// <summary>
        /// Deletes a vendor note
        /// </summary>
        /// <param name="vendorNote">The vendor note</param>
        public virtual void DeleteVendorNote(VendorNote vendorNote)
        {
            if (vendorNote == null)
                throw new ArgumentNullException("vendorNote");

            _vendorNoteRepository.Delete(vendorNote);

            //event notification
            _eventPublisher.EntityDeleted(vendorNote);
        }

        #endregion
    }
}
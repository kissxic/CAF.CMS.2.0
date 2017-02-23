using Autofac;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Domain.Cms.PageSettings;
using CAF.Infrastructure.Core.Domain.Common;
using CAF.Infrastructure.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
namespace CAF.WebSite.Application.Services.PageSettings
{
    public class SlideAdInfoService : ISlideAdInfoService
    {
        private readonly IRepository<SlideAdInfo> _slideAdInfoRepository;
        private readonly IRepository<ImageAdInfo> _imageAdInfoRepository;
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly IEventPublisher _eventPublisher;

        public SlideAdInfoService(IRepository<SlideAdInfo> slideAdInfoRepository,
            IRepository<ImageAdInfo> imageAdInfoRepository,
            AdminAreaSettings adminAreaSettings,
            IComponentContext ctx,
            IEventPublisher eventPublisher)
        {
            this._slideAdInfoRepository = slideAdInfoRepository;
            this._imageAdInfoRepository = imageAdInfoRepository;
            this._adminAreaSettings = adminAreaSettings;
            _eventPublisher = eventPublisher;
        }
        #region Methods

        /// <summary>
        /// Deletes a slideAdInfo
        /// </summary>
        /// <param name="slideAdInfo">SlideAdInfo</param>
        public virtual void DeleteSlideAdInfo(SlideAdInfo slideAdInfo)
        {
            if (slideAdInfo == null)
                throw new ArgumentNullException("slideAdInfo");

            _slideAdInfoRepository.Delete(slideAdInfo);

            //event notification
            _eventPublisher.EntityDeleted(slideAdInfo);
        }

        /// <summary>
        /// Gets a slideAdInfo
        /// </summary>
        /// <param name="slideAdInfoId">The slideAdInfo identifier</param>
        /// <returns>SlideAdInfo</returns>
        public virtual SlideAdInfo GetSlideAdInfoById(int slideAdInfoId)
        {
            if (slideAdInfoId == 0)
                return null;

            return _slideAdInfoRepository.GetById(slideAdInfoId);
        }



        /// <summary>
        /// Gets all slideAdInfos
        /// </summary>
        /// <param name="siteId">Site identifier; pass 0 to load all records</param>
        /// <returns>SlideAdInfos</returns>
        public virtual IList<SlideAdInfo> GetAllSlideAdInfos(int slideAdTypeId = 0)
        {
            var query = _slideAdInfoRepository.Table;
            if (slideAdTypeId != 0)
                query = query.Where(x => x.SlideAdTypeId == slideAdTypeId);
            query = query.OrderBy(t => t.CreatedOnUtc).ThenBy(t => t.DisplayOrder);

            return query.ToList();
        }

        /// <summary>
        /// Inserts a slideAdInfo
        /// </summary>
        /// <param name="slideAdInfo">SlideAdInfo</param>
        public virtual void InsertSlideAdInfo(SlideAdInfo slideAdInfo)
        {
            if (slideAdInfo == null)
                throw new ArgumentNullException("slideAdInfo");

            _slideAdInfoRepository.Insert(slideAdInfo);

            //event notification
            _eventPublisher.EntityInserted(slideAdInfo);
        }

        /// <summary>
        /// Updates the slideAdInfo
        /// </summary>
        /// <param name="slideAdInfo">SlideAdInfo</param>
        public virtual void UpdateSlideAdInfo(SlideAdInfo slideAdInfo)
        {
            if (slideAdInfo == null)
                throw new ArgumentNullException("slideAdInfo");

            _slideAdInfoRepository.Update(slideAdInfo);

            //event notification
            _eventPublisher.EntityUpdated(slideAdInfo);
        }



        /// <summary>
        /// Deletes a imageAdInfo
        /// </summary>
        /// <param name="imageAdInfo">ImageAdInfo</param>
        public virtual void DeleteImageAdInfo(ImageAdInfo imageAdInfo)
        {
            if (imageAdInfo == null)
                throw new ArgumentNullException("imageAdInfo");

            _imageAdInfoRepository.Delete(imageAdInfo);

            //event notification
            _eventPublisher.EntityDeleted(imageAdInfo);
        }

        /// <summary>
        /// Gets a imageAdInfo
        /// </summary>
        /// <param name="imageAdInfoId">The imageAdInfo identifier</param>
        /// <returns>ImageAdInfo</returns>
        public virtual ImageAdInfo GetImageAdInfoById(int imageAdInfoId)
        {
            if (imageAdInfoId == 0)
                return null;

            return _imageAdInfoRepository.GetById(imageAdInfoId);
        }
        /// <summary>
        ///  Gets a imageAdInfo
        /// </summary>
        /// <param name="postionId"></param>
        /// <returns></returns>
        public virtual ImageAdInfo GetImageAdInfoByPostionId(int postionId)
        {
            if (postionId == 0)
                return null;
            return _imageAdInfoRepository.Where(x => x.PostionId == postionId)?.FirstOrDefault();
        }


        /// <summary>
        /// Gets all imageAdInfos
        /// </summary>
        /// <param name="siteId">Site identifier; pass 0 to load all records</param>
        /// <returns>ImageAdInfos</returns>
        public virtual IList<ImageAdInfo> GetAllImageAdInfos()
        {
            var query = _imageAdInfoRepository.Table;
            query = query.OrderBy(t => t.PostionId);

            return query.ToList();
        }

        /// <summary>
        /// Inserts a imageAdInfo
        /// </summary>
        /// <param name="imageAdInfo">ImageAdInfo</param>
        public virtual void InsertImageAdInfo(ImageAdInfo imageAdInfo)
        {
            if (imageAdInfo == null)
                throw new ArgumentNullException("imageAdInfo");

            _imageAdInfoRepository.Insert(imageAdInfo);

            //event notification
            _eventPublisher.EntityInserted(imageAdInfo);
        }

        /// <summary>
        /// Updates the imageAdInfo
        /// </summary>
        /// <param name="imageAdInfo">ImageAdInfo</param>
        public virtual void UpdateImageAdInfo(ImageAdInfo imageAdInfo)
        {
            if (imageAdInfo == null)
                throw new ArgumentNullException("imageAdInfo");
            if (imageAdInfo.Id == 0)
                _imageAdInfoRepository.Insert(imageAdInfo);
            else
                _imageAdInfoRepository.Update(imageAdInfo);

            //event notification
            _eventPublisher.EntityUpdated(imageAdInfo);
        }

        #endregion
    }
}

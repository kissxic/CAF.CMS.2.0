using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Cms.Manufacturers;
using CAF.Infrastructure.Core.Domain.Cms.Topic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CAF.Infrastructure.Core.Caching
{
	public partial class DisplayControl : IDisplayControl
	{
		public static readonly HashSet<Type> CandidateTypes = new HashSet<Type>(new Type[] 
		{
			typeof(ArticleComment),
			typeof(ArticleCategory),
			typeof(Manufacturer),
			typeof(Article),
			typeof(ArticleAlbum),
			typeof(ArticleSpecificationAttribute),
			typeof(SpecificationAttributeOption),
			typeof(ProductVariantAttribute),
			typeof(ProductVariantAttributeValue),
			typeof(ProductVariantAttributeCombination),
			typeof(ArticleCategory),
			typeof(Topic)
		});

		private readonly HashSet<BaseEntity> _entities = new HashSet<BaseEntity>();

		private bool? _isUncacheableRequest;

		public void Announce(BaseEntity entity)
		{
			if (entity != null)
			{
				_entities.Add(entity);
			}
		}

		public bool IsDisplayed(BaseEntity entity)
		{
			if (entity == null)
				return false;

			return _entities.Contains(entity);
		}

		public void MarkRequestAsUncacheable()
		{
			// First wins: subsequent calls should not be able to cancel this
			_isUncacheableRequest = true;
		}

		public bool IsUncacheableRequest
		{
			get
			{
				return _isUncacheableRequest.GetValueOrDefault() == true;
			}
		}

		public IEnumerable<string> GetCacheControlTagsFor(BaseEntity entity)
		{
			Guard.NotNull(entity, nameof(entity));

			// TODO: purge all Articles when: DeliveryTime, QuantityUnit, MeasureWeight change

			if (entity.IsTransientRecord())
			{
				yield break;
			}

			var type = entity.GetUnproxiedType();

			if (!CandidateTypes.Contains(type))
			{
				yield break;
			}

			if (type == typeof(ArticleComment))
			{
				yield return "a" + ((ArticleComment)entity).ArticleId;
			}

			else if (type == typeof(ArticleCategory))
			{
				yield return "c" + entity.Id;
			}
			else if (type == typeof(Manufacturer))
			{
				yield return "m" + entity.Id;
			}
			else if (type == typeof(Article))
			{
				var Article = ((Article)entity);
				yield return "a" + entity.Id;
			 
			}
			else if (type == typeof(ArticleAlbum))
			{
				yield return "a" + ((ArticleAlbum)entity).ArticleId;
			}
			else if (type == typeof(ArticleSpecificationAttribute))
			{
				yield return "a" + ((ArticleSpecificationAttribute)entity).ArticleId;
			}
			else if (type == typeof(SpecificationAttributeOption))
			{
				foreach (var attr in ((SpecificationAttributeOption)entity).ArticleSpecificationAttributes)
				{
					yield return "a" + attr.ArticleId;
				}
			}
			else if (type == typeof(ProductVariantAttribute))
			{
				yield return "a" + ((ProductVariantAttribute)entity).ArticleId;
			}
			else if (type == typeof(ProductVariantAttributeValue))
			{
				yield return "a" + ((ProductVariantAttributeValue)entity).ProductVariantAttribute.ArticleId;
			}
			else if (type == typeof(ProductVariantAttributeCombination))
			{
				yield return "a" + ((ProductVariantAttributeCombination)entity).ArticleId;
			}			 
			else if (type == typeof(RelatedArticle))
			{
				yield return "a" + ((RelatedArticle)entity).ArticleId1;
				yield return "a" + ((RelatedArticle)entity).ArticleId2;
			}
			else if (type == typeof(ArticleCategory))
			{
				yield return "c" + ((ArticleCategory)entity).Id;
			}
			else if (type == typeof(Topic))
			{
				var topic = ((Topic)entity);
				if (!topic.RenderAsWidget)
				{
					yield return "t" + topic.Id;
				}
			}
		}

		public IEnumerable<string> GetAllCacheControlTags()
		{
			var tags = _entities
				.Where(x => x.Id > 0)
				.SelectMany(x => GetCacheControlTagsFor(x))
				.Where(x => x != null)
				.Distinct()
				.ToArray();

			return tags;
		}
	}
}

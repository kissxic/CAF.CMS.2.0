using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Directory;
using System;
using System.Runtime.Serialization;
namespace CAF.Infrastructure.Core.Domain.Common
{
	[DataContract]
	public class Address : BaseEntity, ICloneable
    {
        /// <summary>
        /// Gets or sets the first name
        /// </summary>
		[DataMember]
		public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name
        /// </summary>
		[DataMember]
		public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email
        /// </summary>
		[DataMember]
		public string Email { get; set; }

        /// <summary>
        /// Gets or sets the company
        /// </summary>
		[DataMember]
		public string Company { get; set; }

        /// <summary>
        /// Gets or sets the country identifier
        /// </summary>
		[DataMember]
		public int? CountryId { get; set; }

        /// <summary>
        /// Gets or sets the state/province identifier
        /// </summary>
		[DataMember]
		public int? StateProvinceId { get; set; }

        /// <summary>
        /// Gets or sets the City identifier
        /// </summary>
        [DataMember]
        public int? CityId { get; set; }
        /// <summary>
        /// Gets or sets the District identifier
        /// </summary>
        [DataMember]
        public int? DistrictId { get; set; }
        /// <summary>
        /// Gets or sets the address 1
        /// </summary>
		[DataMember]
		public string Address1 { get; set; }

        /// <summary>
        /// Gets or sets the address 2
        /// </summary>
		[DataMember]
		public string Address2 { get; set; }

        /// <summary>
        /// Gets or sets the zip/postal code
        /// </summary>
		[DataMember]
		public string ZipPostalCode { get; set; }

        /// <summary>
        /// Gets or sets the phone number
        /// </summary>
		[DataMember]
		public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the fax number
        /// </summary>
		[DataMember]
		public string FaxNumber { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
		[DataMember]
		public DateTime CreatedOnUtc { get; set; }
        
        /// <summary>
        /// Gets or sets the country
        /// </summary>
		[DataMember]
		public virtual Country Country { get; set; }

        /// <summary>
        /// Gets or sets the state/province
        /// </summary>
		[DataMember]
		public virtual StateProvince StateProvince { get; set; }


        /// Gets or sets the City
        /// </summary>
        [DataMember]
        public virtual City City { get; set; }
        /// Gets or sets the District
        /// </summary>
        [DataMember]
        public virtual District District { get; set; }

        public object Clone()
        {
            var addr = new Address()
            {
                FirstName = this.FirstName,
                LastName = this.LastName,
                Email = this.Email,
                Company = this.Company,
                Country = this.Country,
                CountryId = this.CountryId,
                StateProvince = this.StateProvince,
                StateProvinceId = this.StateProvinceId,
                City = this.City,
                CityId = this.CityId,
                District = this.District,
                DistrictId = this.DistrictId,
                Address1 = this.Address1,
                Address2 = this.Address2,
                ZipPostalCode = this.ZipPostalCode,
                PhoneNumber = this.PhoneNumber,
                FaxNumber = this.FaxNumber,
                CreatedOnUtc = this.CreatedOnUtc,
            };
            return addr;
        }
    }
}

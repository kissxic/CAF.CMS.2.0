﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web.Hosting;
using CAF.Infrastructure.Data.Setup;


namespace CAF.Infrastructure.Data
{
	public class SqlServerDataProvider : IEfDataProvider
    {
        /// <summary>
        /// Get connection factory
        /// </summary>
        /// <returns>Connection factory</returns>
        public virtual IDbConnectionFactory GetConnectionFactory()
        {
            return new SqlConnectionFactory();
        }

        /// <summary>
        /// A value indicating whether this data provider supports stored procedures
        /// </summary>
        public bool StoredProceduresSupported
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a support database parameter object (used by stored procedures)
        /// </summary>
        /// <returns>Parameter</returns>
        public DbParameter GetParameter()
        {
            return new SqlParameter();
        }

		public string ProviderInvariantName
		{
			get { return "System.Data.SqlClient"; }
		}


        /// <summary>
        /// A value indicating whether this data provider supports backup
        /// </summary>
        public virtual bool BackupSupported
        {
            get { return true; }
        }
    }
}

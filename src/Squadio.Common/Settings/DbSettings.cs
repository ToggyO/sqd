using System;
using System.Collections.Generic;
using System.Text;

namespace Squadio.Common.Settings
{
    public class DbSettings
    {
        public string PostgresConnectionString =>
            $"Host={DB_HOST};Port={DB_PORT};Username={DB_USER};Password={DB_PASSWORD};Database={DB_NAME};";

        public string DB_HOST { set; get; }
        public string DB_PORT { set; get; }
        public string DB_USER { set; get; }
        public string DB_PASSWORD { set; get; }
        public string DB_NAME { set; get; }
    }
}

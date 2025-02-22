using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Dapper;

namespace Beatecho.DAL
{
    public class DbHelper
    {
        public static string ConnString = @"Server=localhost;Port=5432;User id=postgres;Password=178982az;Database=beatecho";
    }
}

using Razbor.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public static class DatabaseExtensions
    {
        public static async Task AddTable<TTable>(this DatabaseContext context, TTable table)
            where TTable : class
        {
            await context.AddAsync(table);
            await context.SaveChangesAsync();
        }
    }
}

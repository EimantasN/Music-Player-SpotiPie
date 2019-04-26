using Database;
using Models.BackEnd;
using System;
using System.Threading.Tasks;

namespace Service
{
    public class ErrorService : IErrorService
    {
        private readonly SpotyPieIDbContext _ctx;

        public ErrorService(SpotyPieIDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<Error> ReportAsync(string msg, string method)
        {
            try
            {
                Error error = new Error(msg, method);
                await _ctx.Errors.AddAsync(error);
                await _ctx.SaveChangesAsync();
                return error;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}

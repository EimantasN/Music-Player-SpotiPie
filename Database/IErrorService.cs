using Models.BackEnd;
using System.Threading.Tasks;

namespace Database
{
    public interface IErrorService
    {
        Task<Error> ReportAsync(string msg, string method);
    }
}

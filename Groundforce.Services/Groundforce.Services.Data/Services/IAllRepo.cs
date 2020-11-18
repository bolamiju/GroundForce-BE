using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public interface IAllRepo
    {
        Task<T> Add<T>(T param);

    }
}

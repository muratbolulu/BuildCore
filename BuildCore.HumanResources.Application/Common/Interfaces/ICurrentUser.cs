using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildCore.HumanResources.Application.Common.Interfaces
{
    public interface ICurrentUser
    {
        //string? → sistem kullanıcı olmadan da çalışabilsin(background job, migration vs.)
        string? UserId { get; }
    }
}

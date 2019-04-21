using Mobile_Api.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mobile_Api.Interfaces
{
    public interface IBaseInterface
    {
        int GetId();

        void SetModelType(RvType type);
    }
}

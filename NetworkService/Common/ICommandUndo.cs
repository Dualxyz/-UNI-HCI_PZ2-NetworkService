﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Common
{
    public interface ICommandUndo
    {
        void UnExecute();
    }
}

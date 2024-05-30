using NetworkService.Common;
using System.Collections.Generic;

namespace NetworkService.Model
{
    public class StaticData
    {
        public static Stack<ICommandUndo> myICommands = new Stack<ICommandUndo>();
    }
}

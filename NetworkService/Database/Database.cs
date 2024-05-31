using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Database
{
    public class Database
    {
        public static ObservableCollection<T4_Entity> Entities { get; set; } =  new ObservableCollection<T4_Entity>();
    }
}

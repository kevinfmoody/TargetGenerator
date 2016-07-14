using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetGenerator
{
    class ArrivalProcedureStore
    {
        private static readonly ArrivalProcedureStore instance = new ArrivalProcedureStore();
        public Dictionary<string, ArrivalProcedure> arrivalProcedures { get; set; }

        private ArrivalProcedureStore()
        {
            this.arrivalProcedures = new Dictionary<string, ArrivalProcedure>();
        }

        public static ArrivalProcedureStore Instance { get { return instance; } }
    }
}

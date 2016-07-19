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

        public ArrivalProcedure arrivalProcedure(string identifier)
        {
            ArrivalProcedure procedure = null;
            if (arrivalProcedures.ContainsKey(identifier))
            {
                procedure = arrivalProcedures[identifier];
            }
            else
            {
                string identifierWithoutNumber = identifier.Substring(0, identifier.Length - 1);
                for (int i = 9; i >= 0; i--)
                {
                    if (arrivalProcedures.ContainsKey(identifierWithoutNumber + i))
                    {
                        procedure = arrivalProcedures[identifierWithoutNumber + i];
                        break;
                    }
                }
            }
            return procedure;
        }
    }
}

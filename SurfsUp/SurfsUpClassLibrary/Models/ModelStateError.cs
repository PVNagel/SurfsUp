using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfsUpClassLibrary.Models
{
    [Serializable]
    public class ModelStateError
    {
        public string Key { get; set; }
        public string ErrorMessage { get; set; }
    }
}

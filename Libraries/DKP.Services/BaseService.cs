using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using log4net;

namespace DKP.Services
{
    public class BaseService
    {
        public ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}

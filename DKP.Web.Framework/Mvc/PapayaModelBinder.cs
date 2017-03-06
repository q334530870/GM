using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace DKP.Web.Framework.Mvc
{
    public class PapayaModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var model = base.BindModel(controllerContext, bindingContext);
            if (model is BasePapayaModel)
            {
                ((BasePapayaModel)model).BindModel(controllerContext, bindingContext);
            }
            return model;
        }
    }
}

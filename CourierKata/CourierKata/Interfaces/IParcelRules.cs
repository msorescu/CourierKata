using CourierKata.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourierKata.Interfaces
{
    interface IParcelRules
    {
        double OverweightChargeRule(ShippingRate shippingRate);
    }
}

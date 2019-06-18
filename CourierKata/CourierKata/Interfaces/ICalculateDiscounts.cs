using CourierKata.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourierKata.Interfaces
{
    interface ICalculateDiscounts
    {
        List<ParcelDiscounts> GetDiscountForParcelCode(int discountSe, ParcelCode code= ParcelCode.Small);       
    }
}

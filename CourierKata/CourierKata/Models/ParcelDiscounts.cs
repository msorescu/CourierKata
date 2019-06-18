using System;
using System.Collections.Generic;
using System.Text;

namespace CourierKata.Models
{
    public class ParcelDiscounts
    {
        public double Value { get; set; }
        public Parcel DiscountedParcel { get; set; }
        public List<Parcel> Parcels { get; set; }
    }
}

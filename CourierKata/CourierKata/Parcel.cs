using CourierKata.Interfaces;
using CourierKata.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourierKata
{  
    public class Parcel : IParcelRules
    {

        double _weight; 

        public Guid Id { get; set; }
        public string Name { get; set; }
        public ParcelCode Code { get; set; }
        public double ShippingCharge { get; private set; }
        public double OverweightCharge { get; private set; }
        public double TotalPrice { get; private set; }

        public Parcel()
        { }

        public Parcel(double width, double height, double weight = 0)
        {
            _weight = weight;
            SetParcelType(width, height);
        }


        public void SetShippingPrice(Dictionary<ParcelCode, ShippingRate> shippingRatesByCode)
        {
            if (!shippingRatesByCode.ContainsKey(Code))
            {
                throw new Exception("Code " + Code + " is not available in the price dictionary");
            }

            ShippingCharge = shippingRatesByCode[Code].Charge;
            IParcelRules parcelRules = new Parcel();
            OverweightCharge = parcelRules.OverweightChargeRule(shippingRatesByCode[Code]);
            TotalPrice = ShippingCharge + OverweightCharge;
        }

        

        private double GetLargestDimension(double width, double height)
        {
            return width > height ? width : height;
        }

        private void SetParcelType(double width, double height)
        {
            if (_weight >= 50)
            {
                Name = "Heavy";
                Code = ParcelCode.Heavy;
                return;
            }

            var largestDimensionCm = GetLargestDimension(width,height);

            if (largestDimensionCm < 10)
            {
                Name = "Small";
                Code = ParcelCode.Small;
            }
            else if (largestDimensionCm < 50)
            {
                Name = "Medium";
                Code = ParcelCode.Medium;
            }
            else if (largestDimensionCm < 100)
            {
                Name = "Large";
                Code = ParcelCode.Large;
            }
            else
            {
                Name = "XL";
                Code = ParcelCode.XL;
            }
        }

        double IParcelRules.OverweightChargeRule(ShippingRate shippingRate)
        {
            if (_weight > shippingRate.WeightLimitKg)
            {
                return (_weight - shippingRate.WeightLimitKg) * shippingRate.OverweightChargePerKg;
            }

            return 0;
        }
    }
}

using CourierKata.Interfaces;
using CourierKata.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourierKata
{
    public class Order : ICalculateDiscounts
    {
        public bool Speedy { get; set; } = false;
        public double TotalPrice { get; private set; } = 0.0;
        public double TotalDiscount { get; private set; } = 0.0;
        public double SpeedyShippingPrice { get; private set; } = 0.0;
        public List<ParcelDiscounts> Discounts { get; set; }
        public StringBuilder PriceDetails { get; }

        private readonly ConcurrentDictionary<Guid, Parcel> _parcels = new ConcurrentDictionary<Guid, Parcel>();
        private Dictionary<ParcelCode, ShippingRate> ShippingRatesByCode { get; set; }


        public Order()
        { PriceDetails = new StringBuilder(); }

        public Order(List<Parcel> parcels, Dictionary<ParcelCode, ShippingRate> shippingRatesByCode, bool speedy=false):this()
        {
            ShippingRatesByCode = shippingRatesByCode;
            Speedy = speedy;
            AddRangeParcels(parcels);
            CalculatePrice();
        }


        public void AddParcel (Parcel parcel)
        {
            parcel.Id = Guid.NewGuid();
            _parcels[parcel.Id] = parcel;
        }

        public void AddRangeParcels(IEnumerable<Parcel> parcels)
        {
            foreach (var parcel in parcels)
            {
                AddParcel(parcel);
            }
        }


        public IEnumerable<Parcel> GetAllParcels() => _parcels.Values;


        public Parcel RemoveParcel(Parcel parcel)
        {
            Parcel removed;
            _parcels.TryRemove(parcel.Id, out removed);
            return removed;
        }


        public string PrintOrderPrice()
        {
            if (Speedy)
            {
                PriceDetails.AppendLine("=========== SpeedyShippingDelivery ===========");
                PriceDetails.AppendFormat($"Order price is {TotalPrice}\n\n");
                PriceDetails.AppendFormat("Order speedy price is {SpeedyShippingPrice}\n\n");
            }
            else
            {
                PriceDetails.AppendLine("=========== RegularShippingDelivery ===========");
                PriceDetails.AppendFormat($"Order price is {TotalPrice}\n\n");
            }
            PriceDetails.AppendFormat($"Total Discounts : {TotalDiscount}\n\n");

            return PriceDetails.ToString();
        }

        private void CalculatePrice()
        {
            var totalPrice =0.0;
            foreach (var parcel in _parcels.Values)
            {
                parcel.SetShippingPrice(ShippingRatesByCode);
                PriceDetails.AppendFormat($"{parcel.Name} parcel price : {parcel.TotalPrice}.\n");
                TotalPrice += parcel.TotalPrice;
            }

            ICalculateDiscounts orderDiscount = new object() as ICalculateDiscounts;
                
            Discounts = CalculateDiscounts(orderDiscount);

            TotalDiscount = Discounts.Sum(discount => discount.Value);

            TotalPrice -= TotalDiscount;

            if (Speedy)
            {
                SpeedyShippingPrice = totalPrice;
                TotalPrice += SpeedyShippingPrice;
            }
            
        }

       

        bool ExistParcelsWithMixedCode() => _parcels.Values.GroupBy(parcel => parcel.Code).Count() > 1;
        

        List<ParcelDiscounts> CalculateDiscounts(ICalculateDiscounts orderDiscountRules)
        {
            var discounts = new List<ParcelDiscounts>();
            //small parcel mania
            discounts.AddRange(orderDiscountRules.GetDiscountForParcelCode(4, ParcelCode.Small));
            //medium parcel mania
            discounts.AddRange(orderDiscountRules.GetDiscountForParcelCode(3, ParcelCode.Medium));

            if (ExistParcelsWithMixedCode())
            {
                //mixed parcel mania
                discounts.AddRange(orderDiscountRules.GetDiscountForParcelCode(5));
            }
            return discounts;
        }

        List<ParcelDiscounts> ICalculateDiscounts.GetDiscountForParcelCode(int discountSet, ParcelCode code)
        {
            var discounts = new List<ParcelDiscounts>();

            var groupedParcels = _parcels.Values
                .Where(parcel => parcel.Code == code)
                .OrderBy(parcel => parcel.TotalPrice)
                .ToList();

            var numberOfRequiredDiscounts = (int)Math.Floor((double)groupedParcels.Count / discountSet);

            var offset = 0;

            while (discounts.Count < numberOfRequiredDiscounts)
            {
                var discount = new ParcelDiscounts
                {
                    DiscountedParcel = groupedParcels[offset],
                    Parcels = groupedParcels.GetRange(offset, discountSet),
                    Value = groupedParcels[offset].TotalPrice
                };

                discounts.Add(discount);

                offset = offset + discountSet;
            }

            return discounts;
        }

    }
}

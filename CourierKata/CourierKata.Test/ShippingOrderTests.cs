using CourierKata.Models;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Sdk;

namespace CourierKata.Test
{
    public class ShippingOrderTests
    {
        static readonly Dictionary<ParcelCode, ShippingRate> shippingRatesByCode = new Dictionary<ParcelCode, ShippingRate>
        {
            {ParcelCode.Small, new ShippingRate(3, 1, 2)},
            {ParcelCode.Medium, new ShippingRate(8, 3, 2)},
            {ParcelCode.Large, new ShippingRate(15, 6, 2)},
            {ParcelCode.XL, new ShippingRate(25, 10, 2)},
            {ParcelCode.Heavy, new ShippingRate(50, 50, 1)}
        };

        public class CourierKataTest1
        {
            [Fact]
            public void Test_Total_The_Price_Of_An_Order()
            {
                var expectedCost = 51.00;
 
                var parcels = new List<Parcel>
                {
                    new Parcel(5, 9),
                    new Parcel(11, 49),
                    new Parcel(5, 51),
                    new Parcel(101, 9),

                };
                 
                var underTest = new Order(parcels, shippingRatesByCode);
                Assert.Equal(expectedCost, underTest.TotalPrice);
            }
        }

        public class CourierKataTest2
        {
            [Fact]
            public void Test_When_Double_The_Price_Of_An_Order_And_When_Speedy_Shipping()
            {
                var expectedCost = 102.00;

                var parcels = new List<Parcel>
                {
                    new Parcel(5, 9),
                    new Parcel(11, 49),
                    new Parcel(5, 51),
                    new Parcel(101, 9),

                };

                var underTest = new Order(parcels,shippingRatesByCode, true);
                Assert.Equal(expectedCost, underTest.TotalPrice);
            }

            
        }

        public class CourierKataTest3
        {
            [Fact]
            public void Test_When_Charge_An_Extra_2kg_For_A_Package()
            {
                var expectedCost = 59.00;

                var parcels = new List<Parcel>
                {
                    new Parcel(5, 9, 2),
                    new Parcel(11, 49, 4),
                    new Parcel(5, 51, 7),
                    new Parcel(101, 9, 11),

                };

                var underTest = new Order(parcels, shippingRatesByCode);
                Assert.Equal(expectedCost, underTest.TotalPrice);
            }
        }
        
    }
}

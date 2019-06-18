# CourierKata

You work for a courier company and have been tasked with creating a code library to
calculate the cost of sending an order of parcels.
* The API for the library should be programmatic. There is no need to implement a CLI,
HTTP, or any other transport layer
* Try not to peek ahead at future steps and commit your working as you go
* Input can be in any form you choose
* Output should be a collection of items with their individual cost and type, as well as
total cost
* In all circumstances the cheapest option for sending each parcel should be selected

## Implementation Steps

1. The initial implementation just needs to take into account a parcel's size. For each size
type there is a fixed delivery cost
* Small parcel: all dimensions < 10cm. Cost $3
* Medium parcel: all dimensions < 50cm. Cost $8
* Large parcel: all dimensions < 100cm. Cost $15
* XL parcel: any dimension >= 100cm. Cost $25

2. Thanks to logistics improvements we can deliver parcels faster. This means we can
charge more money. Speedy shipping can be selected by the user to take advantage of our
improvements.
* This doubles the cost of the entire order
* Speedy shipping should be listed as a separate item in the output, with its associated
cost
* Speedy shipping should not impact the price of individual parcels, i.e. their individual
cost should remain the same as it was before

3. There have been complaints from delivery drivers that people are taking advantage of our
dimension only shipping costs. A new weight limit has been added for each parcel type, over
which a charge per kg of weight applies
+$2/kg over weight limit for parcel size:
Small parcel: 1kg
* Medium parcel: 3kg
* Large parcel: 6kg
* XL parcel: 10kg

4. Some of the extra weight charges for certain goods were excessive. A new parcel type
has been added to try and address overweight parcels
Heavy parcel (limit 50kg), $50. +$1/kg over

5. In order to award those who send multiple parcels, special discounts have been
introduced.
* Small parcel mania! Every 4th small parcel in an order is free!
* Medium parcel mania! Every 3rd medium parcel in an order is free!
* Mixed parcel mania! Every 5th parcel in an order is free!
* Each parcel can only be used in a discount once
* Within each discount, the cheapest parcel is the free one
* The combination of discounts which saves the most money should be selected every
time

Example:
6x medium parcel. 3 x $8, 3x $10. 1st discount should include all 3 $8 parcels and save $8.
2nd discount should include all 3 $10 parcels and save $10.
* Just like speedy shipping, discounts should be listed as a separate item in the output,
with associated saving, e.g. "-2"
* Discounts should not impact the price of individual parcels, i.e. their individual cost
should remain the same as it was before
* Speedy shipping applies after discounts are taken into account

## The Solution
The solution offers up two projects:
* CourierKata - the main library
* CourierKata.Test - xUnit tests for the library

### TL;DR Example of Calculating an Order
```cs
var shippingRatesByCode = new Dictionary<ParcelCode, ShippingRate>
    {
        {ParcelCode.Small, new ShippingRate(3, 1, 2)},
        {ParcelCode.Medium, new ShippingRate(8, 3, 2)},
        {ParcelCode.Large, new ShippingRate(15, 6, 2)},
        {ParcelCode.XL, new ShippingRate(25, 10, 2)},
        {ParcelCode.Heavy, new ShippingRate(50, 50, 1)}
    }

var parcels = new List<Parcel>
    {
        new Parcel(5, 9, 2),
        new Parcel(11, 49, 4),
        new Parcel(5, 51, 7),
        new Parcel(101, 9, 11),
    };

var order = new ShippingOrder(parcels, shippingRatesByCode);
```

### ShippingOrder
The main entrypoint into the solution is the `ShippingOrder` class. To calculate the price of an order of an order you simply need to provide the list of parcels and shipping rate dictionary keyed by by parcel type on the constructor.

```cs
var order = new ShippingOrder(parcels, shippingRatesByParcelCode);
```

Simply constructing this order will calculate everything for you, from parcel types to discounts to the total price.

This interface allows you to retrieve a list of parcels and shipping codes elsewhere in your application logic and allows the library to operate without side effects (eg I/O operations).

Speedy shipping can be enabled by passing true as the final parameter on the constructor:

```cs
var speedyOrder = new ShippingOrder(parcels, shippingRatesByParcelCode, true);
```

### Parcels
Parcels are constructed by specifying the width, height and weight on the constructor. The parcel will determine its type based on the specified values.

```cs
var parcel = new Parcel(widthCm, heightCm, weightKg);
```

### ShippingRate
This class represents a shipping rate for a given parcel type.

It is constructed with the base charge for the type, a weight limit and the charge per extra kg over the limit.

```cs
var rate = ShippingRate(charge, weightLimitKg, overweightChargePerKg);
```

For constructing an order, you will supply a dictionary of shipping rates keyed by the ParcelCode enum. This allows for shipping rate changes to flex without requiring code changes.

### Tests
Unit tests for the project are available to exercise the code to confirm the functionality required by the kata, however they are not production ready tests (to be discussed later).

You will find the tests in `CourierKata.Test`.

## Review and lessons learned
Overall carrying out the Kata was a great experience for the author - highlighting clearly that the author could do with some more kata practice!

Some success was achieved, however the solution is lacking on many fronts and there are some good lessons to learn.

### Deductions / Assumptions
Working in a vacuum, some things in the brief would have required a quick conversation to ensure the intent was honored in implementation.

Due to the nature of the task however the following assumptions / decisions were made:
* Prices are always assumed to be `USD`
  * This is hard coded into the order class and would be all that was required to be changed to support other currencies
  * Supplying different ShippingRates by currency would be the correct way to do currency conversions
* Heavy Parcels
  * It isn't clear exactly what makes a parcel a Heavy Parcel
  * The assumption was made that any parcel weighing at least 50kg would be classed as a heavy parcel
* Discount grouping
  * This section was a bit confusing and took some thinking
  * By looking at the example it was deduced that each discount should contain a group of parcels making up the discount
  * These parcels were then to be made up of grouped by cost (ascending)
  * This does mean that the cheapest packages will not always be discounted, as per the example: 8,8,8,10,10,10 would result in one discount of 8 and one of 10 - when really the cheapest packages are 8 and 8

### Completion
At the two hour mark exercises 1-4 were 'complete' when defined as 'the code works'. A specific commit marks this point: https://github.com/jjmschofield/CourierKata/commit/49e22bb1002115623a1b56850cf1a8e63280e1f6

Following this some minor reactors were implemented (to do with readability rather then extensibility) and a further hour was dedicated to answering the final (and most interesting exercise).

Of the final exercise the following was achieved:
* Small / medium parcel mania
  * With parcels being grouped up and used in a discount once only
  * With discount groups being based on ranges of parcels ordered by price
  * Original prices not affected
* Speedy shipping applied after discounts take affect

Of the final exercise, the following wasn't attempted:
* Mixed package mania
  * Simple really, a further method which discounts parcels already included in a discount and then finds groups of 5 as with the other parcel manias
* Cheapest calculation
  * By ordering small -> medium -> mixed there is some attempt to find the cheapest combination of discounts
  * However there is lurking complexity here, with the overweight charge as it is - I suspect that there are edge cases where discounting a small/medium parcel combination would result in packages that would be more effectively discounted in mixed parcel mania
  * Further test cases and thinking would be required to make this a guaranteed solution

With one exercise not started there are some lessons to learn:
* Initially standard unit test practice was being implemented, with each class tested in isolation
  * This proved to costly and was ejected from in preference of just testing against the reqs of the kata
  * This sped up dev but has hurt the test quality dramatically
* Project structure has been approached from the point of view of well composed production code
  * A quicker, dirtier style may have reduced overhead and allowed to reach higher completion within the time limit

### Code Quality
The code structure for the project is reasonable and has the following benefits:
* Simple interface for integration
* Extended easily
* Pretty readable / clear
* Very testable
* Good encapsulation
* Easy to inject inputs independent of application code

However:
* Driving everything through constructors feels a little bit mucky
  * Calculation seems a bit magical and is likely opaque without diving into the source
  * Perhaps some other pattern (Factory / Builder) or a more functional approach might make things more transparent
* Class names
  * These are very generic and may collide with consuming services, maybe increasing the cognitive load for a consumer
  * A little bit of thought and experimentation would improve this
* Single responsibility and general class structure
  * There is probably some improvements given some thought to make the code a bit cleaner
  * Should parcels by calculating prices and types - or simply just be structs holding their dimensions and weight?
    * The idea here being to focus on a a series of more functional helpers to derive calculations
    * This would reduce the amount of mutation and allow for a more declarative consumption pattern, but maybe at the cost of encapsulation?
  * Should ShippingOrder be broken down into smaller chunks?
    * This class seems to provide a single front to a lot of complexity - but also does a lot of the work itself
    * Some better thinking here might protect this class from becoming a scary 2k liner in 4 years
  * Should calculations be set into the public getters rather then as stored values on objects?
    * This would reduce some of the mutation going on and issues related to state being altered out of band

### Test Coverage
The good news with the tests is that they work and cover ~98% of the solution.

The bad news is that they:
1. Are not testing individual system components but rather everything at once
1. Have poorly defined test cases (frequently testing multiple things, especially kata five)
1. Could benefit from having more declarative inputs
1. Cover lines well but probably don't cover all behaviors / combinations of behaviors

In sum, whilst confident they are testing functionality correctly, they would be a maintenance problem - as failures will not be transparent. A refactor on these to address the above would be very welcome.

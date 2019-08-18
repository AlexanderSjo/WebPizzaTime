using System;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using WebPizzaTime;

namespace WebPizzaTimeTests
{
    public class Tests
    {
        // No more time for more tests...
        
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestGetPossibleTimes_AllPojectionOk()
        {
            // TestData
            var testData =
                "{\"ScheduleResult\":{\"Schedules\":[{\"ContractTimeMinutes\":480,\"Date\":\"\\/Date(1450051200000+0000)\\/\",\"IsFullDayAbsence\":false,\"Name\":\"Daniel Billsus\",\"PersonId\":\"4fd900ad-2b33-469c-87ac-9b5e015b2564\",\"Projection\":[{\"Color\":\"#FFFF00\",\"Description\":\"Ok\",\"Start\":\"\\/Date(1450094400000+0000)\\/\",\"minutes\":50},{\"Color\":\"#1E90FF\",\"Description\":\"Ok\",\"Start\":\"\\/Date(1450097400000+0000)\\/\",\"minutes\":130},{\"Color\":\"#FF0000\",\"Description\":\"Ok\",\"Start\":\"\\/Date(1450105200000+0000)\\/\",\"minutes\":15}]}]}}";
            var dataObj = JObject.Parse(testData);
            var rootObject = new RootObject(dataObj);
            
            //Test
            var result = Controller.GetPossibleTimes(rootObject);
            
            //Assert
            Assert.AreEqual(DateTimeOffset.Parse("2015-12-14 13:00:00", CultureInfo.InvariantCulture), result.Item2.First().First());
            Assert.AreEqual(DateTimeOffset.Parse("2015-12-14 16:00:00", CultureInfo.InvariantCulture), result.Item2.First().Last());
            Assert.AreEqual(13, result.Item2.First().Count);
            Assert.Pass();
        }
        [Test]
        public void TestGetPossibleTimes_FirstPojectionNotOK()
        {
            //TestData
            var testData =
                "{\"ScheduleResult\":{\"Schedules\":[{\"ContractTimeMinutes\":480,\"Date\":\"\\/Date(1450051200000+0000)\\/\",\"IsFullDayAbsence\":false,\"Name\":\"Daniel Billsus\",\"PersonId\":\"4fd900ad-2b33-469c-87ac-9b5e015b2564\",\"Projection\":[{\"Color\":\"#FFFF00\",\"Description\":\"Lunch\",\"Start\":\"\\/Date(1450094400000+0000)\\/\",\"minutes\":60},{\"Color\":\"#1E90FF\",\"Description\":\"Ok\",\"Start\":\"\\/Date(1450098000000+0000)\\/\",\"minutes\":120},{\"Color\":\"#FF0000\",\"Description\":\"Ok\",\"Start\":\"\\/Date(1450105200000+0000)\\/\",\"minutes\":15}]}]}}";
            var dataObj = JObject.Parse(testData);
            var rootObject = new RootObject(dataObj);
            
            // PreAssert testData: first Projection Not Ok = Lunch
            Assert.AreEqual("Lunch", rootObject.ScheduleResult.Schedules[0].Projection[0].Description);
            
            //Test
            var result = Controller.GetPossibleTimes(rootObject);
            
            //Assert
            Assert.AreEqual(DateTimeOffset.Parse("2015-12-14 14:00:00", CultureInfo.InvariantCulture), result.Item2.First().First());
            Assert.AreEqual(DateTimeOffset.Parse("2015-12-14 16:00:00", CultureInfo.InvariantCulture), result.Item2.First().Last());
            Assert.AreEqual(9, result.Item2.First().Count);
            Assert.Pass();
        }
        
        [Test]
        public void TestGetPossibleTimes_SecondPojectionNotOK()
        {
            //TestData
            var testData =
                "{\"ScheduleResult\":{\"Schedules\":[{\"ContractTimeMinutes\":480,\"Date\":\"\\/Date(1450051200000+0000)\\/\",\"IsFullDayAbsence\":false,\"Name\":\"Daniel Billsus\",\"PersonId\":\"4fd900ad-2b33-469c-87ac-9b5e015b2564\",\"Projection\":[{\"Color\":\"#FFFF00\",\"Description\":\"Ok\",\"Start\":\"\\/Date(1450094400000+0000)\\/\",\"minutes\":60},{\"Color\":\"#1E90FF\",\"Description\":\"Lunch\",\"Start\":\"\\/Date(1450098000000+0000)\\/\",\"minutes\":120},{\"Color\":\"#FF0000\",\"Description\":\"Ok\",\"Start\":\"\\/Date(1450105200000+0000)\\/\",\"minutes\":15}]}]}}";
            var dataObj = JObject.Parse(testData);
            var rootObject = new RootObject(dataObj);
            
            // PreAssert testData: second Projection Not Ok = Lunch
            Assert.AreEqual("Lunch", rootObject.ScheduleResult.Schedules[0].Projection[1].Description);
            
            //Test
            var result = Controller.GetPossibleTimes(rootObject);
            
            //Assert
            Assert.AreEqual(DateTimeOffset.Parse("2015-12-14 13:00:00", CultureInfo.InvariantCulture), result.Item2.First().First());
            Assert.AreEqual(DateTimeOffset.Parse("2015-12-14 16:00:00", CultureInfo.InvariantCulture), result.Item2.First().Last());
            Assert.AreEqual(5, result.Item2.First().Count);
            Assert.Pass();
        }
        
        [Test]
        public void TestGetPossibleTimes_LastPojectionNotOK()
        {
            //TestData
            var testData =
                "{\"ScheduleResult\":{\"Schedules\":[{\"ContractTimeMinutes\":480,\"Date\":\"\\/Date(1450051200000+0000)\\/\",\"IsFullDayAbsence\":false,\"Name\":\"Daniel Billsus\",\"PersonId\":\"4fd900ad-2b33-469c-87ac-9b5e015b2564\",\"Projection\":[{\"Color\":\"#FFFF00\",\"Description\":\"Ok\",\"Start\":\"\\/Date(1450094400000+0000)\\/\",\"minutes\":60},{\"Color\":\"#1E90FF\",\"Description\":\"Ok\",\"Start\":\"\\/Date(1450098000000+0000)\\/\",\"minutes\":120},{\"Color\":\"#FF0000\",\"Description\":\"Lunch\",\"Start\":\"\\/Date(1450105200000+0000)\\/\",\"minutes\":15}]}]}}";
            var dataObj = JObject.Parse(testData);
            var rootObject = new RootObject(dataObj);
            
            // PreAssert testData: last Projection Not Ok = Lunch
            Assert.AreEqual("Lunch", rootObject.ScheduleResult.Schedules[0].Projection[2].Description);
            
            //Test
            var result = Controller.GetPossibleTimes(rootObject);
            
            //Assert
            Assert.AreEqual(DateTimeOffset.Parse("2015-12-14 13:00:00", CultureInfo.InvariantCulture), result.Item2.First().First());
            Assert.AreEqual(DateTimeOffset.Parse("2015-12-14 15:45:00", CultureInfo.InvariantCulture), result.Item2.First().Last());
            Assert.AreEqual(12, result.Item2.First().Count);
            Assert.Pass();
        }
        
        [Test]
        public void TestGetPossibleTimes_OnlyLastPojectionOK()
        {
            //TestData
            var testData =
                "{\"ScheduleResult\":{\"Schedules\":[{\"ContractTimeMinutes\":480,\"Date\":\"\\/Date(1450051200000+0000)\\/\",\"IsFullDayAbsence\":false,\"Name\":\"Daniel Billsus\",\"PersonId\":\"4fd900ad-2b33-469c-87ac-9b5e015b2564\",\"Projection\":[{\"Color\":\"#FFFF00\",\"Description\":\"Lunch\",\"Start\":\"\\/Date(1450094400000+0000)\\/\",\"minutes\":60},{\"Color\":\"#1E90FF\",\"Description\":\"Lunch\",\"Start\":\"\\/Date(1450098000000+0000)\\/\",\"minutes\":120},{\"Color\":\"#FF0000\",\"Description\":\"Ok\",\"Start\":\"\\/Date(1450105200000+0000)\\/\",\"minutes\":15}]}]}}";
            var dataObj = JObject.Parse(testData);
            var rootObject = new RootObject(dataObj);
            
            // PreAssert testData:
            Assert.AreEqual("Lunch", rootObject.ScheduleResult.Schedules[0].Projection[0].Description);
            Assert.AreEqual("Lunch", rootObject.ScheduleResult.Schedules[0].Projection[1].Description);

            //Test
            var result = Controller.GetPossibleTimes(rootObject);
            
            //Assert
            Assert.AreEqual(DateTimeOffset.Parse("2015-12-14 16:00:00", CultureInfo.InvariantCulture), result.Item2.First().First());
            Assert.AreEqual(DateTimeOffset.Parse("2015-12-14 16:00:00", CultureInfo.InvariantCulture), result.Item2.First().Last());
            Assert.AreEqual(1, result.Item2.First().Count);
            Assert.Pass();
        }

        [Test]
        public void TestTimeRoundUp()
        {
            var result = Controller.TimeRoundUp(DateTimeOffset.Parse("2015-12-14 14:50:00"));
            
            Assert.AreEqual(DateTimeOffset.Parse("2015-12-14 15:00:00"), result);
        }
        [Test]
        public void TestTimeRoundUp_NoChange()
        {
            var result = Controller.TimeRoundUp(DateTimeOffset.Parse("2015-12-14 15:00:00"));
            
            Assert.AreEqual(DateTimeOffset.Parse("2015-12-14 15:00:00"), result);
        }
        
        [Test]
        public void TestTimeRoundDown()
        {
            var result = Controller.TimeRoundDown(DateTimeOffset.Parse("2015-12-14 14:50:00"));
            
            Assert.AreEqual(DateTimeOffset.Parse("2015-12-14 14:45:00"), result);
        }
        
        [Test]
        public void TestTimeRoundDown_NoChange()
        {
            var result = Controller.TimeRoundDown(DateTimeOffset.Parse("2015-12-14 15:00:00"));
            
            Assert.AreEqual(DateTimeOffset.Parse("2015-12-14 15:00:00"), result);
        }
    }
}
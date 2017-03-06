using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using iDai360.Scheduler.Task.DataAccess.Service;

namespace iDai360.Scheduler.Test
{
    [TestClass]
    public class SubscriptionServiceTest
    {
        [TestMethod]
        public void TestConfirm()
        {
            SubscriptionService.Confirm();
        }
        
        [TestMethod]
        public void TestConditionalityExpiryCheck()
        {
            SubscriptionService.ConditionalityExpiryCheck();
        }
    }
}

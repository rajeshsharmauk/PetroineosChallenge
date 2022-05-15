using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PetroineosChallenge.Server;
using FluentAssertions;
using Services;

namespace PetroineosChallenge.UnitTests
{
    [TestFixture]
    public class Tests
    {
        private List<PowerTrade> _powerTrades;
        private Processor _processor;        

        [SetUp]
        public void SetUp()
        {
            _processor = new Processor();
            _powerTrades = new List<PowerTrade>();

            PowerTrade powerTrade;
            int numTrades = 2;

            for (int i=1; i <= numTrades; i++)
            {
                powerTrade = PowerTrade.Create(new DateTime(2020, 05, 12), 24);

                powerTrade.Periods[0].Volume = i * 10;
                powerTrade.Periods[1].Volume = i * 20;
                powerTrade.Periods[2].Volume = i * 30;
                powerTrade.Periods[3].Volume = i * 40;
                powerTrade.Periods[4].Volume = i * 50;
                powerTrade.Periods[5].Volume = i * 60;
                powerTrade.Periods[6].Volume = i * 70;
                powerTrade.Periods[7].Volume = i * 80;
                powerTrade.Periods[8].Volume = i * 90;
                powerTrade.Periods[9].Volume = i * 100;
                powerTrade.Periods[10].Volume = i * 110;
                powerTrade.Periods[11].Volume = i * 120;
                powerTrade.Periods[12].Volume = i * 130;
                powerTrade.Periods[13].Volume = i * 140;
                powerTrade.Periods[14].Volume = i * 150;
                powerTrade.Periods[15].Volume = i * 160;
                powerTrade.Periods[16].Volume = i * 170;
                powerTrade.Periods[17].Volume = i * 180;
                powerTrade.Periods[18].Volume = i * 190;
                powerTrade.Periods[19].Volume = i * 200;
                powerTrade.Periods[20].Volume = i * 210;
                powerTrade.Periods[21].Volume = i * 220;
                powerTrade.Periods[22].Volume = i * 230;
                powerTrade.Periods[23].Volume = i * 240;

                _powerTrades.Add(powerTrade);
            }
        }

        [Test]
        public void ListOfPowerTrades_HasCorrectNumberOfTrades()
        {
            int expected = 2;

            _powerTrades.Count().Should().Be(expected);
        }

        [Test]
        public void ATrade_HasCorrectNumberOfTimePeriods()
        {
            int expected = 24;

            int count = _powerTrades.Select(x => x).First().Periods.Count();
            count.Should().Be(expected);
        }

        [Test]
        public void ATimePeriod_HasCorrectTotalVolume()
        {
            string actual = "04:00";
            int expected = 180;

            var totalTimePeriods = _processor.GetTotals(_powerTrades);

            totalTimePeriods
                .Where(x => x.TimePeriod == actual)
                .Select(x => x.Total).Should().Equal(expected);
        }

        [Test]
        public void ATotalVolume_HasCorrectTimePeriod()
        {
            int actual = 360;
            string expected = "10:00";

            var totalTimePeriods = _processor.GetTotals(_powerTrades);

            totalTimePeriods
                .Where(x => x.Total == actual)
                .Select(x => x.TimePeriod).Should().Equal(expected);
        }
    }
}

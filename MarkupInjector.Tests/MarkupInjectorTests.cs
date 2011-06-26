using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Diagnostics;

namespace MarkupInjector.Tests
{
    [TestFixture]
    public class MarkupInjectorTests
    {
        [SetUp]
        public void Setup()
        {
            Settings.HeadListeners.Clear();
            Settings.BodyListeners.Clear();
        }

        [Test]
        public void Should_Change_Settings_When_Body_Event_Is_Subscribed()
        {
            InjectMarkup.InBody(() => "blah");

            Assert.IsTrue(Settings.ShouldInterceptResponse);
            Assert.That(Settings.BodyListeners.Count, Is.EqualTo(1));

            Settings.BodyListeners.Clear();

            Assert.IsFalse(Settings.ShouldInterceptResponse);
            Assert.That(Settings.BodyListeners.Count, Is.EqualTo(0));
        }

        [Test]
        public void Should_Change_Settings_When_Head_Event_Is_Subscribed()
        {
            InjectMarkup.InHead(() => "blah");

            Assert.IsTrue(Settings.ShouldInterceptResponse);
            Assert.That(Settings.HeadListeners.Count, Is.EqualTo(1));

            Settings.HeadListeners.Clear();

            Assert.IsFalse(Settings.ShouldInterceptResponse);
            Assert.That(Settings.HeadListeners.Count, Is.EqualTo(0));
        }

        [Test]
        public void Should_Only_Change_Settings_When_There_Are_No_More_Listeners()
        {
            InjectMarkup.InHead(() => "blah");
            InjectMarkup.InBody(() => "blah");

            Assert.IsTrue(Settings.ShouldInterceptResponse);
            Assert.That(Settings.HeadListeners.Count, Is.EqualTo(1));
            Assert.That(Settings.BodyListeners.Count, Is.EqualTo(1));

            Settings.BodyListeners.Clear();

            Assert.IsTrue(Settings.ShouldInterceptResponse);
            Assert.That(Settings.HeadListeners.Count, Is.EqualTo(1));
            Assert.That(Settings.BodyListeners.Count, Is.EqualTo(0));

        }
    }
}

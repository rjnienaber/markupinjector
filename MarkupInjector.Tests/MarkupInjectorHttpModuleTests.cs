using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Web;
using Moq;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.ComponentModel;

namespace MarkupInjector.Tests
{
    [TestFixture]
    public class MarkupInjectorHttpModuleTests : TestContainer
    {
        public override void Setup()
        {
            Settings.BodyListeners.Clear();
            Settings.HeadListeners.Clear();
        }

        [Test]
        public void Should_Not_Do_Anything_When_There_Are_No_Listeners()
        {
            var module = new MarkupInjectorHttpModule();
            module.ReleaseRequestState(null);
        }

        [Test]
        public void Should_Not_Do_Anything_When_Not_Html_Response()
        {
            var response = Factory.Create<HttpResponseBase>();
            var context = Factory.Create<HttpContextBase>();
            context.Setup(c => c.Response).Returns(response.Object);

            response.Setup(r => r.ContentType).Returns("text/javascript");

            Settings.HeadListeners.Add(() => "blah");

            var module = new MarkupInjectorHttpModule();
            module.ReleaseRequestState(context.Object);
        }

        [Test]
        public void Should_Not_Do_Anything_When_Its_A_Redirect()
        {
            var response = Factory.Create<HttpResponseBase>();
            var context = Factory.Create<HttpContextBase>();
            context.Setup(c => c.Response).Returns(response.Object);

            response.Setup(r => r.ContentType).Returns("text/html");
            response.Setup(r => r.StatusCode).Returns(301);

            Settings.HeadListeners.Add(() => "blah");

            var module = new MarkupInjectorHttpModule();
            module.ReleaseRequestState(context.Object);
        }

        [Test]
        public void Should_Inject_When_Normal_Html_Document()
        {
            var response = Factory.Create<HttpResponseBase>(MockBehavior.Loose);
            var context = Factory.Create<HttpContextBase>();
            context.Setup(c => c.Response).Returns(response.Object);

            response.Setup(r => r.ContentType).Returns("text/html");
            response.Setup(r => r.StatusCode).Returns(200);
            response.Setup(r => r.ContentEncoding).Returns(Encoding.Unicode);
            response.Setup(r => r.Filter).Returns(new MemoryStream());

            Settings.HeadListeners.Add(() => "blah");
            Settings.BodyListeners.Add(() => "blah");
            var module = Factory.Create<MarkupInjectorHttpModule>();
            module.CallBase = true;
            var filter = new InsertMarkupFilter(response.Object);
            module.Setup(m => m.GetFilter(response.Object)).Returns(filter);

            Func<string, MulticastDelegate> getEventDelegate = name =>
            {
                FieldInfo my_event_FieldInfo = filter.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
                return (MulticastDelegate)my_event_FieldInfo.GetValue(filter);
            };

            Assert.IsNull(getEventDelegate("EndOfHeadDetected"));
            Assert.IsNull(getEventDelegate("EndOfBodyDetected"));

            module.Object.ReleaseRequestState(context.Object);

            Assert.NotNull(getEventDelegate("EndOfHeadDetected"));
            Assert.NotNull(getEventDelegate("EndOfBodyDetected"));
        }
    }
}

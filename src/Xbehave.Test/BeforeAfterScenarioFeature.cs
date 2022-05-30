using System;
using System.Globalization;
using System.Linq;
using Xbehave.Test.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Xbehave.Test
{
    public class BeforeAfterScenarioFeature : Feature
    {
        [Background]
        public void Background() =>
            "Given no events have occurred"
                .x(() => typeof(BeforeAfterScenarioFeature).ClearTestEvents());

        [Scenario]
        public void BeforeAfterAttribute(Type feature, ITestResultMessage[] results)
        {
            "Given a scenario with a before and after attribute"
                .x(() => feature = typeof(ScenarioWithBeforeAfterScenarioAttribute));

            "When I run the scenario"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then the attributes before and after methods are called before and after the scenario"
                .x(() => Assert.Equal(
                    new[] { "before1", "step1", "step2", "step3", "after1" },
                    typeof(BeforeAfterScenarioFeature).GetTestEvents()));
        }

        [Scenario]
        public void ThrowsBefore(Type feature, ITestResultMessage[] results)
        {
            "Given a scenario with a throw before attribute"
                .x(() => feature = typeof(ScenarioWithThrowBeforeAttribute));

            "When I run the scenario"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then there is a single test failure"
                .x(() => Assert.Single(results.OfType<ITestFailed>()));
        }

        [Scenario]
        public void ThrowsAfter(Type feature, ITestResultMessage[] results)
        {
            "Given a scenario with a throw after attribute"
                .x(() => feature = typeof(ScenarioWithThrowAfterAttribute));

            "When I run the scenario"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then there is a single test failure"
                .x(() => Assert.Single(results.OfType<ITestFailed>()));
        }

        public static class ScenarioWithBeforeAfterScenarioAttribute
        {
            [BeforeAfter]
            [Scenario]
            public static void Scenario()
            {
                "Given"
                    .x(() => typeof(BeforeAfterScenarioFeature).SaveTestEvent("step1"));

                "When"
                    .x(() => typeof(BeforeAfterScenarioFeature).SaveTestEvent("step2"));

                "Then"
                    .x(() => typeof(BeforeAfterScenarioFeature).SaveTestEvent("step3"));
            }
        }

        public static class ScenarioWithThrowBeforeAttribute
        {
            [ThrowBefore]
            [Scenario]
            public static void Scenario()
            {
                "Given"
                    .x(() => { });

                "When"
                    .x(() => { });

                "Then"
                    .x(() => { });
            }
        }

        public static class ScenarioWithThrowAfterAttribute
        {
            [ThrowAfter]
            [Scenario]
            public static void Scenario()
            {
                "Given"
                    .x(() => { });

                "When"
                    .x(() => { });

                "Then"
                    .x(() => { });
            }
        }

        private sealed class BeforeAfter : BeforeAfterScenarioAttribute
        {
            private static int beforeCount;
            private static int afterCount;

            public override void Before(System.Reflection.MethodInfo methodUnderTest) =>
                typeof(BeforeAfterScenarioFeature)
                    .SaveTestEvent("before" + (++beforeCount).ToString(CultureInfo.InvariantCulture));

            public override void After(System.Reflection.MethodInfo methodUnderTest) =>
                typeof(BeforeAfterScenarioFeature)
                    .SaveTestEvent("after" + (++afterCount).ToString(CultureInfo.InvariantCulture));
        }

        private sealed class ThrowBefore : BeforeAfterScenarioAttribute
        {
            public override void Before(System.Reflection.MethodInfo methodUnderTest) => throw new InvalidOperationException();
        }

        private sealed class ThrowAfter : BeforeAfterScenarioAttribute
        {
            public override void After(System.Reflection.MethodInfo methodUnderTest) => throw new InvalidOperationException();
        }
    }
}

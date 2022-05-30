using System;
using System.Collections.Generic;
using System.Linq;
using Xbehave.Sdk;
using Xbehave.Test.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Xbehave.Test
{
    public class StepDefinitionFilterFeature : Feature
    {
        [Scenario]
        public void SkipAll(Type feature, ITestResultMessage[] results)
        {
            "Given a scenario marked with SkipAll"
                .x(() => feature = typeof(ScenarioWithSkipAll));

            "When I run the scenario"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then the steps are recognized"
                .x(() => Assert.NotEmpty(results));

            "And the steps are skipped"
                .x(() => Assert.All(results, result => Assert.IsAssignableFrom<ITestSkipped>(result)));
        }

        [Scenario]
        public void ContinueAfterThen(Type feature, ITestResultMessage[] results)
        {
            "Given a scenario marked with ContinueAfterThen"
                .x(() => feature = typeof(ScenarioWithContinueAfterThen));

            "When I run the scenario"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then there are four results"
                .x(() => Assert.Equal(4, results.Length));

            "Then the first two steps pass"
                .x(() => Assert.All(results.Take(2), result => Assert.IsAssignableFrom<ITestPassed>(result)));

            "And the third step fails"
                .x(() => Assert.All(results.Skip(2).Take(1), result => Assert.IsAssignableFrom<ITestFailed>(result)));

            "And the fourth step passes"
                .x(() => Assert.All(results.Skip(3).Take(1), result => Assert.IsAssignableFrom<ITestPassed>(result)));
        }

        [Scenario]
        public void BackgroundSuffixes(Type feature, ITestResultMessage[] results)
        {
            "Given a scenario marked with BackgroundSuffixes"
                .x(() => feature = typeof(ScenarioWithBackgroundSuffixes));

            "When I run the scenario"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then the first result has a background suffix"
                .x(() => Assert.EndsWith("(Background)", results[0].Test.DisplayName));
        }

        private sealed class SkipAllAttribute : Attribute, IFilter<IStepDefinition>
        {
            public IEnumerable<IStepDefinition> Filter(IEnumerable<IStepDefinition> steps) =>
                steps.Select(step => step.Skip("test"));
        }

        public class ScenarioWithSkipAll
        {
            [Scenario]
            [SkipAll]
            public void Scenario()
            {
                "Given something"
                    .x(() => { });

                "When something"
                    .x(() => { });

                "Then something"
                    .x(() => { });
            }
        }

        private sealed class ContinueAfterThenAttribute : Attribute, IFilter<IStepDefinition>
        {
            public IEnumerable<IStepDefinition> Filter(IEnumerable<IStepDefinition> steps)
            {
                var then = false;
                return steps.Select(step => step.OnFailure(
                    then || (then = step.Text.StartsWith("Then ", StringComparison.OrdinalIgnoreCase))
                    ? RemainingSteps.Run
                    : RemainingSteps.Skip));
            }
        }

        public class ScenarioWithContinueAfterThen
        {
            [Scenario]
            [ContinueAfterThen]
            public void Scenario()
            {
                "Given something"
                    .x(() => { });

                "When something"
                    .x(() => { });

                "Then something"
                    .x(() => throw new InvalidOperationException());

                "And something"
                    .x(() => { });
            }
        }

        private sealed class BackgroundSuffixesAttribute : Attribute, IFilter<IStepDefinition>
        {
            public IEnumerable<IStepDefinition> Filter(IEnumerable<IStepDefinition> steps) =>
                steps.Select(step => step.DisplayText((stepText, isBackgroundStep) =>
                    stepText + (isBackgroundStep ? " (Background)" : null)));
        }

        public class ScenarioWithBackgroundSuffixes
        {
            [Background]
            public void Background() =>
                "Given something"
                    .x(() => { });

            [Scenario]
            [BackgroundSuffixes]
            public void Scenario()
            {
                "Given something"
                    .x(() => { });

                "When something"
                    .x(() => { });

                "Then something"
                    .x(() => throw new InvalidOperationException());

                "And something"
                    .x(() => { });
            }
        }
    }
}

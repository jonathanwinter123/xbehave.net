using System;
using Xbehave.Test.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Xbehave.Test
{
    public class TraitFeature : Feature
    {
        [Scenario]
        [Example("bar", 1)]
        [Example("baz", 1)]
        [Example("bazz", 0)]
        public void ScenariosWithTraits(string traitValue, int expectedResultCount, Type feature, ITestResultMessage[] results)
        {
            "Given two single step scenarios, 'foo' traits of 'bar' and 'baz' respectively"
                .x(() => feature = typeof(TwoSingleStepScenariosWithFooTraitsOfBarAndBazRespectively));

            $"When I run the feature specifying the 'foo' trait of '{traitValue}'"
                .x(() => results = this.Run<ITestResultMessage>(feature, "foo", traitValue));

            "Then there is only {1} result"
                .x(() => Assert.Equal(expectedResultCount, results.Length));
        }

        public class TwoSingleStepScenariosWithFooTraitsOfBarAndBazRespectively
        {
            [Scenario]
            [Trait("foo", "bar")]
            public void Bar() =>
                "Bar"
                    .x(() => { });

            [Scenario]
            [Trait("foo", "baz")]
            public void Baz() =>
                "Baz"
                    .x(() => { });
        }
    }
}

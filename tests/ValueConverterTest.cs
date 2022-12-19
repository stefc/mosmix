using Xunit;

namespace stefc.mosmix.tests;

public class ValueConverterTest
{
    [Fact]
    public void DateTimeConverterTest()
    {

        var timeStamp = "2021-12-20T22:00:00.000Z";
        // "2008-06-11T16:11:20.0904778Z";

        var valueConverter = ValueConverter.For(typeof(DateTime));

        if (valueConverter(timeStamp, out var value))
        {
            var actual = (DateTime)value;
            Assert.Equal(20, actual.Day);
            Assert.Equal(2021, actual.Year);
            Assert.Equal(12, actual.Month);
            Assert.Equal(22, actual.Hour);
            Assert.Equal(0, actual.Minute);
            Assert.Equal(0, actual.Second);
        }
        else
        {
            Assert.False(true);
        }
    }
}